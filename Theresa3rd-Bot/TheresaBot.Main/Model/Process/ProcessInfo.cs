using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Process
{
    public class ProcessInfo
    {
        public long GroupId { get; set; }

        public long MemberId { get; set; }

        public bool IsFinish { get; set; }

        public GroupCommand GroupCommand { get; set; }

        public List<StepInfo> StepInfos { get; set; }

        public ProcessInfo(GroupCommand command)
        {
            IsFinish = false;
            GroupCommand = command;
            GroupId = command.GroupId;
            MemberId = command.MemberId;
            StepInfos = new List<StepInfo>();
        }

        public StepInfo CreateStep(string question, int waitSecond = 60)
        {
            var stepInfo = new StepInfo(GroupCommand, question, waitSecond);
            StepInfos.Add(stepInfo);
            return stepInfo;
        }

        public StepInfo CreateStep(string question, Func<string, Task> checkInput, int waitSecond = 60)
        {
            var stepInfo = new StepInfo(GroupCommand, question, waitSecond, checkInput);
            StepInfos.Add(stepInfo);
            return stepInfo;
        }

        public StepInfo CreateStep(string question, Func<GroupRelay, Task> checkInput, int waitSecond = 60)
        {
            var stepInfo = new StepInfo(GroupCommand, question, waitSecond, checkInput);
            StepInfos.Add(stepInfo);
            return stepInfo;
        }

        public async Task StartProcessing()
        {
            try
            {
                foreach (var stepInfo in StepInfos)
                {
                    stepInfo.StartStep();
                    await GroupCommand.ReplyGroupMessageWithAtAsync(stepInfo.Question);
                    await StartAndWaitAsync(stepInfo);
                }
            }
            finally
            {
                IsFinish = true;
            }
        }

        private async Task StartAndWaitAsync(StepInfo stepInfo)
        {
            while (true)
            {
                if (stepInfo.IsFinish) break;
                int secondDiff = DateTime.Now.SecondDiff(stepInfo.StartTime.Value);
                if (secondDiff < 0 || secondDiff >= stepInfo.WaitSecond)
                {
                    throw new StepTimeoutException("操作超时了，请重新发送指令开始操作");
                }
                await Task.Delay(500);
            }
        }

    }
}
