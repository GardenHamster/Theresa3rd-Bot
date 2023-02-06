using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using TheresaBot.Main.Common;

namespace TheresaBot.Main.Helper
{
    public static class HttpHelper
    {
        private static readonly IHttpClientFactory ProxyHttpClientFactory;
        private static readonly IHttpClientFactory DefaultHttpClientFactory;

        public static readonly string[] UserAgents = new string[] {
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
            ProxyHttpClientFactory = new ServiceCollection().AddHttpClient("ProxyClient").ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    Proxy = new WebProxy(new Uri(BotConfig.PixivConfig.HttpProxy), BypassOnLocal: true),
                    UseProxy = true,
                };
            }).Services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        }

        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string url, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            HttpClient client = DefaultHttpClientFactory.CreateClient();
            client.BaseAddress = new Uri(url);
            client.addHeaders(headerDic);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            HttpResponseMessage response = await client.GetAsync(url);
            //response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// HttpGet
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<string> GetWithProxyAsync(string url, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            HttpClient client = ProxyHttpClientFactory.CreateClient("ProxyClient");
            client.BaseAddress = new Uri(url);
            client.addHeaders(headerDic);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            HttpResponseMessage response = await client.GetAsync(url);
            //response.EnsureSuccessStatusCode();
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
        public static async Task<string> PostJsonAsync(string url, string postJsonStr, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            HttpContent content = new StringContent(postJsonStr);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient client = DefaultHttpClientFactory.CreateClient();
            client.addHeaders(headerDic);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            HttpResponseMessage response = await client.PostAsync(url, content);
            //response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// HttpPostForm
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramDic"></param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostFormForHtml(string url, Dictionary<string, string> paramDic, Dictionary<string, string> headerDic = null, int timeout = 60000)
        {
            HttpClient client = DefaultHttpClientFactory.CreateClient();
            client.addHeaders(headerDic);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            return await client.PostAsync(url, new FormUrlEncodedContent(paramDic));
        }

        /// <summary>
        /// post方式提交图片
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="imgPath"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<string> PostImageAsync(string postUrl, FileInfo imageFile, Dictionary<string, string> headerDic = null, int timeout = 60000)
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
        public static async Task<FileInfo> DownImgAsync(string imgUrl, Dictionary<string, string> headerDic = null, int timeout = 120000)
        {
            string suffix = StringHelper.getSuffixByUrl(imgUrl);
            if (string.IsNullOrEmpty(suffix)) suffix = "jpg";
            string fullFileName = StringHelper.get16UUID() + "." + suffix;
            string fullImageSavePath = Path.Combine(FilePath.GetDownFileSavePath(), fullFileName);
            return await HttpHelper.DownFileAsync(imgUrl, fullImageSavePath, headerDic, timeout);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="fullImageSavePath"></param>
        /// <param name="headerDic"></param>
        /// <returns></returns>
        public static async Task<FileInfo> DownFileAsync(string imgUrl, string fullImageSavePath, Dictionary<string, string> headerDic = null, int timeout = 120000)
        {
            if (File.Exists(fullImageSavePath)) return new FileInfo(fullImageSavePath);
            HttpClient client = DefaultHttpClientFactory.CreateClient();
            client.addHeaders(headerDic);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            byte[] urlContents = await client.GetByteArrayAsync(new Uri(imgUrl));
            using FileStream fileStream = new FileStream(fullImageSavePath, FileMode.CreateNew);
            fileStream.Write(urlContents, 0, urlContents.Length);
            FileInfo fileInfo = new FileInfo(fullImageSavePath);
            if (fileInfo.Length == 0) throw new Exception("文件下载失败，文件大小为0kb");
            return fileInfo;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="fullImageSavePath"></param>
        /// <param name="headerDic"></param>
        /// <returns></returns>
        public static async Task<FileInfo> DownFileWithProxyAsync(string imgUrl, string fullImageSavePath, Dictionary<string, string> headerDic = null, int timeout = 120000)
        {
            if (File.Exists(fullImageSavePath)) return new FileInfo(fullImageSavePath);
            HttpClient client = ProxyHttpClientFactory.CreateClient("ProxyClient");
            client.addHeaders(headerDic);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            client.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            byte[] urlContents = await client.GetByteArrayAsync(new Uri(imgUrl));
            using FileStream fileStream = new FileStream(fullImageSavePath, FileMode.CreateNew);
            fileStream.Write(urlContents, 0, urlContents.Length);
            FileInfo fileInfo = new FileInfo(fullImageSavePath);
            if (fileInfo.Length == 0) new Exception("文件下载失败，文件大小为0kb");
            return fileInfo;
        }

        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headerDic"></param>
        /// <returns></returns>
        public static void addHeaders(this HttpClient client, Dictionary<string, string> headerDic)
        {
            if (headerDic is null) return;
            foreach (var item in headerDic) client.DefaultRequestHeaders.Add(item.Key, item.Value);
        }

        /// <summary>
        /// 获取随机UserAgent
        /// </summary>
        /// <returns></returns>
        public static string GetRandomUserAgent()
        {
            int randomIndex = new Random(DateTime.Now.Millisecond).Next(0, UserAgents.Length);
            return UserAgents[randomIndex];
        }

        /// <summary>
        /// 获取响应的html内容
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static async Task<string> GetContentStringAsync(this HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "GetContentString异常");
                return string.Empty;
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
