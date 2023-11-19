using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using TheresaBot.Main.Common;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Result;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Helper;
using TheresaBot.MiraiHttpApi.Result;

namespace TheresaBot.MiraiHttpApi.Session
{
    public class MiraiSession : BaseSession
    {
        public override PlatformType PlatformType { get; } = PlatformType.Mirai;

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, string message)
        {
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, new PlainMessage(message));
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMessageAsync(long groupId, List<BaseContent> contents, List<long> atMembers = null, bool isAtAll = false)
        {
            if (contents.Count == 0) return MiraiResult.Undo;
            List<IChatMessage> msgList = new List<IChatMessage>();
            if (isAtAll) msgList.Add(new AtAllMessage());
            if (atMembers is null) atMembers = new();
            foreach (long memberId in atMembers)
            {
                msgList.Add(new AtMessage(memberId));
                msgList.Add(new PlainMessage(" "));
            }
            msgList.AddRange(await contents.ToMiraiMessageAsync(UploadTarget.Group));
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMessageWithAtAsync(long groupId, long memberId, string message)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(memberId));
            msgList.Add(new PlainMessage(" "));
            msgList.Add(new PlainMessage(message));
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMessageWithAtAsync(long groupId, long memberId, List<BaseContent> contents)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(memberId));
            msgList.Add(new PlainMessage(" "));
            msgList.AddRange(await contents.ToMiraiMessageAsync(UploadTarget.Group));
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMessageWithQuoteAsync(long groupId, long memberId, long quoteMsgId, string message)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(memberId));
            msgList.Add(new PlainMessage(" "));
            msgList.Add(new PlainMessage(message));
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray(), (int)quoteMsgId);
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMessageWithQuoteAsync(long groupId, long memberId, long quoteMsgId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return MiraiResult.Undo;
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(memberId));
            msgList.Add(new PlainMessage(" "));
            msgList.AddRange(await contents.ToMiraiMessageAsync(UploadTarget.Group));
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray(), (int)quoteMsgId);
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendGroupMergeAsync(long groupId, List<BaseContent[]> contentLists)
        {
            if (contentLists.Count == 0) return MiraiResult.Undo;
            List<IForwardMessageNode> nodeList = new List<IForwardMessageNode>();
            foreach (var contentArr in contentLists)
            {
                nodeList.Add(new ForwardMessageNode(BotConfig.BotName, BotConfig.BotQQ, DateTime.Now, await contentArr.ToList().ToMiraiMessageAsync(UploadTarget.Group)));
            }
            var msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, new ForwardMessage(nodeList.ToArray()));
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendFriendMessageAsync(long memberId, string message)
        {
            var msgId = await MiraiHelper.Session.SendFriendMessageAsync(memberId, new PlainMessage(message));
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendFriendMessageAsync(long memberId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return MiraiResult.Undo;
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync(UploadTarget.Friend);
            var msgId = await MiraiHelper.Session.SendFriendMessageAsync(memberId, msgList);
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendTempMessageAsync(long groupId, long memberId, string message)
        {
            var msgId = await MiraiHelper.Session.SendTempMessageAsync(memberId, groupId, new PlainMessage(message));
            return new MiraiResult(msgId);
        }

        public override async Task<BaseResult> SendTempMessageAsync(long groupId, long memberId, List<BaseContent> contents)
        {
            var msgId = await MiraiHelper.Session.SendTempMessageAsync(memberId, groupId, await contents.ToMiraiMessageAsync(UploadTarget.Temp));
            return new MiraiResult(msgId);
        }

        public override async Task RevokeGroupMessageAsync(long groupId, long messageId)
        {
            await MiraiHelper.Session.RevokeMessageAsync((int)messageId, groupId);
        }

        public override async Task MuteGroupMemberAsync(long groupId, long memberId, int seconds)
        {
            var duration = TimeSpan.FromSeconds(seconds);
            await MiraiHelper.Session.MuteAsync(memberId, groupId, duration);
        }

    }
}
