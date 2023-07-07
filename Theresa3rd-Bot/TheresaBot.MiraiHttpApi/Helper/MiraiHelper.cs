using Mirai.CSharp.Builders;
using Mirai.CSharp.HttpApi.Builder;
using Mirai.CSharp.HttpApi.Invoking;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Options;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.MiraiHttpApi.Command;
using TheresaBot.MiraiHttpApi.Common;
using TheresaBot.MiraiHttpApi.Event;

namespace TheresaBot.MiraiHttpApi.Helper
{
    public static class MiraiHelper
    {
        public static IServiceProvider Services;

        public static IServiceScope Scope;

        public static IMiraiHttpSession Session;

        public static async Task ConnectMirai()
        {
            try
            {
                LogHelper.Info("尝试连接到mirai-console...");
                Services = new ServiceCollection().AddMiraiBaseFramework()
                                                               .Services
                                                               .AddDefaultMiraiHttpFramework()
                                                               .AddInvoker<MiraiHttpMessageHandlerInvoker>()
                                                               .AddHandler<BotInvitedJoinGroupEvent>()
                                                               .AddHandler<FriendMessageEvent>()
                                                               .AddHandler<GroupApplyEvent>()
                                                               .AddHandler<GroupMessageEvent>()
                                                               .AddHandler<NewFriendApplyEvent>()
                                                               .AddHandler<GroupMemberJoinedEvent>()
                                                               .AddHandler<DisconnectedEvent>()
                                                               .AddClient<MiraiHttpSession>()
                                                               .Services
                                                               .Configure<MiraiHttpSessionOptions>(options =>
                                                               {
                                                                   options.Host = MiraiConfig.Host;
                                                                   options.Port = MiraiConfig.Port;
                                                                   options.AuthKey = MiraiConfig.AuthKey;
                                                                   options.SuppressAwaitMessageInvoker = true;
                                                               })
                                                               .AddLogging()
                                                               .BuildServiceProvider();
                Scope = Services.CreateAsyncScope();
                Services = Scope.ServiceProvider;
                Session = Services.GetRequiredService<IMiraiHttpSession>();
                await Session.ConnectAsync(MiraiConfig.BotQQ);
                LogHelper.Info("已成功连接到mirai-console...");
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "连接到mirai-console失败");
                throw;
            }
        }

        /// <summary>
        /// 加载MiraiHttpApi配置
        /// </summary>
        public static void LoadAppSettings(IConfiguration configuration)
        {
            MiraiConfig.ConnectionString = configuration["Database:ConnectionString"];
            MiraiConfig.Host = configuration["Mirai:host"];
            MiraiConfig.Port = Convert.ToInt32(configuration["Mirai:port"]);
            MiraiConfig.AuthKey = configuration["Mirai:authKey"];
            MiraiConfig.BotQQ = Convert.ToInt64(configuration["Mirai:botQQ"]);
        }

        /// <summary>
        /// 获取机器人信息
        /// </summary>
        /// <returns></returns>
        public static async Task LoadBotProfileAsync()
        {
            try
            {
                IBotProfile profile = await Session.GetBotProfileAsync();
                if (profile is null) throw new Exception("Bot名片获取失败");
                MiraiConfig.BotName = profile?.Nickname ?? "Bot";
                LogHelper.Info($"Bot名片获取完毕，QQNumber={Session.QQNumber}，Nickname={profile?.Nickname ?? ""}");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Bot名片获取失败");
            }
        }

        public static async Task SendStartUpMessageAsync()
        {
            await Task.Delay(3000);
            IChatMessage welcomeMessage = new PlainMessage(BusinessHelper.GetStartUpMessage());
            foreach (var memberId in BotConfig.PermissionsConfig.SuperManagers)
            {
                await Session.SendFriendMessageAsync(memberId, welcomeMessage);
                await Task.Delay(1000);
            }
        }

