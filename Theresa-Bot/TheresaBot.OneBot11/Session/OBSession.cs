using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using TheresaBot.Core.Common;
using TheresaBot.Core.Model.Bot;
using TheresaBot.Core.Model.Content;
using TheresaBot.Core.Model.Result;
using TheresaBot.Core.Session;
using TheresaBot.Core.Type;
using TheresaBot.OneBot11.Helper;
using TheresaBot.OneBot11.Result;

namespace TheresaBot.OneBot11.Session
{
    public class OBSession : BaseSession
    {
        public override PlatformType PlatformType => PlatformType.OneBot11;

        public override async Task<BotInfos> GetBotInfosAsync()
        {
            return await OBHelper.GetBotInfosAsync();
        }

        public override async Task<GroupInfos[]> GetGroupInfosAsync()
        {
            return await OBHelper.GetGroupInfosAsync();
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, string message)
        {
            var result = await OBHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(message));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, List<BaseContent> contents, List<long> atMembers = null, bool isAtAll = false)
        {
            if (contents.Count == 0) return BaseResult.Undo;
            List<CqMsg> msgList = new List<CqMsg>();
            if (isAtAll) msgList.Add(CqAtMsg.AtAll);
            if (atMembers is null) atMembers = new();
            foreach (long memberId in atMembers)
            {
                msgList.Add(new CqAtMsg(memberId));
                msgList.Add(new CqTextMsg(" "));
            }
            msgList.AddRange(contents.ToOBMessageAsync());
            var result = await OBHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendGroupMessageWithAtAsync(long groupId, long memberId, string message)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            msgList.Add(new CqAtMsg(memberId));
            msgList.Add(new CqTextMsg(message));
            var result = await OBHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendGroupMessageWithAtAsync(long groupId, long memberId, List<BaseContent> contents)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            msgList.Add(new CqAtMsg(memberId));
            msgList.AddRange(contents.ToOBMessageAsync());
            var result = await OBHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendGroupMessageWithQuoteAsync(long groupId, long memberId, long quoteMsgId, string message)
        {
            List<CqMsg> msgList = new List<CqMsg>();
            msgList.Add(new CqReplyMsg(quoteMsgId));
            msgList.Add(new CqTextMsg(" "));//避免Replay和At不能同时使用
            msgList.Add(new CqAtMsg(memberId));
            msgList.Add(new CqTextMsg(message));
            var result = await OBHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendGroupMessageWithQuoteAsync(long groupId, long memberId, long quoteMsgId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return BaseResult.Undo;
            List<CqMsg> msgList = new List<CqMsg>();
            msgList.Add(new CqReplyMsg(quoteMsgId));
            msgList.Add(new CqTextMsg(" "));//避免Replay和At不能同时使用
            msgList.Add(new CqAtMsg(memberId));
            msgList.AddRange(contents.ToOBMessageAsync());
            var result = await OBHelper.Session.SendGroupMessageAsync(groupId, new CqMessage(msgList));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendGroupMergeAsync(long groupId, List<BaseContent[]> contentLists)
        {
            if (contentLists.Count == 0) return BaseResult.Undo;
            var nodeList = contentLists.Select(o => new CqForwardMessageNode(BotConfig.BotName, BotConfig.BotQQ, new CqMessage(o.ToList().ToOBMessageAsync()))).ToList();
            var result = await OBHelper.Session.SendGroupForwardMessageAsync(groupId, new CqForwardMessage(nodeList));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendGroupForwardAsync(long groupId, List<ForwardContent> contents)
        {
            if (contents.Count == 0) return BaseResult.Undo;
            var nodeList = new List<CqForwardMessageNode>();
            foreach (var content in contents)
            {
                if (content.Contents is null || content.Contents.Length == 0) continue;
                var memberId = content.MemberId <= 0 ? BotConfig.BotQQ : content.MemberId;
                var memberName = content.MemberName is null ? memberId.ToString() : content.MemberName;
                nodeList.Add(new CqForwardMessageNode(memberName, memberId, new CqMessage(content.Contents.ToList().ToOBMessageAsync())));
            }
            var result = await OBHelper.Session.SendGroupForwardMessageAsync(groupId, new CqForwardMessage(nodeList));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendFriendMessageAsync(long memberId, string message)
        {
            var result = await OBHelper.Session.SendPrivateMessageAsync(memberId, new CqMessage(message));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendFriendMessageAsync(long memberId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return BaseResult.Undo;
            CqMsg[] msgList = contents.ToOBMessageAsync();
            var result = await OBHelper.Session.SendPrivateMessageAsync(memberId, new CqMessage(msgList));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendTempMessageAsync(long groupId, long memberId, string message)
        {
            var result = await OBHelper.Session.SendPrivateMessageAsync(memberId, new CqMessage(message));
            return new OBResult(result, result.MessageId);
        }

        public override async Task<BaseResult> SendTempMessageAsync(long groupId, long memberId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return BaseResult.Undo;
            CqMsg[] msgList = contents.ToOBMessageAsync();
            var result = await OBHelper.Session.SendPrivateMessageAsync(memberId, new CqMessage(msgList));
            return new OBResult(result, result.MessageId);
        }

        public override async Task RevokeGroupMessageAsync(long groupId, long messageId)
        {
            await OBHelper.Session.RecallMessageAsync(messageId);
        }

        public override async Task MuteGroupMemberAsync(long groupId, long memberId, int seconds)
        {
            var duration = TimeSpan.FromSeconds(seconds);
            await OBHelper.Session.BanGroupMemberAsync(groupId, memberId, duration);
        }

    }
}
