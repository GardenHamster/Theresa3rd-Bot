using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TheresaBot.Main.Common;

namespace TheresaBot.Main.Helper
{
    public static class StringHelper
    {
        /// <summary>
        /// 忽略大小写判断字符串是否相等
        /// </summary>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            str1 = str1.ToUpper().Trim();
            str2 = str2.ToUpper().Trim();
            return str1 == str2;
        }

        /// <summary>
        /// 忽略大小写判断str1是否包含str2
        /// </summary>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string str1, string str2)
        {
            str1 = str1.ToUpper().Trim();
            str2 = str2.ToUpper().Trim();
            return str1.Contains(str2);
        }

        /// <summary>
        /// 获取32长度的UUID
        /// </summary>
        /// <returns></returns>
        public static string RandomUUID32()
        {
            return System.Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 获取16长度的UUID
        /// </summary>
        /// <returns></returns>
        public static string RandomUUID16()
        {
            return RandomUUID32().Substring(16, 16);
        }

        /// <summary>
        /// 获取8长度的UUID
        /// </summary>
        /// <returns></returns>
        public static string RandomUUID8()
        {
            return RandomUUID32().Substring(24, 8);
        }

        /// <summary>
        /// 截断字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="keepLength"></param>
        /// <returns></returns>
        public static string CutString(this string str, int keepLength = 100, string endString = "...")
        {
            if (str is null) return null;
            str = str.Trim();
            if (str.Length <= keepLength) return str;
            if (string.IsNullOrEmpty(endString)) return str.Substring(0, keepLength);
            string returnStr = str.Substring(0, keepLength - endString.Length);
            return returnStr + endString;
        }

        /// <summary>
        /// 判断一个字符串是否与另外一个字符串相似
        /// </summary>
        /// <param name="str"></param>
        /// <param name="compare"></param>
        /// <param name="similarity">0~1的小数</param>
        /// <returns></returns>
        public static bool IsSimilar(this string str, string compare, decimal similarity)
        {
            if (string.IsNullOrEmpty(str)) return false;
            if (string.IsNullOrEmpty(compare)) return false;
            var count = str.Where(o => compare.Contains(o)).Count();
            return Convert.ToDecimal(count) / str.Length >= similarity;
        }

        /// <summary>
        /// 提取一个分隔符以后的全部内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string TakeAfter(this string str, string separator)
        {
            int startIndex = str.IndexOf(separator);
            if (startIndex < 0) return string.Empty;
            return str.Substring(startIndex).Trim();
        }

        /// <summary>
        /// 根据命令提取关键词(命令后的字符全部视为一条关键词)
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string SplitKeyWord(this string instruction, string command)
        {
            command = command.Trim();
            instruction = instruction.Trim();
            string commandLower = command.ToLower();
            string messageLower = instruction.ToLower();
            if (messageLower.StartsWith(commandLower) == false) return String.Empty;
            return instruction.Substring(command.Length, instruction.Length - command.Length).Trim();
        }

        /// <summary>
        /// 根据命令提取参数(通过空格拆分参数)
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string[] SplitKeyParams(this string instruction, string command)
        {
            string paramStr = instruction.Trim().SplitKeyWord(command);
            string[] paramArr = paramStr.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            return paramArr.Where(o => string.IsNullOrWhiteSpace(o) == false).Select(o => o.Trim()).ToArray();
        }

        /// <summary>
        /// 将多标签拆分为每个单独的标签
        /// </summary>
        /// <param name="message"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string[] SplitPixivTags(this string tags)
        {
            if (string.IsNullOrWhiteSpace(tags)) return new string[0];
            string[] tagArr = tags.Split(new string[] { " ", ",", "，" }, StringSplitOptions.RemoveEmptyEntries);
            return tagArr.Where(o => string.IsNullOrWhiteSpace(o) == false).Select(o => o.Trim()).ToArray();
        }

        /// <summary>
        /// 通过逗号或者换行符拆分参数
        /// </summary>
        /// <returns></returns>
        public static string[] SplitParams(this string value)
        {
            var paramArr = value.Split(new string[] { ",", "，", "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return paramArr.Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => o.Trim()).ToArray();
        }

        /// <summary>
        /// 拆分cookie,返回键值对,不区分key大小写
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static Dictionary<string, string> SplitCookie(this string cookie)
        {
            if (string.IsNullOrWhiteSpace(cookie)) return new();
            var cookieDic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var cookieArr = cookie.Trim().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in cookieArr)
            {
                var kvArr = item.Trim().Split("=", StringSplitOptions.RemoveEmptyEntries);
                if (kvArr.Length == 0) continue;
                var key = kvArr[0].Trim();
                if (string.IsNullOrWhiteSpace(key)) continue;
                var value = kvArr.Length > 1 ? kvArr[1].Trim() : string.Empty;
                cookieDic[key] = value;
            }
            return cookieDic;
        }

        /// <summary>
        /// 将键值对重新连接为cookie
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string JoinCookie(this Dictionary<string, string> cookieDic)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in cookieDic)
            {
                if (builder.Length > 0) builder.Append(" ");
                builder.Append($"{item.Key}={item.Value};");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 连接参数
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="paramKey"></param>
        /// <returns></returns>
        public static string JoinParam(this List<int> ids, string paramKey)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var id in ids)
            {
                if (builder.Length > 0) builder.Append("&");
                builder.Append($"{paramKey}={id}");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 使用分隔符连接一个集合
        /// </summary>
        /// <param name="strList"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinToString<T1, T2>(this Dictionary<T1, T2> dic, string connector = "：", string separator = "\r\n")
        {
            if (dic.Count == 0) return string.Empty;
            var strList = dic.Select(o => $"{o.Key}{connector}{o.Value}").ToList();
            return string.Join(separator, strList);
        }

        /// <summary>
        /// 使用分隔符连接一个集合
        /// </summary>
        /// <param name="strList"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinToString<T>(this List<T> strList, string separator = "，")
        {
            if (strList.Count == 0) return string.Empty;
            return string.Join(separator, strList);
        }

        /// <summary>
        /// 使用分隔符连接一个数组
        /// </summary>
        /// <param name="strList"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinToString<T>(this T[] strList, string separator = "，")
        {
            if (strList.Length == 0) return string.Empty;
            return string.Join(separator, strList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="prefix"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinCommands(this List<string> commands, string prefix = "", string separator = "/")
        {
            if (string.IsNullOrEmpty(prefix)) prefix = BotConfig.DefaultPrefix;
            return commands?.Select(o => $"{prefix}{o}")?.ToList()?.JoinToString(separator) ?? string.Empty;
        }

        /// <summary>
        /// 过滤表情符号
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FilterEmoji(this string str)
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

        // <summary>
        /// MD5加密(32位)
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <returns></returns>
        public static string ToMD5(this string str)
        {
            MD5 md5 = MD5.Create();
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            byte[] byteNew = md5.ComputeHash(byteOld);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// 判断字符串是否纯数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsPureNumber(this string str)
        {
            return Regex.IsMatch(str, @"^\d+$");
        }

        /// <summary>
        /// 判断字符串是否为空行
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmptyLine(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return true;
            str = str.Trim().ToLower();
            str = str.Replace("\r", string.Empty);
            str = str.Replace("\n", string.Empty);
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 去除字符串头部和尾部的空行
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimLine(this string str)
        {
            str = str?.Trim();
            if (string.IsNullOrEmpty(str)) return string.Empty;
            while (str.StartsWith("\r")) str = str.Substring(2, str.Length - 2).Trim();
            while (str.StartsWith("\n")) str = str.Substring(2, str.Length - 2).Trim();
            while (str.EndsWith("\r")) str = str.Substring(0, str.Length - 2).Trim();
            while (str.EndsWith("\n")) str = str.Substring(0, str.Length - 2).Trim();
            return str;
        }

        /// <summary>
        /// 在字符串尾部添加换行符(如果没有)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AppendNewLineToEnd(this string str)
        {
            str = str.Trim();
            if (str.EndsWith("\r")) return str;
            if (str.EndsWith("\n")) return str;
            return str + "\r\n";
        }

        /// <summary>
        /// 移除字符串尾部添加换行符(如果有)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveNewLineToEnd(this string str)
        {
            str = str.Trim();
            if (str.EndsWith("\r\n")) return str.Substring(0, str.Length - 4);
            if (str.EndsWith("\n\r")) return str.Substring(0, str.Length - 4);
            if (str.EndsWith("\r")) return str.Substring(0, str.Length - 2);
            if (str.EndsWith("\n")) return str.Substring(0, str.Length - 2);
            return str;
        }

        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
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


    }
}
