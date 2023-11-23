using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Model.Infos;
using TheresaBot.Main.Model.Pixiv;

namespace TheresaBot.Main.Helper
{
    public static class PixivHelper
    {
        private static readonly string Pixiv_DNS_AND_SNI = "www.pixivision.net";

        private static readonly string Pixiv_Client_Name = "PixivClient";

        private static readonly IHttpClientFactory PixivHttpClientFactory;

        static PixivHelper()
        {
            IServiceCollection pixivServiceCollection = new ServiceCollection();
            pixivServiceCollection.AddHttpClient(Pixiv_Client_Name).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler()
                {
                    ConnectCallback = (info, token) =>
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        socket.Connect(Pixiv_DNS_AND_SNI, 443);
                        var stream = new NetworkStream(socket, true);
                        SslStream sslstream = new SslStream(stream, false);
                        sslstream.AuthenticateAsClient(new SslClientAuthenticationOptions
                        {
                            TargetHost = Pixiv_DNS_AND_SNI,
                            ApplicationProtocols = new List<SslApplicationProtocol>(new SslApplicationProtocol[] { SslApplicationProtocol.Http3 })
                        });
                        return new ValueTask<Stream>(sslstream);
                    }
                };
            });
            PixivHttpClientFactory = pixivServiceCollection.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        }

        public static async Task<PixivSearch> GetPixivSearchAsync(string keyword, int pageNo, bool isMatchAll, bool includeR18)
        {
            string operation = $"获取标签:{keyword}作品";
            string referer = HttpUrl.getPixivSearchReferer();
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivSearchUrl(keyword, pageNo, isMatchAll, includeR18);
            return await GetPixivResultAsync<PixivSearch>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivWorkInfo> GetPixivWorkInfoAsync(string workId, int? retryTimes = null)
        {
            string operation = $"获取pid:{workId}信息";
            if (retryTimes is null) retryTimes = BotConfig.PixivConfig.ErrRetryTimes;
            string referer = HttpUrl.getPixivArtworksReferer(workId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivWorkInfoUrl(workId);
            return await GetPixivResultAsync<PixivWorkInfo>(postUrl, operation, headerDic, retryTimes.Value);
        }

        public static async Task<PixivUserProfileIllusts> GetPixivUserProfileIllustsAsync(string userid, List<int> workIds, bool isFirstPage)
        {
            string operation = $"获取画师:{userid}作品信息";
            int retryTimes = BotConfig.PixivConfig.ErrRetryTimes;
            string referer = HttpUrl.getPixivUserReferer(userid);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.GetPixivUserProfileIllustsAsync(userid, workIds, isFirstPage);
            return await GetPixivResultAsync<PixivUserProfileIllusts>(postUrl, operation, headerDic, retryTimes);
        }

        public static async Task<PixivUserProfileAll> GetPixivUserProfileAllAsync(string userId)
        {
            string operation = $"获取画师作品:{userId}";
            string referer = HttpUrl.getPixivUserReferer(userId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserProfileAllUrl(userId);
            return await GetPixivResultAsync<PixivUserProfileAll>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivUserProfileTop> GetPixivUserProfileTopAsync(string userId)
        {
            string operation = $"获取画师信息:{userId}";
            string referer = HttpUrl.getPixivUserReferer(userId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserProfileTopUrl(userId);
            return await GetPixivResultAsync<PixivUserProfileTop>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivUgoiraMeta> GetPixivUgoiraMetaAsync(string workId)
        {
            string operation = $"获取动图pid信息:{workId}";
            string referer = HttpUrl.getPixivArtworksReferer(workId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUgoiraMetaUrl(workId);
            return await GetPixivResultAsync<PixivUgoiraMeta>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivFollow> GetPixivFollowAsync(long loginId, int offset, int limit)
        {
            string operation = "获取关注列表";
            string referer = HttpUrl.getPixivFollowReferer(loginId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivFollowUrl(loginId, offset, limit);
            return await GetPixivResultAsync<PixivFollow>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivBookmarks> GetPixivBookmarkAsync(long loginId, int offset, int limit)
        {
            string operation = "获取收藏列表";
            string referer = HttpUrl.getPixivBookmarkReferer(loginId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivBookmarkUrl(loginId, offset, limit);
            return await GetPixivResultAsync<PixivBookmarks>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivFollowLatest> GetPixivFollowLatestAsync(int page)
        {
            string operation = "获取关注画师作品信息";
            string referer = HttpUrl.getPixivFollowLatestReferer();
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivFollowLatestUrl(page);
            return await GetPixivResultAsync<PixivFollowLatest>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivRankingData> GetPixivRankingData(string mode, int page, string date = "")
        {
            string operation = "获取排行信息";
            string referer = HttpUrl.getPixivReferer();
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivRankingUrl(mode, page, date);
            return await GetPixivRankingAsync<PixivRankingData>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<FileInfo> DownPixivImgAsync(string downloadUrl, string pixivIdStr, string fullFileName = null)
        {
            int pixivId = Convert.ToInt32(pixivIdStr);
            string referer = HttpUrl.getPixivArtworksReferer(pixivIdStr);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            return await DownPixivImgAsync(downloadUrl, pixivId, headerDic, fullFileName, BotConfig.PixivConfig.ImgRetryTimes);
        }

        public static async Task<FileInfo> DownPixivImgBySizeAsync(string pixivIdStr, string originUrl)
        {
            int pixivId = Convert.ToInt32(pixivIdStr);
            string downloadUrl = GetImgUrlBySize(originUrl);
            string fullFileName = GetImgNameBySize(originUrl);
            string referer = HttpUrl.getPixivArtworksReferer(pixivIdStr);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            return await DownPixivImgAsync(downloadUrl, pixivId, headerDic, fullFileName, BotConfig.PixivConfig.ImgRetryTimes);
        }

        public static async Task<FileInfo> DownPixivFileAsync(string pixivId, string downloadUrl, string fullFileName = null)
        {
            string referer = HttpUrl.getPixivArtworksReferer(pixivId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            return await DownPixivFileAsync(downloadUrl, headerDic, fullFileName, BotConfig.PixivConfig.ImgRetryTimes);
        }

        private static async Task<T> GetPixivResultAsync<T>(string url, string operation, Dictionary<string, string> headerDic = null, int retryTimes = 0, int timeout = 60000) where T : class
        {
            if (retryTimes < 0) retryTimes = 0;
            while (retryTimes >= 0)
            {
                try
                {
                    string json = await GetPixivJsonAsync(url, headerDic, timeout);
                    json = json.Replace("[]", "null");
                    PixivResult<T> jsonDto = JsonConvert.DeserializeObject<PixivResult<T>>(json);
                    if (jsonDto.error) throw new ApiException($"{operation}失败，pixiv api error，api message={jsonDto.message}");
                    return jsonDto.body;
                }
                catch (ApiException)
                {
                    if (--retryTimes < 0) throw;
                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    if (--retryTimes < 0) throw new PixivException(ex, $"{operation}失败");
                    await Task.Delay(2000);
                }
            }
            return null;
        }

        private static async Task<T> GetPixivRankingAsync<T>(string url, string operation, Dictionary<string, string> headerDic = null, int retryTimes = 0, int timeout = 60000)
        {
            if (retryTimes < 0) retryTimes = 0;
            while (retryTimes >= 0)
            {
                try
                {
                    string json = await GetPixivJsonAsync(url, headerDic, timeout);
                    json = json.Replace("[]", "null");
                    string error = json?.CheckPixivRankingError();
                    if (string.IsNullOrEmpty(error) == false) throw new NoRankingException(error);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (NoRankingException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (--retryTimes < 0) throw new PixivException(ex, $"{operation}失败");
                    await Task.Delay(2000);
                }
            }
            return default(T);
        }

        private static string CheckPixivRankingError(this string jsonStr)
        {
            try
            {
                JObject jo = JObject.Parse(jsonStr);
                if (jo.ContainsKey("error")) return jo["error"]?.ToString() ?? string.Empty;
                return string.Empty;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return string.Empty;
            }
        }

        private static async Task<string> GetPixivJsonAsync(string url, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            if (BotConfig.PixivConfig.FreeProxy)
            {
                return await PixivHelper.GetAsync(url, headerDic, timeout);
            }
            else if (string.IsNullOrWhiteSpace(BotConfig.PixivConfig.HttpProxy) == false)
            {
                return await HttpHelper.GetWithProxyAsync(url, headerDic, timeout);
            }
            else
            {
                return await HttpHelper.GetAsync(url, headerDic, timeout);
            }
        }

        private static async Task<FileInfo> DownPixivImgAsync(string url, int pixivId, Dictionary<string, string> headerDic = null, string fullFileName = null, int retryTimes = 0, int timeout = 60000)
        {
            if (retryTimes < 0) retryTimes = 0;
            while (retryTimes >= 0)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(fullFileName)) fullFileName = new HttpFileInfo(url).FullFileName;
                    string fullImgSavePath = Path.Combine(FilePath.GetPixivImgDirectory(pixivId), fullFileName);
                    if (File.Exists(fullImgSavePath)) return new FileInfo(fullImgSavePath);
                    return await DownPixivImgAsync(url, fullImgSavePath, headerDic, timeout);
                }
                catch (Exception ex)
                {
                    if (--retryTimes < 0)
                    {
                        LogHelper.Error(ex, $"DownPixivImgAsync异常，url={url}");
                        return null;
                    }
                    await Task.Delay(3000);
                }
            }
            return null;
        }

        private static async Task<FileInfo> DownPixivImgAsync(string url, string fullISavePath, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            if (BotConfig.PixivConfig.FreeProxy)
            {
                return await HttpHelper.DownFileAsync(url.ToDownloadProxyUrl(), fullISavePath, null, timeout);
            }
            else if (string.IsNullOrWhiteSpace(BotConfig.PixivConfig.ImgProxy) == false)
            {
                return await HttpHelper.DownFileAsync(url.ToDownloadProxyUrl(), fullISavePath, null, timeout);
            }
            else if (string.IsNullOrWhiteSpace(BotConfig.PixivConfig.HttpProxy) == false)
            {
                return await HttpHelper.DownFileWithProxyAsync(url.ToPixivOriginUrl(), fullISavePath, headerDic);
            }
            else
            {
                return await HttpHelper.DownFileAsync(url.ToPixivOriginUrl(), fullISavePath, headerDic);
            }
        }

        private static async Task<FileInfo> DownPixivFileAsync(string url, Dictionary<string, string> headerDic = null, string fullFileName = null, int retryTimes = 0, int timeout = 60000)
        {
            if (retryTimes < 0) retryTimes = 0;
            while (retryTimes >= 0)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(fullFileName)) fullFileName = new HttpFileInfo(url).FullFileName;
                    string fullFileSavePath = FilePath.GetTempFileSavePath(fullFileName);
                    return await DownPixivFileAsync(url, fullFileSavePath, headerDic, timeout);
                }
                catch (Exception ex)
                {
                    if (--retryTimes < 0)
                    {
                        LogHelper.Error(ex, $"DownPixivFileAsync异常，url={url}");
                        return null;
                    }
                    await Task.Delay(3000);
                }
            }
            return null;
        }

        private static async Task<FileInfo> DownPixivFileAsync(string url, string fullImgSavePath, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            if (BotConfig.PixivConfig.FreeProxy)
            {
                return await HttpHelper.DownFileAsync(url.ToDownloadProxyUrl(), fullImgSavePath);
            }
            else if (string.IsNullOrWhiteSpace(BotConfig.PixivConfig.ImgProxy) == false)
            {
                return await HttpHelper.DownFileAsync(url.ToDownloadProxyUrl(), fullImgSavePath);
            }
            else if (string.IsNullOrWhiteSpace(BotConfig.PixivConfig.HttpProxy) == false)
            {
                return await HttpHelper.DownFileWithProxyAsync(url.ToPixivOriginUrl(), fullImgSavePath, headerDic);
            }
            else
            {
                return await HttpHelper.DownFileAsync(url.ToPixivOriginUrl(), fullImgSavePath, headerDic);
            }
        }

        private static Dictionary<string, string> GetPixivHeader(string referer)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            headerDic.Add("cookie", WebsiteDatas.Pixiv.Cookie);
            headerDic.Add("referer", referer);
            //headerDic.Add("accept", "application/json");
            //headerDic.Add("sec-fetch-mode", "cors");
            //headerDic.Add("sec-fetch-site", "same-origin");
            //headerDic.Add("x-user-id", Setting.Pixiv.XUserId);
            return headerDic;
        }

        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private static async Task<string> GetAsync(string url, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            using HttpClient client = GetHttpClient();
            client.BaseAddress = new Uri(url);
            client.AddHeaders(headerDic);
            client.DefaultRequestHeaders.Add("User-Agent", HttpHelper.GetRandomUserAgent());
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            if (BotConfig.PixivConfig.FreeProxy) url = url.ToHttpUrl();
            using var response = await client.GetAsync(url);
            using var content = response.Content;
            return await content.ReadAsStringAsync();
        }

        /// <summary>
        /// 获取一个pixiv免代理的HttpClient
        /// </summary>
        /// <returns></returns>
        private static HttpClient GetHttpClient()
        {
            HttpClient httpClient = PixivHttpClientFactory.CreateClient(Pixiv_Client_Name);
            httpClient.DefaultRequestVersion = HttpVersion.Version30;
            return httpClient;
        }

        /// <summary>
        /// 将一个Https地址转换为Http地址
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <returns></returns>
        private static string ToHttpUrl(this string httpUrl)
        {
            return httpUrl.Replace("https://", "http://");
        }

        /// <summary>
        /// 转换为Pixiv原图地址
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public static string ToPixivOriginUrl(this string imgUrl)
        {
            imgUrl = imgUrl.Replace(HttpUrl.DefaultPixivImgProxy, "https://i.pximg.net");
            imgUrl = imgUrl.Replace("https://i.pixiv.cat", "https://i.pximg.net");
            imgUrl = imgUrl.Replace("https://i.pixiv.re", "https://i.pximg.net");
            imgUrl = imgUrl.Replace("https://i.pixiv.nl", "https://i.pximg.net");
            return imgUrl;
        }

        /// <summary>
        /// 转换为图片代理下载地址
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public static string ToDownloadProxyUrl(this string imgUrl)
        {
            string proxyUrl = BotConfig.PixivConfig.ImgProxy;
            if (string.IsNullOrWhiteSpace(proxyUrl)) proxyUrl = HttpUrl.DefaultPixivImgProxy;
            imgUrl = imgUrl.Replace("https://i.pximg.net", proxyUrl);
            imgUrl = imgUrl.Replace("https://i.pixiv.cat", proxyUrl);
            imgUrl = imgUrl.Replace("https://i.pixiv.re", proxyUrl);
            imgUrl = imgUrl.Replace("https://i.pixiv.nl", proxyUrl);
            return imgUrl;
        }

        /// <summary>
        /// 转换为代理图片链接地址
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public static string ToOpenProxyLink(this string imgUrl)
        {
            string proxyUrl = BotConfig.PixivConfig.OriginUrlProxy;
            if (string.IsNullOrWhiteSpace(proxyUrl)) proxyUrl = "https://原图代理链接未配置";
            imgUrl = imgUrl.Replace("https://i.pximg.net", proxyUrl);
            imgUrl = imgUrl.Replace("https://i.pixiv.cat", proxyUrl);
            imgUrl = imgUrl.Replace("https://i.pixiv.re", proxyUrl);
            imgUrl = imgUrl.Replace("https://i.pixiv.nl", proxyUrl);
            return imgUrl;
        }

        /// <summary>
        /// 将original地址转换为Thumb格式的地址
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        public static string OriginToThumbUrl(this string originalUrl)
        {
            PixivWorkPath workPath = originalUrl.GetOriginalWorkPath();
            return $"{workPath.Host}/c/240x240/img-master/{workPath.ImgPath}_master1200.jpg";
        }

        /// <summary>
        /// 将original地址转换为Small格式的地址
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        public static string OriginToSmallUrl(this string originalUrl)
        {
            PixivWorkPath workPath = originalUrl.GetOriginalWorkPath();
            return $"{workPath.Host}/c/540x540_70/img-master/{workPath.ImgPath}_master1200.jpg";
        }

        /// <summary>
        /// 将Original地址转换为Regular格式的地址
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        public static string OriginToRegularUrl(this string originalUrl)
        {
            PixivWorkPath workPath = originalUrl.GetOriginalWorkPath();
            return $"{workPath.Host}/img-master/{workPath.ImgPath}_master1200.jpg";
        }


        /// <summary>
        /// 将thumb地址转换为Small格式的地址
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        public static string ThumbToSmallUrl(this string originalUrl)
        {
            PixivWorkPath workPath = originalUrl.GetThumbWorkPath();
            return $"{workPath.Host}/c/540x540_70/img-master/{workPath.ImgPath}_master1200.jpg";
        }


        /// <summary>
        /// 获取保存预览图的文件名
        /// </summary>
        /// <param name="downUrl"></param>
        /// <param name="pixivId"></param>
        /// <returns></returns>
        public static string GetPreviewImgSaveName(this string downUrl, string pixivId)
        {
            HttpFileInfo httpFileInfo = new HttpFileInfo(downUrl);
            return $"{pixivId}_p0_preview.{httpFileInfo.FileExtension}";
        }

        /// <summary>
        /// 根据配置文件设置的图片大小获取图片下载地址
        /// </summary>
        /// <returns></returns>
        private static string GetImgUrlBySize(string originalUrl)
        {
            string imgSize = BotConfig.PixivConfig.ImgSize?.ToLower();
            if (imgSize == "original") return originalUrl;
            if (imgSize == "regular") return originalUrl.OriginToRegularUrl();
            if (imgSize == "small") return originalUrl.OriginToSmallUrl();
            if (imgSize == "thumb") return originalUrl.OriginToThumbUrl();
            return originalUrl.OriginToThumbUrl();
        }

        /// <summary>
        /// 根据配置文件设置的图片大小获取图片下载地址
        /// </summary>
        /// <returns></returns>
        private static string GetImgNameBySize(string originalUrl)
        {
            string imgSize = BotConfig.PixivConfig.ImgSize?.ToLower();
            HttpFileInfo httpFileInfo = new HttpFileInfo(originalUrl);
            if (imgSize == "original") return httpFileInfo.FullFileName;
            if (imgSize == "regular") return $"{httpFileInfo.FileName}_regular.{httpFileInfo.FileExtension}";
            if (imgSize == "small") return $"{httpFileInfo.FileName}_small.{httpFileInfo.FileExtension}";
            if (imgSize == "thumb") return $"{httpFileInfo.FileName}_thumb.{httpFileInfo.FileExtension}";
            return $"{httpFileInfo.FileName}_thumb.{httpFileInfo.FileExtension}";
        }

        /// <summary>
        /// 拆解originalUrl,返回host和文件目录等信息
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        public static PixivWorkPath GetOriginalWorkPath(this string originalUrl)
        {
            originalUrl = originalUrl.Trim();
            string[] arr = originalUrl.Split("/img-original/", StringSplitOptions.RemoveEmptyEntries);
            string[] arr2 = arr[1].Split('.', StringSplitOptions.RemoveEmptyEntries);
            return new PixivWorkPath(arr[0], arr2[0], arr2[1]);
        }

        /// <summary>
        /// 拆解regularUrl,返回host和文件目录等信息
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        public static PixivWorkPath GetThumbWorkPath(this string thumbUrl)
        {
            thumbUrl = thumbUrl.Trim();
            string[] arr = thumbUrl.Split(new String[] { "/img-master/", "/custom-thumb/" }, StringSplitOptions.RemoveEmptyEntries);
            string[] arr2 = thumbUrl.Split("/c/", StringSplitOptions.RemoveEmptyEntries);
            string[] arr3 = arr[1].Split(new String[] { "_square", "_custom" }, StringSplitOptions.RemoveEmptyEntries);
            string[] arr4 = arr[1].Split('.', StringSplitOptions.RemoveEmptyEntries);
            return new PixivWorkPath(arr2[0], arr3[0], arr4[1]);
        }

    }
}
