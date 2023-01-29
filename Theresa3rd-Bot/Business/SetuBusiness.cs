using AnimatedGif;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Base;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public abstract class SetuBusiness
    {
        /// <summary>
        /// 根据配置文件设置的图片大小获取图片下载地址
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        public string getDownImgUrl(string originalUrl)
        {
            string imgSize = BotConfig.PixivConfig.ImgSize?.ToLower();
            if (imgSize == "original") return originalUrl;
            if (imgSize == "regular") return originalUrl.ToRegularUrl();
            if (imgSize == "small") return originalUrl.ToSmallUrl();
            if (imgSize == "thumb") return originalUrl.ToThumbUrl();
            return originalUrl.ToThumbUrl();
        }

        /// <summary>
        /// 根据配置使用代理或者直连下载图片
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        public async Task<List<FileInfo>> downPixivImgsAsync(BaseWorkInfo pixivWorkInfo)
        {
            try
            {
                if (pixivWorkInfo.IsGif)
                {
                    return new List<FileInfo>() { await downAndComposeGifAsync(pixivWorkInfo.PixivId) };
                }
                List<FileInfo> imgList = new List<FileInfo>();
                List<string> originUrls = pixivWorkInfo.getOriginalUrls();
                int maxCount = BotConfig.PixivConfig.ImgShowMaximum <= 0 ? originUrls.Count : BotConfig.PixivConfig.ImgShowMaximum;
                for (int i = 0; i < maxCount && i < originUrls.Count; i++)
                {
                    imgList.Add(await downPixivImgAsync(pixivWorkInfo.PixivId, originUrls[i], BotConfig.PixivConfig.ImgRetryTimes));
                }
                return imgList;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "PixivBusiness.downImg下载图片失败");
                return null;
            }
        }

        private async Task<FileInfo> downPixivImgAsync(string pixivId, string originUrl, int retryTimes, string fullFileName = null)
        {
            if (retryTimes < 0) retryTimes = 0;
            while (retryTimes >= 0)
            {
                try
                {
                    string downloadUrl = getDownImgUrl(originUrl);
                    if (string.IsNullOrWhiteSpace(fullFileName)) fullFileName = originUrl.getHttpFileName();
                    string fullImgSavePath = Path.Combine(FilePath.getDownImgSavePath(), fullFileName);
                    Dictionary<string, string> headerDic = new Dictionary<string, string>();
                    headerDic.Add("Referer", HttpUrl.getPixivArtworksReferer(pixivId));
                    headerDic.Add("Cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);
                    if (BotConfig.PixivConfig.FreeProxy || string.IsNullOrWhiteSpace(BotConfig.PixivConfig.ImgProxy) == false)
                    {
                        return await HttpHelper.DownFileAsync(downloadUrl.ToProxyUrl(), fullImgSavePath);
                    }
                    else if (string.IsNullOrWhiteSpace(BotConfig.PixivConfig.HttpProxy) == false)
                    {
                        return await HttpHelper.DownFileWithProxyAsync(downloadUrl.ToPximgUrl(), fullImgSavePath, headerDic);
                    }
                    else
                    {
                        return await HttpHelper.DownFileAsync(downloadUrl.ToPximgUrl(), fullImgSavePath, headerDic);
                    }
                }
                catch (Exception)
                {
                    if (--retryTimes < 0) throw;
                    await Task.Delay(3000);
                }
            }
            return null;
        }

        /// <summary>
        /// 下载动图zip包并合成gif图片
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        protected async Task<FileInfo> downAndComposeGifAsync(string pixivId)
        {
            try
            {
                string fullGifSavePath = Path.Combine(FilePath.getDownImgSavePath(), $"{pixivId}.gif");
                if (File.Exists(fullGifSavePath)) return new FileInfo(fullGifSavePath);

                PixivResult<PixivUgoiraMeta> pixivUgoiraMetaDto = await PixivHelper.GetPixivUgoiraMetaAsync(pixivId);
                string fullZipSavePath = Path.Combine(FilePath.getDownImgSavePath(), $"{StringHelper.get16UUID()}.zip");
                string zipHttpUrl = pixivUgoiraMetaDto.body.src;

                Dictionary<string, string> headerDic = new Dictionary<string, string>();
                headerDic.Add("Referer", HttpUrl.getPixivArtworksReferer(pixivId));
                headerDic.Add("Cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);

                if (BotConfig.PixivConfig.FreeProxy || string.IsNullOrWhiteSpace(BotConfig.PixivConfig.ImgProxy) == false)
                {
                    await HttpHelper.DownFileAsync(zipHttpUrl.ToProxyUrl(), fullZipSavePath);
                }
                else if (string.IsNullOrWhiteSpace(BotConfig.PixivConfig.HttpProxy) == false)
                {
                    await HttpHelper.DownFileWithProxyAsync(zipHttpUrl.ToPximgUrl(), fullZipSavePath, headerDic);
                }
                else
                {
                    await HttpHelper.DownFileAsync(zipHttpUrl.ToPximgUrl(), fullZipSavePath, headerDic);
                }

                string unZipDirPath = Path.Combine(FilePath.getDownImgSavePath(), pixivId);
                ZipHelper.ZipToFile(fullZipSavePath, unZipDirPath);
                DirectoryInfo directoryInfo = new DirectoryInfo(unZipDirPath);
                FileInfo[] files = directoryInfo.GetFiles();
                List<PixivUgoiraMetaFrames> frames = pixivUgoiraMetaDto.body.frames;
                using AnimatedGifCreator gif = AnimatedGif.AnimatedGif.Create(fullGifSavePath, 0);
                foreach (FileInfo file in files)
                {
                    PixivUgoiraMetaFrames frame = frames.Where(o => o.file.Trim() == file.Name).FirstOrDefault();
                    int delay = frame is null ? 60 : frame.delay;
                    using Image img = Image.FromFile(file.FullName);
                    gif.AddFrame(img, delay, GifQuality.Bit8);
                    await Task.Delay(1000);
                }
                FileHelper.deleteFile(fullZipSavePath);
                FileHelper.deleteDirectory(unZipDirPath);
                return new FileInfo(fullGifSavePath);
            }
            catch (Exception ex)
            {
                string errMsg = "gif合成失败";
                LogHelper.Error(ex, errMsg);
                ReportHelper.SendError(ex, errMsg);
                return null;
            }
        }


    }
}
