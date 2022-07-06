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
using Theresa3rd_Bot.Exceptions;
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

        public static async Task<PixivSearchDto> GetPixivSearchAsync(string keyword, int pageNo, bool isMatchAll, bool includeR18)
        {
            try
            {
                string referer = HttpUrl.getPixivSearchReferer();
                Dictionary<string, string> headerDic = GetPixivHeader(referer);
                string postUrl = HttpUrl.getPixivSearchUrl(keyword, pageNo, isMatchAll, includeR18);
                string json = await GetPixivAsync(postUrl, headerDic);
                json = json.Replace("[]", "null");
                return JsonConvert.DeserializeObject<PixivSearchDto>(json);
            }
            catch (Exception ex)
            {
                throw new BaseException(ex, $"搜索pixiv标签{keyword}失败");
            }
        }

        public static async Task<PixivWorkInfoDto> GetPixivWorkInfoAsync(string workId)
        {
            try
            {
                await Task.Delay(1000);
                string referer = HttpUrl.getPixivArtworksReferer(workId);
                Dictionary<string, string> headerDic = GetPixivHeader(referer);
                string postUrl = HttpUrl.getPixivWorkInfoUrl(workId);
                string json = await GetPixivAsync(postUrl, headerDic);
                json = json.Replace("[]", "null");
                return JsonConvert.DeserializeObject<PixivWorkInfoDto>(json);
            }
            catch (Exception ex)
            {
                throw new BaseException(ex, $"获取pixiv作品信息{workId}失败");
            }
        }

        public static async Task<PixivUserInfoDto> GetPixivUserInfoAsync(string userId)
        {
            try
            {
                await Task.Delay(1000);
                string referer = HttpUrl.getPixivUserWorkInfoReferer(userId);
                Dictionary<string, string> headerDic = GetPixivHeader(referer);
                string postUrl = HttpUrl.getPixivUserWorkInfoUrl(userId);
                string json = await GetPixivAsync(postUrl, headerDic);
                json = json.Replace("[]", "null");
                return JsonConvert.DeserializeObject<PixivUserInfoDto>(json);
            }
            catch (Exception ex)
            {
                throw new BaseException(ex, $"获取pixiv画师{userId}作品列表失败");
            }
        }

        public static async Task<PixivUgoiraMetaDto> GetPixivUgoiraMetaAsync(string workId)
        {
            try
            {
                await Task.Delay(1000);
                string referer = HttpUrl.getPixivArtworksReferer(workId);
                Dictionary<string, string> headerDic = GetPixivHeader(referer);
                string postUrl = HttpUrl.getPixivUgoiraMetaUrl(workId);
                string json = await GetPixivAsync(postUrl, headerDic);
                json = json.Replace("[]", "null");
                return JsonConvert.DeserializeObject<PixivUgoiraMetaDto>(json);
            }
            catch (Exception ex)
            {
                throw new BaseException(ex, $"获取pixiv动图{workId}失败");
            }
        }

        public static async Task<PixivFollowDto> GetPixivFollowAsync(long loginId, int offset, int limit)
        {
            try
            {
                await Task.Delay(1000);
                string referer = HttpUrl.getPixivFollowReferer(loginId);
                Dictionary<string, string> headerDic = GetPixivHeader(referer);
                string postUrl = HttpUrl.getPixivFollowUrl(loginId, offset, limit);
                string json = await GetPixivAsync(postUrl, headerDic);
                json = json.Replace("[]", "null");
                return JsonConvert.DeserializeObject<PixivFollowDto>(json);
            }
            catch (Exception ex)
            {
                throw new BaseException(ex, $"获取pixiv关注列表失败");
            }
        }

        public static async Task<PixivBookmarksDto> GetPixivBookmarkAsync(long loginId, int offset, int limit)
        {
            try
            {
                await Task.Delay(1000);
                string referer = HttpUrl.getPixivBookmarkReferer(loginId);
                Dictionary<string, string> headerDic = GetPixivHeader(referer);
                string postUrl = HttpUrl.getPixivBookmarkUrl(loginId, offset, limit);
                string json = await GetPixivAsync(postUrl, headerDic);
                json = json.Replace("[]", "null");
                return JsonConvert.DeserializeObject<PixivBookmarksDto>(json);
            }
            catch (Exception ex)
            {
                throw new BaseException(ex, $"获取pixiv收藏列表失败");
            }
        }

        public static async Task<PixivFollowLatestDto> GetPixivFollowLatestAsync(int page)
        {
            try
            {
                await Task.Delay(1000);
                string referer = HttpUrl.getPixivFollowLatestReferer();
                Dictionary<string, string> headerDic = GetPixivHeader(referer);
                string postUrl = HttpUrl.getPixivFollowLatestUrl(page);
                string json = await GetPixivAsync(postUrl, headerDic);
                json = json.Replace("[]", "null");
                return JsonConvert.DeserializeObject<PixivFollowLatestDto>(json);
            }
            catch (Exception ex)
            {
                throw new BaseException(ex, $"获取pixiv关注最新作品失败");
            }
        }

        public static async Task<string> GetPixivAsync(string url, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            if (BotConfig.GeneralConfig.PixivFreeProxy)
            {
                return await PixivHelper.GetAsync(url, headerDic, timeout);
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
            if (BotConfig.GeneralConfig.PixivFreeProxy) url = url.ToHttpUrl();
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


    }
}
