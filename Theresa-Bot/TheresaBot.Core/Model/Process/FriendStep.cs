using TheresaBot.Core.Command;
using TheresaBot.Core.Exceptions;
using TheresaBot.Core.Helper;
using TheresaBot.Core.Relay;

namespace TheresaBot.Core.Model.Process
{
    public record FriendStep<T> : BaseStep
    {
        public PrivateCommand FriendCommand { get; init; }

        public Func<string, Task<T>> CheckInputFunc { get; init; }

        public Func<BaseRelay, Task<T>> CheckReplyFunc { get; init; }

        public T Answer { get; private set; }

        public FriendStep(PrivateCommand command, string question, int waitSecond, Func<string, Task<T>> checkInput) : base(question, waitSecond)
        {
            FriendCommand = command;
            CheckInputFunc = checkInput;
        }

        public FriendStep(PrivateCommand command, string question, int waitSecond, Func<BaseRelay, Task<T>> checkReply) : base(question, waitSecond)
        {
            FriendCommand = command;
            CheckReplyFunc = checkReply;
        }

        public override async Task SendQuestion()
        {
            await FriendCommand.ReplyPrivateMessageAsync(Question);
        }

        public override async Task ReplyError(BaseRelay relay, ProcessException ex)
        {
            await FriendCommand.ReplyPrivateMessageAsync(ex.RemindMessage);
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
                await FriendCommand.ReplyPrivateMessageAsync(ex.RemindMessage);
                return false;
            }
        }

    }
}
