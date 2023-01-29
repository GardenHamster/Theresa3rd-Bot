using System;
using TheresaBot.Main.Model.Cache;
using TheresaBot.Main.Model.Content;
using TheresaBot.Main.Model.Invoker;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Command
{
    public abstract class GroupCommand : BaseCommand
    {
        public long GroupId { get; init; }
        public CommandHandler<GroupCommand> HandlerInvoker { get; init; }

        public GroupCommand(CommandHandler<GroupCommand> invoker, int msgId, string[] keyWords, string instruction, long groupId, long memberId) : base(msgId, keyWords, invoker.CommandType, instruction, memberId)
        {
            this.HandlerInvoker = invoker;
            this.MemberId = memberId;
            this.GroupId = groupId;
        }

        public abstract List<string> GetReplyImageUrls();

        public abstract StepDetail CreateStepDetail(int waitSecond, string question, Func<GroupCommand, GroupRelay, Task<bool>> checkInput);

        public abstract Task<int> ReplyGroupMessageAsync(string message, bool isAt = false);

        public abstract Task<int> ReplyGroupMessageAsync(List<BaseContent> chainList, bool isAt = false);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(string plainMsg);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(params BaseContent[] chainArr);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(List<BaseContent> chainList);

        public abstract Task<int> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg = "");

        public abstract Task RevokeGroupMessageAsync(int messageId, long groupId);

        public abstract Task RevokeGroupMessageAsync(List<int> messageIds, long groupId);

        public abstract Task ReplyGroupSetuAndRevokeAsync(List<BaseContent> workMsgs, List<FileInfo> setuFiles, int revokeInterval, bool isAt = false);

        public abstract Task SendTempSetuAsync(List<BaseContent> workMsgs, List<FileInfo> setuFiles = null);

        public override async Task<bool> InvokeAsync()
        {
            return await HandlerInvoker.HandleMethod.Invoke(this);
        }

    }
}
