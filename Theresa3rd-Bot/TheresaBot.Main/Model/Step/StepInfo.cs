using TheresaBot.Main.Command;
using TheresaBot.Main.Helper;

namespace TheresaBot.Main.Model.Step
{
    public class StepInfo
    {
        public long GroupId { get; set; }

        public long MemberId { get; set; }

        public GroupCommand GroupCommand { get; set; }

        public bool IsActive { get; set; }

        public bool IsRemindTimeout { get; set; }

        public List<StepDetail> StepDetails { get; set; }

        public StepInfo(GroupCommand command, bool isRemindTimeout = true)
        {
            this.IsActive = true;
            this.GroupCommand = command;
            this.GroupId = command.GroupId;
            this.MemberId = command.MemberId;
            this.IsRemindTimeout = isRemindTimeout;
            this.StepDetails = new List<StepDetail>();
        }

        public void AddStep(StepDetail stepDetail)
        {
            StepDetails.Add(stepDetail);
        }

        public Task<bool> HandleStep()
        {
            return Task.Run(async () =>
            {
                try
                {
                    IsActive = true;
                    if (StepDetails is null || StepDetails.Count == 0) return false;
                    for (int i = 0; i < StepDetails.Count; i++)
                    {
                        StepDetail stepDetail = StepDetails[i];
                        if (stepDetail.IsFinish) continue;
                        if (stepDetail.StartTime is null) stepDetail.StartStep();
                        if (stepDetail.StepQuestion != null)
                        {
                            stepDetail.Question = await stepDetail.StepQuestion(this, stepDetail);
                            if (string.IsNullOrEmpty(stepDetail.Question)) return false;
                        }

                        await GroupCommand.ReplyGroupMessageWithAtAsync(stepDetail.Question);

                        while (true)
                        {
                            if (stepDetail.IsFinish) break;
                            int secondDiff = DateTimeHelper.GetSecondDiff(stepDetail.StartTime.Value, DateTime.Now);
                            if (secondDiff < 0 || secondDiff >= stepDetail.WaitSecond)
                            {
                                if (IsRemindTimeout) await GroupCommand.ReplyGroupMessageWithAtAsync("操作超时了，请重新发送指令开始操作");
                                return false;
                            }
                            await Task.Delay(500);
                        }
                    }
                    return true;
                }
                finally
                {
                    IsActive = false;
                }
            });
        }


    }
}
