using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheresaBot.Main.Helper;
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
            IChatMessage[] msgList = await new List<BaseContent>(contents).ToMiraiMessageAsync();
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList);
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return 0;
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync();
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList);
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents, bool isAtAll = false)
        {
            if (contents.Count == 0) return 0;
            List<IChatMessage> msgList = new();
            if (isAtAll) msgList.Add(new AtAllMessage());
            msgList.AddRange(await contents.ToMiraiMessageAsync());
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
            msgList.AddRange(await contents.ToMiraiMessageAsync());
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
        }

        public override async Task<int[]> SendGroupMessageAsync(long groupId, SetuContent setuContent, bool sendImgBehind)
        {
            List<int> msgIds = new List<int>();
            List<BaseContent> setuInfos = setuContent.SetuInfos ?? new();
            List<FileInfo> setuFiles = setuContent.SetuImages ?? new();

            if (sendImgBehind)
            {
                int workMsgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, await setuInfos.ToMiraiMessageAsync());
                await Task.Delay(1000);
                List<IChatMessage> imgMsgs = await MiraiHelper.UploadPictureAsync(setuFiles, UploadTarget.Group);
                int imgMsgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, imgMsgs.ToArray());
                msgIds.Add(workMsgId);
                msgIds.Add(imgMsgId);
            }
            else
            {
                List<IChatMessage> msgList = new List<IChatMessage>();
                msgList.AddRange(await setuInfos.ToMiraiMessageAsync());
                msgList.AddRange(await MiraiHelper.UploadPictureAsync(setuFiles, UploadTarget.Group));
                msgIds.Add(await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray()));
            }
            return msgIds.ToArray();
        }

        public override async Task<int> SendGroupMergeAsync(long groupId, params List<BaseContent>[] contentLists)
        {
            if (contentLists.Length == 0) return 0;
            List<IForwardMessageNode> nodeList = new();
            foreach (var contentList in contentLists)
            {
                nodeList.Add(new ForwardMessageNode(MiraiConfig.MiraiBotName, MiraiConfig.MiraiBotQQ, DateTime.Now, await contentList.ToMiraiMessageAsync()));
            }
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, new ForwardMessage(nodeList.ToArray()));
        }

        public override async Task<int> SendGroupMergeAsync(long groupId, List<SetuContent> setuContents)
        {
            if (setuContents.Count == 0) return 0;
            List<IForwardMessageNode> nodeList = new();
            foreach (var content in setuContents)
            {
                List<IChatMessage> msgList = new List<IChatMessage>();
                msgList.AddRange(await content.SetuInfos.ToMiraiMessageAsync());
                msgList.AddRange(await content.SetuImages.UploadPictureAsync(UploadTarget.Group));
                nodeList.Add(new ForwardMessageNode(MiraiConfig.MiraiBotName, MiraiConfig.MiraiBotQQ, DateTime.Now, msgList.ToArray()));
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
            IChatMessage[] msgList = await new List<BaseContent>(contents).ToMiraiMessageAsync();
            return await MiraiHelper.Session.SendFriendMessageAsync(memberId, msgList);
        }

        public override async Task<int> SendFriendMessageAsync(long memberId, List<BaseContent> contents)
        {
            if (contents.Count == 0) return 0;
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync();
            return await MiraiHelper.Session.SendFriendMessageAsync(memberId, msgList);
        }

    }
}
