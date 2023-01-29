using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Theresa3rd_Bot.BotPlatform.Base.Command;
using Theresa3rd_Bot.BotPlatform.Mirai.Util;
using Theresa3rd_Bot.Common;
using Theresa3rd_Bot.Model.Content;
using Theresa3rd_Bot.Model.Invoker;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Command
{
    public class MiraiGroupCommand : GroupCommand
    {
        public IGroupMessageEventArgs Args { get; set; }
        public IMiraiHttpSession Session { get; set; }

        public MiraiGroupCommand(CommandHandler<GroupCommand> invoker, IMiraiHttpSession session, IGroupMessageEventArgs args, string[] keyWords, string instruction, long groupId, long memberId) : base(invoker, keyWords, instruction, groupId, memberId)
        {
            this.Args = args;
            this.Session = session;
        }

        public override async Task<int> ReplyGroupMessageAsync(string message, bool isAt = false)
        {
            List<IChatMessage> msgList = new List<IChatMessage>() { new PlainMessage(message) };
            if (isAt) msgList.Add(new AtMessage(MemberId));
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<int> ReplyGroupMessageAsync(List<ChatContent> chainList, bool isAt = false)
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

        public override async Task<int> ReplyGroupMessageWithAtAsync(params ChatContent[] chainArr)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(MemberId));
            msgList.AddRange(await new List<ChatContent>(chainArr).ToMiraiMessageAsync());
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<int> ReplyGroupMessageWithAtAsync(List<ChatContent> chainList)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(MemberId));
            msgList.AddRange(await chainList.ToMiraiMessageAsync());
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<int> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            if (template.StartsWith(" ") == false) template = " " + template;
            List<IChatMessage> msgList = await template.SplitToChainAsync().ToMiraiMessageAsync();
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task RevokeGroupMessageAsync(int messageId, long groupId)
        {
            try
            {
                await RevokeGroupMessageAsync(messageId, GroupId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群消息撤回失败");
                ReportHelper.SendError(ex, "群消息撤回失败");
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

        public async Task<List<IChatMessage>> UploadPictureAsync(List<FileInfo> setuFiles, UploadTarget target)
        {
            List<IChatMessage> imgMsgs = new List<IChatMessage>();
            foreach (FileInfo setuFile in setuFiles)
            {
                if (setuFile is null)
                {

                    imgMsgs.AddRange(await BusinessHelper.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, SendTarget.Group).ToMiraiMessageAsync());
                }
                else
                {
                    imgMsgs.Add((IChatMessage)await Session.UploadPictureAsync(target, setuFile.FullName));
                }
            }
            return imgMsgs;
        }

        public override async Task SendGroupSetuAsync(List<ChatContent> workMsgs, List<FileInfo> setuFiles, long groupId, bool isShowImg)
        {
            try
            {
                List<IChatMessage> imgMsgs = new List<IChatMessage>();
                if (isShowImg && setuFiles != null && setuFiles.Count > 0)
                {
                    imgMsgs = await UploadPictureAsync(setuFiles, UploadTarget.Group);
                }

                if (BotConfig.PixivConfig.SendImgBehind && imgMsgs.Count > 0)
                {
                    List<IChatMessage> miraiWorkMsgs = await workMsgs.ToMiraiMessageAsync();
                    int workMsgId = await Session.SendGroupMessageAsync(groupId, miraiWorkMsgs.ToArray());
                    await Task.Delay(500);
                    await Session.SendGroupMessageAsync(groupId, imgMsgs.ToArray(), workMsgId);
                }
                else
                {
                    List<IChatMessage> msgList = new List<IChatMessage>();
                    msgList.AddRange(await workMsgs.ToMiraiMessageAsync());
                    msgList.AddRange(imgMsgs);
                    await Session.SendGroupMessageAsync(groupId, msgList.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SendGroupSetuAsync异常");
            }
        }

        public override async Task ReplyGroupSetuAndRevokeAsync(List<ChatContent> workContents, List<FileInfo> setuFiles, int revokeInterval, bool isAt = false)
        {
            try
            {
                List<int> msgIds = new List<int>();
                List<IChatMessage> imgMsgs = new List<IChatMessage>();
                if (setuFiles != null && setuFiles.Count > 0)
                {
                    imgMsgs = await UploadPictureAsync(setuFiles, UploadTarget.Group);
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
                LogHelper.Error(ex, "SendGroupSetuAndRevokeAsync异常");
            }
        }


        public override async Task SendTempSetuAsync(List<ChatContent> workContents, List<FileInfo> setuFiles = null)
        {
            try
            {
                List<IChatMessage> imgMsgs = new List<IChatMessage>();
                if (setuFiles != null && setuFiles.Count > 0)
                {
                    imgMsgs = await UploadPictureAsync(setuFiles, UploadTarget.Temp);
                }

                List<IChatMessage> workMsgs = await workContents.ToMiraiMessageAsync();
                if (BotConfig.PixivConfig.SendImgBehind && imgMsgs.Count > 0)
                {
                    await Session.SendTempMessageAsync(MemberId, GroupId, workMsgs.ToArray());
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

        /// <summary>
        /// 发送错误记录
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="message"></param>
        private static void sendReport(long groupId, string message)
        {
            try
            {
                MiraiHelper.Session.SendGroupMessageAsync(groupId, new PlainMessage(message)).Wait();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }




    }
}
