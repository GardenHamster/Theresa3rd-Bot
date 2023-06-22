using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using Mirai.CSharp.Models;
using System;
using System.Collections.Generic;
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

        public MiraiGroupCommand(CommandType commandType, IMiraiHttpSession session, IGroupMessageEventArgs args, string instruction, string command, long groupId, long memberId)
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
            msgList.AddRange(await chainList.ToMiraiMessageAsync(UploadTarget.Group));
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
            msgList.AddRange(await new List<BaseContent>(chainArr).ToMiraiMessageAsync(UploadTarget.Group));
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<int> ReplyGroupMessageWithAtAsync(List<BaseContent> chainList)
        {
            List<IChatMessage> msgList = new List<IChatMessage>();
            msgList.Add(new AtMessage(MemberId));
            msgList.AddRange(await chainList.ToMiraiMessageAsync(UploadTarget.Group));
            return await Session.SendGroupMessageAsync(GroupId, msgList.ToArray());
        }

        public override async Task<int> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "")
        {
            template = template?.Trim()?.TrimLine();
            if (string.IsNullOrWhiteSpace(template)) template = defaultmsg;
            if (string.IsNullOrWhiteSpace(template)) return 0;
            if (template.StartsWith(" ") == false) template = " " + template;
            IChatMessage[] msgList = await template.SplitToChainAsync().ToMiraiMessageAsync(UploadTarget.Group);
            return await Session.SendGroupMessageAsync(GroupId, msgList);
        }

        public override async Task<int> SendTempMessageAsync(List<BaseContent> contentList)
        {
            IChatMessage[] msgArr = await contentList.ToMiraiMessageAsync(UploadTarget.Group);
            return await Session.SendTempMessageAsync(MemberId, GroupId, msgArr);
        }

        public override async Task RevokeGroupMessageAsync(long messageId, long groupId, int revokeInterval = 0)
        {
            try
            {
                if (messageId <= 0) return;
                if (revokeInterval > 0) await Task.Delay(revokeInterval * 1000);
                await Session.RevokeMessageAsync((int)messageId, groupId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "群消息撤回失败");
            }
        }


    }
}
