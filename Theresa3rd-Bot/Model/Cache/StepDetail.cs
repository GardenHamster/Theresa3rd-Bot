using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Threading.Tasks;
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

        public IGroupMessageEventArgs Args { get; set; }

        public Func<IMiraiHttpSession, IGroupMessageEventArgs, string, Task<bool>> CheckInput { get; set; }

        public Func<IMiraiHttpSession, IGroupMessageEventArgs, StepInfo, StepDetail, Task<string>> StepQuestion { get; set; }

        public StepDetail(int waitSecond, string question, Func<IMiraiHttpSession, IGroupMessageEventArgs, string, Task<bool>> checkInput = null)
        {
            this.WaitSecond = waitSecond;
            this.Question = question;
            this.CheckInput = checkInput;
        }

        public StepDetail(
            int waitSecond,
            Func<IMiraiHttpSession, IGroupMessageEventArgs, StepInfo, StepDetail, Task<string>> stepQuestion,
            Func<IMiraiHttpSession, IGroupMessageEventArgs, string, Task<bool>> checkInput = null)
        {
            this.WaitSecond = waitSecond;
            this.StepQuestion = stepQuestion;
            this.CheckInput = checkInput;
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
            this.Answer = answer?.Trim();
            this.IsFinish = true;
        }



    }
}
