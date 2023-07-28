using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Step
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

        public StepInfo(GroupCommand command, string question, int waitSecond)
        {
            this.GroupCommand = command;
            this.WaitSecond = waitSecond;
            this.Question = question;
            this.CheckInputFunc = (o) => Task.CompletedTask;
        }

        public StepInfo(GroupCommand command, string question, int waitSecond, Func<string, Task> checkInput)
        {
            this.GroupCommand = command;
            this.WaitSecond = waitSecond;
            this.Question = question;
            this.CheckInputFunc = checkInput;
        }

        public void StartStep()
        {
            this.StartTime = DateTime.Now;
        }

        public void FinishStep(GroupRelay relay)
        {
            this.Relay = relay;
            this.Answer = relay.Answer;
            this.IsFinish = true;
        }

        public async Task<bool> CheckInputAsync(GroupRelay relay)
        {
            try
            {
                await CheckInputFunc(relay.Answer);
                return true;
            }
            catch (ProcessException ex)
            {
                await GroupCommand.ReplyGroupMessageWithAtAsync(ex.RemindMessage);
                return false;
            }
        }

        public string AnswerForString()
        {
            return Answer;
        }

        public T AnswerForEnum<T>() where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), Convert.ToInt32(Answer));
        }



    }
}
