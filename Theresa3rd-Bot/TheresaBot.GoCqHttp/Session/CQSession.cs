using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using TheresaBot.GoCqHttp.Common;
using TheresaBot.GoCqHttp.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.GoCqHttp.Session
{
    public class CQSession : BaseSession
    {
        public override PlatformType PlatformType { get; } = PlatformType.GoCQHttp;

        public override async Task<long> SendGroupMessageAsync(long groupId, string message)
        {
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(message));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long> SendGroupMessageAsync(long groupId, params BaseContent[] contents)
        {
            if (contents.Length == 0) return 0;
            CqMsg[] msgList = contents.ToList().ToCQMessageAsync();
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long> SendGroupMessageAsync(long groupId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return 0;
            CqMsg[] msgList = contents.ToCQMessageAsync();
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long> SendGroupMessageAsync(long groupId, List<BaseContent> contents, bool isAtAll = false)
        {
            if (contents.Count == 0) return 0;
            List<CqMsg> msgList = new();
            if (isAtAll) msgList.Add(CqAtMsg.AtAll);
            msgList.AddRange(contents.ToCQMessageAsync());
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long> SendGroupMessageAsync(long groupId, List<BaseContent> contents, List<long> atMembers, bool isAtAll = false)
        {
            if (contents.Count == 0) return 0;
            List<CqMsg> msgList = new();
            if (isAtAll) msgList.Add(CqAtMsg.AtAll);
            if (atMembers is null) atMembers = new();
            foreach (long memberId in atMembers) msgList.Add(new CqAtMsg(memberId));
            msgList.AddRange(contents.ToCQMessageAsync());
            var result = await CQHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long> SendGroupMergeAsync(long groupId, params List<BaseContent>[] contentLists)
        {
            if (contentLists.Length == 0) return 0;
            var nodeList = contentLists.Select(o => new CqForwardMessageNode(CQConfig.BotName, CQConfig.BotQQ, new CqMessage(o.ToCQMessageAsync()))).ToList();
            var result = await CQHelper.Session.SendGroupForwardMessageAsync(groupId, new CqForwardMessage(nodeList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long> SendFriendMessageAsync(long memberId, string message)
        {
            var result = await CQHelper.Session.SendPrivateMessageAsync(memberId, new CqMessage(message));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long> SendFriendMessageAsync(long memberId, params BaseContent[] contents)
        {
            if (contents.Length == 0) return 0;
            CqMsg[] msgList = contents.ToList().ToCQMessageAsync();
            var result = await CQHelper.Session.SendPrivateMessageAsync(memberId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

        public override async Task<long> SendFriendMessageAsync(long memberId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return 0;
            CqMsg[] msgList = contents.ToCQMessageAsync();
            var result = await CQHelper.Session.SendPrivateMessageAsync(memberId, new CqMessage(msgList));
            return result is null ? 0 : result.MessageId;
        }

    }
}
