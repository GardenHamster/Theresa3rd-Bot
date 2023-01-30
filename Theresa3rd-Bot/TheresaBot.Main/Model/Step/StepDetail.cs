using TheresaBot.Main.Command;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Step
{
    public abstract class StepDetail
    {
        public bool IsFinish { get; set; }

        public DateTime? StartTime { get; set; }

        public int WaitSecond { get; set; }

        public string Question { get; set; }

        public GroupRelay Relay { get; set; }

        public string Answer { get; set; }

        public Func<GroupCommand, GroupRelay, Task<bool>> CheckInput { get; set; }

        public Func<StepInfo, StepDetail, Task<string>> StepQuestion { get; set; }

        public StepDetail(int waitSecond, string question, Func<GroupCommand, GroupRelay, Task<bool>> checkInput = null)
        {
            this.WaitSecond = waitSecond;
            this.Question = question;
            this.CheckInput = checkInput;
        }

        public StepDetail(int waitSecond, Func<StepInfo, StepDetail, Task<string>> stepQuestion, Func<GroupCommand, GroupRelay, Task<bool>> checkInput = null)
        {
            this.WaitSecond = waitSecond;
            this.StepQuestion = stepQuestion;
            this.CheckInput = checkInput;
        }

        public abstract List<string> GetReplyImageUrls();

        public void StartStep()
        {
            this.StartTime = DateTime.Now;
        }

        public void FinishStep(GroupRelay relay)
        {
            this.Relay = relay;
            this.Answer = relay.Message;
            this.IsFinish = true;
        }



    }
}
