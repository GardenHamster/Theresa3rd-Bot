using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Util
{
    public static class StringHelper
    {
        private readonly static Regex CQCodeRegex = new Regex(@"\[CQ[^\u4E00-\u9FA5]*\]", RegexOptions.IgnoreCase);
        private readonly static Regex EmojiRegex = new Regex(@"\[emoji[^\u4E00-\u9FA5]*\]", RegexOptions.IgnoreCase);

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
        public static string cutString(this string str, int keepLength = 100)
        {
            if (str == null) return null;
            str = str.Trim();
            if (str.Length <= keepLength) return str;
            return str.Substring(0, keepLength);
        }

        /// <summary>
        /// 提取关键词
        /// </summary>
        /// <param name="message"></param>
        /// <param name="commandStr"></param>
        /// <returns></returns>
        public static string splitKeyWord(string message, string commandStr)
        {
            string[] messageSplit = message.Split(new string[] { commandStr }, StringSplitOptions.RemoveEmptyEntries);
            if (messageSplit.Length < 2) return null;
            return messageSplit[1].Trim();
        }

        /// <summary>
        /// 拆分cookie,返回键值对
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static Dictionary<string, string> splitCookie(string cookie)
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
        public static Dictionary<string, string> splitHttpUrl(string httpUrl)
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

        public static string getDateTimeMillisecondStr()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }

        public static string isContainsWord(string str, List<string> containWords)
        {
            foreach (string word in containWords)
            {
                if (str.Contains(word)) return word;
            }
            return null;
        }

        public static string isContainsWord(string str, string[] containWords)
        {
            foreach (string word in containWords)
            {
                if (str.Contains(word)) return word;
            }
            return null;
        }

        public static string[] isContainsWord(string str, string[][] containWords)
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

        public static string getHttpUrlWithoutParam(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            int questionMarkIndex = url.IndexOf("?");
            if (questionMarkIndex < 0) return url;
            return url.Substring(0, questionMarkIndex);
        }

        public static string getFullFileNameByUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            string[] splitArr = url.Split(new char[] { '/' });
            return splitArr[splitArr.Length - 1];
        }

        public static string getSuffixByUrl(string url)
        {
            int lastPointIndex = url.LastIndexOf(".");
            int lastSlashIndex = url.LastIndexOf("/");
            if (lastPointIndex < 0) return "";
            if (lastPointIndex < lastSlashIndex) return "";
            string fileSuffix = url.Substring(lastPointIndex + 1, url.Length - lastPointIndex - 1);
            if (fileSuffix == null || fileSuffix.Trim().Length == 0) return "";
            return fileSuffix;
        }

        /// <summary>
        /// 过滤表情符号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string filterEmoji(string str)
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
        public static bool checkStrSimilar(string str1,string str2)
        {
            int matchCount = 0;
            int previousIndex = -1;
            if (str1 == null || str2 == null) return false;
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
        public static string getMD532bit(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }

        /// <summary>
        /// 获取str的关键词word后面的全部内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string getStrAfterWord(string str,string word)
        {
            int index = str.IndexOf(word);
            return str.Substring(index + word.Length, str.Length - index - word.Length);
        }

        /// <summary>
        /// url参数转码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string getUrlParamEncode(string param)
        {
            return param.Replace("+", "%2B").Replace("/", "%20").Replace("?", "%3F").Replace("%", "%25").Replace("#", "%23").Replace("&", "%26").Replace("-", "%3D");
        }

        /// <summary>
        /// 判断字符串是否纯数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isPureNumber(string str)
        {
            return Regex.IsMatch(str, @"^\d+$");
        }

        public static string getSupplyNameWithoutSymbol(string itemName)
        {
            if (itemName == null) return "";
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
        /// 提取一个消息中的纯文本信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string getCharText(string message)
        {
            if (message == null) return "";
            if (isShareChar(message)) return "";
            message = CQCodeRegex.Replace(message, "");
            message = EmojiRegex.Replace(message, "");
            return message.Trim();
        }

        /// <summary>
        /// 判断聊天记录中是否含有分享内容
        /// </summary>
        /// <returns></returns>
        public static bool isShareChar(string message)
        {
            if (message == null) return false;
            string messageLower = message.ToLower();
            if (messageLower.Contains("xml")) return true;
            if (messageLower.Contains("http")) return true;
            return false;
        }


    }
}
