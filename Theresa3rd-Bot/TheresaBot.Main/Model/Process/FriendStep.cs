using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Process
{
    public record FriendStep<T> : BaseStep
    {
        public FriendCommand FriendCommand { get; init; }

        public Func<string, Task<T>> CheckInputFunc { get; init; }

        public Func<BaseRelay, Task<T>> CheckReplyFunc { get; init; }

        public T Answer { get; private set; }

        public FriendStep(FriendCommand command, string question, int waitSecond) : base(question, waitSecond)
        {
            FriendCommand = command;
        }

        public FriendStep(FriendCommand command, string question, int waitSecond, Func<string, Task<T>> checkInput) : base(question, waitSecond)
        {
            FriendCommand = command;
            CheckInputFunc = checkInput;
        }

        public FriendStep(FriendCommand command, string question, int waitSecond, Func<BaseRelay, Task<T>> checkReply) : base(question, waitSecond)
        {
            FriendCommand = command;
            CheckReplyFunc = checkReply;
        }

        public override async Task SendQuestion()
        {
            await FriendCommand.ReplyFriendMessageAsync(Question);
        }

        public override async Task ReplyError(BaseRelay relay, ProcessException ex)
        {
            await FriendCommand.ReplyFriendMessageAsync(ex.RemindMessage);
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
                await FriendCommand.ReplyFriendMessageAsync(ex.RemindMessage);
                return false;
            }
        }

    }
}
