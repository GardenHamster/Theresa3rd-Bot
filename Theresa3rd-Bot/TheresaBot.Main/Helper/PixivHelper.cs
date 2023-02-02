using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using TheresaBot.Main.Common;
using TheresaBot.Main.Exceptions;
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

        public static async Task<PixivResult<PixivSearch>> GetPixivSearchAsync(string keyword, int pageNo, bool isMatchAll, bool includeR18)
        {
            string operation = $"获取标签:{keyword}作品";
            string referer = HttpUrl.getPixivSearchReferer();
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivSearchUrl(keyword, pageNo, isMatchAll, includeR18);
            return await GetPixivResultAsync<PixivSearch>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivResult<PixivWorkInfo>> GetPixivWorkInfoAsync(string workId, int? retryTimes = null)
        {
            string operation = $"获取pid:{workId}信息";
            if (retryTimes is null) retryTimes = BotConfig.PixivConfig.ErrRetryTimes;
            string referer = HttpUrl.getPixivArtworksReferer(workId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivWorkInfoUrl(workId);
            return await GetPixivResultAsync<PixivWorkInfo>(postUrl, operation, headerDic, retryTimes.Value);
        }

        public static async Task<PixivResult<PixivUserInfo>> GetPixivUserInfoAsync(string userId)
        {
            string operation = $"获取uid信息:{userId}";
            string referer = HttpUrl.getPixivUserWorkInfoReferer(userId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserWorkInfoUrl(userId);
            return await GetPixivResultAsync<PixivUserInfo>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivResult<PixivUgoiraMeta>> GetPixivUgoiraMetaAsync(string workId)
        {
            string operation = $"获取动图pid信息:{workId}";
            string referer = HttpUrl.getPixivArtworksReferer(workId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUgoiraMetaUrl(workId);
            return await GetPixivResultAsync<PixivUgoiraMeta>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivResult<PixivFollow>> GetPixivFollowAsync(long loginId, int offset, int limit)
        {
            string operation = "获取关注列表";
            string referer = HttpUrl.getPixivFollowReferer(loginId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivFollowUrl(loginId, offset, limit);
            return await GetPixivResultAsync<PixivFollow>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivResult<PixivBookmarks>> GetPixivBookmarkAsync(long loginId, int offset, int limit)
        {
            string operation = "获取收藏列表";
            string referer = HttpUrl.getPixivBookmarkReferer(loginId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivBookmarkUrl(loginId, offset, limit);
            return await GetPixivResultAsync<PixivBookmarks>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        public static async Task<PixivResult<PixivFollowLatest>> GetPixivFollowLatestAsync(int page)
        {
            string operation = "获取关注画师作品信息";
            string referer = HttpUrl.getPixivFollowLatestReferer();
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivFollowLatestUrl(page);
            return await GetPixivResultAsync<PixivFollowLatest>(postUrl, operation, headerDic, BotConfig.PixivConfig.ErrRetryTimes);
        }

        private static async Task<PixivResult<T>> GetPixivResultAsync<T>(string url, string operation, Dictionary<string, string> headerDic = null, int retryTimes = 0, int timeout = 60000)
        {
            if (retryTimes < 0) retryTimes = 0;
            while (retryTimes >= 0)
            {
                try
                {
                    string json = await GetPixivJsonAsync(url, headerDic, retryTimes, timeout);
                    json = json.Replace("[]", "null");
                    PixivResult<T> jsonDto = JsonConvert.DeserializeObject<PixivResult<T>>(json);
                    if (jsonDto.error) throw new ApiException($"{operation}失败，pixiv api error，api message={jsonDto.message}");
                    return jsonDto;
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

        private static async Task<string> GetPixivJsonAsync(string url, Dictionary<string, string> headerDic = null, int retryTimes = 0, int timeout = 60000)
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

        private static Dictionary<string, string> GetPixivHeader(string referer)
        {
            Dictionary<string, string> headerDic = new Dictionary<string, string>();
            headerDic.Add("cookie", BotConfig.WebsiteConfig.Pixiv.Cookie);
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
            HttpClient client = GetHttpClient();
            client.BaseAddress = new Uri(url);
            client.addHeaders(headerDic);
            client.DefaultRequestHeaders.Add("User-Agent", HttpHelper.GetRandomUserAgent());
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            if (BotConfig.PixivConfig.FreeProxy) url = url.ToHttpUrl();
            HttpResponseMessage response = await client.GetAsync(url);
            //response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 获取一个pixiv免代理的HttpClient
        /// </summary>
        /// <returns></returns>
        private static HttpClient GetHttpClient()
        {
            HttpClient httpClient = PixivHttpClientFactory.CreateClient(Pixiv_Client_Name);
            //httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
            httpClient.DefaultRequestVersion = HttpVersion.Version30;
            //httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            //httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
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
        /// 转换为代理地址
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public static string ToPximgUrl(this string imgUrl)
        {
            imgUrl = imgUrl.Replace(HttpUrl.DefaultPixivImgProxy, "https://i.pximg.net");
            imgUrl = imgUrl.Replace("https://i.pixiv.cat", "https://i.pximg.net");
            imgUrl = imgUrl.Replace("https://i.pixiv.re", "https://i.pximg.net");
            imgUrl = imgUrl.Replace("https://i.pixiv.nl", "https://i.pximg.net");
            return imgUrl;
        }

        /// <summary>
        /// 转换为pixiv原地址
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public static string ToProxyUrl(this string imgUrl)
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
        /// 转换为pixiv原地址
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public static string ToOrginProxyUrl(this string imgUrl)
        {
            string proxyUrl = BotConfig.PixivConfig.OriginUrlProxy;
            if (string.IsNullOrWhiteSpace(proxyUrl)) proxyUrl = HttpUrl.DefaultPixivImgProxy;
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
        public static string ToThumbUrl(this string originalUrl)
        {
            PixivWorkPath workPath = originalUrl.getWorkPath();
            return $"{workPath.Host}/c/240x240/img-master/{workPath.ImgPath}_master1200.jpg";
        }

        /// <summary>
        /// 将original地址转换为Small格式的地址
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        public static string ToSmallUrl(this string originalUrl)
        {
            PixivWorkPath workPath = originalUrl.getWorkPath();
            return $"{workPath.Host}/c/540x540_70/img-master/{workPath.ImgPath}_master1200.jpg";
        }

        /// <summary>
        /// 将original地址转换为Regular格式的地址
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        public static string ToRegularUrl(this string originalUrl)
        {
            PixivWorkPath workPath = originalUrl.getWorkPath();
            return $"{workPath.Host}/img-master/{workPath.ImgPath}_master1200.jpg";
        }

        /// <summary>
        /// 拆解originalUrl,返回host和文件目录等信息
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        public static PixivWorkPath getWorkPath(this string originalUrl)
        {
            originalUrl = originalUrl.Trim();
            string[] arr = originalUrl.Split("/img-original/", StringSplitOptions.RemoveEmptyEntries);
            string[] arr2 = arr[1].Split('.', StringSplitOptions.RemoveEmptyEntries);
            return new PixivWorkPath(arr[0], arr2[0], arr2[1]);
        }


    }
}
