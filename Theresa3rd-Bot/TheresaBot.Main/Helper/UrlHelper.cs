namespace TheresaBot.Main.Helper
{
    public static class UrlHelper
    {
        //// <summary>
        /// 拆分httpUrl,返回参数键值对
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <returns></returns>
        public static Dictionary<string, string> SplitHttpParams(this string httpUrl)
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(httpUrl)) return paramDic;
            int questionIndex = httpUrl.IndexOf("?");
            if (questionIndex < 0 || questionIndex == httpUrl.Length - 1) return paramDic;
            string paramStr = httpUrl.Substring(questionIndex + 1).Trim();
            string[] paramArr = paramStr.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in paramArr)
            {
                string[] paramKVArr = item.Trim().Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (paramKVArr.Length == 0) continue;
                string key = paramKVArr[0].Trim();
                string value = paramKVArr.Length > 1 ? paramKVArr[1].Trim() : "";
                paramDic[key] = value;
            }
            return paramDic;
        }

        /// <summary>
        /// 根据key提取http参数
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <param name="paramKey"></param>
        /// <returns></returns>
        public static string TakeHttpParam(this string httpUrl, string paramKey)
        {
            var paramDic = httpUrl?.SplitHttpParams() ?? new Dictionary<string, string>();
            foreach (var item in paramDic)
            {
                if (item.Key.Trim().ToLower() != paramKey.ToLower()) continue;
                return item.Value?.Trim() ?? string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 提取参数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="commandStr"></param>
        /// <returns></returns>
        public static string[] SplitHttpUrl(this string value)
        {
            string urlStr = value.Split(new char[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries)[0];
            return urlStr.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 格式化url
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <returns></returns>
        public static string FormatHttpUrl(this string httpUrl, bool defaultHttps = true)
        {
            if (string.IsNullOrWhiteSpace(httpUrl)) return string.Empty;
            httpUrl = httpUrl.Trim();
            string lowerUrl = httpUrl.ToLower();
            if (lowerUrl.StartsWith("//"))
            {
                string header = defaultHttps ? "https:" : "http:";
                httpUrl = header + httpUrl;
                lowerUrl = header + lowerUrl;
            }
            else if (!lowerUrl.StartsWith("http"))
            {
                string header = defaultHttps ? "https://" : "http://";
                httpUrl = header + httpUrl;
                lowerUrl = header + lowerUrl;
            }
            while (lowerUrl.EndsWith("/"))
            {
                httpUrl = httpUrl.Substring(0, httpUrl.Length - 1);
                lowerUrl = lowerUrl.Substring(0, lowerUrl.Length - 1);
            }
            return httpUrl;
        }

        /// <summary>
        /// 提取路径中以/分割的最后一个字符串
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <returns></returns>
        public static string TakeHttpLast(this string httpUrl)
        {
            if (string.IsNullOrWhiteSpace(httpUrl)) return string.Empty;
            httpUrl = httpUrl.Replace("http://", string.Empty);
            httpUrl = httpUrl.Replace("https://", string.Empty);
            httpUrl = httpUrl.Trim();
            var index = httpUrl.IndexOf("?");
            var baseUrl = index < 0 ? httpUrl : httpUrl.Substring(0, index);
            var splits = baseUrl.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length < 2) return string.Empty;
            return splits.Last().Trim();
        }


    }
}
