using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Theresa3rd_Bot.Model.Content;
using Theresa3rd_Bot.Model.Invoker;

namespace Theresa3rd_Bot.BotPlatform.Base.Command
{
    public abstract class GroupCommand : BaseCommand
    {
        public long GroupId { get; init; }
        public CommandHandler<GroupCommand> HandlerInvoker { get; init; }

        public GroupCommand(CommandHandler<GroupCommand> invoker, string[] keyWords, string instruction, long groupId, long memberId) : base(keyWords, invoker.CommandType, instruction, memberId)
        {
            this.HandlerInvoker = invoker;
            this.MemberId = memberId;
            this.GroupId = groupId;
        }

        public abstract Task<int> ReplyGroupMessageAsync(string message, bool isAt = false);

        public abstract Task<int> ReplyGroupMessageAsync(List<ChatContent> chainList, bool isAt = false);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(string plainMsg);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(params ChatContent[] chainArr);

        public abstract Task<int> ReplyGroupMessageWithAtAsync(List<ChatContent> chainList);

        public abstract Task<int> ReplyGroupTemplateWithAtAsync(string template, string defaultmsg);

        public abstract Task RevokeGroupMessageAsync(int messageId, long groupId);

        public abstract Task RevokeGroupMessageAsync(List<int> messageIds, long groupId);

        public abstract Task SendGroupSetuAsync(List<ChatContent> workMsgs, List<FileInfo> setuFiles, long groupId, bool isShowImg);

        public abstract Task ReplyGroupSetuAndRevokeAsync(List<ChatContent> workMsgs, List<FileInfo> setuFiles, int revokeInterval, bool isAt = false);

        public abstract Task SendTempSetuAsync(List<ChatContent> workMsgs, List<FileInfo> setuFiles = null);

        public override async Task<bool> InvokeAsync()
        {
            return await HandlerInvoker.HandleMethod.Invoke(this);
        }

    }
}
