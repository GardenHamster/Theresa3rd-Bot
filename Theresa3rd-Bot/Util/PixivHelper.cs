using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Pixiv;

namespace Theresa3rd_Bot.Util
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

        //if ()
        /*-------------------------------------------------------------接口相关--------------------------------------------------------------------------*/

        public async static Task<PixivSearchDto> GetPixivSearchAsync(string keyword, int pageNo, bool isMatchAll, bool includeR18)
        {
            string referer = HttpUrl.getPixivSearchReferer();
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivSearchUrl(keyword, pageNo, isMatchAll, includeR18);
            string json = await GetAsync(postUrl, headerDic);
            return JsonConvert.DeserializeObject<PixivSearchDto>(json);
        }

        public async static Task<PixivWorkInfoDto> GetPixivWorkInfoAsync(string wordId)
        {
            string referer = HttpUrl.getPixivArtworksReferer(wordId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivWorkInfoUrl(wordId);
            string json = await GetAsync(postUrl, headerDic);
            return JsonConvert.DeserializeObject<PixivWorkInfoDto>(json);
        }

        public async static Task<PixivUserWorkInfoDto> GetPixivUserWorkInfoAsync(string userId)
        {
            string referer = HttpUrl.getPixivUserWorkInfoReferer(userId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserWorkInfoUrl(userId);
            string json = await GetAsync(postUrl, headerDic);
            if (string.IsNullOrEmpty(json) == false && json.Contains("\"illusts\":[]")) return null;
            return JsonConvert.DeserializeObject<PixivUserWorkInfoDto>(json);
        }

        public async static Task<PixivUserInfoDto> GetPixivUserInfoAsync(string userId)
        {
            string referer = HttpUrl.getPixivUserWorkInfoReferer(userId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUserWorkInfoUrl(userId);
            string json = await GetAsync(postUrl, headerDic);
            return JsonConvert.DeserializeObject<PixivUserInfoDto>(json);
        }

        public async static Task<PixivUgoiraMetaDto> GetPixivUgoiraMetaAsync(string wordId)
        {
            string referer = HttpUrl.getPixivArtworksReferer(wordId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivUgoiraMetaUrl(wordId);
            string json = await GetAsync(postUrl, headerDic);
            return JsonConvert.DeserializeObject<PixivUgoiraMetaDto>(json);
        }

        public async static Task<PixivFollowDto> GetPixivFollowAsync(long loginId, int offset, int limit)
        {
            string referer = HttpUrl.getPixivFollowReferer(loginId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivFollowUrl(loginId, offset, limit);
            string json = await GetAsync(postUrl, headerDic);
            return JsonConvert.DeserializeObject<PixivFollowDto>(json);
        }

        public async static Task<PixivBookmarksDto> GetPixivBookmarkAsync(long loginId, int offset, int limit)
        {
            string referer = HttpUrl.getPixivBookmarkReferer(loginId);
            Dictionary<string, string> headerDic = GetPixivHeader(referer);
            string postUrl = HttpUrl.getPixivBookmarkUrl(loginId, offset, limit);
            string json = await GetAsync(postUrl, headerDic);
            return JsonConvert.DeserializeObject<PixivBookmarksDto>(json);
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
            if (BotConfig.GeneralConfig.PixivFreeProxy)
            {
                HttpClient client = GetHttpClient();
                client.BaseAddress = new Uri(url);
                client.addHeaders(headerDic);
                client.DefaultRequestHeaders.Add("User-Agent", HttpHelper.GetRandomUserAgent());
                client.Timeout = TimeSpan.FromMilliseconds(timeout);
                if (BotConfig.GeneralConfig.PixivFreeProxy) url = url.ToHttpUrl();
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return await HttpHelper.GetAsync(url, headerDic);
            }
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


    }
}