        public static async Task ReplyRelevantCommandsAsync(string instruction, long groupId, long memberId)
        {
            try
            {
                String similarCommands = CommandHelper.GetSimilarGroupCommandStrs(instruction);
                if (string.IsNullOrWhiteSpace(similarCommands)) return;
                List<IChatMessage> msgList = new List<IChatMessage>();
                msgList.Add(new AtMessage(memberId));
                msgList.Add(new PlainMessage($"不存在的指令，你想要输入的指令是不是【{similarCommands}】?"));
                await Session.SendGroupMessageAsync(groupId, msgList.ToArray());
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
        public static MiraiGroupCommand CheckCommand(this string instruction, CommandHandler<GroupCommand> handler, IMiraiHttpSession session, IGroupMessageEventArgs args, long groupId, long memberId)
        {
            if (handler.Commands is null || handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (instruction.CheckCommand(handler, session, args, command, groupId, memberId) is { } botCommand) return botCommand;
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
        private static MiraiGroupCommand CheckCommand(this string instruction, CommandHandler<GroupCommand> handler, IMiraiHttpSession session, IGroupMessageEventArgs args, string command, long groupId, long memberId)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;
            if (instruction.ToUpper().StartsWith(command.ToUpper()) == false) return null;
            return new(handler, session, args, instruction, command, groupId, memberId);
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
        public static MiraiFriendCommand CheckCommand(this string instruction, CommandHandler<FriendCommand> handler, IMiraiHttpSession session, IFriendMessageEventArgs args, long memberId)
        {
            if (handler.Commands is null || handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (instruction.CheckCommand(handler, session, args, command, memberId) is { } botCommand) return botCommand;
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
        private static MiraiFriendCommand CheckCommand(this string instruction, CommandHandler<FriendCommand> handler, IMiraiHttpSession session, IFriendMessageEventArgs args, string command, long memberId)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;
            if (instruction.StartsWith(command) == false) return null;
            return new(handler, session, args, instruction, command, memberId);
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
        public static MiraiGroupQuoteCommand CheckCommand(this string instruction, CommandHandler<GroupQuoteCommand> handler, IMiraiHttpSession session, IGroupMessageEventArgs args, long groupId, long memberId)
        {
            if (handler.Commands is null || handler.Commands.Count == 0) return null;
            foreach (string command in handler.Commands)
            {
                if (instruction.CheckCommand(handler, session, args, command, groupId, memberId) is { } botCommand) return botCommand;
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
        private static MiraiGroupQuoteCommand CheckCommand(this string instruction, CommandHandler<GroupQuoteCommand> handler, IMiraiHttpSession session, IGroupMessageEventArgs args, string command, long groupId, long memberId)
        {
            if (string.IsNullOrWhiteSpace(command)) return null;
            if (instruction.ToUpper().StartsWith(command.ToUpper()) == false) return null;
            return new(handler, session, args, instruction, command, groupId, memberId);
        }

        /// <summary>
        /// 获取消息id
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static long GetMessageId(this IGroupMessageEventArgs args)
        {
            try
            {
                SourceMessage sourceMessage = (SourceMessage)args.Chain.First();
                return sourceMessage.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "GetMessageId异常");
                return 0;
            }
        }

        /// <summary>
        /// 获取消息id
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int GetMessageId(this IFriendMessageEventArgs args)
        {
            try
            {
                SourceMessage sourceMessage = (SourceMessage)args.Chain.First();
                return sourceMessage.Id;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "GetMessageId异常");
                return 0;
            }
        }

        /// <summary>
        /// 将通用消息转为Mirai消息
        /// </summary>
        /// <param name="chatContents"></param>
        /// <returns></returns>
        public static async Task<IChatMessage[]> ToMiraiMessageAsync(this List<BaseContent> chatContents, UploadTarget uploadTarget)
        {
            BaseContent lastPlainContent = chatContents.Where(o => o is PlainContent).LastOrDefault();
            int lastPlainIndex = lastPlainContent is null ? -1 : chatContents.LastIndexOf(lastPlainContent);
            List<IChatMessage> chatList = new List<IChatMessage>();
            for (int i = 0; i < chatContents.Count; i++)
            {
                BaseContent content = chatContents[i];
                bool isNewLine = lastPlainIndex > 0 && i < lastPlainIndex;
                IChatMessage chatMessage = await content.ToMiraiMessageAsync(uploadTarget, isNewLine);
                if (chatMessage is not null) chatList.Add(chatMessage);
            }
            return chatList.ToArray();
        }

        /// <summary>
        /// 将通用消息转为Mirai消息
        /// </summary>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public static async Task<IChatMessage> ToMiraiMessageAsync(this BaseContent chatContent, UploadTarget uploadTarget, bool isNewLine)
        {
            if (chatContent is PlainContent plainContent)
            {
                string message = plainContent.Content + (isNewLine && plainContent.NewLine ? "\r\n" : string.Empty);
                return string.IsNullOrEmpty(plainContent.Content) ? null : new PlainMessage(message);
            }
            if (chatContent is LocalImageContent localImageContent)
            {
                return localImageContent.FileInfo is null ? null : await UploadPictureAsync(localImageContent, uploadTarget);
            }
            if (chatContent is WebImageContent webImageContent)
            {
                return string.IsNullOrWhiteSpace(webImageContent.Url) ? null : new ImageMessage(null, webImageContent.Url, null);
            }
            return null;
        }

        /// <summary>
        /// 上传图片，返回mirai图片消息
        /// </summary>
        /// <param name="imageContent"></param>
        /// <returns></returns>
        private static async Task<IChatMessage> UploadPictureAsync(LocalImageContent imageContent, UploadTarget target)
        {
            FileInfo imageFile = imageContent.FileInfo;
            if (imageFile is null || imageFile.Exists == false) return null;
            return (IChatMessage)await Session.UploadPictureAsync(target, imageFile.FullName);
        }

    }
}
