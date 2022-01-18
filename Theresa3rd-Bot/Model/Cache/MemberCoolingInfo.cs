using System;

namespace Theresa3rd_Bot.Model.Cache
{
    public class MemberCoolingInfo
    {
        public long MemberId { get; set; }

        public bool Handing { get; set; }

        public DateTime? LastGetSTTime { get; set; }

        public DateTime? LastSaucenaoTime { get; set; }

        public MemberCoolingInfo(long memberId)
        {
            this.MemberId = memberId;
        }

    }
}
