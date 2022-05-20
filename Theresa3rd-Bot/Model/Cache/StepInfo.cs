using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Cache
{
    public class StepInfo
    {
        public long GroupId { get; set; }

        public long MemberId { get; set; }

        public bool IsActive { get; set; }

        public List<StepDetail> StepDetails { get; set; }

        public StepInfo(long groupId, long memberId)
        {
            this.IsActive = true;
            this.GroupId = groupId;
            this.MemberId = memberId;
            this.StepDetails = new List<StepDetail>();
        }

        public void AddStep(StepDetail stepDetail)
        {
            StepDetails.Add(stepDetail);
        }


    }
}
