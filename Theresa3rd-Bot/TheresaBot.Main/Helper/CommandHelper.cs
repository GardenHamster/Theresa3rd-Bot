using System.Text;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Invoker;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Helper
{
    public static class CommandHelper
    {
        public static async Task ReplyProcessingMessageAsync(this GroupCommand command, string template)
        {
            if (string.IsNullOrWhiteSpace(template) == false) return;
            await command.ReplyGroupTemplateWithAtAsync(template);
        }

        public static async Task<int> ReplyGroupMessageAsync(this GroupCommand command, List<BaseContent> contentList, int revokeInterval, bool sendImgBehind, bool isAt = true)
        {
            return await command.ReplyAndRevokeAsync(contentList, revokeInterval, isAt);
        }

        public static async Task<int[]> ReplyGroupSetuAsync(this GroupCommand command, SetuContent setuContent, int revokeInterval, bool sendImgBehind, bool isAt = true)
        {
            int[] msgIds = await command.ReplyAndRevokeAsync(setuContent, revokeInterval, sendImgBehind, isAt);
            if (msgIds.Where(o => o < 0).Any() && BotConfig.PixivConfig.ImgResend != ResendType.None)
            {
                await Task.Delay(1000);
                SetuContent resendContent = setuContent.ToResendContent(BotConfig.PixivConfig.ImgResend);
                msgIds = await command.ReplyAndRevokeAsync(resendContent, revokeInterval, sendImgBehind, isAt);
            }
            return msgIds;
        }

        public static async Task<int> ReplyGroupSetuAsync(this GroupCommand command, List<SetuContent> setuContents, int revokeInterval, bool isAt = true)
        {
            int msgId = await command.ReplyAndRevokeAsync(setuContents, revokeInterval, isAt);
            if (msgId < 0 && BotConfig.PixivConfig.ImgResend != ResendType.None)
            {
                await Task.Delay(1000);
                List<SetuContent> resendContents = setuContents.ToResendContent(BotConfig.PixivConfig.ImgResend);
                msgId = await command.ReplyAndRevokeAsync(setuContents, revokeInterval, isAt);
            }
            return msgId;
        }

        private static async Task<int> ReplyAndRevokeAsync(this GroupCommand command, List<BaseContent> contentList, int revokeInterval, bool isAt = false)
        {
            var msgId = await command.ReplyGroupMessageAsync(contentList, isAt);
            if (revokeInterval > 0)
            {
                Task revokeTask = command.RevokeGroupMessageAsync(msgId, command.GroupId, revokeInterval);
            }
            return msgId;
        }

        private static async Task<int[]> ReplyAndRevokeAsync(this GroupCommand command, SetuContent setuContent, int revokeInterval, bool sendImgBehind, bool isAt = false)
        {
            List<int> msgIds = new List<int>();
            List<BaseContent> msgContents = setuContent.SetuInfos ?? new();
            List<BaseContent> imgContents = setuContent.SetuImages.ToLocalImageContent().Cast<BaseContent>().ToList();

            if (sendImgBehind)
            {
                int workMsgId = await command.ReplyGroupMessageAsync(msgContents, isAt);
                await Task.Delay(1000);
                int imgMsgId = await command.ReplyGroupMessageAsync(imgContents, false);
                msgIds.Add(workMsgId);
                msgIds.Add(imgMsgId);
            }
            else
            {
                var contentList = msgContents.Concat(imgContents).ToList();
                msgIds.Add(await command.ReplyGroupMessageAsync(contentList, isAt));
            }

            if (revokeInterval > 0)
            {
                Task revokeTask = command.RevokeGroupMessageAsync(msgIds, command.GroupId, revokeInterval);
            }
            return msgIds.ToArray();
        }

        private static async Task<int> ReplyAndRevokeAsync(this GroupCommand command, List<SetuContent> setuContents, int revokeInterval, bool isAt = false)
        {
            var contentList = new List<BaseContent>();
            foreach (var setuContent in setuContents)
            {
                contentList.AddRange(setuContent.SetuInfos ?? new());
                contentList.AddRange(setuContent.SetuImages.ToLocalImageContent().Cast<BaseContent>());
            }

            var msgId = await command.ReplyGroupMessageAsync(contentList, isAt);
            if (revokeInterval > 0)
            {
                Task revokeTask = command.RevokeGroupMessageAsync(msgId, command.GroupId, revokeInterval);
            }
            return msgId;
        }

        public static async Task SendTempSetuAsync(this GroupCommand command, List<SetuContent> setuContents)
        {
            List<BaseContent> contentList = setuContents.ToBaseContent();
            int msgId = await command.SendTempMessageAsync(contentList);
            if (msgId < 0 && BotConfig.PixivConfig.ImgResend != ResendType.None)
            {
                await Task.Delay(1000);
                List<SetuContent> resendContents = setuContents.ToResendContent(BotConfig.PixivConfig.ImgResend);
                await command.SendTempMessageAsync(resendContents.ToBaseContent());
            }
        }

        public static async Task<int[]> SendTempSetuAsync(this GroupCommand command, SetuContent setuContent, bool sendImgBehind)
        {
            try
            {
                if (setuContent is null) return new int[0];
                List<int> msgIds = new List<int>();
                List<BaseContent> msgContents = setuContent.SetuInfos ?? new();
                List<BaseContent> imgContents = setuContent.SetuImages.ToLocalImageContent().Cast<BaseContent>().ToList();

                if (sendImgBehind)
                {
                    msgIds.Add(await command.SendTempMessageAsync(msgContents));
                    await Task.Delay(1000);
                    msgIds.Add(await command.SendTempMessageAsync(imgContents));
                }
                else
                {
                    var contentList = msgContents.Concat(imgContents).ToList();
                    msgIds.Add(await command.SendTempMessageAsync(contentList));
                }
                return msgIds.ToArray();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SendTempSetuAsync异常");
                return new[] { 0 };
            }
        }

        public static async Task RevokeGroupMessageAsync(this GroupCommand command, List<int> messageIds, long groupId, int revokeInterval = 0)
        {
            foreach (int messageId in messageIds)
            {
                await command.RevokeGroupMessageAsync(messageId, groupId, revokeInterval);
                await Task.Delay(500);
            }
        }

        public static string GetSimilarGroupCommandStrs(string keyword)
        {
            List<string> groupCommands = GetSimilarGroupCommands(keyword);
            if (groupCommands.Count == 0) return string.Empty;
            StringBuilder builder = new StringBuilder();
            foreach (string command in groupCommands)
            {
                if (builder.Length > 0) builder.Append('/');
                builder.Append($"{BotConfig.GeneralConfig.Prefix}{command}");
            }
            return builder.ToString();
        }

        public static List<string> GetSimilarGroupCommands(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return new();
            string splitString = keyword.ToLower().Replace(" ", string.Empty).Trim();
            List<char> charList = splitString.ToList().Distinct().ToList();
            charList.RemoveAll(o => o.ToString() == BotConfig.GeneralConfig.Prefix);
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

        public static List<string> GetGroupCommands()
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
