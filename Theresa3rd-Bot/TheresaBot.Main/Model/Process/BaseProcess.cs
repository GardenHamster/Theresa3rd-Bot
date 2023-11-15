using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Process
{
    public abstract class BaseProcess
    {
        public bool IsFinish { get; set; }

        public long MemberId { get; set; }

        public List<BaseStep> StepInfos { get; set; }

        public BaseProcess(long memberId)
        {
            IsFinish = false;
            MemberId = memberId;
            StepInfos = new List<BaseStep>();
        }

        public abstract BaseStep CreateStep(string question, int waitSecond = 60);

        public abstract BaseStep CreateStep<T>(string question, Func<string, Task<T>> checkInput, int waitSecond = 60);

        public abstract BaseStep CreateStep<T>(string question, Func<BaseRelay, Task<T>> checkInput, int waitSecond = 60);

        public virtual async Task StartProcessing()
        {
            try
            {
                foreach (var stepInfo in StepInfos)
                {
                    await stepInfo.SendQuestion();
                    await stepInfo.StartAndWaitAsync();
                }
            }
            finally
            {
                IsFinish = true;
            }
        }



    }
}
