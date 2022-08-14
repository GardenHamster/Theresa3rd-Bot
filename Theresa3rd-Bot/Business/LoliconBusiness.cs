using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Lolicon;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class LoliconBusiness : SetuBusiness
    {
        public string getWorkInfo(LoliconDataV2 loliconData, FileInfo fileInfo, DateTime startTime, long todayLeft, string template = "")
        {
            if (string.IsNullOrWhiteSpace(template)) return getDefaultWorkInfo(loliconData, fileInfo, startTime);
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            double sizeMB = fileInfo == null ? 0 : MathHelper.getMbWithByte(fileInfo.Length);
            template = template.Replace("{TodayLeft}", todayLeft.ToString());
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SetuConfig.RevokeInterval.ToString());
            template = template.Replace("{IllustTitle}", loliconData.title);
            template = template.Replace("{PixivId}", loliconData.pid.ToString());
            template = template.Replace("{UserName}", loliconData.author);
            template = template.Replace("{UserId}", loliconData.uid);
            template = template.Replace("{SizeMB}", sizeMB.ToString());
            template = template.Replace("{CostSecond}", costSecond.ToString());
            template = template.Replace("{Tags}", BusinessHelper.JoinPixivTagsStr(loliconData.tags, BotConfig.GeneralConfig.PixivTagShowMaximum));
            template = template.Replace("{Urls}", getProxyUrl(loliconData.urls.original));
            return template;
        }

        public string getDefaultWorkInfo(LoliconDataV2 loliconData, FileInfo fileInfo, DateTime startTime)
        {
            StringBuilder workInfoStr = new StringBuilder();
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            double sizeMB = fileInfo == null ? 0 : MathHelper.getMbWithByte(fileInfo.Length);
            workInfoStr.AppendLine($"标题：{loliconData.title}，画师：{loliconData.author}，画师id：{loliconData.uid}，大小：{sizeMB}MB，耗时：{costSecond}s");
            workInfoStr.AppendLine($"标签：{BusinessHelper.JoinPixivTagsStr(loliconData.tags, BotConfig.GeneralConfig.PixivTagShowMaximum)}");
            workInfoStr.Append(getProxyUrl(loliconData.urls.original));
            return workInfoStr.ToString();
        }

        public async Task<LoliconResultV2> getLoliconResultAsync(int r18Mode,string[] tags = null)
        {
            LoliconParamV2 param = new LoliconParamV2(r18Mode, 1, "i.pixiv.re", tags == null || tags.Length == 0 ? null : tags);
            string httpUrl = HttpUrl.getLoliconApiV2Url();
            string postJson = JsonConvert.SerializeObject(param);
            string json = await HttpHelper.PostJsonAsync(httpUrl, postJson);
            return JsonConvert.DeserializeObject<LoliconResultV2>(json);
        }

        public async Task<FileInfo> downImgAsync(LoliconDataV2 loliconData)
        {
            try
            {
                string fullFileName = $"{loliconData.pid}.jpg";
                string fullImageSavePath = Path.Combine(FilePath.getDownImgSavePath(), fullFileName);
                string imgUrl = getDownImgUrl(loliconData);
                if (BotConfig.GeneralConfig.DownWithProxy || BotConfig.GeneralConfig.PixivFreeProxy)
                {
                    imgUrl = getProxyUrl(imgUrl);
                    return await HttpHelper.DownFileAsync(imgUrl, fullImageSavePath);
                }
                else
                {
                    imgUrl = getOriginalUrl(imgUrl);
                    Dictionary<string, string> headerDic = new Dictionary<string, string>();
                    headerDic.Add("Referer", HttpUrl.getPixivArtworksReferer(loliconData.pid.ToString()));
                    headerDic.Add("Cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);
                    return await HttpHelper.DownFileAsync(imgUrl, fullImageSavePath, headerDic);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "LoliconBusiness.downImg下载图片失败");
                return null;
            }
        }

        /// <summary>
        /// 根据配置文件设置的图片大小获取图片下载地址
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        private string getDownImgUrl(LoliconDataV2 loliconData)
        {
            string imgSize = BotConfig.GeneralConfig.PixivImgSize?.ToLower();
            string returnlUrl = loliconData.urls?.original;
            if (imgSize == "original")
            {
                return returnlUrl;
            }
            else if (imgSize == "regular")
            {
                string replaceStr = $"{loliconData.pid}_p{loliconData.p}";
                returnlUrl = returnlUrl.Replace("img-original", "img-master");
                returnlUrl = returnlUrl.Replace(replaceStr, $"{replaceStr}_master1200");
                return returnlUrl;
            }
            else if (imgSize == "small")
            {
                string replaceStr = $"{loliconData.pid}_p{loliconData.p}";
                returnlUrl = returnlUrl.Replace("img-original", "c/540x540_70/img-master");
                returnlUrl = returnlUrl.Replace(replaceStr, $"{replaceStr}_master1200");
                return returnlUrl;
            }
            else if (imgSize == "thumb")
            {
                string replaceStr = $"{loliconData.pid}_p{loliconData.p}";
                returnlUrl = returnlUrl.Replace("img-original", "c/250x250_80_a2/img-master");
                returnlUrl = returnlUrl.Replace(replaceStr, $"{replaceStr}_square1200");
                return returnlUrl;
            }
            else
            {
                string replaceStr = $"{loliconData.pid}_p{loliconData.p}";
                returnlUrl = returnlUrl.Replace("img-original", "c/250x250_80_a2/img-master");
                returnlUrl = returnlUrl.Replace(replaceStr, $"{replaceStr}_square1200");
                return returnlUrl;
            }
        }



    }
}
