using Mirai.CSharp.HttpApi.Models.EventArgs;
using System;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Cache
{
    public class StepDetail
    {
        public bool IsFinish { get; set; }

        public DateTime? StartTime { get; set; }

        public int WaitSecond { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        private delegate bool CheckAnswer(string message);

        public IGroupMessageEventArgs Args { get; set; }

        public StepDetail(int waitSecond, string question, CheckAnswer checkAnswer)
        {
            this.WaitSecond = waitSecond;
            this.Question = question;
        }

        public bool IsTimeout()
        {
            if (StartTime == null) return false;
            int seconds = DateTimeHelper.GetSecondDiff(StartTime.Value, DateTime.Now);
            return seconds >= WaitSecond;
        }

        public void StartStep()
        {
            this.StartTime = DateTime.Now;
        }

        public void FinishStep(IGroupMessageEventArgs args, string answer)
        {
            this.Args = args;
            this.IsFinish = true;
            this.Answer = answer;
        }



    }
}
