using System;
using System.Threading.Tasks;
using TheresaBot.Main.BotPlatform.Base.Command;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Model.Cache
{
    public class StepDetail
    {
        public bool IsFinish { get; set; }

        public DateTime? StartTime { get; set; }

        public int WaitSecond { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public Func<GroupCommand, string, Task<bool>> CheckInput { get; set; }

        public Func<StepInfo, StepDetail, Task<string>> StepQuestion { get; set; }

        public StepDetail(int waitSecond, string question, Func<GroupCommand, string, Task<bool>> checkInput = null)
        {
            this.WaitSecond = waitSecond;
            this.Question = question;
            this.CheckInput = checkInput;
        }

        public StepDetail(int waitSecond, Func<StepInfo, StepDetail, Task<string>> stepQuestion, Func<GroupCommand, string, Task<bool>> checkInput = null)
        {
            this.WaitSecond = waitSecond;
            this.StepQuestion = stepQuestion;
            this.CheckInput = checkInput;
        }

        public bool IsTimeout()
        {
            if (StartTime is null) return false;
            int seconds = DateTimeHelper.GetSecondDiff(StartTime.Value, DateTime.Now);
            return seconds >= WaitSecond;
        }

        public void StartStep()
        {
            this.StartTime = DateTime.Now;
        }

        public void FinishStep(string answer)
        {
            this.Answer = answer?.Trim();
            this.IsFinish = true;
        }



    }
}
