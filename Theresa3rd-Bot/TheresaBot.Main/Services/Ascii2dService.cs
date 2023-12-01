using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Net;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Ascii2d;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Services
{
    internal class Ascii2dService
    {
        /// <summary>
        /// 从Ascii2d中搜索结果
        /// </summary>
        /// <param name="imgHttpUrl"></param>
        /// <returns></returns>
        public async Task<Ascii2dResult> SearchResultAsync(string imgHttpUrl)
        {
            DateTime startTime = DateTime.Now;
            string colorUrl = await ToColorUrlAsync(imgHttpUrl);
            string bovwUrl = colorUrl.Replace("/color/", "/bovw/");
            var headerDic = new Dictionary<string, string>() { { "User-Agent", "Mozilla/5.0" } };
            string ascii2dHtml = await HttpHelper.GetHtmlAsync(bovwUrl, headerDic);
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument document = await htmlParser.ParseDocumentAsync(ascii2dHtml);
            IEnumerable<IElement> domList = document.All.Where(m => m.ClassList.Contains("detail-box"));

            List<Ascii2dItem> itemList = new List<Ascii2dItem>();
            if (domList is null || domList.Count() == 0) return new Ascii2dResult(itemList, startTime, 0);
            foreach (IElement resultElement in domList)
            {
                IHtmlCollection<IElement> linkList = resultElement.GetElementsByTagName("a");
                if (linkList is null || linkList.Length == 0) continue;
                foreach (IElement linkElement in linkList)
                {
                    Ascii2dItem saucenaoItem = ToAscii2dItem(linkElement);
                    if (saucenaoItem is null) continue;
                    if (itemList.Any(o => o.SourceUrl == saucenaoItem.SourceUrl)) continue;
                    itemList.Add(saucenaoItem);
                }
            }
            return new Ascii2dResult(itemList, startTime, domList.Count());
        }

        /// <summary>
        /// 封装成为Ascii2d数据对象
        /// </summary>
        /// <param name="linkElement"></param>
        /// <returns></returns>
        private Ascii2dItem ToAscii2dItem(IElement linkElement)
        {
            string href = linkElement.GetAttribute("href")?.Trim();
            if (string.IsNullOrWhiteSpace(href)) return null;
            string hrefLower = href.ToLower();

            //https://www.pixiv.net/artworks/100378274
            if (hrefLower.Contains("www.pixiv.net/artworks"))
            {
                return new Ascii2dItem(SetuSourceType.Pixiv, href, href.TakeHttpLast());
            }
            //https://twitter.com/1_tri_pic/status/1560897111624802304
            if (hrefLower.Contains("twitter.com") && hrefLower.Contains("/status/"))
            {
                return new Ascii2dItem(SetuSourceType.Twitter, href, href.TakeHttpLast());
            }

            return null;
        }

        /// <summary>
        /// 拉取源信息
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public async Task FetchOrigin(List<Ascii2dItem> itemList)
        {
            foreach (var item in itemList)
            {
                await FetchOrigin(item);
            }
        }

        /// <summary>
        /// 拉取源信息
        /// </summary>
        /// <param name="ascii2dItem"></param>
        /// <returns></returns>
        public async Task FetchOrigin(Ascii2dItem ascii2dItem)
        {
            try
            {
                if (ascii2dItem.SourceType == SetuSourceType.Pixiv)
                {
                    ascii2dItem.PixivWorkInfo = await PixivHelper.GetPixivWorkInfoAsync(ascii2dItem.SourceId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "获取Ascii2d返回的Pixiv作品信息失败");
            }
        }

        /// <summary>
        /// 请求ascii2d,获取返回结果
        /// </summary>
        /// <param name="imgHttpUrl"></param>
        /// <returns></returns>
        /// <exception cref="PixivException"></exception>
        private async static Task<string> ToColorUrlAsync(string imgHttpUrl)
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>() { { "uri", imgHttpUrl } };
            Dictionary<string, string> headerDic = new Dictionary<string, string>() { { "User-Agent", "Mozilla/5.0" } };
            string ascii2dUrl = BotConfig.SaucenaoConfig.Ascii2dWithIp ? HttpUrl.Ascii2dIpUrl : HttpUrl.Ascii2dDomainUrl;
            HttpResponseMessage response = await HttpHelper.PostFormForHtml(ascii2dUrl, paramDic, headerDic);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Redirect)
            {
                string contentString = await response.GetContentStringAsync();
                Exception innerException = new Exception(contentString);
                throw new PixivException(innerException, $"ascii2d返回StatusCode：{(int)response.StatusCode}");
            }
            return response.RequestMessage.RequestUri.AbsoluteUri;
        }

    }
}
