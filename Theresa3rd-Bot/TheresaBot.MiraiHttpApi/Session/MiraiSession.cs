using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Session;
using TheresaBot.MiraiHttpApi.Common;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Session
{
    public class MiraiSession : BaseSession
    {
        public override async Task<int> SendFriendMessageAsync(long memberId, string message)
        {
            return await MiraiHelper.Session.SendFriendMessageAsync(memberId, new PlainMessage(message));
        }

        public override async Task<int> SendFriendMessageAsync(long memberId, params BaseContent[] contents)
        {
            IChatMessage[] msgList = await new List<BaseContent>(contents).ToMiraiMessageAsync();
            return await MiraiHelper.Session.SendFriendMessageAsync(memberId, msgList);
        }

        public override async Task<int> SendFriendMessageAsync(long memberId, List<BaseContent> contents)
        {
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync();
            return await MiraiHelper.Session.SendFriendMessageAsync(memberId, msgList);
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, string message)
        {
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, new PlainMessage(message));
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, params BaseContent[] contents)
        {
            IChatMessage[] msgList = await new List<BaseContent>(contents).ToMiraiMessageAsync();
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList);
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents)
        {
            IChatMessage[] msgList = await contents.ToMiraiMessageAsync();
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList);
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents, bool isAtAll = false)
        {
            List<IChatMessage> msgList = new();
            if (isAtAll) msgList.Add(new AtAllMessage());
            msgList.AddRange(await contents.ToMiraiMessageAsync());
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
        }

        public override async Task<int> SendGroupMessageAsync(long groupId, List<BaseContent> contents, List<long> atMembers, bool isAtAll = false)
        {
            List<IChatMessage> msgList = new();
            if (isAtAll) msgList.Add(new AtAllMessage());
            if (atMembers is not null)
            {
                foreach (long memberId in atMembers) msgList.Add(new AtMessage(memberId));
            }
            msgList.AddRange(await contents.ToMiraiMessageAsync());
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
        }

        public override async Task<int> SendGroupMergeMessageAsync(long groupId, params List<BaseContent>[] contentLists)
        {
            List<IForwardMessageNode> nodeList = new();
            foreach (var contentList in contentLists)
            {
                nodeList.Add(new ForwardMessageNode(MiraiConfig.MiraiBotName, MiraiConfig.MiraiBotQQ, DateTime.Now, await contentList.ToMiraiMessageAsync()));
            }
            return await MiraiHelper.Session.SendGroupMessageAsync(groupId, new ForwardMessage(nodeList.ToArray()));
        }

        public override async Task<int> SendGroupMergeMessageAsync(long groupId, List<SetuContent> setuContents)
        {
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

        public override async Task<int[]> SendGroupSetuAsync(List<SetuContent> setuContents, long groupId, bool sendMerge, int margeEachPage = 0)
        {
            if (sendMerge == false || margeEachPage <= 0)
            {
                return await SendGroupSetuAsync(setuContents, groupId, sendMerge);
            }

            int startIndex = 0;
            List<int> msgIds = new List<int>();
            while (startIndex < setuContents.Count)
            {
                List<SetuContent> pageContents = setuContents.Skip(startIndex).Take(margeEachPage).ToList();
                msgIds.AddRange(await SendGroupSetuAsync(pageContents, groupId, sendMerge));
                startIndex += margeEachPage;
            }
            return msgIds.ToArray();
        }

        public override async Task<int[]> SendGroupSetuAsync(List<SetuContent> setuContents, long groupId, bool sendMerge)
        {
            if (sendMerge)
            {
                int msgId = await SendGroupMergeMessageAsync(groupId, setuContents);
                if (msgId < 0)
                {
                    await Task.Delay(1000);
                    msgId = await SendGroupMergeMessageAsync(groupId, setuContents.Select(o => o with { SetuImages = new() }).ToList());
                }
                return new[] { msgId };
            }
            else
            {
                List<int> msgIdList = new List<int>();
                foreach (var content in setuContents)
                {
                    msgIdList.AddRange(await SendGroupSetuAsync(content, groupId));
                    await Task.Delay(1000);
                }
                return msgIdList.ToArray();
            }
        }

        public override async Task<int[]> SendGroupMergeSetuAsync(List<SetuContent> setuContents, List<SetuContent> headerContents, long groupId, int eachPage)
        {
            int startIndex = 0;
            if (eachPage <= 0) return new[] { 0 };
            List<int> msgIds = new List<int>();
            while (startIndex < setuContents.Count)
            {
                List<SetuContent> pageContents = new List<SetuContent>();
                pageContents.AddRange(headerContents);
                pageContents.AddRange(setuContents.Skip(startIndex).Take(eachPage).ToList());
                msgIds.AddRange(await SendGroupSetuAsync(pageContents, groupId, true));
                startIndex += eachPage;
            }
            return msgIds.ToArray();
        }

        public override async Task<int[]> SendGroupSetuAsync(SetuContent setuContent, long groupId)
        {
            return await SendGroupSetuAsync(setuContent.SetuInfos, setuContent.SetuImages, groupId);
        }

        public override async Task<int[]> SendGroupSetuAsync(List<BaseContent> setuInfos, List<FileInfo> setuFiles, long groupId)
        {
            try
            {
                List<int> msgIds = new List<int>();
                List<IChatMessage> imgMsgs = new List<IChatMessage>();
                if (setuFiles != null && setuFiles.Count > 0)
                {
                    imgMsgs = await MiraiHelper.UploadPictureAsync(setuFiles, UploadTarget.Group);
                }

                if (BotConfig.PixivConfig.SendImgBehind && imgMsgs.Count > 0)
                {
                    int workMsgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, await setuInfos.ToMiraiMessageAsync());
                    await Task.Delay(500);
                    int imgMsgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, imgMsgs.ToArray());
                    msgIds.Add(workMsgId);
                    msgIds.Add(imgMsgId);
                }
                else
                {
                    List<IChatMessage> msgList = new List<IChatMessage>();
                    msgList.AddRange(await setuInfos.ToMiraiMessageAsync());
                    msgList.AddRange(imgMsgs);
                    int msgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray());
                    if (msgId < 0)
                    {
                        await Task.Delay(1000);
                        msgId = await MiraiHelper.Session.SendGroupMessageWithoutImageAsync(groupId, msgList);
                    }
                }
                return msgIds.ToArray();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SendGroupSetuAsync异常");
                return new int[0];
            }
        }





    }
}
