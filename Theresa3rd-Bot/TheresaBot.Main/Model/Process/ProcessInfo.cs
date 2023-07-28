using TheresaBot.Main.Command;
using TheresaBot.Main.Exceptions;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Model.Step
{
    public class ProcessInfo
    {
        public long GroupId { get; set; }

        public long MemberId { get; set; }

        public bool IsFinish { get; set; }

        public GroupCommand GroupCommand { get; set; }

        public List<StepInfo> StepDetails { get; set; }

        public ProcessInfo(GroupCommand command)
        {
            this.IsFinish = false;
            this.GroupCommand = command;
            this.GroupId = command.GroupId;
            this.MemberId = command.MemberId;
            this.StepDetails = new List<StepInfo>();
        }

        public StepInfo CreateStep(string question, int waitSecond = 60)
        {
            var stepDetail = new StepInfo(GroupCommand, question, waitSecond);
            StepDetails.Add(stepDetail);
            return stepDetail;
        }

        public StepInfo CreateStep(string question, Func<string, Task> checkInput, int waitSecond = 60)
        {
            var stepDetail = new StepInfo(GroupCommand, question, waitSecond, checkInput);
            StepDetails.Add(stepDetail);
            return stepDetail;
        }

        public async Task StartProcessing()
        {
            try
            {
                for (int i = 0; i < StepDetails.Count; i++)
                {
                    StepInfo stepDetail = StepDetails[i];
                    if (stepDetail.IsFinish) continue;
                    if (stepDetail.StartTime is null) stepDetail.StartStep();
                    await GroupCommand.ReplyGroupMessageWithAtAsync(stepDetail.Question);
                    while (true)
                    {
                        if (stepDetail.IsFinish) break;
                        int secondDiff = DateTimeHelper.GetSecondDiff(stepDetail.StartTime.Value, DateTime.Now);
                        if (secondDiff < 0 || secondDiff >= stepDetail.WaitSecond)
                        {
                            throw new ProcessException("操作超时了，请重新发送指令开始操作");
                        }
                        await Task.Delay(1000);
                    }
                }
            }
            finally
            {
                IsFinish = true;
            }
        }


    }
}
