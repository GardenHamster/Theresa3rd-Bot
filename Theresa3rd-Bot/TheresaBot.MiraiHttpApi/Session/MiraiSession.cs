using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Session;
using TheresaBot.MiraiHttpApi.Common;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Session
{
    public class MiraiSession : BaseSession
    {
        public override async Task<int> SendGroupMessageAsync(long groupId, string message)
        {
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, new PlainMessage(message));
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, params BaseContent[] contents)
        {
            if (contents.Length == 0) return 0;
            IChatMessage[] msgList = await new List<BaseContent>(contents).ToMiraiMessageAsync(UploadTarget.Group);
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList);
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return 0;
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync(UploadTarget.Group);
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList);
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents, bool isAtAll = false)
        {
            if (contents.Count == 0) return 0;
            List<IChatMessage> msgList = new();
            if (isAtAll) msgList.Add(new AtAllMessage());
            msgList.AddRange(await contents.ToMiraiMessageAsync(UploadTarget.Group));
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents, List<long> atMembers, bool isAtAll = false)
        {
            if (contents.Count == 0) return 0;
            List<IChatMessage> msgList = new();
            if (isAtAll) msgList.Add(new AtAllMessage());
            if (atMembers is not null)
            {
                foreach (long memberId in atMembers) msgList.Add(new AtMessage(memberId));
            }
            msgList.AddRange(await contents.ToMiraiMessageAsync(UploadTarget.Group));
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
        }

        public override async Task<int> SendGroupMergeAsync(long groupId, params List<BaseContent>[] contentLists)
        {
            if (contentLists.Length == 0) return 0;
            List<IForwardMessageNode> nodeList = new();
            foreach (var contentList in contentLists)
            {
                nodeList.Add(new ForwardMessageNode(MiraiConfig.MiraiBotName, MiraiConfig.MiraiBotQQ, DateTime.Now, await contentList.ToMiraiMessageAsync(UploadTarget.Group)));
            }
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, new ForwardMessage(nodeList.ToArray()));
        }

        public override async Task<int> SendFriendMessageAsync(long memberId, string message)
        {
            return await MiraiHelper.Session.SendFriendMessageAsync(memberId, new PlainMessage(message));
        }

        public override async Task<int> SendFriendMessageAsync(long memberId, params BaseContent[] contents)
        {
            if (contents.Length == 0) return 0;
            IChatMessage[] msgList = await new List<BaseContent>(contents).ToMiraiMessageAsync(UploadTarget.Group);
            return await MiraiHelper.Session.SendFriendMessageAsync(memberId, msgList);
        }

        public override async Task<int> SendFriendMessageAsync(long memberId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return 0;
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync(UploadTarget.Group);
            return await MiraiHelper.Session.SendFriendMessageAsync(memberId, msgList);
        }

    }
}
