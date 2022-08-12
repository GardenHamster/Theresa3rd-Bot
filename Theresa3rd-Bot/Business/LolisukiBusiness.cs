using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Lolisuki;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class LolisukiBusiness : SetuBusiness
    {
        public string getWorkInfo(LolisukiData lolisukiData, FileInfo fileInfo, DateTime startTime, long todayLeft, string template = "")
        {
            if (string.IsNullOrWhiteSpace(template)) return getDefaultWorkInfo(lolisukiData, fileInfo, startTime);
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            double sizeMB = fileInfo == null ? 0 : MathHelper.getMbWithByte(fileInfo.Length);
            template = template.Replace("{TodayLeft}", todayLeft.ToString());
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SetuConfig.RevokeInterval.ToString());
            template = template.Replace("{IllustTitle}", lolisukiData.title);
            template = template.Replace("{PixivId}", lolisukiData.pid.ToString());
            template = template.Replace("{UserName}", lolisukiData.author);
            template = template.Replace("{UserId}", lolisukiData.uid);
            template = template.Replace("{Level}", lolisukiData.level.ToString());
            template = template.Replace("{SizeMB}", sizeMB.ToString());
            template = template.Replace("{CostSecond}", costSecond.ToString());
            template = template.Replace("{Tags}", BusinessHelper.JoinPixivTagsStr(lolisukiData.tags, BotConfig.GeneralConfig.PixivTagShowMaximum));
            template = template.Replace("{Urls}", getProxyUrl(lolisukiData.urls.original));
            return template;
        }

        public string getDefaultWorkInfo(LolisukiData lolisukiData, FileInfo fileInfo, DateTime startTime)
        {
            StringBuilder workInfoStr = new StringBuilder();
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            double sizeMB = fileInfo == null ? 0 : MathHelper.getMbWithByte(fileInfo.Length);
            workInfoStr.AppendLine($"标题：{lolisukiData.title}，画师：{lolisukiData.author}，画师id：{lolisukiData.uid}，Level：{lolisukiData.level}，大小：{sizeMB}MB，耗时：{costSecond}s");
            workInfoStr.AppendLine($"标签：{BusinessHelper.JoinPixivTagsStr(lolisukiData.tags, BotConfig.GeneralConfig.PixivTagShowMaximum)}");
            workInfoStr.Append(getProxyUrl(lolisukiData.urls.original));
            return workInfoStr.ToString();
        }

        public async Task<LolisukiResult> getLolisukiResultAsync(int r18Mode, string level, string[] tags = null)
        {
            string[] postTags = tags == null || tags.Length == 0 ? new string[0] : tags;
            LolisukiParam param = new LolisukiParam(r18Mode, 1, "i.pixiv.re", postTags, level, 0);
            string httpUrl = HttpUrl.getLolisukiApiUrl();
            string postJson = JsonConvert.SerializeObject(param);
            string json = await HttpHelper.PostJsonAsync(httpUrl, postJson);
            return JsonConvert.DeserializeObject<LolisukiResult>(json);
        }

        

        public async Task<FileInfo> downImgAsync(LolisukiData lolisukiData)
        {
            try
            {
                string fullFileName = $"{lolisukiData.pid}.jpg";
                string fullImageSavePath = Path.Combine(FilePath.getDownImgSavePath(), fullFileName);
                string imgUrl = getDownImgUrl(lolisukiData);
                if (BotConfig.GeneralConfig.DownWithProxy || BotConfig.GeneralConfig.PixivFreeProxy)
                {
                    imgUrl = getProxyUrl(imgUrl);
                    return await HttpHelper.DownFileAsync(imgUrl, fullImageSavePath);
                }
                else
                {
                    imgUrl = getOriginalUrl(imgUrl);
                    Dictionary<string, string> headerDic = new Dictionary<string, string>();
                    headerDic.Add("Referer", HttpUrl.getPixivArtworksReferer(lolisukiData.pid.ToString()));
                    headerDic.Add("Cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);
                    return await HttpHelper.DownFileAsync(imgUrl, fullImageSavePath, headerDic);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "LolisukiBusiness.downImg下载图片失败");
                return null;
            }
        }

        /// <summary>
        /// 根据配置文件设置的图片大小获取图片下载地址
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        private string getDownImgUrl(LolisukiData lolisukiData)
        {
            string imgSize = BotConfig.GeneralConfig.PixivImgSize?.ToLower();
            if (imgSize == "original") return lolisukiData.urls?.original;
            if (imgSize == "regular") return lolisukiData.urls?.regular;
            if (imgSize == "small") return lolisukiData.urls?.small;
            if (imgSize == "thumb") return lolisukiData.urls?.thumb;
            return lolisukiData.urls?.original;
        }


    }
}
