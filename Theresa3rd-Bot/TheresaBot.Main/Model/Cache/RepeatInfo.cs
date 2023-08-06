namespace TheresaBot.Main.Model.Cache
{
    public class RepeatInfo
    {
        public long MemberId { get; set; }

        public string SendContent { get; set; }

        public DateTime SendTime { get; set; }

        public RepeatInfo(long memberId, string sendContent)
        {
            this.MemberId = memberId;
            this.SendContent = sendContent;
            this.SendTime = DateTime.Now;
        }

    }
}
