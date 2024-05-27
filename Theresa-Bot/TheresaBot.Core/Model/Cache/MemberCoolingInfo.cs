namespace TheresaBot.Core.Model.Cache
{
    public class MemberCoolingInfo
    {
        public long MemberId { get; set; }

        public bool Handing { get; set; }

        public DateTime? LastGetSetuTime { get; set; }

        public DateTime? LastSaucenaoTime { get; set; }

        public MemberCoolingInfo(long memberId)
        {
            this.MemberId = memberId;
        }

    }
}
