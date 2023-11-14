using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TheresaBot.Main.Common;
using TheresaBot.Main.Datas;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Pixiv;

namespace TheresaBot.Main.Helper
{
    public static class BusinessHelper
    {
        /// <summary>
        /// image码
        /// </summary>
        private static string ImageCodeRegex = @"\[image:[^\[\]]+?\]";

        /// <summary>
        /// image头
        /// </summary>
        private static string ImageCodeHeader = @"[image:";

        /// <summary>
        /// 匹配对应的指令前缀
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string MatchPrefix(this string message)
        {
            var prefixs = BotConfig.GeneralConfig.Prefixs;
            if (prefixs is null || prefixs.Count == 0) return string.Empty;
            message = message?.Trim() ?? string.Empty;
            var prefix = prefixs.Where(o => message.StartsWith(o)).FirstOrDefault();
            return string.IsNullOrWhiteSpace(prefix) ? string.Empty : prefix;
        }

        /// <summary>
        /// 判断字符串是否为pixivId
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsPixivId(this string str)
        {
            long pixivId = 0;
            if (!long.TryParse(str, out pixivId)) return false;
            return pixivId > 30000000;
        }

        /// <summary>
        /// 判断标签中是否包含R18标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsR18(this string tagStr)
        {
            if (string.IsNullOrWhiteSpace(tagStr)) return false;
            string tagUp = tagStr.ToUpper();
            return tagUp.Contains("R-18") || tagUp.Contains("R18") || tagUp.Contains("R18G");
        }

        /// <summary>
        /// 判断标签中是否包含R18标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsR18(this List<string> tags)
        {
            if (tags is null || tags.Count == 0) return false;
            return tags.Any(o => o.IsR18());
        }

        /// <summary>
        /// 判断标签中是否包含AI生成标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsAI(this string tagStr)
        {
            if (string.IsNullOrWhiteSpace(tagStr)) return false;
            string tagUp = tagStr.ToUpper();
            return tagUp == "AI" || tagUp.StartsWith("AIART") || tagUp.Contains("NOVELAI") || tagUp.Contains("AI生成") || tagUp.Contains("AI绘图") || tagUp.Contains("AIイラスト");
        }

        /// <summary>
        /// 判断标签中是否包含AI生成标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsAI(this List<string> tags)
        {
            if (tags is null || tags.Count == 0) return false;
            return tags.Any(o => o.IsAI());
        }

        /// <summary>
        /// 判断标签中是否包含不合法标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsImproper(this List<string> tags)
        {
            if (tags is null || tags.Count == 0) return false;
            if (tags.Any(o => o.ToUpper().Replace("-", "").Replace(" ", "").Contains("R18G"))) return true;
            return false;
        }

        /// <summary>
        /// 判断标签中是否包含动图标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsGif(this List<string> tags)
        {
            if (tags is null || tags.Count == 0) return false;
            return tags.Any(o => o == "うごイラ" || o == "动图" || o == "動圖" || o.ToLower() == "ugoira");
        }

        /// <summary>
        /// 检查pixiv cookie是否已经过期
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsPixivCookieAvailable()
        {
            if (string.IsNullOrWhiteSpace(WebsiteDatas.Pixiv.Cookie)) return false;
            if (DateTime.Now > WebsiteDatas.Pixiv.CookieExpireDate) return false;
            if (WebsiteDatas.Pixiv.UserId <= 0) return false;
            return true;
        }

        /// <summary>
        /// 判断字符串是否为符合yyyyMMdd格式的日期字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsShortDateStr(this string str)
        {
            DateTime outTime = DateTime.Now;
            return DateTime.TryParseExact(str, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out outTime);
        }

        /// <summary>
        /// 将Id字符串通过逗号拆分成一个int集合
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<int> SplitToIdList(this string str)
        {
            if (string.IsNullOrEmpty(str)) return new();
            var splitArr = str.Trim().Split(new char[] { '，', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var numList = new List<int>();
            foreach (var item in splitArr)
            {
                int num = 0;
                if (int.TryParse(item, out num)) numList.Add(num);
            }
            return numList;
        }

        /// <summary>
        /// 将模版转换为消息链
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static List<BaseContent> SplitToChainAsync(this string template)
        {
            template = template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) return new();
            List<BaseContent> chatContents = new List<BaseContent>();
            List<string> splitList = SplitImageCode(template);
            foreach (var item in splitList)
            {
                string code = item?.Trim();
                if (string.IsNullOrEmpty(code)) continue;
                if (code.IsEmptyLine()) continue;
                if (Regex.Match(code, ImageCodeRegex).Success)
                {
                    string path = code.Substring(ImageCodeHeader.Length, code.Length - ImageCodeHeader.Length - 1);
                    if (File.Exists(path) == false) continue;
                    chatContents.Add(new LocalImageContent(new FileInfo(path)));
                }
                else
                {
                    chatContents.Add(new PlainContent(item, false));
                }
            }
            return chatContents;
        }

        public static void ShowBackstageInfos()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------");
            Console.WriteLine($"后台密码：{BotConfig.BackstageConfig.Password}");
            Console.WriteLine($"你可以在配置文件【Config/Backstage.yml】中修改后台密码：Password");
            Console.WriteLine("----------------------------------------------------------------------------------------");
            Console.WriteLine($"访问下列地址配置Bot相关功能：");
            foreach (var address in IPAddressHelper.GetLocalBackstageUrls())
            {
                Console.WriteLine($"{address}");
            }
            Console.WriteLine("----------------------------------------------------------------------------------------");
        }

