using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Cache
{
    public class StepInfo
    {
        public long MemberId { get; set; }

        public bool IsActive { get; set; }

        public List<StepDetail> StepList { get; set; }

        public StepInfo(long memberId, List<StepDetail> stepList)
        {
            IsActive = true;
            MemberId = memberId;
            StepList = stepList;
        }

    }
}
