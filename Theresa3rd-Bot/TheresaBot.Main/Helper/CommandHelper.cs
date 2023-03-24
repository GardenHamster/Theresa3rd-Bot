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
        public static async Task ReplyGroupSetuAndRevokeAsync(this GroupCommand command, SetuContent setuContent, int revokeInterval, bool sendImgBehind, bool isAt = false)
        {
            int[] msgIdArr = await command.ReplyGroupMessageAndRevokeAsync(setuContent, revokeInterval, sendImgBehind, isAt);
            if (msgIdArr.Where(o => o < 0).Any() && BotConfig.PixivConfig.ImgResend != ResendType.None)
            {
                await Task.Delay(1000);
                SetuContent resendContent = setuContent.ToResendContent(BotConfig.PixivConfig.ImgResend);
                msgIdArr = await command.ReplyGroupMessageAndRevokeAsync(resendContent, revokeInterval, sendImgBehind, isAt);
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
