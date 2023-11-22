using System.Text;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Invoker;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Helper
{
    public static class CommandHelper
    {

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令头，否则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instruction"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static string CheckCommand<T>(this string instruction, CommandHandler<T> handler) where T : BaseCommand
        {
            if (handler.Commands is null) return null;
            if (handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (string.IsNullOrWhiteSpace(command)) continue;
                if (instruction.StartsWith(command, StringComparison.OrdinalIgnoreCase)) return command;
            }
            return null;
        }

        /// <summary>
        /// 发送涩图，然后撤回
        /// </summary>
        /// <param name="command"></param>
        /// <param name="setuContent"></param>
        /// <param name="revokeInterval">撤回延时，0表示不撤回</param>
        /// <param name="sendImgBehind"></param>
        /// <returns></returns>
        public static async Task<BaseResult[]> ReplyGroupSetuAsync(this GroupCommand command, SetuContent setuContent, int revokeInterval, bool sendImgBehind)
        {
            var resendType = BotConfig.PixivConfig.ImgResend;
            var results = await command.ReplyAndRevokeAsync(setuContent, revokeInterval, sendImgBehind);
            if (results.Any(o => o.IsFailed) && resendType != ResendType.None) //发送失败后重发
            {
                await Task.Delay(1000);
                var resendContent = setuContent.ToResendContent(resendType);
                Console.WriteLine($"涩图内容发送失败，尝试使用{resendType}后再次发送...");
                results = await command.ReplyAndRevokeAsync(resendContent, revokeInterval, sendImgBehind);
            }
            return results;
        }

        /// <summary>
        /// 发送涩图，然后撤回
        /// </summary>
        /// <param name="command"></param>
        /// <param name="setuContents"></param>
        /// <param name="revokeInterval">撤回延时，0表示不撤回</param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyGroupSetuAsync(this GroupCommand command, List<SetuContent> setuContents, int revokeInterval)
        {
            var resendType = BotConfig.PixivConfig.ImgResend;
            var results = await command.ReplyAndRevokeAsync(setuContents, revokeInterval);
            if (results.IsFailed && resendType != ResendType.None)
            {
                await Task.Delay(1000);
                var resendContents = setuContents.ToResendContent(resendType);
                Console.WriteLine($"涩图内容发送失败，尝试使用{resendType}后再次发送...");
                results = await command.ReplyAndRevokeAsync(resendContents, revokeInterval);
            }
            return results;
        }

        /// <summary>
        /// 发送涩图，然后撤回
        /// </summary>
        /// <param name="command"></param>
        /// <param name="setuContent"></param>
        /// <param name="revokeInterval">撤回延时，0表示不撤回</param>
        /// <param name="sendImgBehind"></param>
        /// <param name="isAt"></param>
        /// <returns></returns>
        private static async Task<BaseResult[]> ReplyAndRevokeAsync(this GroupCommand command, SetuContent setuContent, int revokeInterval, bool sendImgBehind)
        {
            List<BaseResult> results = new List<BaseResult>();
            List<BaseContent> msgContents = setuContent.SetuInfos ?? new();
            List<BaseContent> imgContents = setuContent.SetuImages.ToBaseContent().SetDefaultImage().ToList();
            if (sendImgBehind)
            {
                BaseResult workMsgResult = await command.ReplyGroupMessageWithQuoteAsync(msgContents);
                await Task.Delay(1000);
                BaseResult imgMsgResult = await command.ReplyGroupMessageWithQuoteAsync(imgContents);
                results.Add(workMsgResult);
                results.Add(imgMsgResult);
            }
            else
            {
                List<BaseContent> contentList = msgContents.Concat(imgContents).ToList();
                BaseResult msgResult = await command.ReplyGroupMessageWithQuoteAsync(contentList);
                results.Add(msgResult);
            }
            if (revokeInterval > 0)
            {
                Task revokeTask = command.RevokeGroupMessageAsync(results.ToArray(), command.GroupId, revokeInterval);
            }
            return results.ToArray();
        }

        /// <summary>
        /// 发送涩图，然后撤回
        /// </summary>
        /// <param name="command"></param>
        /// <param name="setuContents"></param>
        /// <param name="revokeInterval">撤回延时，0表示不撤回</param>
        /// <param name="isAt"></param>
        /// <returns></returns>
        private static async Task<BaseResult> ReplyAndRevokeAsync(this GroupCommand command, List<SetuContent> setuContents, int revokeInterval)
        {
            List<BaseContent> contentList = setuContents.ToBaseContent().SetDefaultImage();
            BaseResult result = await command.ReplyGroupMessageWithQuoteAsync(contentList);
            Task revokeTask = command.RevokeGroupMessageAsync(result, command.GroupId, revokeInterval);
            return result;
        }


        /// <summary>
        /// 发送消息，然后撤回
        /// </summary>
        /// <param name="command"></param>
        /// <param name="contentList"></param>
        /// <param name="revokeInterval">撤回延时，0表示不撤回</param>
        /// <param name="isAt"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyAndRevokeAsync(this GroupCommand command, List<BaseContent> contentList, int revokeInterval)
        {
            BaseResult result = await command.ReplyGroupMessageWithQuoteAsync(contentList);
            Task revokeTask = command.RevokeGroupMessageAsync(result, command.GroupId, revokeInterval);
            return result;
        }

        /// <summary>
        /// 回复一条正在处理中的提示信息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="template"></param>
        /// <param name="delay">发送模版后延迟毫秒数</param>
        /// <returns></returns>
        public static async Task ReplyProcessingMessageAsync(this GroupCommand command, string template, int delay = 1000)
        {
            if (string.IsNullOrWhiteSpace(template)) return;
            await command.ReplyGroupMessageWithAtAsync(template);
            if (delay > 0) await Task.Delay(delay);
        }

        /// <summary>
        /// 私发涩图
        /// </summary>
        /// <param name="command"></param>
        /// <param name="setuContents"></param>
        /// <returns></returns>
        public static async Task<BaseResult> SendTempSetuAsync(this GroupCommand command, List<SetuContent> setuContents)
        {
            List<BaseContent> contentList = setuContents.ToBaseContent().SetDefaultImage();
            return await command.SendTempMessageAsync(contentList);
        }

        /// <summary>
        /// 私发涩图
        /// </summary>
        /// <param name="command"></param>
        /// <param name="setuContent"></param>
        /// <param name="sendImgBehind"></param>
        /// <returns></returns>
        public static async Task<BaseResult[]> SendTempSetuAsync(this GroupCommand command, SetuContent setuContent, bool sendImgBehind)
        {
            List<BaseResult> results = new List<BaseResult>();
            List<BaseContent> msgContents = setuContent.SetuInfos ?? new();
            List<BaseContent> imgContents = setuContent.SetuImages.ToBaseContent().SetDefaultImage().ToList();

            if (sendImgBehind)
            {
                results.Add(await command.SendTempMessageAsync(msgContents));
                await Task.Delay(1000);
                results.Add(await command.SendTempMessageAsync(imgContents));
            }
            else
            {
                List<BaseContent> contentList = msgContents.Concat(imgContents).ToList();
                results.Add(await command.SendTempMessageAsync(contentList));
            }

            return results.ToArray();
        }

        /// <summary>
        /// 立即撤回消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="messageId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static async Task RevokeGroupMessageAsync(this GroupCommand command, long messageId, long groupId)
        {
            try
            {
                if (messageId == 0) return;
                await command.Session.RevokeGroupMessageAsync(groupId, messageId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群消息撤回失败");
            }
        }

        /// <summary>
        /// 延时撤回消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="results"></param>
        /// <param name="groupId"></param>
        /// <param name="revokeInterval">撤回延时，0表示不撤回</param>
        /// <returns></returns>
        public static async Task RevokeGroupMessageAsync(this GroupCommand command, BaseResult[] results, long groupId, int revokeInterval)
        {
            if (revokeInterval <= 0) return;
            foreach (BaseResult result in results)
            {
                if (result.IsFailed || result.IsSuccess == false) continue;
                Task revokeTask = command.RevokeGroupMessageAsync(result, groupId, revokeInterval);
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 延时撤回消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="result"></param>
        /// <param name="groupId"></param>
        /// <param name="revokeInterval">撤回延时，0表示不撤回</param>
        /// <returns></returns>
        public static async Task RevokeGroupMessageAsync(this GroupCommand command, BaseResult result, long groupId, int revokeInterval)
        {
            try
            {
                if (result.IsFailed) return;
                if (result.IsSuccess == false) return;
                if (revokeInterval <= 0) return;
                await Task.Delay(revokeInterval * 1000);
                await command.Session.RevokeGroupMessageAsync(groupId, result.MessageId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群消息撤回失败");
            }
        }

        /// <summary>
        /// 回复错误
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyError(this GroupCommand command, Exception ex, string message = "")
        {
            if (ex is null) return BaseResult.Undo;
            List<BaseContent> contents = ex.GetErrorContents(message);
            return await command.Session.SendGroupMessageWithQuoteAsync(command.GroupId, command.MemberId, command.MessageId, contents);
        }

        /// <summary>
        /// 回复错误
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyError(this FriendCommand command, Exception ex, string message = "")
        {
            if (ex is null) return BaseResult.Undo;
            List<BaseContent> contents = ex.GetErrorContents(message);
            return await command.Session.SendFriendMessageAsync(command.MemberId, contents);
        }

        /// <summary>
        /// 艾特并使用模版回复群消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="template"></param>
        /// <param name="defaultmsg"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyGroupTemplateWithQuoteAsync(this GroupCommand command, string template, string defaultmsg = "")
        {
            template = template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return BaseResult.Undo;
            if (template.StartsWith(" ") == false) template = " " + template;
            return await command.Session.SendGroupMessageWithQuoteAsync(command.GroupId, command.MemberId, command.MessageId, template.SplitToChainAsync());
        }

        /// <summary>
        /// 私聊并使用模版回复群消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="template"></param>
        /// <param name="defaultmsg"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyFriendTemplateAsync(this FriendCommand command, string template, string defaultmsg)
        {
            template = template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return BaseResult.Undo;
            return await command.Session.SendFriendMessageAsync(command.MemberId, template.SplitToChainAsync());
        }

        /// <summary>
        /// 引用并回复群消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyGroupMessageWithQuoteAsync(this GroupCommand command, string message)
        {
            return await command.Session.SendGroupMessageWithQuoteAsync(command.GroupId, command.MemberId, command.MessageId, message);
        }

        /// <summary>
        /// 引用并回复群消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyGroupMessageWithQuoteAsync(this GroupCommand command, List<BaseContent> contents)
        {
            return await command.Session.SendGroupMessageWithQuoteAsync(command.GroupId, command.MemberId, command.MessageId, contents);
        }

        /// <summary>
        /// 艾特并回复群消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyGroupMessageWithAtAsync(this GroupCommand command, string message)
        {
            return await command.Session.SendGroupMessageWithAtAsync(command.GroupId, command.MemberId, message);
        }

        /// <summary>
        /// 艾特并回复群消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyGroupMessageWithAtAsync(this GroupCommand command, List<BaseContent> contents)
        {
            return await command.Session.SendGroupMessageWithAtAsync(command.GroupId, command.MemberId, contents);
        }

        /// <summary>
        /// 发送临时消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static async Task<BaseResult> SendTempMessageAsync(this GroupCommand command, List<BaseContent> contents)
        {
            return await command.Session.SendTempMessageAsync(command.GroupId, command.MemberId, contents);
        }

        /// <summary>
        /// 发送好友消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyFriendMessageAsync(this FriendCommand command, string message)
        {
            return await command.Session.SendFriendMessageAsync(command.MemberId, message);
        }

        /// <summary>
        /// 发送好友消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static async Task<BaseResult> ReplyFriendMessageAsync(this FriendCommand command, List<BaseContent> contents)
        {
            return await command.Session.SendFriendMessageAsync(command.MemberId, contents);
        }

        /// <summary>
        /// 根据关键词模糊搜索相关的可用指令
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static string GetSimilarGroupCommandStrs(string keyword)
        {
            List<string> groupCommands = GetSimilarGroupCommands(keyword);
            if (groupCommands.Count == 0) return string.Empty;
            StringBuilder builder = new StringBuilder();
            foreach (string command in groupCommands)
            {
                if (builder.Length > 0) builder.Append('/');
                builder.Append($"{BotConfig.DefaultPrefix}{command}");
            }
            return builder.ToString();
        }

        private static List<string> GetSimilarGroupCommands(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return new();
            string splitString = keyword.ToLower().Replace(" ", string.Empty).Trim();
            List<char> charList = splitString.ToList().Distinct().ToList();
            List<string> similarCommands = new List<string>();
            List<string> groupCommands = GetGroupCommands();
            foreach (char c in charList)
            {
                foreach (string command in groupCommands)
                {
                    if (command.Contains(c)) similarCommands.Add(command);
                }
            }
            return similarCommands.Distinct().ToList();
        }

        private static List<string> GetGroupCommands()
        {
            List<string> returnList = new List<string>();
            foreach (var item in HandlerInvokers.GroupCommands)
            {
                returnList.AddRange(item.Commands);
            }
            return returnList.Distinct().ToList();
        }



    }
}