        /// <summary>
        /// 获取欢迎消息
        /// </summary>
        /// <returns></returns>
        public static string GetStartUpMessage()
        {
            StringBuilder msgBuilder = new StringBuilder();
            msgBuilder.AppendLine($"欢迎使用【Theresa3rd-Bot v{BotConfig.BotVersion}】");
            msgBuilder.AppendLine($"群聊发送【#菜单】可以查看所有指令");
            msgBuilder.AppendLine($"局域网下访问下列地址进行Bot相关配置");
            foreach (var item in IPAddressHelper.GetLocalBackstageUrls())
            {
                msgBuilder.AppendLine(item);
            }
            msgBuilder.AppendLine($"部署或者使用教程请访问");
            msgBuilder.Append($"{BotConfig.BotHomepage}");
            return msgBuilder.ToString();
        }

        /// <summary>
        /// 处理错误后返回的提示内容
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static List<BaseContent> GetErrorContents(this Exception ex, string message = "")
        {
            string template = BotConfig.GeneralConfig.ErrorMsg;
            if (string.IsNullOrWhiteSpace(template)) template = "出了点小问题，稍后再试吧~";
            if (template.StartsWith(" ") == false) template = " " + template;
            string errorMsg = ex is BaseException ? ex.Message : message;
            if (string.IsNullOrWhiteSpace(errorMsg)) errorMsg = message;
            List<BaseContent> contents = new List<BaseContent>();
            contents.AddRange(template.SplitToChainAsync());
            contents.Add(new PlainContent(errorMsg, false));
            return contents;
        }

        /// <summary>
        /// 根据image码拆分模版
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static List<string> SplitImageCode(this string template)
        {
            if (string.IsNullOrEmpty(template)) return new List<string>();
            string[] textArr = Regex.Split(template, ImageCodeRegex);
            if (textArr.Length == 1) return new List<string>() { template };
            List<string> SplitList = new List<string>();
            for (int i = 0; i < textArr.Length; i++)
            {
                string text = textArr[i];
                if (text.Length == 0) continue;
                int textIndex = template.IndexOf(text);
                if (textIndex > 0)
                {
                    SplitList.Add(template.Substring(0, textIndex));
                    template = template.Substring(textIndex, template.Length - textIndex);
                }
                SplitList.Add(text);
                template = template.Substring(text.Length, template.Length - text.Length);
            }
            if (template.Length > 0) SplitList.Add(template);
            return SplitList;
        }

        /// <summary>
        /// 拼接pixiv作品路径
        /// </summary>
        /// <param name="pixivWorkInfo"></param>
        /// <returns></returns>
        public static string JoinPixivImgOriginUrls(PixivWorkInfo pixivWorkInfo)
        {
            StringBuilder LinkBuilder = new StringBuilder();
            int maxCount = BotConfig.PixivConfig.UrlShowMaximum > 0 ? BotConfig.PixivConfig.UrlShowMaximum : pixivWorkInfo.pageCount;
            for (int i = 0; i < maxCount && i < pixivWorkInfo.pageCount; i++)
            {
                string imgUrl = pixivWorkInfo.urls.original.ToOpenProxyLink();
                if (i > 0) imgUrl = imgUrl.Replace("_p0.", $"_p{i}.");
                if (LinkBuilder.Length > 0) LinkBuilder.AppendLine();
                LinkBuilder.Append(imgUrl);
            }
            return LinkBuilder.ToString();
        }

        /// <summary>
        /// 拼接pixiv标签
        /// </summary>
        /// <param name="pixivTag"></param>
        /// <param name="maxShowCount"></param>
        /// <returns></returns>
        public static string JoinPixivTagsStr(this PixivTags pixivTag, int maxShowCount = 0)
        {
            if (pixivTag?.tags is null) return String.Empty;
            List<string> tags = pixivTag.tags.Select(o => o.tag).ToList();
            return JoinPixivTagsStr(tags, maxShowCount);
        }

        /// <summary>
        /// 拼接pixiv标签
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="maxShowCount"></param>
        /// <returns></returns>
        public static string JoinPixivTagsStr(this List<string> tags, int maxShowCount = 0)
        {
            string tagstr = "";
            if (tags is null || tags.Count == 0) return String.Empty;
            for (int i = 0; i < tags.Count; i++)
            {
                if (maxShowCount > 0 && i >= maxShowCount) return $"{tagstr}，...";
                if (tagstr.Length > 0) tagstr += "，";
                tagstr += tags[i];
            }
            return tagstr;
        }


    }
}
