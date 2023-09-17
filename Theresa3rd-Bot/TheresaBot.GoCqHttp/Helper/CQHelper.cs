using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using TheresaBot.GoCqHttp.Common;
using TheresaBot.GoCqHttp.Plugin;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;

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
                Session.UsePlugin(new GroupMemberIncreasePlugin());
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
        /// 将通用消息转为CQ消息
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static CqMsg[] ToCQMessageAsync(this List<BaseContent> contents)
        {
            List<CqMsg> cqMsgs = new List<CqMsg>();
            for (int i = 0; i < contents.Count; i++)
            {
                BaseContent content = contents[i];
                if (content is PlainContent plainContent)
                {
                    plainContent.FormatNewLine(i == contents.Count - 1);
                }
                CqMsg chatMessage = content.ToCQMessageAsync();
                if (chatMessage is not null) cqMsgs.Add(chatMessage);
            }
            return cqMsgs.ToArray();
        }

        /// <summary>
        /// 将通用消息转为CQ消息
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static CqMsg ToCQMessageAsync(this BaseContent content)
        {
            if (content is PlainContent plainContent)
            {
                return string.IsNullOrEmpty(plainContent.Content) ? null : new CqTextMsg(plainContent.Content);
            }
            if (content is LocalImageContent localImageContent)
            {
                return localImageContent.FileInfo is null ? null : ToCQMessageAsync(localImageContent.FileInfo);
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
        public static CqMsg ToCQMessageAsync(FileInfo fileInfo)
        {
            using FileStream fileStream = File.OpenRead(fileInfo.FullName);
            return CqImageMsg.FromStream(fileStream);
        }


    }
}
