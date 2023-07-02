using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Type;

namespace TheresaBot.GoCqHttp.Command
{
    public class CQGroupCommand : GroupCommand
    {
        private ICqActionSession Session;
        private CqGroupMessagePostContext Args;
        public override PlatformType PlatformType { get; } = PlatformType.GoCQHttp;

        public CQGroupCommand(ICqActionSession session, CqGroupMessagePostContext args, CommandType commandType, string instruction, string command, long groupId, long memberId)
            : base(commandType, args.MessageId, instruction, command, groupId, memberId)
        {
            Args = args;
            Session = session;
        }

        public CQGroupCommand(CommandHandler<GroupCommand> invoker, ICqActionSession session, CqGroupMessagePostContext args, string instruction, string command, long groupId, long memberId)
            : base(invoker, args.MessageId, instruction, command, groupId, memberId)
        {
            Args = args;
            Session = session;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Message.OfType<CqImageMsg>().Select(o => o.Image).ToList();
        }

        public override long GetQuoteMessageId()
        {
            return Args.Message.OfType<CqReplyMsg>().FirstOrDefault()?.Id ?? 0;
        }

        public override async Task<long?> ReplyGroupMessageAsync(string message, bool isAt = false)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            if (isAt) msgList.Add(new CqAtMsg(MemberId));
            msgList.AddRange(new CqMessage(message));
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long?> ReplyGroupMessageAsync(List<BaseContent> chainList, bool isAt = false)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            if (isAt) msgList.Add(new CqAtMsg(MemberId));
            msgList.AddRange(chainList.ToCQMessageAsync());
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long?> ReplyGroupMessageWithAtAsync(string plainMsg)
        {
            if (plainMsg.StartsWith(" ") == false) plainMsg = " " + plainMsg;
            List<CqMsg> msgList = new List<CqMsg>();
            msgList.Add(new CqAtMsg(MemberId));
            msgList.Add(new CqTextMsg(plainMsg));
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long?> ReplyGroupMessageWithAtAsync(params BaseContent[] chainArr)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            msgList.Add(new CqAtMsg(MemberId));
            msgList.AddRange(chainArr.ToList().ToCQMessageAsync());
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long?> ReplyGroupMessageWithAtAsync(List<BaseContent> chainList)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            msgList.Add(new CqAtMsg(MemberId));
            msgList.AddRange(chainList.ToCQMessageAsync());
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long?> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "")
        {
            template = template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            if (template.StartsWith(" ") == false) template = " " + template;
            CqMsg[] msgList = template.SplitToChainAsync().ToCQMessageAsync();
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long?> SendTempMessageAsync(List<BaseContent> contentList)
        {
            CqMsg[] msgArr = contentList.ToCQMessageAsync();
            var result = await Session.SendPrivateMessageAsync(MemberId, GroupId, new CqMessage(msgArr));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task RevokeGroupMessageAsync(long msgId, long groupId)
        {
            try
            {
                if (msgId >= 0) return;
                await Session.RecallMessageAsync(msgId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群消息撤回失败");
            }
        }

        public override async Task Test()
        {
            await Task.CompletedTask;
        }


    }
}
