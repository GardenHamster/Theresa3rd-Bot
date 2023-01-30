using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheresaBot.Main.Common;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Session;
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

        public override async Task SendGroupSetuAsync(List<BaseContent> workMsgs, List<FileInfo> setuFiles, long groupId, bool isShowImg)
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
                    int workMsgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, await workMsgs.ToMiraiMessageAsync());
                    await Task.Delay(500);
                    int imgMsgId = await MiraiHelper.Session.SendGroupMessageAsync(groupId, imgMsgs.ToArray());
                    msgIds.Add(workMsgId);
                    msgIds.Add(imgMsgId);
                }
                else
                {
                    List<IChatMessage> msgList = new List<IChatMessage>();
                    msgList.AddRange(await workMsgs.ToMiraiMessageAsync());
                    msgList.AddRange(imgMsgs);
                    msgIds.Add(await MiraiHelper.Session.SendGroupMessageAsync(groupId, msgList.ToArray()));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SendGroupSetuAsync异常");
            }
        }

    }
}
