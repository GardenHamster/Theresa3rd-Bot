using Mirai.CSharp.HttpApi.Models.EventArgs;
using System;

namespace Theresa3rd_Bot.Model.Cache
{
    public class StepDetail
    {
        public bool IsFinish { get; set; }

        public DateTime? StartTime { get; set; }

        public int WaitSecond { get; set; }

        public string Question { get; set; }

        public IGroupMessageEventArgs Args { get; set; }

        public StepDetail(int waitSecond,string question)
        {
            this.WaitSecond = waitSecond;
            this.Question = question;
        }

        public void StartStep()
        {
            this.StartTime = DateTime.Now;
        }

        public void FinishStep(IGroupMessageEventArgs args)
        {
            this.Args = args;
            this.IsFinish= true;
        }




    }
}
