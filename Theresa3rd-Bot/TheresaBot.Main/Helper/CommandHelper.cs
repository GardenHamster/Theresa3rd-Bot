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
        public static async Task ReplyGroupSetuAndRevokeAsync(this GroupCommand command, SetuContent setuContent, int revokeInterval, bool sendImgBehind, bool isAt = true)
        {
            int[] msgIdArr = await command.ReplyGroupMessageAndRevokeAsync(setuContent, revokeInterval, sendImgBehind, isAt);
            if (msgIdArr.Where(o => o < 0).Any() && BotConfig.PixivConfig.ImgResend != ResendType.None)
            {
                await Task.Delay(1000);
                SetuContent resendContent = setuContent.ToResendContent(BotConfig.PixivConfig.ImgResend);
                msgIdArr = await command.ReplyGroupMessageAndRevokeAsync(resendContent, revokeInterval, sendImgBehind, isAt);
            }
        }

        public static async Task<int[]> ReplyGroupMessageAndRevokeAsync(this GroupCommand command, SetuContent setuContent, int revokeInterval, bool sendImgBehind, bool isAt = false)
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

        public static async Task<int[]> ReplyTempMessageAsync(this GroupCommand command, SetuContent setuContent, bool sendImgBehind)
        {
            try
            {
                if (setuContent is null) return new int[0];
                List<int> msgIds = new List<int>();
                List<BaseContent> msgContents = setuContent.SetuInfos ?? new();
                List<BaseContent> imgContents = setuContent.SetuImages.ToLocalImageContent().Cast<BaseContent>().ToList();

                if (sendImgBehind)
                {
                    msgIds.Add(await command.ReplyTempMessageAsync(msgContents));
                    await Task.Delay(1000);
                    msgIds.Add(await command.ReplyTempMessageAsync(imgContents));
                }
                else
                {
                    var contentList = msgContents.Concat(imgContents).ToList();
                    msgIds.Add(await command.ReplyTempMessageAsync(contentList));
                }
                return msgIds.ToArray();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SendTempSetuAsync异常");
                return new[] { 0 };
            }
        }

        public static async Task ReplyGroupSaucenaoAndRevokeAsync(this GroupCommand command, List<SetuContent> setuContents, int revokeInterval, bool isAt = true)
        {
            List<BaseContent> contentList = setuContents.ToBaseContent();
            int msgId = await command.ReplyGroupMessageAndRevokeAsync(contentList, revokeInterval, isAt);
            if (msgId < 0 && BotConfig.PixivConfig.ImgResend != ResendType.None)
            {
                await Task.Delay(1000);
                List<SetuContent> resendContents = setuContents.ToResendContent(BotConfig.PixivConfig.ImgResend);
                await command.ReplyGroupMessageAndRevokeAsync(resendContents.ToBaseContent(), revokeInterval, isAt);
            }
        }

        public static async Task ReplyTempSaucenaoAsync(this GroupCommand command, List<SetuContent> setuContents)
        {
            List<BaseContent> contentList = setuContents.ToBaseContent();
            int msgId = await command.ReplyTempMessageAsync(contentList);
            if (msgId < 0 && BotConfig.PixivConfig.ImgResend != ResendType.None)
            {
                await Task.Delay(1000);
                List<SetuContent> resendContents = setuContents.ToResendContent(BotConfig.PixivConfig.ImgResend);
                await command.ReplyTempMessageAsync(resendContents.ToBaseContent());
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
