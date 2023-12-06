using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Process
{
    public abstract record BaseStep
    {
        public BaseRelay Relay { get; private set; }

        public bool IsFinish { get; private set; }

        public int WaitSecond { get; private set; }

        public string Question { get; private set; }

        public abstract Task SendQuestion();

        public abstract Task ReplyError(BaseRelay relay, ProcessException ex);

        public abstract Task<bool> CheckInputAsync(BaseRelay relay);

        public BaseStep(string question, int waitSecond)
        {
            WaitSecond = waitSecond;
            Question = question;
        }

        public void Finish(BaseRelay relay)
        {
            Relay = relay;
            IsFinish = true;
        }

        public async Task StartAndWaitAsync()
        {
            for (int second = WaitSecond; second > 0; second--)
            {
                if (IsFinish) return;
                await Task.Delay(1000);
            }
            throw new StepTimeoutException("操作超时了，请重新发送指令开始操作");
        }



    }
}
