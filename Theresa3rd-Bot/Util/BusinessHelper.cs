using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Config;
using Theresa3rd_Bot.Model.Pixiv;
using Theresa3rd_Bot.Model.PO;

namespace Theresa3rd_Bot.Util
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
        /// 判断是否以某一个指令开头
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool StartWithCommand(this string instructions, string command)
        {
            return string.IsNullOrWhiteSpace(command) == false && instructions.ToLower().StartsWith(command.Trim().ToLower());
        }

        /// <summary>
        /// 判断是否以某一个指令开头
        /// </summary>
        /// <param name="instructions"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool StartWithCommand(this string instructions, string[] commands)
        {
            if (commands == null || commands.Length == 0) return false;
            return commands.Where(o => string.IsNullOrWhiteSpace(o) == false && instructions.ToLower().StartsWith(o.Trim().ToLower())).Any();
        }

        /// <summary>
        /// 判断是否可以处理一个群的消息
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static bool IsHandleMessage(long groupId)
        {
            PermissionsConfig permissionsConfig = BotConfig.PermissionsConfig;
            if (permissionsConfig == null) return false;
            List<long> acceptGroups = permissionsConfig.AcceptGroups;
            if (acceptGroups == null) return false;
            return acceptGroups.Contains(groupId);
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
            if (tags == null || tags.Count == 0) return false;
            return tags.Where(o => o.IsR18()).Any();
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
            if (tags == null || tags.Count == 0) return false;
            return tags.Where(o => o.IsAI()).Any();
        }

        /// <summary>
        /// 判断标签中是否包含不合法标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsImproper(this List<string> tags)
        {
            if (tags == null || tags.Count == 0) return false;
            if (tags.Where(o => o.ToUpper().Replace("-", "").Replace(" ", "").Contains("R18G")).Any()) return true;
            return false;
        }

        /// <summary>
        /// 判断标签中是否包含被禁止的标签，有则返回内容，否则返回null
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static string hasBanTags(this List<string> tags)
        {
            if (tags == null || tags.Count == 0) return null;
            List<string> banTags = new List<string>();
            foreach (string tag in tags)
            {
                List<BanWordPO> banList = BotConfig.BanSetuTagList.Where(o => tag.Trim().ToUpper().Contains(o.KeyWord.Trim().ToUpper())).ToList();
                if (banList.Count > 0) banTags.AddRange(banList.Select(o => o.KeyWord).ToList());
            }
            return banTags.Count > 0 ? String.Join('，', banTags) : null;
        }

        /// <summary>
        /// 判断标签中是否包含动图标签
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static bool IsGif(this List<string> tags)
        {
            if (tags == null || tags.Count == 0) return false;
            return tags.Where(o => o == "うごイラ" || o == "动图" || o == "動圖" || o.ToLower() == "ugoira").Any();
        }

        /// <summary>
        /// 检查pixiv cookie是否已经过期
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsPixivCookieAvailable()
        {
            if (string.IsNullOrWhiteSpace(BotConfig.WebsiteConfig.Pixiv.Cookie)) return false;
            if (DateTime.Now > BotConfig.WebsiteConfig.Pixiv.CookieExpireDate) return false;
            if (BotConfig.WebsiteConfig.Pixiv.UserId <= 0) return false;
            return true;
        }

        /// <summary>
        /// 将模版转换为消息链
        /// </summary>
        /// <param name="session"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static async Task<List<IChatMessage>> SplitToChainAsync(this IMiraiHttpSession session, string template, UploadTarget uploadTarget = UploadTarget.Group)
        {
            List<IChatMessage> chatMessages = new List<IChatMessage>();
            List<string> splitList = SplitImageCode(template);
            foreach (var item in splitList)
            {
                string code = item.Trim();
                if (string.IsNullOrEmpty(item)) continue;
                if (Regex.Match(code, ImageCodeRegex).Success)
                {
                    string path = code.Substring(ImageCodeHeader.Length, code.Length - ImageCodeHeader.Length - 1);
                    if (File.Exists(path) == false) continue;
                    chatMessages.Add((IChatMessage)await session.UploadPictureAsync(uploadTarget, path));
                }
                else
                {
                    chatMessages.Add(new PlainMessage(item));
                }
            }
            return chatMessages;
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
        /// <param name="maxShowCount"></param>
        /// <returns></returns>
        public static string JoinPixivImgOrginUrls(PixivWorkInfo pixivWorkInfo)
        {
            StringBuilder LinkBuilder = new StringBuilder();
            int maxCount = BotConfig.PixivConfig.UrlShowMaximum > 0 ? BotConfig.PixivConfig.UrlShowMaximum : pixivWorkInfo.pageCount;
            for (int i = 0; i < maxCount && i < pixivWorkInfo.pageCount; i++)
            {
                string imgUrl = pixivWorkInfo.urls.original.ToOrginProxyUrl();
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
        public static string JoinPixivTagsStr(PixivTags pixivTag, int maxShowCount = 0)
        {
            if (pixivTag?.tags == null) return String.Empty;
            List<string> tags = pixivTag.tags.Select(o => o.tag).ToList();
            return JoinPixivTagsStr(tags, maxShowCount);
        }

        /// <summary>
        /// 拼接pixiv标签
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="maxShowCount"></param>
        /// <returns></returns>
        public static string JoinPixivTagsStr(List<string> tags, int maxShowCount = 0)
        {
            string tagstr = "";
            if (tags == null || tags.Count == 0) return String.Empty;
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
