using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Process
{
    public record StepInfo
    {
        public GroupCommand GroupCommand { get; init; }

        public bool IsFinish { get; private set; }

        public DateTime? StartTime { get; private set; }

        public int WaitSecond { get; private set; }

        public string Question { get; private set; }

        public GroupRelay Relay { get; private set; }

        public string Answer { get; private set; }

        public Func<string, Task> CheckInputFunc { get; set; }

        public Func<GroupRelay, Task> CheckReplyFunc { get; set; }

        public StepInfo(GroupCommand command, string question, int waitSecond)
        {
            GroupCommand = command;
            WaitSecond = waitSecond;
            Question = question;
            CheckInputFunc = (o) => Task.CompletedTask;
            CheckReplyFunc = (o) => Task.CompletedTask;
        }

        public StepInfo(GroupCommand command, string question, int waitSecond, Func<string, Task> checkInput)
        {
            GroupCommand = command;
            WaitSecond = waitSecond;
            Question = question;
            CheckInputFunc = checkInput;
            CheckReplyFunc = (o) => Task.CompletedTask;
        }

        public StepInfo(GroupCommand command, string question, int waitSecond, Func<GroupRelay, Task> checkReply)
        {
            GroupCommand = command;
            WaitSecond = waitSecond;
            Question = question;
            CheckInputFunc = (o) => Task.CompletedTask;
            CheckReplyFunc = checkReply;
        }

        public string AnswerForString() => Answer;

        public int AnswerForInt() => Convert.ToInt32(Answer);

        public long AnswerForLong() => Convert.ToInt64(Answer);

        public T AnswerForEnum<T>() where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), Convert.ToInt32(Answer));
        }

        public void StartStep()
        {
            StartTime = DateTime.Now;
        }

        public void FinishStep(GroupRelay relay)
        {
            Relay = relay;
            Answer = relay.Message;
            IsFinish = true;
        }

        public async Task<bool> CheckInputAsync(GroupRelay relay)
        {
            try
            {
                await CheckReplyFunc(relay);
                await CheckInputFunc(relay.Message);
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
