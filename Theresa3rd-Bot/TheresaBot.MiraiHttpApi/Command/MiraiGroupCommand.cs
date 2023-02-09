using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheresaBot.Main.Command;
using TheresaBot.Main.Common;
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

        public override async Task RevokeGroupMessageAsync(List<int> messageIds, long groupId)
        {
            foreach (int messageId in messageIds)
            {
                if (messageId <= 0) continue;
                await RevokeGroupMessageAsync(messageId, groupId);
                await Task.Delay(500);
            }
        }

        public override async Task ReplyGroupSetuAndRevokeAsync(List<BaseContent> workContents, List<FileInfo> setuFiles, int revokeInterval, bool isAt = false)
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
                    int workMsgId = await ReplyGroupMessageAsync(workContents, isAt);
                    await Task.Delay(500);
                    int imgMsgId = await Session.SendGroupMessageAsync(GroupId, imgMsgs.ToArray());
                    msgIds.Add(workMsgId);
                    msgIds.Add(imgMsgId);
                }
                else
                {
                    List<IChatMessage> msgList = new List<IChatMessage>();
                    if (isAt) msgList.Add(new AtMessage(MemberId));
                    msgList.AddRange(await workContents.ToMiraiMessageAsync());
                    msgList.AddRange(imgMsgs);
                    msgIds.Add(await Session.SendGroupMessageAsync(GroupId, msgList.ToArray()));
                }

                if (revokeInterval > 0)
                {
                    await Task.Delay(revokeInterval * 1000);
                    await RevokeGroupMessageAsync(msgIds, GroupId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "ReplyGroupSetuAndRevokeAsync异常");
            }
        }

        public override async Task SendTempSetuAsync(List<BaseContent> workContents, List<FileInfo> setuFiles = null)
        {
            try
            {
                List<IChatMessage> imgMsgs = new List<IChatMessage>();
                if (setuFiles != null && setuFiles.Count > 0)
                {
                    imgMsgs = await MiraiHelper.UploadPictureAsync(setuFiles, UploadTarget.Temp);
                }

                IChatMessage[] workMsgs = await workContents.ToMiraiMessageAsync();
                if (BotConfig.PixivConfig.SendImgBehind && imgMsgs.Count > 0)
                {
                    await Session.SendTempMessageAsync(MemberId, GroupId, workMsgs);
                    await Task.Delay(500);
                    await Session.SendTempMessageAsync(MemberId, GroupId, imgMsgs.ToArray());
                }
                else
                {
                    List<IChatMessage> msgList = new List<IChatMessage>();
                    msgList.AddRange(workMsgs);
                    msgList.AddRange(imgMsgs);
                    await Session.SendTempMessageAsync(MemberId, GroupId, msgList.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SendTempSetuAsync异常");
            }
        }

    }
}
