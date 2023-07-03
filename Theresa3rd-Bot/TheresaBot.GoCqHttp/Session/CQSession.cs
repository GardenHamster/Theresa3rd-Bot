using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using TheresaBot.GoCqHttp.Common;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.GoCqHttp.Result;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Result;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.GoCqHttp.Session
{
    public class CQSession : BaseSession
    {
        public override PlatformType PlatformType { get; } = PlatformType.GoCQHttp;

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, string message)
        {
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(message));
            return new CQResult(result);
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, params BaseContent[] contents)
        {
            if (contents.Length == 0) return CQResult.Undo;
            CqMsg[] msgList = contents.ToList().ToCQMessageAsync();
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return CQResult.Undo;
            CqMsg[] msgList = contents.ToCQMessageAsync();
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, List<BaseContent> contents, bool isAtAll = false)
        {
            if (contents.Count == 0) return CQResult.Undo;
            List<CqMsg> msgList = new List<CqMsg>();
            if (isAtAll) msgList.Add(CqAtMsg.AtAll);
            msgList.AddRange(contents.ToCQMessageAsync());
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, List<BaseContent> contents, List<long> atMembers, bool isAtAll = false)
        {
            if (contents.Count == 0) return CQResult.Undo;
            List<CqMsg> msgList = new List<CqMsg>();
            if (isAtAll) msgList.Add(CqAtMsg.AtAll);
            if (atMembers is null) atMembers = new();
            foreach (long memberId in atMembers) msgList.Add(new CqAtMsg(memberId));
            msgList.AddRange(contents.ToCQMessageAsync());
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> SendGroupMergeAsync(long groupId, params List<BaseContent>[] contentLists)
        {
            if (contentLists.Length == 0) return CQResult.Undo;
            var nodeList = contentLists.Select(o => new CqForwardMessageNode(CQConfig.BotName, CQConfig.BotQQ, new CqMessage(o.ToCQMessageAsync()))).ToList();
            var result = await CQHelper.Session.SendGroupForwardMessageAsync(groupId, new CqForwardMessage(nodeList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> SendFriendMessageAsync(long memberId, string message)
        {
            var result = await CQHelper.Session.SendPrivateMessageAsync(memberId, new CqMessage(message));
            return new CQResult(result);
        }

        public override async Task<BaseResult> SendFriendMessageAsync(long memberId, params BaseContent[] contents)
        {
            if (contents.Length == 0) return CQResult.Undo;
            CqMsg[] msgList = contents.ToList().ToCQMessageAsync();
            var result = await CQHelper.Session.SendPrivateMessageAsync(memberId, new CqMessage(msgList));
            return new CQResult(result);
        }

        public override async Task<BaseResult> SendFriendMessageAsync(long memberId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return CQResult.Undo;
            CqMsg[] msgList = contents.ToCQMessageAsync();
            var result = await CQHelper.Session.SendPrivateMessageAsync(memberId, new CqMessage(msgList));
            return new CQResult(result);
        }

    }
}
