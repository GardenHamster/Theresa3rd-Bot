using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Exceptions;
using Theresa3rd_Bot.Model.Lolisuki;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class LolisukiBusiness : SetuBusiness
    {
        public string getWorkInfo(LolisukiData lolisukiData, DateTime startTime, long todayLeft, string template = "")
        {
            if (string.IsNullOrWhiteSpace(template)) return getDefaultWorkInfo(lolisukiData, startTime);
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            template = template.Replace("{TodayLeft}", todayLeft.ToString());
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SetuConfig.RevokeInterval.ToString());
            template = template.Replace("{IllustTitle}", lolisukiData.title);
            template = template.Replace("{PixivId}", lolisukiData.pid.ToString());
            template = template.Replace("{UserName}", lolisukiData.author);
            template = template.Replace("{UserId}", lolisukiData.uid);
            template = template.Replace("{Level}", lolisukiData.level.ToString());
            template = template.Replace("{Taste}", lolisukiData.taste.ToString());
            template = template.Replace("{SizeMB}", "??");
            template = template.Replace("{CostSecond}", costSecond.ToString());
            template = template.Replace("{Tags}", BusinessHelper.JoinPixivTagsStr(lolisukiData.tags, BotConfig.PixivConfig.TagShowMaximum));
            template = template.Replace("{Urls}", lolisukiData.urls.original.ToOrginProxyUrl());
            return template;
        }

        public string getDefaultWorkInfo(LolisukiData lolisukiData, DateTime startTime)
        {
            StringBuilder workInfoStr = new StringBuilder();
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            workInfoStr.AppendLine($"标题：{lolisukiData.title}，画师：{lolisukiData.author}，画师id：{lolisukiData.uid}，Level：{lolisukiData.level}，分类：{lolisukiData.taste}，耗时：{costSecond}s");
            workInfoStr.AppendLine($"标签：{BusinessHelper.JoinPixivTagsStr(lolisukiData.tags, BotConfig.PixivConfig.TagShowMaximum)}");
            workInfoStr.Append(lolisukiData.urls.original.ToOrginProxyUrl());
            return workInfoStr.ToString();
        }

        public async Task<LolisukiResult> getLolisukiResultAsync(int r18Mode, int aiMode, string level, int num = 1, string[] tags = null)
        {
            string[] postTags = tags == null || tags.Length == 0 ? new string[0] : tags;
            LolisukiParam param = new LolisukiParam(r18Mode, aiMode, num, "i.pixiv.re", postTags, level, 0);
            string httpUrl = HttpUrl.getLolisukiApiUrl();
            string postJson = JsonConvert.SerializeObject(param);
            string json = await HttpHelper.PostJsonAsync(httpUrl, postJson);
            LolisukiResult result = JsonConvert.DeserializeObject<LolisukiResult>(json);
            if (result.code != 0) throw new ApiException($"lolisuki api error,message = {result.error}");
            return result;
        }


    }
}
