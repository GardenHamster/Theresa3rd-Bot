using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.Saucenao;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class SaucenaoBusiness
    {

        public async Task<SaucenaoSearch> getSearchResultAsync(string imgHttpUrl)
        {
            DateTime startTime = DateTime.Now;
            string saucenaoHtml = await SearchResultAsync(imgHttpUrl);
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument document = await htmlParser.ParseDocumentAsync(saucenaoHtml);
            IEnumerable<IElement> domList = document.All.Where(m => m.ClassList.Contains("result"));
            List<SaucenaoSearch> similarSearchList = getSaucenaoSearchList(domList, startTime);
            if (similarSearchList == null || similarSearchList.Count == 0) return null;
            for (int i = 0; i < similarSearchList.Count; i++)
            {
                try
                {
                    //作品id可能已经不存在,通过pixiv接口获取信息时可能会报错,所以要catch
                    if (similarSearchList[i].SourceType == SaucenaoSourceType.Pixiv)
                    {
                        PixivWorkInfoDto pixivWorkInfoDto = await PixivHelper.GetPixivWorkInfoAsync(similarSearchList[i].SourceId);
                        if (pixivWorkInfoDto == null || pixivWorkInfoDto.error == true) continue;
                        similarSearchList[i].pixivWorkInfoDto = pixivWorkInfoDto;
                        return similarSearchList[i];
                    }
                    if (similarSearchList[i].SourceType == SaucenaoSourceType.Twitter)
                    {
                        return similarSearchList[i];
                    }
                    if (similarSearchList[i].SourceType == SaucenaoSourceType.FanBox)
                    {
                        return similarSearchList[i];
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex,$"获取原图时出现异常，作品id:{similarSearchList[i].SourceId}可能不存在");
                }
            }
            return null;
        }

        

        public List<SaucenaoSearch> getSaucenaoSearchList(IEnumerable<IElement> domList, DateTime startDateTime)
        {
            List<SaucenaoSearch> searchList = new List<SaucenaoSearch>();
            if (domList == null || domList.Count() == 0) return searchList;
            foreach (IElement resultElement in domList)
            {
                SaucenaoSourceType sourceType = getSourceType(resultElement);
                if (sourceType == SaucenaoSourceType.Other) continue;

                IHtmlCollection<IElement> similarityList = resultElement.GetElementsByClassName("resultsimilarityinfo");
                string similarity = similarityList != null && similarityList.Length > 0 ? similarityList.First().InnerHtml : "??.??%";
                IHtmlCollection<IElement> resulttableList = resultElement.GetElementsByClassName("resulttablecontent");
                if (resulttableList == null || resulttableList.Length == 0) continue;
                IHtmlCollection<IElement> linkifyList = resulttableList[0].GetElementsByTagName("a");
                if (linkifyList == null || linkifyList.Length == 0) continue;

                if (sourceType == SaucenaoSourceType.Pixiv)
                {
                    string sourceId = getPixivWorkId(linkifyList);
                    if (string.IsNullOrEmpty(sourceId)) continue;
                    string sourceUrl = getPixivWorkUrl(linkifyList);
                    if (string.IsNullOrEmpty(sourceUrl)) sourceUrl = "";
                    searchList.Add(new SaucenaoSearch(SaucenaoSourceType.Pixiv, startDateTime, sourceUrl, sourceId, similarity, domList.Count()));
                }

                if (sourceType == SaucenaoSourceType.Twitter)
                {
                    string sourceUrl = getTwitterWorkUrl(linkifyList);
                    if (string.IsNullOrEmpty(sourceUrl)) sourceUrl = "";
                    searchList.Add(new SaucenaoSearch(SaucenaoSourceType.Twitter, startDateTime, sourceUrl, "", similarity, domList.Count()));
                }

                if (sourceType == SaucenaoSourceType.FanBox)
                {
                    string sourceUrl = getFanBoxWorkUrl(linkifyList);
                    if (string.IsNullOrEmpty(sourceUrl)) sourceUrl = "";
                    searchList.Add(new SaucenaoSearch(SaucenaoSourceType.FanBox, startDateTime, sourceUrl, "", similarity, domList.Count()));
                }
            }
            return searchList;
        }

        /// <summary>
        /// 提取pixiv作品id
        /// </summary>
        /// <param name="linkifyList"></param>
        /// <returns></returns>
        public string getPixivWorkId(IHtmlCollection<IElement> linkifyList)
        {
            foreach (var item in linkifyList)
            {
                string href = item.GetAttribute("href");
                if (string.IsNullOrEmpty(href)) continue;
                Dictionary<string, string> paramDic = StringHelper.splitHttpUrl(href);
                if (paramDic.ContainsKey("illust_id")) return paramDic["illust_id"];
            }
            return null;
        }

        /// <summary>
        /// 提取pixiv作品连接
        /// </summary>
        /// <param name="linkifyList"></param>
        /// <returns></returns>
        public string getPixivWorkUrl(IHtmlCollection<IElement> linkifyList)
        {
            foreach (var item in linkifyList)
            {
                string href = item.GetAttribute("href");
                if (string.IsNullOrEmpty(href)) continue;
                if (href.Contains("illust_id")) return href.Trim();
            }
            return null;
        }

        /// <summary>
        /// 提取pixiv作品连接
        /// </summary>
        /// <param name="linkifyList"></param>
        /// <returns></returns>
        public string getTwitterWorkUrl(IHtmlCollection<IElement> linkifyList)
        {
            foreach (var item in linkifyList)
            {
                string href = item.GetAttribute("href");
                if (string.IsNullOrEmpty(href)) continue;
                return href.Trim();
            }
            return null;
        }

        /// <summary>
        /// 提取pixiv作品连接
        /// </summary>
        /// <param name="linkifyList"></param>
        /// <returns></returns>
        public string getFanBoxWorkUrl(IHtmlCollection<IElement> linkifyList)
        {
            foreach (var item in linkifyList)
            {
                string href = item.GetAttribute("href");
                if (string.IsNullOrEmpty(href)) continue;
                return href.Trim();
            }
            return null;
        }

        /// <summary>
        /// 提取原图类型
        /// </summary>
        /// <param name="resultElement"></param>
        /// <returns></returns>
        public SaucenaoSourceType getSourceType(IElement resultElement)
        {
            IHtmlCollection<IElement> contentList = resultElement.GetElementsByClassName("resultcontentcolumn");
            if (contentList == null || contentList.Length == 0) return SaucenaoSourceType.Other;
            IElement contentElement = contentList[0];

            IHtmlCollection<IElement> strongList = contentElement.GetElementsByTagName("strong");
            if (strongList != null || strongList.Length > 0)
            {
                foreach (IElement strongElement in strongList)
                {
                    string innerHtml = strongElement.InnerHtml == null ? "" : strongElement.InnerHtml.ToLower().Trim();
                    if (innerHtml.StartsWith("pixiv")) return SaucenaoSourceType.Pixiv;
                    if (innerHtml.StartsWith("tweet") || innerHtml.StartsWith("twitter")) return SaucenaoSourceType.Twitter;
                }
            }

            IHtmlCollection<IElement> aList = contentElement.GetElementsByTagName("a");
            if (aList != null || aList.Length > 0)
            {
                foreach (IElement aElement in aList)
                {
                    string innerHtml = aElement.InnerHtml == null ? "" : aElement.InnerHtml.ToLower().Trim();
                    if (innerHtml.StartsWith("pixiv")) return SaucenaoSourceType.Pixiv;
                    if (innerHtml.StartsWith("tweet") || innerHtml.StartsWith("twitter")) return SaucenaoSourceType.Twitter;
                    if (innerHtml.Contains(".fanbox")) return SaucenaoSourceType.FanBox;
                }
            }

            return SaucenaoSourceType.Other;
        }


        public async static Task<string> SearchResultAsync(string imgHttpUrl)
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>()
            {
                {"url",imgHttpUrl }
            };
            return await HttpHelper.PostFormAsync(HttpUrl.SaucenaoUrl, paramDic);
        }


    }
}
