using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using EleCho.GoCqHttpSdk.Post;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.GoCqHttp.Result;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Result;
using TheresaBot.Main.Type;

namespace TheresaBot.GoCqHttp.Command
{
    public class CQGroupCommand : GroupCommand
    {
        private ICqActionSession Session { get; init; }

        private CqGroupMessagePostContext Args { get; init; }

        public override PlatformType PlatformType { get; } = PlatformType.GoCQHttp;

        public override long MsgId => Args.MessageId;

        public override long GroupId => Args.GroupId;

        public override long MemberId => Args.Sender.UserId;

        public CQGroupCommand(ICqActionSession session, CqGroupMessagePostContext args, CommandType commandType, string instruction, string command)
            : base(commandType, instruction, command)
        {
            Args = args;
            Session = session;
        }

        public CQGroupCommand(CommandHandler<GroupCommand> invoker, ICqActionSession session, CqGroupMessagePostContext args, string instruction, string command)
            : base(invoker, instruction, command)
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

        public override async Task<BaseResult> ReplyGroupMessageAsync(string message, bool isAt = false)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            if (isAt) msgList.Add(new CqAtMsg(MemberId));
            msgList.AddRange(new CqMessage(message));
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> ReplyGroupMessageAsync(List<BaseContent> chainList, bool isAt = false)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            if (isAt) msgList.Add(new CqAtMsg(MemberId));
            msgList.AddRange(chainList.ToCQMessageAsync());
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> ReplyGroupMessageWithAtAsync(string plainMsg)
        {
            if (plainMsg.StartsWith(" ") == false) plainMsg = " " + plainMsg;
            List<CqMsg> msgList = new List<CqMsg>();
            msgList.Add(new CqAtMsg(MemberId));
            msgList.Add(new CqTextMsg(plainMsg));
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> ReplyGroupMessageWithAtAsync(params BaseContent[] chainArr)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            msgList.Add(new CqAtMsg(MemberId));
            msgList.AddRange(chainArr.ToList().ToCQMessageAsync());
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> ReplyGroupMessageWithAtAsync(List<BaseContent> chainList)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            msgList.Add(new CqAtMsg(MemberId));
            msgList.AddRange(chainList.ToCQMessageAsync());
            var result = await Session.SendGroupMessageAsync(GroupId, new CqMessage(msgList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> SendTempMessageAsync(List<BaseContent> contentList)
        {
            CqMsg[] msgArr = contentList.ToCQMessageAsync();
            var result = await Session.SendPrivateMessageAsync(MemberId, GroupId, new CqMessage(msgArr));
            return new CQResult(result);
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
