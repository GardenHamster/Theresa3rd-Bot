using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TheresaBot.Main.Helper
{
    public static class StringHelper
    {

        /// <summary>
        /// 获取32长度的UUID
        /// </summary>
        /// <returns></returns>
        public static string get32UUID()
        {
            return System.Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 获取16长度的UUID
        /// </summary>
        /// <returns></returns>
        public static string get16UUID()
        {
            return get32UUID().Substring(0, 16);
        }

        /// <summary>
        /// 截断字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="keepLength"></param>
        /// <returns></returns>
        public static string cutString(this string str, int keepLength = 100, string endString = "...")
        {
            if (str is null) return null;
            str = str.Trim();
            if (str.Length <= keepLength) return str;
            if (string.IsNullOrEmpty(endString)) return str.Substring(0, keepLength);
            string returnStr = str.Substring(0, keepLength - endString.Length);
            return returnStr + endString;
        }

        /// <summary>
        /// 根据命令提取关键词(命令后的字符全部视为一条关键词)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string splitKeyWord(this string message, string command)
        {
            string[] splitArr = message.Trim().Split(new string[] { command }, StringSplitOptions.RemoveEmptyEntries);
            if (splitArr.Length < 1) return String.Empty;
            if (message.StartsWith(command)) return splitArr[0];
            return splitArr[splitArr.Length - 1].Trim();
        }

        /// <summary>
        /// 根据命令提取参数(通过空格拆分参数)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string[] splitKeyParams(this string message, string command)
        {
            string paramStr = message.Trim().splitKeyWord(command);
            if (paramStr is null) return new string[0];
            return paramStr.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 通过逗号或者换行符拆分参数
        /// </summary>
        /// <returns></returns>
        public static string[] splitParams(this string value)
        {
            return value.Split(new string[] { ",", "，", "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 拆分cookie,返回键值对
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static Dictionary<string, string> splitCookie(this string cookie)
        {
            Dictionary<string, string> cookieDic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(cookie)) return cookieDic;
            string[] cookieArr = cookie.Trim().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in cookieArr)
            {
                string[] cookieKVArr = item.Trim().Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (cookieKVArr.Length == 0) continue;
                string key = cookieKVArr[0].Trim();
                string value = cookieKVArr.Length > 1 ? cookieKVArr[1].Trim() : "";
                cookieDic[key] = value;
            }
            return cookieDic;
        }

        //// <summary>
        /// 拆分httpUrl,返回参数键值对
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <returns></returns>
        public static Dictionary<string, string> splitHttpParams(this string httpUrl)
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
        /// 提取参数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="commandStr"></param>
        /// <returns></returns>
        public static string[] splitHttpUrl(this string value)
        {
            string urlStr = value.Split(new char[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries)[0];
            return urlStr.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 从一个http url中提取文件名
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <returns></returns>
        public static string getHttpFileName(this string httpUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(httpUrl)) return null;
                string splitStr = httpUrl.Split('?')[0].Trim();
                string[] splitArr = splitStr.Split('/');
                return splitArr.Last().Trim();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 将键值对重新连接为cookie
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string joinCookie(this Dictionary<string, string> cookieDic)
        {
            StringBuilder cookieBuilder = new StringBuilder();
            foreach (var item in cookieDic)
            {
                if (cookieBuilder.Length > 0) cookieBuilder.Append(" ");
                cookieBuilder.Append($"{item.Key}={item.Value};");
            }
            return cookieBuilder.ToString();
        }


        public static string isContainsWord(this string str, List<string> containWords)
        {
            foreach (string word in containWords)
            {
                if (str.Contains(word)) return word;
            }
            return null;
        }

        public static string isContainsWord(this string str, string[] containWords)
        {
            foreach (string word in containWords)
            {
                if (str.Contains(word)) return word;
            }
            return null;
        }

        public static string[] isContainsWord(this string str, string[][] containWords)
        {
            foreach (string[] words in containWords)
            {
                foreach (string word in words)
                {
                    if (str.Contains(word)) return words;
                }
            }
            return null;
        }

        public static string removePunctuation(this string str)
        {
            string[] punctuation = new string[] { " ", "：", ":", "·", ",", ".", "•", "，", "。", "(", ")", "（", "）", "-", "—", "☆", "Δ" };
            foreach (string item in punctuation) str = str.Replace(item, "");
            return str;
        }

        public static string getHttpUrlWithoutParam(this string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            int questionMarkIndex = url.IndexOf("?");
            if (questionMarkIndex < 0) return url;
            return url.Substring(0, questionMarkIndex);
        }

        public static string getSuffixByUrl(this string url)
        {
            int lastPointIndex = url.LastIndexOf(".");
            int lastSlashIndex = url.LastIndexOf("/");
            if (lastPointIndex < 0) return "";
            if (lastPointIndex < lastSlashIndex) return "";
            string fileSuffix = url.Substring(lastPointIndex + 1, url.Length - lastPointIndex - 1);
            if (fileSuffix is null || fileSuffix.Trim().Length == 0) return "";
            return fileSuffix;
        }

        /// <summary>
        /// 过滤表情符号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string filterEmoji(this string str)
        {
            try
            {
                foreach (char character in str)
                {
                    byte[] bts = Encoding.UTF32.GetBytes(character.ToString());
                    if (bts[0].ToString() == "253" && bts[1].ToString() == "255")
                    {
                        str = str.Replace(character.ToString(), "");
                    }
                }
                return str;
            }
            catch (Exception)
            {
                return str;
            }
        }


        /// <summary>
        /// 判断str2是否和str1相似
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool checkStrSimilar(string str1, string str2)
        {
            int matchCount = 0;
            int previousIndex = -1;
            if (str1 is null || str2 is null) return false;
            str1 = str1.Replace(" ", "").Trim().ToLower();
            str2 = str2.Replace(" ", "").Trim().ToLower();
            if (str1.Length == 0 || str2.Length == 0) return false;
            if (str1.Length <= 3) return str2 == str1;
            char[] charArray = str2.ToCharArray();
            foreach (char item in charArray)
            {
                int wordIndex = str1.IndexOf(item, previousIndex < 0 ? 0 : previousIndex);
                if (wordIndex > -1 && wordIndex > previousIndex)
                {
                    matchCount++;
                    previousIndex = wordIndex;
                }
            }
            return matchCount >= str1.Length * 0.75;
        }

        // <summary>
        /// MD5加密(32位)
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <returns></returns>
        public static string getMD532bit(this string str)
        {
            MD5 md5 = MD5.Create();
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            byte[] byteNew = md5.ComputeHash(byteOld);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// 获取str的关键词word后面的全部内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string getStrAfterWord(this string str, string word)
        {
            int index = str.IndexOf(word);
            return str.Substring(index + word.Length, str.Length - index - word.Length);
        }


        /// <summary>
        /// 判断字符串是否纯数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isPureNumber(this string str)
        {
            return Regex.IsMatch(str, @"^\d+$");
        }


        public static string getSupplyNameWithoutSymbol(this string itemName)
        {
            if (itemName is null) return "";
            itemName = itemName.Replace("(", "");
            itemName = itemName.Replace(")", "");
            itemName = itemName.Replace("（", "");
            itemName = itemName.Replace("）", "");
            itemName = itemName.Replace("☆", "");
            return itemName.Trim();
        }

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string getRandomString(int length)
        {
            StringBuilder stringBuilder = new StringBuilder(length);
            string randomStringTemplate = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int pos = random.Next(0, randomStringTemplate.Length);
                stringBuilder.Append(randomStringTemplate[pos]);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 判断聊天记录中是否含有分享内容
        /// </summary>
        /// <returns></returns>
        public static bool isShareChar(this string message)
        {
            if (message is null) return false;
            string messageLower = message.ToLower();
            if (messageLower.Contains("xml")) return true;
            if (messageLower.Contains("http")) return true;
            return false;
        }


    }
}
