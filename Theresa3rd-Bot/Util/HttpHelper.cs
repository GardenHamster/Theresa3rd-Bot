using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;

namespace Theresa3rd_Bot.Util
{
    public static class HttpHelper
    {
        private static readonly string Pixiv_DNS_AND_SNI = "www.pixivision.net";

        private static readonly string Pixiv_Client_Name = "PixivClient";

        private static readonly IHttpClientFactory DefaultHttpClientFactory;

        private static readonly IHttpClientFactory PixivHttpClientFactory;

        public static readonly string[] UserAgent = new string[] {
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.163 Safari/535.1",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.103 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_0) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11",
            "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:6.0) Gecko/20100101 Firefox/6.0",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:2.0.1) Gecko/20100101 Firefox/4.0.1",
            "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10_6_8; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-us) AppleWebKit/534.50 (KHTML, like Gecko) Version/5.1 Safari/534.50",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36",
            "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.64 Safari/537.11",
            "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.133 Safari/534.16",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_0) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.56 Safari/535.11",
            "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:34.0) Gecko/20100101 Firefox/34.0",
            "Mozilla/5.0 (X11; U; Linux x86_64; zh-CN; rv:1.9.2.10) Gecko/20100922 Ubuntu/10.10 (maverick) Firefox/3.6.10",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv,2.0.1) Gecko/20100101 Firefox/4.0.1",
            "Mozilla/5.0 (Windows NT 6.1; rv,2.0.1) Gecko/20100101 Firefox/4.0.1"
        };

        static HttpHelper()
        {
            DefaultHttpClientFactory = new ServiceCollection().AddHttpClient().BuildServiceProvider().GetRequiredService<IHttpClientFactory>();

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
                            ApplicationProtocols = new List<SslApplicationProtocol>(new SslApplicationProtocol[] { SslApplicationProtocol.Http11 })
                        });
                        return new ValueTask<Stream>(sslstream);
                    }
                };
            });
            PixivHttpClientFactory = pixivServiceCollection.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        }


        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            HttpClient client = DefaultHttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(url);
            client.addHeaders(headerDic);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<string> PixivGetAsync(string url, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            HttpClient client = GetPixivHttpClient();
            client.BaseAddress = new Uri(url);
            client.addHeaders(headerDic);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// HttpPost
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramStr">参数或者json字符串</param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, string postJsonStr, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            HttpContent content = new StringContent(postJsonStr);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient client = DefaultHttpClientFactory.CreateClient();
            client.addHeaders(headerDic);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// post方式提交图片
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="imgPath"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostImageAsync(string postUrl, FileInfo imageFile, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            using FileStream fs = new FileStream(imageFile.FullName, FileMode.Open, FileAccess.Read);
            HttpClient client = DefaultHttpClientFactory.CreateClient();
            client.addHeaders(headerDic);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            MultipartFormDataContent formData = new MultipartFormDataContent();//表单
            StreamContent fileContent = new StreamContent(fs);//图片stream
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            fileContent.Headers.ContentDisposition.FileName = imageFile.Name;
            fileContent.Headers.ContentDisposition.Name = "file";
            formData.Add(fileContent);
            HttpResponseMessage res = client.PostAsync(postUrl, formData).Result;
            return await res.Content.ReadAsStringAsync();
        }


        /// <summary>
        /// 根据一个网址获取Html内容
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <returns></returns>
        public static async Task<string> GetHtmlAsync(string httpUrl, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            HttpClient client = DefaultHttpClientFactory.CreateClient();
            client.addHeaders(headerDic);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            return await client.GetStringAsync(httpUrl);
        }

        /// <summary>
        /// 下载图片,保存图片名为随机16位uuid
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public static async Task<FileInfo> DownImgAsync(string imgUrl, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            string suffix = StringHelper.getSuffixByUrl(imgUrl);
            if (string.IsNullOrEmpty(suffix)) suffix = "jpg";
            string fullFileName = StringHelper.get16UUID() + "." + suffix;
            string fullImageSavePath = Path.Combine(FilePath.getDownImgSavePath(), fullFileName);
            return await HttpHelper.DownFileAsync(imgUrl, fullImageSavePath, headerDic, timeout);
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="fullImageSavePath"></param>
        /// <param name="headerDic"></param>
        /// <returns></returns>
        public static async Task<FileInfo> DownFileAsync(string imgUrl, string fullImageSavePath, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            if (File.Exists(fullImageSavePath)) return new FileInfo(fullImageSavePath);
            HttpClient client = DefaultHttpClientFactory.CreateClient();
            client.addHeaders(headerDic);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            byte[] urlContents = await client.GetByteArrayAsync(new Uri(imgUrl));
            using FileStream fileStream = new FileStream(fullImageSavePath, FileMode.CreateNew);
            fileStream.Write(urlContents, 0, urlContents.Length);
            return new FileInfo(fullImageSavePath);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="fullImageSavePath"></param>
        /// <param name="headerDic"></param>
        /// <returns></returns>
        public static async Task<FileInfo> DownPixivFileAsync(string imgUrl, string fullImageSavePath, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            if (File.Exists(fullImageSavePath)) return new FileInfo(fullImageSavePath);
            HttpClient client = GetPixivHttpClient();
            client.addHeaders(headerDic);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            byte[] urlContents = await client.GetByteArrayAsync(new Uri(imgUrl));
            using FileStream fileStream = new FileStream(fullImageSavePath, FileMode.CreateNew);
            fileStream.Write(urlContents, 0, urlContents.Length);
            return new FileInfo(fullImageSavePath);
        }

        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headerDic"></param>
        /// <returns></returns>
        private static void addHeaders(this HttpClient client, Dictionary<string, string> headerDic)
        {
            if (headerDic == null) return;
            foreach (var item in headerDic)
            {
                client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 获取随机UserAgent
        /// </summary>
        /// <returns></returns>
        private static string GetRandomUserAgent()
        {
            int randomIndex = new Random(DateTime.Now.Millisecond).Next(0, UserAgent.Length);
            return UserAgent[randomIndex];
        }

        /// <summary>
        /// 获取一个pixiv免代理的HttpClient
        /// </summary>
        /// <returns></returns>
        private static HttpClient GetPixivHttpClient()
        {
            SocketsHttpHandler handler = new SocketsHttpHandler()
            {
                ConnectCallback = (info, token) =>
                {
                    Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(Pixiv_DNS_AND_SNI, 443);
                    var stream = new NetworkStream(socket, true);
                    SslStream sslstream = new SslStream(stream, false);
                    sslstream.AuthenticateAsClient(new SslClientAuthenticationOptions
                    {
                        TargetHost = Pixiv_DNS_AND_SNI,
                        ApplicationProtocols = new List<SslApplicationProtocol>(new SslApplicationProtocol[] { SslApplicationProtocol.Http2 })
                    });
                    return new ValueTask<Stream>(sslstream);
                }
            };

            if (BotConfig.GeneralConfig.PixivFreeProxy)
            {
                HttpClient httpClient = new HttpClient(handler);
                httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                httpClient.DefaultRequestVersion = HttpVersion.Version20;
                httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
                return httpClient;
            }
            else
            {
                return DefaultHttpClientFactory.CreateClient();
            }
        }

        /// <summary>
        /// 创建一个忽略https证书验证的HttpClientHandler
        /// </summary>
        /// <returns></returns>
        private static HttpClientHandler getHttpClientHandler()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13 | SslProtocols.None;
            clientHandler.AllowAutoRedirect = false;
            return clientHandler;
        }


    }
}
