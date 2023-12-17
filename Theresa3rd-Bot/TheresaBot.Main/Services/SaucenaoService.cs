using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Net;
using System.Text;
using TheresaBot.Main.Common;
using TheresaBot.Main.Dao;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Saucenao;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Services
{
    internal class SaucenaoService
    {
        private RequestRecordDao requestRecordDao;

        public SaucenaoService()
        {
            requestRecordDao = new RequestRecordDao();
        }

        public async Task<SaucenaoResult> SearchResultAsync(string imgHttpUrl)
        {
            DateTime startTime = DateTime.Now;
            HtmlParser htmlParser = new HtmlParser();
            string saucenaoHtml = await RequestHtmlAsync(imgHttpUrl);
            IHtmlDocument document = await htmlParser.ParseDocumentAsync(saucenaoHtml);
            IEnumerable<IElement> domList = document.All.Where(m => m.ClassList.Contains("result"));

            List<SaucenaoItem> itemList = new List<SaucenaoItem>();
            if (domList is null || domList.Count() == 0) return new SaucenaoResult(itemList, startTime, 0);
            foreach (IElement resultElement in domList)
            {
                IHtmlCollection<IElement> contentList = resultElement.GetElementsByClassName("resulttablecontent");
                if (contentList is null || contentList.Length == 0) continue;

                IHtmlCollection<IElement> linkifyList = contentList.First().GetElementsByTagName("a");
                if (linkifyList is null || linkifyList.Length == 0) continue;

                decimal similarity = 0;
                IHtmlCollection<IElement> similarityList = resultElement.GetElementsByClassName("resultsimilarityinfo");
                string similarityStr = similarityList != null && similarityList.Length > 0 ? similarityList.First()?.InnerHtml : "00.00";
                if (string.IsNullOrWhiteSpace(similarityStr)) similarityStr = "00.00";
                similarityStr = similarityStr.Replace("%", "");
                decimal.TryParse(similarityStr, out similarity);

                foreach (IElement linkifyElement in linkifyList)
                {
                    SaucenaoItem saucenaoItem = ToSaucenaoItem(linkifyElement, similarity);
                    if (saucenaoItem is null) continue;
                    if (itemList.Any(o => o.SourceUrl == saucenaoItem.SourceUrl)) continue;
                    itemList.Add(saucenaoItem);
                }
            }
            return new SaucenaoResult(itemList, startTime, domList.Count());
        }

        /// <summary>
        /// 封装成为Saucenao数据对象
        /// </summary>
        /// <param name="linkElement"></param>
        /// <param name="similarity"></param>
        /// <returns></returns>
        public SaucenaoItem ToSaucenaoItem(IElement linkElement, decimal similarity)
        {
            string href = linkElement.GetAttribute("href")?.Trim();
            if (string.IsNullOrWhiteSpace(href)) return null;
            string hrefLower = href.ToLower();

            //https://www.pixiv.net/member_illust.php?mode=medium&illust_id=73572009
            if (hrefLower.Contains("www.pixiv.net/member_illust"))
            {
                return new SaucenaoItem(SetuSourceType.Pixiv, href, href.TakeHttpParam("illust_id"), similarity);
            }
            //https://szcb911.fanbox.cc/posts/4045588
            if (hrefLower.Contains("fanbox.cc"))
            {
                return new SaucenaoItem(SetuSourceType.FanBox, href, href.TakeHttpLast(), similarity);
            }
            //https://www.pixiv.net/fanbox/creator/705370
            if (hrefLower.Contains("www.pixiv.net/fanbox"))
            {
                return new SaucenaoItem(SetuSourceType.FanBox, href, href.TakeHttpLast(), similarity);
            }
            //https://yande.re/post/show/523988
            if (hrefLower.Contains("yande.re"))
            {
                return new SaucenaoItem(SetuSourceType.Yande, href, href.TakeHttpLast(), similarity);
            }
            //https://twitter.com/i/web/status/1007548268048416769
            if (hrefLower.Contains("twitter.com"))
            {
                return new SaucenaoItem(SetuSourceType.Twitter, href, href.TakeHttpLast(), similarity);
            }
            //https://danbooru.donmai.us/post/show/3438512
            if (hrefLower.Contains("danbooru.donmai.us"))
            {
                return new SaucenaoItem(SetuSourceType.Danbooru, href, href.TakeHttpLast(), similarity);
            }
            //https://gelbooru.com/index.php?page=post&s=view&id=4639560
            if (hrefLower.Contains("gelbooru.com"))
            {
                return new SaucenaoItem(SetuSourceType.Gelbooru, href, href.TakeHttpParam("id"), similarity);
            }
            //https://konachan.com/post/show/279886
            if (hrefLower.Contains("konachan.com"))
            {
                return new SaucenaoItem(SetuSourceType.Konachan, href, href.TakeHttpLast(), similarity);
            }
            //https://anime-pictures.net/pictures/view_post/602645
            if (hrefLower.Contains("anime-pictures.net"))
            {
                return new SaucenaoItem(SetuSourceType.AnimePictures, href, href.TakeHttpLast(), similarity);
            }

            //https://anidb.net/anime/4427
            //https://bcy.net/illust/detail/120185/1519032
            //https://mangadex.org/chapter/07c706d9-e575-4498-b504-dd85014c555b
            //https://www.mangaupdates.com/series.html?id=13582
            //https://myanimelist.net/manga/5255
            //https://www.imdb.com/title/tt2402101

            return null;
        }

        /// <summary>
        /// 拉取源信息
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public async Task FetchOrigin(List<SaucenaoItem> itemList)
        {
            foreach (var item in itemList)
            {
                await FetchOrigin(item);
            }
        }

        /// <summary>
        /// 拉取源信息
        /// </summary>
        /// <param name="saucenaoItem"></param>
        /// <returns></returns>
        public async Task FetchOrigin(SaucenaoItem saucenaoItem)
        {
            try
            {
                if (saucenaoItem.SourceType == SetuSourceType.Pixiv)
                {
                    saucenaoItem.PixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(saucenaoItem.SourceId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "获取Saucenao返回的Pixiv作品信息失败");
            }
        }

        /// <summary>
        /// 获取提示内容
        /// </summary>
        /// <param name="saucenaoResult"></param>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public BaseContent GetRemindContent(SaucenaoResult saucenaoResult, long groupId, long memberId)
        {
            int leftToday = GetLeftToday(groupId, memberId);
            return new PlainContent(GetRemindMessage(saucenaoResult, leftToday));
        }

        /// <summary>
        /// 获取提示消息
        /// </summary>
        /// <param name="saucenaoResult"></param>
        /// <param name="todayLeft"></param>
        /// <returns></returns>
        public string GetRemindMessage(SaucenaoResult saucenaoResult, long todayLeft)
        {
            string remindTemplate = BotConfig.SaucenaoConfig.Template;
            if (string.IsNullOrWhiteSpace(remindTemplate))
            {
                return GetDefaultRemindMessage(saucenaoResult, todayLeft);
            }
            else
            {
                return GetTemplateRemindMessage(saucenaoResult, remindTemplate, todayLeft);
            }
        }

        /// <summary>
        /// 获取默认提示消息
        /// </summary>
        /// <param name="saucenaoResult"></param>
        /// <param name="todayLeft"></param>
        /// <returns></returns>
        private string GetDefaultRemindMessage(SaucenaoResult saucenaoResult, long todayLeft)
        {
            StringBuilder warnBuilder = new StringBuilder();
            warnBuilder.Append($"共找到 {saucenaoResult.MatchCount} 条匹配信息");
            if (BotConfig.SaucenaoConfig.MaxDaily > 0)
            {
                if (warnBuilder.Length > 0) warnBuilder.Append("，");
                warnBuilder.Append($"今天剩余使用次数{todayLeft}次");
            }
            if (BotConfig.SaucenaoConfig.RevokeInterval > 0)
            {
                if (warnBuilder.Length > 0) warnBuilder.Append("，");
                warnBuilder.Append($"本消息将在{BotConfig.SaucenaoConfig.RevokeInterval}秒后撤回");
            }
            if (BotConfig.SaucenaoConfig.MemberCD > 0)
            {
                if (warnBuilder.Length > 0) warnBuilder.Append("，");
                warnBuilder.Append($"CD{BotConfig.SetuConfig.MemberCD}秒");
            }
            return warnBuilder.ToString();
        }

        /// <summary>
        /// 根据模版返回提示消息
        /// </summary>
        /// <param name="saucenaoResult"></param>
        /// <param name="template"></param>
        /// <param name="todayLeft"></param>
        /// <returns></returns>
        private string GetTemplateRemindMessage(SaucenaoResult saucenaoResult, string template, long todayLeft)
        {
            template = template?.Trim()?.TrimLine();
            template = template.Replace("{MatchCount}", saucenaoResult.MatchCount.ToString());
            template = template.Replace("{TodayLeft}", todayLeft.ToString());
            template = template.Replace("{RevokeInterval}", BotConfig.SaucenaoConfig.RevokeInterval.ToString());
            template = template.Replace("{MemberCD}", BotConfig.SetuConfig.MemberCD.ToString());
            return template;
        }

        /// <summary>
        /// 获取今日剩余可用次数
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public int GetLeftToday(long groupId, long memberId)
        {
            if (BotConfig.SaucenaoConfig.MaxDaily == 0) return 0;
            int todayUseCount = requestRecordDao.GetUsedCountToday(groupId, memberId, CommandType.Saucenao);
            int leftToday = BotConfig.SaucenaoConfig.MaxDaily - todayUseCount - 1;
            return leftToday < 0 ? 0 : leftToday;
        }

        /// <summary>
        /// 过滤结果
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public List<SaucenaoItem> FilterItems(List<SaucenaoItem> itemList)
        {
            return itemList.Where(o => IsFilter(o) == false).ToList();
        }

        /// <summary>
        /// 是否过滤掉一个结果
        /// </summary>
        /// <param name="saucenaoItem"></param>
        /// <returns></returns>
        public bool IsFilter(SaucenaoItem saucenaoItem)
        {
            var pixivId = 0;
            var minSimilarity = BotConfig.SaucenaoConfig.MinSimilarity;
            if (saucenaoItem.Similarity < minSimilarity) return true;
            if (saucenaoItem.SourceType != SetuSourceType.Pixiv) return false;
            if (int.TryParse(saucenaoItem.SourceId, out pixivId) == false) return true;
            if (pixivId < 40000000) return true;
            return false;
        }

        /// <summary>
        /// 对搜索结果进行优先级排序
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public List<SaucenaoItem> SortItems(List<SaucenaoItem> itemList)
        {
            if (itemList.Count == 0) return new List<SaucenaoItem>();
            List<SaucenaoItem> sortList = new List<SaucenaoItem>();
            List<SaucenaoItem> selectList = itemList.OrderByDescending(o => o.Similarity).Take(20).ToList();
            sortList.AddRange(selectList.Where(o => o.SourceType == SetuSourceType.Pixiv && o.Similarity >= BotConfig.SaucenaoConfig.PixivPriority).ToList());
            sortList.AddRange(selectList.Where(o => o.Similarity >= 80).OrderByDescending(o => o.SourceType));
            sortList.AddRange(selectList.OrderByDescending(o => o.Similarity).ThenBy(o => o.SourceType));
            return sortList.Distinct().ToList();
        }

        /// <summary>
        /// 请求saucenao,获取返回结果
        /// </summary>
        /// <param name="imgHttpUrl"></param>
        /// <returns></returns>
        /// <exception cref="PixivException"></exception>
        private async static Task<string> RequestHtmlAsync(string imgHttpUrl)
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>() { { "url", imgHttpUrl } };
            Dictionary<string, string> headerDic = GetHeader();
            HttpResponseMessage response = await HttpHelper.PostFormForHtml(HttpUrl.SaucenaoUrl, paramDic, headerDic);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string contentString = await response.GetContentStringAsync();
                throw new PixivException($"saucenao返回Code：{(int)response.StatusCode}，Content：{contentString.CutString(500)}");
            }
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 获取Header
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> GetHeader()
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            string cookie = WebsiteDatas.Saucenao.Cookie;
            if (string.IsNullOrWhiteSpace(cookie) == false) headerDic.Add("Cookie", cookie);
            return headerDic;
        }

    }
}
