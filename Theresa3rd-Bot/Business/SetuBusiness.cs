using AnimatedGif;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
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
            string imgSize = BotConfig.GeneralConfig.PixivImgSize?.ToLower();
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
        public async Task<FileInfo> downImgAsync(string pixivId, string originUrl, bool isGif)
        {
            try
            {
                if (isGif) return await downAndComposeGifAsync(pixivId);
                string imgUrl = getDownImgUrl(originUrl);
                string fullFileName = $"{pixivId}.jpg";
                string fullImageSavePath = Path.Combine(FilePath.getDownImgSavePath(), fullFileName);
                Dictionary<string, string> headerDic = new Dictionary<string, string>();
                headerDic.Add("Referer", HttpUrl.getPixivArtworksReferer(pixivId));
                headerDic.Add("Cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);
                if (BotConfig.GeneralConfig.PixivFreeProxy || string.IsNullOrWhiteSpace(BotConfig.GeneralConfig.PixivImgProxy) == false)
                {
                    return await HttpHelper.DownFileAsync(imgUrl.ToProxyUrl(), fullImageSavePath);
                }
                else if (string.IsNullOrWhiteSpace(BotConfig.GeneralConfig.PixivHttpProxy) == false)
                {
                    return await HttpHelper.DownFileWithProxyAsync(imgUrl.ToPximgUrl(), fullImageSavePath, headerDic);
                }
                else
                {
                    return await HttpHelper.DownFileAsync(imgUrl.ToPximgUrl(), fullImageSavePath, headerDic);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "PixivBusiness.downImg下载图片失败");
                return null;
            }
        }

        /// <summary>
        /// 下载动图zip包并合成gif图片
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        protected async Task<FileInfo> downAndComposeGifAsync(string pixivId)
        {
            string fullGifSavePath = Path.Combine(FilePath.getDownImgSavePath(), $"{pixivId}.gif");
            if (File.Exists(fullGifSavePath)) return new FileInfo(fullGifSavePath);

            PixivUgoiraMetaDto pixivUgoiraMetaDto = await PixivHelper.GetPixivUgoiraMetaAsync(pixivId);
            string fullZipSavePath = Path.Combine(FilePath.getDownImgSavePath(), $"{StringHelper.get16UUID()}.zip");
            string zipHttpUrl = pixivUgoiraMetaDto.body.src;

            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            headerDic.Add("Referer", HttpUrl.getPixivArtworksReferer(pixivId));
            headerDic.Add("Cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);

            if (BotConfig.GeneralConfig.PixivFreeProxy || string.IsNullOrWhiteSpace(BotConfig.GeneralConfig.PixivImgProxy) == false)
            {
                await HttpHelper.DownFileAsync(zipHttpUrl.ToProxyUrl(), fullZipSavePath);
            }
            else if (string.IsNullOrWhiteSpace(BotConfig.GeneralConfig.PixivHttpProxy) == false)
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
                int delay = frame == null ? 60 : frame.delay;
                using Image img = Image.FromFile(file.FullName);
                gif.AddFrame(img, delay, GifQuality.Bit8);
                await Task.Delay(1000);
            }

            try
            {
                File.Delete(fullZipSavePath);
                Directory.Delete(unZipDirPath, true);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "gif临时文件删除失败");
            }

            return new FileInfo(fullGifSavePath);
        }


    }
}
