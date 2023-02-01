using Newtonsoft.Json;
using System.Text;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Lolicon;

namespace TheresaBot.Main.Business
{
    public class LoliconBusiness : SetuBusiness
    {
        private const int eachPage = 5;

        public string getWorkInfo(LoliconDataV2 loliconData, DateTime startTime, long todayLeft, string template = "")
        {
            if (string.IsNullOrWhiteSpace(template)) return getDefaultWorkInfo(loliconData, startTime);
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            template = template.Replace("{TodayLeft}", todayLeft.ToString());
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SetuConfig.RevokeInterval.ToString());
            template = template.Replace("{IllustTitle}", loliconData.title);
            template = template.Replace("{PixivId}", loliconData.pid.ToString());
            template = template.Replace("{UserName}", loliconData.author);
            template = template.Replace("{UserId}", loliconData.uid);
            template = template.Replace("{SizeMB}", "??");
            template = template.Replace("{CostSecond}", costSecond.ToString());
            template = template.Replace("{Tags}", BusinessHelper.JoinPixivTagsStr(loliconData.tags, BotConfig.PixivConfig.TagShowMaximum));
            template = template.Replace("{Urls}", loliconData.urls.original.ToOrginProxyUrl());
            return template;
        }

        public string getDefaultWorkInfo(LoliconDataV2 loliconData, DateTime startTime)
        {
            StringBuilder workInfoStr = new StringBuilder();
            int costSecond = DateTimeHelper.GetSecondDiff(startTime, DateTime.Now);
            workInfoStr.AppendLine($"标题：{loliconData.title}，画师：{loliconData.author}，画师id：{loliconData.uid}，耗时：{costSecond}s");
            workInfoStr.AppendLine($"标签：{BusinessHelper.JoinPixivTagsStr(loliconData.tags, BotConfig.PixivConfig.TagShowMaximum)}");
            workInfoStr.Append(loliconData.urls.original.ToOrginProxyUrl());
            return workInfoStr.ToString();
        }

        public async Task<List<LoliconDataV2>> getLoliconDataListAsync(int r18Mode, bool excludeAI, int quantity = 1, string[] tags = null)
        {
            List<LoliconDataV2> setuList = new();
            while (quantity > 0)
            {
                int num = quantity >= eachPage ? eachPage : quantity;
                quantity -= eachPage;
                LoliconResultV2 loliconResult = await getLoliconResultAsync(r18Mode, excludeAI, num, tags);
                if (loliconResult?.data is null) continue;
                foreach (var setuInfo in loliconResult.data)
                {
                    setuList.Add(setuInfo);
                }
            }
            return setuList;
        }

        private async Task<LoliconResultV2> getLoliconResultAsync(int r18Mode, bool excludeAI, int quantity = 1, string[] tags = null)
        {
            string[] postTags = tags is null || tags.Length == 0 ? new string[0] : tags;
            LoliconParamV2 param = new LoliconParamV2(r18Mode, excludeAI, quantity, "i.pixiv.re", postTags);
            string httpUrl = HttpUrl.getLoliconApiV2Url();
            string postJson = JsonConvert.SerializeObject(param);
            string json = await HttpHelper.PostJsonAsync(httpUrl, postJson);
            LoliconResultV2 result = JsonConvert.DeserializeObject<LoliconResultV2>(json);
            if (string.IsNullOrWhiteSpace(result.error) == false) throw new ApiException($"lolicon api error,message = {result.error}");
            return result;
        }

    }
}
