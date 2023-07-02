﻿using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
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
        private IMiraiHttpSession Session;
        private IGroupMessageEventArgs Args;
        public override PlatformType PlatformType { get; } = PlatformType.Mirai;

        public MiraiGroupCommand(IMiraiHttpSession session, IGroupMessageEventArgs args, CommandType commandType, string instruction, string command, long groupId, long memberId)
            : base(commandType, args.GetMessageId(), instruction, command, groupId, memberId)
        {
            this.Args = args;
            this.Session = session;
        }

        public MiraiGroupCommand(CommandHandler<GroupCommand> invoker, IMiraiHttpSession session, IGroupMessageEventArgs args, string instruction, string command, long groupId, long memberId)
            : base(invoker, args.GetMessageId(), instruction, command, groupId, memberId)
        {
            this.Args = args;
            this.Session = session;
        }

        public override List<string> GetImageUrls()
        {
            return Args.Chain.OfType<ImageMessage>().Select(o => o.Url).ToList();
        }

        public override long GetQuoteMessageId()
        {
            var quoteMessage = Args.Chain.Where(v => v is QuoteMessage).FirstOrDefault();
            return quoteMessage is null ? 0 : ((QuoteMessage)quoteMessage).Id;
        }

        public override async Task<long?> ReplyGroupMessageAsync(string message, bool isAt = false)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            if (isAt) msgList.Add(new AtMessage(MemberId));
            msgList.Add(new PlainMessage(message));
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<long?> ReplyGroupMessageAsync(List<BaseContent> chainList, bool isAt = false)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            if (isAt) msgList.Add(new AtMessage(MemberId));
            msgList.AddRange(await chainList.ToMiraiMessageAsync(UploadTarget.Group));
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<long?> ReplyGroupMessageWithAtAsync(string plainMsg)
        {
            if (plainMsg.StartsWith(" ") == false) plainMsg = " " + plainMsg;
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(MemberId));
            msgList.Add(new PlainMessage(plainMsg));
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<long?> ReplyGroupMessageWithAtAsync(params BaseContent[] chainArr)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(MemberId));
            msgList.AddRange(await new List<BaseContent>(chainArr).ToMiraiMessageAsync(UploadTarget.Group));
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<long?> ReplyGroupMessageWithAtAsync(List<BaseContent> chainList)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(MemberId));
            msgList.AddRange(await chainList.ToMiraiMessageAsync(UploadTarget.Group));
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<long?> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "")
        {
            template = template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            if (template.StartsWith(" ") == false) template = " " + template;
            IChatMessage[] msgList = await template.SplitToChainAsync().ToMiraiMessageAsync(UploadTarget.Group);
            return await Session.SendGroupMessageAsync(GroupId, msgList);
        }

        public override async Task<long?> SendTempMessageAsync(List<BaseContent> contentList)
        {
            IChatMessage[] msgArr = await contentList.ToMiraiMessageAsync(UploadTarget.Group);
            return await Session.SendTempMessageAsync(MemberId, GroupId, msgArr);
        }

        public override async Task RevokeGroupMessageAsync(long msgId, long groupId)
        {
            try
            {
                if (msgId <= 0) return;
                await Session.RevokeMessageAsync((int)msgId, groupId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群消息撤回失败");
            }
        }


    }
}
