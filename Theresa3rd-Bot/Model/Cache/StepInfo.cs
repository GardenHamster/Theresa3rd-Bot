using Mirai.CSharp.HttpApi.Models.ChatMessages;
using Mirai.CSharp.HttpApi.Models.EventArgs;
using Mirai.CSharp.HttpApi.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Cache
{
    public class StepInfo
    {
        public long GroupId { get; set; }

        public long MemberId { get; set; }

        public bool IsActive { get; set; }

        public bool IsRemindTimeout { get; set; }

        public List<StepDetail> StepDetails { get; set; }

        public StepInfo(long groupId, long memberId, bool isRemindTimeout = true)
        {
            this.IsActive = true;
            this.GroupId = groupId;
            this.MemberId = memberId;
            this.IsRemindTimeout = isRemindTimeout;
            this.StepDetails = new List<StepDetail>();
        }

        public void AddStep(StepDetail stepDetail)
        {
            StepDetails.Add(stepDetail);
        }

        public Task<bool> HandleStep(IMiraiHttpSession session, IGroupMessageEventArgs args)
        {
            return Task.Run(async () =>
            {
                try
                {
                    IsActive = true;
                    if (StepDetails == null || StepDetails.Count == 0) return false;
                    for (int i = 0; i < StepDetails.Count; i++)
                    {
                        StepDetail stepDetail = StepDetails[i];
                        if (stepDetail.IsFinish) continue;
                        if (stepDetail.StartTime == null) stepDetail.StartStep();
                        if (stepDetail.StepQuestion != null)
                        {
                            stepDetail.Question = await stepDetail.StepQuestion(session, args, this, stepDetail);
                            if (string.IsNullOrEmpty(stepDetail.Question)) return false;
                        }

                        await session.SendGroupMessageWithAtAsync(args, stepDetail.Question);

                        while (true)
                        {
                            if (stepDetail.IsFinish) break;
                            int secondDiff = DateTimeHelper.GetSecondDiff(stepDetail.StartTime.Value, DateTime.Now);
                            if (secondDiff < 0 || secondDiff >= stepDetail.WaitSecond)
                            {
                                if(IsRemindTimeout) await session.SendGroupMessageWithAtAsync(args, new PlainMessage(" 操作超时了，请重新发送指令开始操作"));
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
