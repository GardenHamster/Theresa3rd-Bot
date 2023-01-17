using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Theresa3rd_Bot.Common;

namespace Theresa3rd_Bot.Util
{
    public static class SessionHelper
    {
        public static async Task<int> SendMessageWithAtAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, params IChatMessage[] chainArr)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(args.Sender.Id));
            msgList.AddRange(chainArr);
            return await session.SendGroupMessageAsync(args.Sender.Group.Id, msgList.ToArray());
        }

        public static async Task<int> SendMessageWithAtAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, List<IChatMessage> chainList)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(args.Sender.Id));
            msgList.AddRange(chainList);
            return await session.SendGroupMessageAsync(args.Sender.Group.Id, msgList.ToArray());
        }

        public static async Task<int> SendTemplateWithAtAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            List<IChatMessage> chatList = session.SplitToChainAsync(template).Result;
            return await session.SendMessageWithAtAsync(args, chatList);
        }

        public static async Task<int> SendTemplateAsync(this IMiraiHttpSession session, IFriendMessageEventArgs args, string template, string defaultmsg)
        {
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            List<IChatMessage> chatList = session.SplitToChainAsync(template).Result;
            return await session.SendFriendMessageAsync(args.Sender.Id, chatList.ToArray());
        }

        public static async Task RevokeMessageAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            try
            {
                SourceMessage sourceMessage = (SourceMessage)args.Chain.First();
                await session.RevokeMessageAsync(sourceMessage.Id, args.Sender.Group.Id);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群消息撤回失败");
            }
        }

        public static async Task RevokeMessageAsync(this IMiraiHttpSession session, List<int> messageIds, long groupId)
        {
            foreach (int messageId in messageIds)
            {
                if (messageId <= 0) continue;
                await session.RevokeMessageAsync(messageId, groupId);
                await Task.Delay(500);
            }
        }

        public static async Task SendGroupSetuAsync(this IMiraiHttpSession session, List<IChatMessage> workMsgs, FileInfo fileInfo, long groupId, bool isShowImg)
        {
            try
            {
                List<IChatMessage> imgMsgs = new List<IChatMessage>();
                if (isShowImg && fileInfo != null)
                {
                    imgMsgs.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                }
                else if (isShowImg && fileInfo == null)
                {
                    imgMsgs.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, UploadTarget.Group));
                }

                if (BotConfig.PixivConfig.SendImgBehind && imgMsgs.Count > 0)
                {
                    int workMsgId = await session.SendGroupMessageAsync(groupId, workMsgs.ToArray());
                    await Task.Delay(500);
                    await session.SendGroupMessageAsync(groupId, imgMsgs.ToArray(), workMsgId);
                }
                else
                {
                    List<IChatMessage> msgList = new List<IChatMessage>();
                    msgList.AddRange(workMsgs);
                    msgList.AddRange(imgMsgs);
                    await session.SendGroupMessageAsync(groupId, msgList.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SendGroupSetuAndRevokeWithAtAsync异常");
            }
        }

        public static async Task SendGroupSetuAndRevokeWithAtAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, List<IChatMessage> workMsgs, FileInfo fileInfo, bool isShowImg)
        {
            try
            {
                List<int> msgIds = new List<int>();
                List<IChatMessage> imgMsgs = new List<IChatMessage>();
                if (isShowImg && fileInfo != null)
                {
                    imgMsgs.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Group, fileInfo.FullName));
                }
                else if (isShowImg && fileInfo == null)
                {
                    imgMsgs.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, UploadTarget.Group));
                }

                if (BotConfig.PixivConfig.SendImgBehind && imgMsgs.Count > 0)
                {
                    int workMsgId = await session.SendMessageWithAtAsync(args, workMsgs);
                    await Task.Delay(500);
                    int imgMsgId = await session.SendGroupMessageAsync(args.Sender.Group.Id, imgMsgs.ToArray(), workMsgId);
                    msgIds.Add(workMsgId);
                    msgIds.Add(imgMsgId);
                }
                else
                {
                    List<IChatMessage> msgList = new List<IChatMessage>();
                    msgList.AddRange(workMsgs);
                    msgList.AddRange(imgMsgs);
                    msgIds.Add(await session.SendMessageWithAtAsync(args, msgList));
                }

                if (BotConfig.SetuConfig.RevokeInterval > 0)
                {
                    await Task.Delay(BotConfig.SetuConfig.RevokeInterval * 1000);
                    await session.RevokeMessageAsync(msgIds, args.Sender.Group.Id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SendGroupSetuAndRevokeWithAtAsync异常");
            }
        }

        public static async Task SendTempSetuAsync(this IMiraiHttpSession session, IGroupMessageEventArgs args, List<IChatMessage> workMsgs, FileInfo fileInfo, bool isShowImg)
        {
            try
            {
                List<IChatMessage> imgMsgs = new List<IChatMessage>();
                if (isShowImg && fileInfo != null)
                {
                    imgMsgs.Add((IChatMessage)await session.UploadPictureAsync(UploadTarget.Temp, fileInfo.FullName));
                }
                else if (isShowImg && fileInfo == null)
                {
                    imgMsgs.AddRange(await session.SplitToChainAsync(BotConfig.GeneralConfig.DownErrorImg, UploadTarget.Temp));
                }

                if (BotConfig.PixivConfig.SendImgBehind && imgMsgs.Count > 0)
                {
                    await session.SendTempMessageAsync(args.Sender.Id, args.Sender.Group.Id, workMsgs.ToArray());
                    await Task.Delay(500);
                    await session.SendTempMessageAsync(args.Sender.Id, args.Sender.Group.Id, imgMsgs.ToArray());
                }
                else
                {
                    List<IChatMessage> msgList = new List<IChatMessage>();
                    msgList.AddRange(workMsgs);
                    msgList.AddRange(imgMsgs);
                    await session.SendTempMessageAsync(args.Sender.Id, args.Sender.Group.Id, msgList.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "SendTempSetuAsync异常");
            }
        }

    }
}
