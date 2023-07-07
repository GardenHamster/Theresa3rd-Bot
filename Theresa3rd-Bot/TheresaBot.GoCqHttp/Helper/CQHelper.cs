using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Command;
using TheresaBot.GoCqHttp.Common;
using TheresaBot.GoCqHttp.Plugin;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;

namespace TheresaBot.GoCqHttp.Helper
{
    public static class CQHelper
    {
        public static CqWsSession Session;

        public static async Task ConnectGoCqHttp()
        {
            try
            {
                LogHelper.Info("尝试连接到go-cqhttp...");
                CqWsSessionOptions options = new CqWsSessionOptions()
                {
                    BaseUri = new Uri($"ws://{CQConfig.Host}:{CQConfig.Port}"),
                    UseApiEndPoint = true,
                    UseEventEndPoint = true,
                };
                Session = new CqWsSession(options);
                Session.UsePlugin(new GroupMessagePlugin());
                Session.UsePlugin(new PrivateMessagePlugin());
                await Session.StartAsync();
                LogHelper.Info("已成功连接到go-cqhttp...");
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "连接到go-cqhttp失败");
                throw;
            }
        }

        /// <summary>
        /// 加载GoCqHttp配置
        /// </summary>
        public static void LoadAppSettings(IConfiguration configuration)
        {
            CQConfig.ConnectionString = configuration["Database:ConnectionString"];
            CQConfig.Host = configuration["GoCqHttp:host"];
            CQConfig.Port = Convert.ToInt32(configuration["GoCqHttp:port"]);
        }

        /// <summary>
        /// 获取机器人信息
        /// </summary>
        /// <returns></returns>
        public static async Task LoadBotProfileAsync()
        {
            try
            {
                var result = await Session.GetLoginInformationAsync();
                if (result is null) throw new Exception("Bot名片获取失败");
                CQConfig.BotQQ = result?.UserId ?? 0;
                CQConfig.BotName = result?.Nickname ?? "Bot";
                LogHelper.Info($"Bot名片获取完毕，QQNumber={CQConfig.BotQQ}，Nickname={result?.Nickname ?? ""}");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Bot名片获取异常");
            }
        }

        public static async Task SendStartUpMessageAsync()
        {
            await Task.Delay(3000);
            CqMessage welcomeMessage = new CqMessage(BusinessHelper.GetStartUpMessage());
            foreach (var memberId in BotConfig.PermissionsConfig.SuperManagers)
            {
                Session.SendPrivateMessage(memberId, welcomeMessage);
                await Task.Delay(1000);
            }
        }

        public static async Task ReplyRelevantCommandsAsync(string instruction, long groupId, long memberId)
        {
            try
            {
                string similarCommands = CommandHelper.GetSimilarGroupCommandStrs(instruction);
                if (string.IsNullOrWhiteSpace(similarCommands)) return;
                List<CqMsg> msgList = new List<CqMsg>();
                msgList.Add(new CqAtMsg(memberId));
                msgList.Add(new CqTextMsg($"不存在的指令，你想要输入的指令是不是【{similarCommands}】?"));
                await Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="handler"></param>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static CQGroupCommand CheckCommand(this string instruction, CommandHandler<GroupCommand> handler, ICqActionSession session, CqGroupMessagePostContext args)
        {
            if (handler.Commands is null || handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (instruction.CheckCommand(handler, session, args, command) is { } botCommand) return botCommand;
            }
            return null;
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="handler"></param>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="command"></param>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private static CQGroupCommand CheckCommand(this string instruction, CommandHandler<GroupCommand> handler, ICqActionSession session, CqGroupMessagePostContext args, string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;
            if (instruction.ToUpper().StartsWith(command.ToUpper()) == false) return null;
            return new(handler, session, args, instruction, command);
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="handler"></param>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static CQFriendCommand CheckCommand(this string instruction, CommandHandler<FriendCommand> handler, ICqActionSession session, CqPrivateMessagePostContext args)
        {
            if (handler.Commands is null || handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (instruction.CheckCommand(handler, session, args, command) is { } botCommand) return botCommand;
            }
            return null;
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="handler"></param>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="command"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private static CQFriendCommand CheckCommand(this string instruction, CommandHandler<FriendCommand> handler, ICqActionSession session, CqPrivateMessagePostContext args, string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;
            if (instruction.StartsWith(command) == false) return null;
            return new(handler, session, args, instruction, command);
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="handler"></param>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public static CQGroupQuoteCommand CheckCommand(this string instruction, CommandHandler<GroupQuoteCommand> handler, ICqActionSession session, CqGroupMessagePostContext args)
        {
            if (handler.Commands is null || handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (instruction.CheckCommand(handler, session, args, command) is { } botCommand) return botCommand;
            }
            return null;
        }

        /// <summary>
        /// 检查是一条消息是否一条有效指令，如果是返回一个指令对象
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="handler"></param>
        /// <param name="session"></param>
        /// <param name="args"></param>
        /// <param name="command"></param>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        private static CQGroupQuoteCommand CheckCommand(this string instruction, CommandHandler<GroupQuoteCommand> handler, ICqActionSession session, CqGroupMessagePostContext args, string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;
            if (instruction.ToUpper().StartsWith(command.ToUpper()) == false) return null;
            return new(handler, session, args, instruction, command);
        }

        /// <summary>
        /// 将通用消息转为CQ消息
        /// </summary>
        /// <param name="chatContents"></param>
        /// <returns></returns>
        public static CqMsg[] ToCQMessageAsync(this List<BaseContent> chatContents)
        {
            BaseContent lastPlainContent = chatContents.OfType<PlainContent>().LastOrDefault();
            int lastPlainIndex = lastPlainContent is null ? -1 : chatContents.LastIndexOf(lastPlainContent);
            List<CqMsg> chatList = new List<CqMsg>();
            for (int i = 0; i < chatContents.Count; i++)
            {
                BaseContent content = chatContents[i];
                bool isNewLine = lastPlainIndex > 0 && i < lastPlainIndex;
                CqMsg chatMessage = content.ToCQMessageAsync(isNewLine);
                if (chatMessage is not null) chatList.Add(chatMessage);
            }
            return chatList.ToArray();
        }

        /// <summary>
        /// 将通用消息转为CQ消息
        /// </summary>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public static CqMsg ToCQMessageAsync(this BaseContent chatContent, bool isNewLine)
        {
            if (chatContent is PlainContent plainContent)
            {
                string message = plainContent.Content + (isNewLine && plainContent.NewLine ? "\r\n" : string.Empty);
                return string.IsNullOrEmpty(plainContent.Content) ? null : new CqTextMsg(message);
            }
            if (chatContent is LocalImageContent localImageContent)
            {
                return localImageContent.FileInfo is null ? null : CqImageMsg.FromFile(localImageContent.FileInfo.FullName);
            }
            if (chatContent is WebImageContent webImageContent)
            {
                return string.IsNullOrWhiteSpace(webImageContent.Url) ? null : new CqImageMsg(webImageContent.Url);
            }
            return null;
        }


    }
}
