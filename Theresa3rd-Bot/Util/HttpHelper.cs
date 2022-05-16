using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Http;

namespace Theresa3rd_Bot.Util
{
    public static class HttpHelper
    {
        public static string[] UserAgent = new string[] {
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

        public static string getRandomUserAgent()
        {
            int randomIndex = new Random(DateTime.Now.Millisecond).Next(0, UserAgent.Length);
            return UserAgent[randomIndex];
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
            using HttpClientHandler clientHandler = getHttpClientHandler();
            using HttpClient client = new HttpClient(clientHandler);
            client.BaseAddress = new Uri(url);
            client.addHeaders(headerDic);
            client.DefaultRequestHeaders.Add("User-Agent", getRandomUserAgent());
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
        public static string HttpPost(string url, Dictionary<string, string> headerDic = null, string paramStr = null, string contentType = null, int timeout = 15000)
        {
            int retryCount = 2;
            while (retryCount > 0)
            {
                HttpWebRequest request = null;
                HttpWebResponse response = null;
                Stream resSteam = null;
                Stream reqStream = null;
                StreamReader reader = null;
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = string.IsNullOrEmpty(contentType) ? "application/json; charset=UTF-8" : contentType;
                    request.UserAgent = getRandomUserAgent();
                    request.Accept = "*/*";
                    request.Timeout = timeout;
                    byte[] data = Encoding.UTF8.GetBytes(paramStr.ToString());
                    request.ContentLength = data.Length;
                    addHeaders(request, headerDic);//添加请求头
                    reqStream = request.GetRequestStream();
                    reqStream.Write(data, 0, data.Length);
                    response = (HttpWebResponse)request.GetResponse();
                    resSteam = response.GetResponseStream();
                    reader = new StreamReader(resSteam, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
                catch (Exception)
                {
                    retryCount--;
                    if (retryCount <= 0) throw;
                }
                finally
                {
                    if (reader != null) reader.Close();
                    if (resSteam != null) resSteam.Close();
                    if (reqStream != null) reqStream.Close();
                    if (response != null) response.Close();
                    if (request != null) request.Abort();
                }
            }
            throw new Exception("发送HttpPost请求失败");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramdic"></param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string HttpPostForm(string url, Dictionary<string, string> paramdic = null, Dictionary<string, string> headerDic = null, int timeout = 15000)
        {
            if (paramdic == null) paramdic = new Dictionary<string, string>();
            StringBuilder builder = new StringBuilder();
            foreach (var item in paramdic)
            {
                if (builder.Length > 0) builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
            }
            return HttpPost(url, headerDic, builder.ToString(), "application/x-www-form-urlencoded", timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postJsonStr"></param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string HttpPostJson(string url, string postJsonStr, Dictionary<string, string> headerDic = null, int timeout = 15000)
        {
            return HttpPost(url, headerDic, postJsonStr, "application/json; charset=UTF-8", timeout);
        }

        /// <summary>
        /// post方式提交文件
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="imgPath"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string HttpPostFile(string postUrl, string imgPath, int timeout = 15000)
        {
            int retryCount = 2;
            while (retryCount > 0)
            {
                HttpWebRequest request = null;
                Stream postStream = null;
                HttpWebResponse response = null;
                Stream instream = null;
                StreamReader readerStream = null;
                try
                {
                    request = WebRequest.Create(postUrl) as HttpWebRequest;
                    CookieContainer cookieContainer = new CookieContainer();
                    request.CookieContainer = cookieContainer;
                    request.AllowAutoRedirect = true;
                    request.Method = "POST";
                    request.Timeout = timeout;
                    string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
                    request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
                    request.UserAgent = getRandomUserAgent();
                    byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
                    byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                    FileInfo fileInfo = new FileInfo(imgPath);
                    string fileName = fileInfo.Name;

                    //请求头部信息 
                    StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
                    byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());
                    FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
                    byte[] bArr = new byte[fs.Length];
                    fs.Read(bArr, 0, bArr.Length);
                    fs.Close();

                    postStream = request.GetRequestStream();
                    postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                    postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                    postStream.Write(bArr, 0, bArr.Length);
                    postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                    postStream.Close();

                    response = request.GetResponse() as HttpWebResponse;
                    instream = response.GetResponseStream();
                    readerStream = new StreamReader(instream, Encoding.UTF8);
                    return readerStream.ReadToEnd();
                }
                catch (Exception)
                {
                    retryCount--;
                    if (retryCount <= 0) throw;
                }
                finally
                {
                    if (readerStream != null) readerStream.Close();
                    if (instream != null) instream.Close();
                    if (response != null) response.Close();
                    if (postStream != null) postStream.Close();
                    if (request != null) request.Abort();
                }
            }
            throw new Exception("发送HttpPost请求失败");
        }

        /// <summary>
        /// HttpGet下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fullSavePath"></param>
        /// <param name="headerDic"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string HttpDownload(string url, string fullSavePath, Dictionary<string, string> headerDic = null, int timeout = 15000)
        {
            int retryCount = 2;
            while (retryCount > 0)
            {
                HttpWebRequest request = null;
                HttpWebResponse response = null;
                Stream responseStream = null;
                FileStream fileStream = null;
                try
                {
                    byte[] bArr = new byte[1024];
                    string targetDirPath = System.IO.Path.GetDirectoryName(fullSavePath);
                    if (Directory.Exists(targetDirPath) == false) Directory.CreateDirectory(targetDirPath);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.Method = "GET";
                    request.UserAgent = getRandomUserAgent();
                    request.Timeout = timeout;
                    addHeaders(request, headerDic);//添加请求头
                    response = request.GetResponse() as HttpWebResponse;
                    responseStream = response.GetResponseStream();
                    fileStream = new FileStream(fullSavePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                    while (size > 0)
                    {
                        fileStream.Write(bArr, 0, size);
                        size = responseStream.Read(bArr, 0, (int)bArr.Length);
                    }
                    return fullSavePath;
                }
                catch (Exception)
                {
                    retryCount--;
                    if (retryCount <= 0) throw;
                }
                finally
                {
                    if (fileStream != null) fileStream.Close();
                    if (responseStream != null) responseStream.Close();
                    if (response != null) response.Close();
                    if (request != null) request.Abort();
                }
            }
            throw new Exception("HttpDownload失败");
        }

        /// <summary>
        /// 根据一个网址获取Html内容
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <returns></returns>
        public static string getHtml(string httpUrl, int timeout = 15000)
        {
            int retryCount = 2;
            while (retryCount > 0)
            {
                HttpWebRequest request = null;
                HttpWebResponse response = null;
                Stream receiveStream = null;
                StreamReader readStream = null;
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(httpUrl);
                    request.Timeout = timeout;
                    request.UserAgent = getRandomUserAgent();
                    response = (HttpWebResponse)request.GetResponse();
                    receiveStream = response.GetResponseStream();
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    return readStream.ReadToEnd();
                }
                catch (Exception)
                {
                    retryCount--;
                    if (retryCount <= 0) throw;
                }
                finally
                {
                    if (readStream != null) readStream.Close();
                    if (receiveStream != null) receiveStream.Close();
                    if (response != null) response.Close();
                    if (request != null) request.Abort();
                }
            }
            throw new Exception("获取html失败");
        }

        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headerDic"></param>
        /// <returns></returns>
        private static HttpWebRequest addHeaders(HttpWebRequest request, Dictionary<string, string> headerDic)
        {
            if (headerDic == null) return request;
            foreach (var item in headerDic)
            {
                if (item.Key.ToLower() == "referer")
                {
                    request.Referer = item.Value;
                }
                else if (item.Key.ToLower() == "accept")
                {
                    request.Accept = item.Value;
                }
                else if (item.Key.ToLower() == "host")
                {
                    request.Host = item.Value;
                }
                else if (item.Key.ToLower() == "user-agent")
                {
                    request.UserAgent = item.Value;
                }
                else
                {
                    request.Headers[item.Key] = item.Value;
                }
            }
            return request;
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
        /// 下载图片
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <returns></returns>
        public static async Task<FileInfo> downImgAsync(string imgUrl)
        {
            if (string.IsNullOrEmpty(imgUrl)) return null;
            string suffix = StringHelper.getSuffixByUrl(imgUrl);
            if (string.IsNullOrEmpty(suffix)) suffix = "jpg";
            string fullFileName = StringHelper.get16UUID() + "." + suffix;
            string fullImageSavePath = Path.Combine(FilePath.getDownImgSavePath(), fullFileName);
            return await HttpHelper.downImgAsync(imgUrl, fullImageSavePath, null, null);
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="fullImageSavePath"></param>
        /// <returns></returns>
        public static async Task<FileInfo> downImgAsync(string imgUrl, string fullImageSavePath)
        {
            return await HttpHelper.downImgAsync(imgUrl, fullImageSavePath, null, null);
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="fullImageSavePath"></param>
        /// <param name="referer"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static async Task<FileInfo> downImgAsync(string imgUrl, string fullImageSavePath, string referer = null, string cookie = null)
        {
            try
            {
                if (File.Exists(fullImageSavePath)) return new FileInfo(fullImageSavePath);
                using HttpClientHandler clientHandler = getHttpClientHandler();
                using HttpClient client = new HttpClient(clientHandler);
                if (!string.IsNullOrEmpty(referer)) client.DefaultRequestHeaders.Add("Referer", referer);
                if (!string.IsNullOrEmpty(cookie)) client.DefaultRequestHeaders.Add("Cookie", cookie);
                client.Timeout = TimeSpan.FromSeconds(30);
                byte[] urlContents = await client.GetByteArrayAsync(new Uri(imgUrl));
                using FileStream fs = new FileStream(fullImageSavePath, FileMode.CreateNew);
                fs.Write(urlContents, 0, urlContents.Length);
                return new FileInfo(fullImageSavePath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static HttpClientHandler getHttpClientHandler()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            clientHandler.SslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            clientHandler.AllowAutoRedirect = false;
            return clientHandler;
        }

    }
}
