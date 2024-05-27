using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using TheresaBot.OneBot11.Common;
using TheresaBot.OneBot11.Plugin;
using TheresaBot.Core.Common;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Bot;
using TheresaBot.Core.Model.Content;

namespace TheresaBot.OneBot11.Helper
{
    public static class OBHelper
    {
        public static CqWsSession Session;

        public static async Task ConnectOneBot()
        {
            try
            {
                LogHelper.Info("尝试连接到OneBot11...");
                CqWsSessionOptions options = new CqWsSessionOptions()
                {
                    BaseUri = new Uri($"ws://{OBConfig.Host}:{OBConfig.Port}"),
                    AccessToken = OBConfig.AccessToken,
                    UseApiEndPoint = true,
                    UseEventEndPoint = true,
                };
                Session = new CqWsSession(options);
                Session.UsePlugin(new FriendApplyPlugin());
                Session.UsePlugin(new GroupMessagePlugin());
                Session.UsePlugin(new PrivateMessagePlugin());
                Session.UsePlugin(new GroupMemberIncreasePlugin());
                await Session.StartAsync();
                LogHelper.Info("已成功连接到OneBot11...");
            }
            catch (Exception ex)
            {
                LogHelper.FATAL(ex, "连接到OneBot11...失败");
                throw;
            }
        }

        /// <summary>
        /// 加载OneBot11配置
        /// </summary>
        public static void LoadAppSettings(IConfiguration configuration)
        {
            OBConfig.ConnectionString = configuration["Database:ConnectionString"];
            OBConfig.Host = configuration["OneBot11:host"];
            OBConfig.Port = Convert.ToInt32(configuration["OneBot11:port"]);
            OBConfig.AccessToken = configuration["OneBot11:accessToken"];
        }

        /// <summary>
        /// 获取机器人信息
        /// </summary>
        /// <returns></returns>
        public static async Task<BotInfos> GetBotInfosAsync()
        {
            var result = await Session.GetLoginInformationAsync();
            return new BotInfos(result.UserId, result.Nickname);
        }

        /// <summary>
        /// 获取群列表并缓存到BotConfig中，如果获取失败则返回null
        /// </summary>
        /// <returns></returns>
        public static async Task<GroupInfos[]> GetGroupInfosAsync()
        {
            var groupResult = await Session.GetGroupListAsync();
            return groupResult.Groups.Select(o => new GroupInfos(o.GroupId, o.GroupName)).ToArray();
        }

        /// <summary>
        /// 发送启动消息
        /// </summary>
        /// <returns></returns>
        public static async Task SendStartUpMessageAsync()
        {
            await Task.Delay(3000);
            LogHelper.Console("正在发送启动消息...");
            CqMessage welcomeMessage = new CqMessage(BusinessHelper.GetStartUpMessage());
            foreach (var memberId in BotConfig.SuperManagers)
            {
                if (memberId <= 0) continue;
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
        /// 将通用消息转为CQ消息
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static CqMsg[] ToOBMessageAsync(this List<BaseContent> contents)
        {
            List<CqMsg> cqMsgs = new List<CqMsg>();
            for (int i = 0; i < contents.Count; i++)
            {
                BaseContent content = contents[i];
                if (content is PlainContent plainContent)
                {
                    plainContent.FormatNewLine(i == contents.Count - 1);
                }
                CqMsg chatMessage = content.ToOBMessageAsync();
                if (chatMessage is not null) cqMsgs.Add(chatMessage);
            }
            return cqMsgs.ToArray();
        }

        /// <summary>
        /// 将通用消息转为CQ消息
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static CqMsg ToOBMessageAsync(this BaseContent content)
        {
            if (content is PlainContent plainContent)
            {
                return string.IsNullOrEmpty(plainContent.Content) ? null : new CqTextMsg(plainContent.Content);
            }
            if (content is LocalImageContent localImageContent)
            {
                return localImageContent.FileInfo is null ? null : ToOBMessageAsync(localImageContent.FileInfo);
            }
            if (content is WebImageContent webImageContent)
            {
                return string.IsNullOrWhiteSpace(webImageContent.Url) ? null : new CqImageMsg(webImageContent.Url);
            }
            return null;
        }

        /// <summary>
        /// 转为Base64 CQ图片消息
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static CqMsg ToOBMessageAsync(FileInfo fileInfo)
        {
            using FileStream fileStream = File.OpenRead(fileInfo.FullName);
            return CqImageMsg.FromStream(fileStream);
        }

    }
}
