using Mirai.CSharp.Builders;
using Mirai.CSharp.HttpApi.Builder;
using Mirai.CSharp.HttpApi.Invoking;
using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Options;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Infos;
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
                                                               .AddHandler<FriendApplyEvent>()
                                                               .AddHandler<FriendMessageEvent>()
                                                               .AddHandler<GroupMessageEvent>()
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
                await Session.ConnectAsync(BotConfig.BotQQ);
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
            BotConfig.BotQQ = Convert.ToInt64(configuration["Mirai:botQQ"]);
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
                BotConfig.BotName = profile?.Nickname ?? "Bot";
                LogHelper.Info($"Bot名片获取完毕，QQNumber={Session.QQNumber}，Nickname={profile?.Nickname ?? ""}");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Bot名片获取失败");
            }
        }

        /// <summary>
        /// 获取群列表
        /// </summary>
        /// <returns></returns>
        public static async Task LoadGroupAsync()
        {
            try
            {
                var groupInfos = await Session.GetGroupListAsync();
                if (groupInfos is null) throw new Exception("群列表获取失败");
                BotConfig.GroupInfos = groupInfos.Select(o => new GroupInfos(o.Id, o.Name)).ToList();
                var availableIds = groupInfos.Select(o => o.Id).ToList();
                var acceptIds = BotConfig.PermissionsConfig.AcceptGroups;
                var groupCount = BotConfig.GroupInfos.Count;
                int acceptCount = acceptIds.Where(o => availableIds.Contains(o)).Count();
                var availablCount = acceptIds.Contains(0) ? groupCount : acceptCount;
                LogHelper.Info($"群列表获取完毕，共获取群号 {groupCount} 个，其中已启用群号 {availablCount} 个");
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群列表获取失败");
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
        /// <param name="contents"></param>
        /// <returns></returns>
        public static async Task<IChatMessage[]> ToMiraiMessageAsync(this List<BaseContent> contents, UploadTarget uploadTarget)
        {
            List<IChatMessage> miraiMsgs = new List<IChatMessage>();
            for (int i = 0; i < contents.Count; i++)
            {
                BaseContent content = contents[i];
                if (content is PlainContent plainContent)
                {
                    plainContent.FormatNewLine(i == contents.Count - 1);
                }
                IChatMessage chatMessage = await content.ToMiraiMessageAsync(uploadTarget);
                if (chatMessage is not null) miraiMsgs.Add(chatMessage);
            }
            return miraiMsgs.ToArray();
        }

        /// <summary>
        /// 将通用消息转为Mirai消息
        /// </summary>
        /// <param name="chatContent"></param>
        /// <returns></returns>
        public static async Task<IChatMessage> ToMiraiMessageAsync(this BaseContent chatContent, UploadTarget uploadTarget)
        {
            if (chatContent is PlainContent plainContent)
            {
                return string.IsNullOrEmpty(plainContent.Content) ? null : new PlainMessage(plainContent.Content);
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
