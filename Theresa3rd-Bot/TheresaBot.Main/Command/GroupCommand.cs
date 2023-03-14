﻿using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Reporter;
using TheresaBot.Main.Session;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Command
{
    public abstract class GroupCommand : BaseCommand
    {
        public long GroupId { get; init; }
        public CommandHandler<GroupCommand> HandlerInvoker { get; init; }

        public GroupCommand(CommandHandler<GroupCommand> invoker, int msgId, string instruction, string command, long groupId, long memberId)
            : base(msgId, invoker.CommandType, instruction, command, memberId)
        {
            this.HandlerInvoker = invoker;
            this.MemberId = memberId;
            this.GroupId = groupId;
        }

        public abstract List<string> GetReplyImageUrls();

        public abstract Task<int> ReplyGroupMessageAsync(string message, bool isAt = false);

        public abstract Task<int> ReplyGroupMessageAsync(List<BaseContent> chainList, bool isAt = false);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(string plainMsg);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(params BaseContent[] chainArr);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(List<BaseContent> chainList);

        public abstract Task<int> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "");

        public abstract Task<int[]> ReplyGroupMessageAndRevokeAsync(SetuContent setuContent, int revokeInterval, bool sendImgBehind, bool isAt = false);

        public abstract Task<int[]> ReplyTempMessageAsync(SetuContent setuContent, bool sendImgBehind);

        public abstract Task RevokeGroupMessageAsync(int messageId, long groupId);

        public abstract Task RevokeGroupMessageAsync(List<int> messageIds, long groupId, int revokeInterval = 0);

        public override async Task<bool> InvokeAsync(BaseSession session, BaseReporter reporter)
        {
            return await HandlerInvoker.HandleMethod.Invoke(this, session, reporter);
        }

        public override async Task ReplyError(Exception ex, string message = "")
        {
            List<BaseContent> contents = ex.GetErrorContents(SendTarget.Group, message);
            await ReplyGroupMessageWithAtAsync(contents);
        }

    }
}