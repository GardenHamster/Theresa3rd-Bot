using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Exceptions;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.Saucenao;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Business
{
    public class Ascii2dBusiness
    {
        public async Task<Ascii2dResult> getAscii2dResultAsync(string imgHttpUrl)
        {
            DateTime startTime = DateTime.Now;
            string colorUrl = await GetColorUrlAsync(imgHttpUrl);
            string bovwUrl = colorUrl.Replace("/color/", "/bovw/");
            string ascii2dHtml = await HttpHelper.GetHtmlAsync(bovwUrl);
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument document = await htmlParser.ParseDocumentAsync(ascii2dHtml);
            IEnumerable<IElement> domList = document.All.Where(m => m.ClassList.Contains("detail-box"));

            List<Ascii2dItem> itemList = new List<Ascii2dItem>();
            if (domList == null || domList.Count() == 0) return new Ascii2dResult(itemList, startTime, 0);
            foreach (IElement resultElement in domList)
            {
                IHtmlCollection<IElement> linkList = resultElement.GetElementsByTagName("a");
                if (linkList == null || linkList.Length == 0) continue;
                foreach (IElement linkElement in linkList)
                {
                    Ascii2dItem saucenaoItem = getAscii2dItem(linkElement);
                    if (saucenaoItem == null) continue;
                    if (itemList.Where(o => o.SourceUrl == saucenaoItem.SourceUrl).Any()) continue;
                    itemList.Add(saucenaoItem);
                }
            }
            return new Ascii2dResult(itemList, startTime, domList.Count());
        }


        public Ascii2dItem getAscii2dItem(IElement linkElement)
        {
            string href = linkElement.GetAttribute("href");
            if (string.IsNullOrWhiteSpace(href)) return null;

            href = href.Trim();
            string hrefLower = href.ToLower();

            //https://www.pixiv.net/artworks/100378274
            if (hrefLower.Contains("www.pixiv.net/artworks"))
            {
                string illustId = href.splitHttpUrl().LastOrDefault() ?? string.Empty;
                return new Ascii2dItem(SetuSourceType.Pixiv, href, illustId);
            }

            //https://twitter.com/1_tri_pic/status/1560897111624802304
            if (hrefLower.Contains("twitter.com") && hrefLower.Contains("/status/"))
            {
                string illustId = href.splitHttpUrl().LastOrDefault() ?? string.Empty;
                return new Ascii2dItem(SetuSourceType.Twitter, href, illustId);
            }
            return null;
        }

        public async Task<List<Ascii2dItem>> getBestMatchAsync(List<Ascii2dItem> itemList)
        {
            if (itemList == null || itemList.Count == 0) return null;
            List<Ascii2dItem> matchList = new List<Ascii2dItem>();
            for (int i = 0; i < itemList.Count; i++)
            {
                try
                {
                    Ascii2dItem ascii2dItem = itemList[i];
                    if (ascii2dItem.SourceType == SetuSourceType.Pixiv)
                    {
                        PixivWorkInfoDto pixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(ascii2dItem.SourceId);
                        if (pixivWorkInfo == null || pixivWorkInfo.error == true) continue;
                        ascii2dItem.PixivWorkInfo = pixivWorkInfo;
                    }
                    if (ascii2dItem.SourceType == SetuSourceType.Twitter)
                    {
                        //TODO
                    }
                    matchList.Add(ascii2dItem);
                }
                catch (Exception)
                {
                }
            }
            return matchList;
        }

        /// <summary>
        /// 请求ascii2d,获取返回结果
        /// </summary>
        /// <param name="imgHttpUrl"></param>
        /// <returns></returns>
        /// <exception cref="BaseException"></exception>
        private async static Task<string> GetColorUrlAsync(string imgHttpUrl)
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>() { { "uri", imgHttpUrl } };
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            HttpResponseMessage response = await HttpHelper.PostFormForHtml(HttpUrl.Ascii2dUrl, paramDic, headerDic);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string contentString = await response.GetContentStringAsync();
                throw new BaseException($"ascii2d返回Code：{(int)response.StatusCode}，Content：{contentString.cutString(500)}");
            }
            return response.RequestMessage.RequestUri.AbsoluteUri;
        }

        /// <summary>
        /// 请求saucenao,获取返回结果
        /// </summary>
        /// <param name="imgHttpUrl"></param>
        /// <returns></returns>
        /// <exception cref="BaseException"></exception>
        private async static Task<string> SearchResultAsync(string imgHttpUrl)
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>() { { "url", imgHttpUrl } };
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            HttpResponseMessage response = await HttpHelper.PostFormForHtml(HttpUrl.SaucenaoUrl, paramDic, headerDic);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string contentString = await response.GetContentStringAsync();
                throw new BaseException($"ascii2d返回Code：{(int)response.StatusCode}，Content：{contentString.cutString(500)}");
            }
            return await response.Content.ReadAsStringAsync();
        }



    }
}
