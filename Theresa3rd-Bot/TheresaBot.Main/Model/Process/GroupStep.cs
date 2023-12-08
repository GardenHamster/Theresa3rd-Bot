using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Process
{
    public record GroupStep<T> : BaseStep
    {
        public GroupCommand GroupCommand { get; init; }

        public Func<string, Task<T>> CheckInputFunc { get; set; }

        public Func<BaseRelay, Task<T>> CheckReplyFunc { get; set; }

        public T Answer { get; private set; }

        public GroupStep(GroupCommand command, string question, int waitSecond, Func<string, Task<T>> checkInput) : base(question, waitSecond)
        {
            GroupCommand = command;
            CheckInputFunc = checkInput;
        }

        public GroupStep(GroupCommand command, string question, int waitSecond, Func<BaseRelay, Task<T>> checkReply) : base(question, waitSecond)
        {
            GroupCommand = command;
            CheckReplyFunc = checkReply;
        }

        public override async Task SendQuestion()
        {
            await GroupCommand.ReplyGroupMessageWithAtAsync(Question);
        }

        public override async Task ReplyError(BaseRelay relay, ProcessException ex)
        {
            await GroupCommand.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
        }

        public override async Task<bool> CheckInputAsync(BaseRelay relay)
        {
            try
            {
                if (CheckReplyFunc is not null)
                {
                    Answer = await CheckReplyFunc(relay);
                }
                if (CheckInputFunc is not null)
                {
                    Answer = await CheckInputFunc(relay.Message);
                }
                return true;
            }
            catch (NoAnswerException)
            {
                return false;
            }
            catch (ProcessException ex)
            {
                await GroupCommand.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
                return false;
            }
        }

    }
}
