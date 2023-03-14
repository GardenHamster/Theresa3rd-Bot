﻿using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Type;
using TheresaBot.MiraiHttpApi.Helper;

namespace TheresaBot.MiraiHttpApi.Command
{
    public class MiraiGroupCommand : GroupCommand
    {
        public IGroupMessageEventArgs Args { get; set; }
        public IMiraiHttpSession Session { get; set; }

        public MiraiGroupCommand(CommandHandler<GroupCommand> invoker, IMiraiHttpSession session, IGroupMessageEventArgs args, string instruction, string command, long groupId, long memberId)
            : base(invoker, args.GetMessageId(), instruction, command, groupId, memberId)
        {
            this.Args = args;
            this.Session = session;
        }

        public override List<string> GetReplyImageUrls()
        {
            return Args.Chain.Where(o => o is ImageMessage).Select(o => ((ImageMessage)o).Url).ToList();
        }

        public override async Task<int> ReplyGroupMessageAsync(string message, bool isAt = false)
        {
            List<IChatMessage> msgList = new List<IChatMessage>() { new PlainMessage(message) };
            if (isAt) msgList.Add(new AtMessage(MemberId));
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<int> ReplyGroupMessageAsync(List<BaseContent> chainList, bool isAt = false)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            if (isAt) msgList.Add(new AtMessage(MemberId));
            msgList.AddRange(await chainList.ToMiraiMessageAsync());
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<int> ReplyGroupMessageWithAtAsync(string plainMsg)
        {
            if (plainMsg.StartsWith(" ") == false) plainMsg = " " + plainMsg;
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(MemberId));
            msgList.Add(new PlainMessage(plainMsg));
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<int> ReplyGroupMessageWithAtAsync(params BaseContent[] chainArr)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(MemberId));
            msgList.AddRange(await new List<BaseContent>(chainArr).ToMiraiMessageAsync());
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<int> ReplyGroupMessageWithAtAsync(List<BaseContent> chainList)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(MemberId));
            msgList.AddRange(await chainList.ToMiraiMessageAsync());
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<int> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "")
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            if (template.StartsWith(" ") == false) template = " " + template;
            IChatMessage[] msgList = await template.SplitToChainAsync(SendTarget.Group).ToMiraiMessageAsync();
            return await Session.SendGroupMessageAsync(GroupId, msgList);
        }

        public override async Task<int[]> ReplyGroupMessageAndRevokeAsync(SetuContent setuContent, int revokeInterval, bool sendImgBehind, bool isAt = false)
        {
            try
            {
                if (setuContent is null) return new int[0];
                List<int> msgIds = new List<int>();
                List<BaseContent> setuInfos = setuContent.SetuInfos ?? new();
                List<FileInfo> setuFiles = setuContent.SetuImages ?? new();
                
                if (sendImgBehind)
                {
                    int workMsgId = await ReplyGroupMessageAsync(setuInfos, isAt);
                    await Task.Delay(1000);
                    List<IChatMessage> imgMsgs = await MiraiHelper.UploadPictureAsync(setuFiles, UploadTarget.Group);
                    int imgMsgId = await Session.SendGroupMessageAsync(GroupId, imgMsgs.ToArray());
                    msgIds.Add(workMsgId);
                    msgIds.Add(imgMsgId);
                }
                else
                {
                    List<IChatMessage> msgList = new List<IChatMessage>();
                    if (isAt) msgList.Add(new AtMessage(MemberId));
                    msgList.AddRange(await setuInfos.ToMiraiMessageAsync());
                    msgList.AddRange(await MiraiHelper.UploadPictureAsync(setuFiles, UploadTarget.Group));
                    msgIds.Add(await Session.SendGroupMessageAsync(GroupId, msgList.ToArray()));
                }

                if (revokeInterval > 0)
                {
                    Task revokeTask = RevokeGroupMessageAsync(msgIds, GroupId, revokeInterval);
                }
                return msgIds.ToArray();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReplyGroupMessageAndRevokeAsync异常");
                return new[] { 0 };
            }
        }

        public override async Task<int[]> ReplyTempMessageAsync(SetuContent setuContent, bool sendImgBehind)
        {
            try
            {
                if (setuContent is null) return new int[0];
                List<int> msgIds = new List<int>();
                List<BaseContent> setuInfos = setuContent.SetuInfos ?? new();
                List<FileInfo> setuFiles = setuContent.SetuImages ?? new();
                
                if (sendImgBehind)
                {
                    IChatMessage[] workMsgs = await setuInfos.ToMiraiMessageAsync();
                    int workMsgId = await Session.SendTempMessageAsync(MemberId, GroupId, workMsgs);
                    await Task.Delay(500);
                    List<IChatMessage> imgMsgs = await MiraiHelper.UploadPictureAsync(setuFiles, UploadTarget.Temp);
                    int imgMsgId = await Session.SendTempMessageAsync(MemberId, GroupId, imgMsgs.ToArray());
                    msgIds.Add(workMsgId);
                    msgIds.Add(imgMsgId);
                }
                else
                {
                    List<IChatMessage> msgList = new List<IChatMessage>();
                    msgList.AddRange(await setuInfos.ToMiraiMessageAsync());
                    msgList.AddRange(await MiraiHelper.UploadPictureAsync(setuFiles, UploadTarget.Temp));
                    msgIds.Add(await Session.SendTempMessageAsync(MemberId, GroupId, msgList.ToArray()));
                }
                return msgIds.ToArray();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SendTempSetuAsync异常");
                return new[] { 0 };
            }
        }

        public override async Task RevokeGroupMessageAsync(int messageId, long groupId)
        {
            try
            {
                await Session.RevokeMessageAsync(messageId, groupId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群消息撤回失败");
            }
        }

        public override async Task RevokeGroupMessageAsync(List<int> messageIds, long groupId, int revokeInterval = 0)
        {
            if (revokeInterval > 0) await Task.Delay(revokeInterval * 1000);
            foreach (int messageId in messageIds)
            {
                if (messageId <= 0) continue;
                await RevokeGroupMessageAsync(messageId, groupId);
                await Task.Delay(500);
            }
        }


    }
}
