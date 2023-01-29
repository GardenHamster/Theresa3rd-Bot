using System;

namespace TheresaBot.Main.Model.Cache
{
    public class RepeatInfo
    {
        public long MemberId { get; set; }

        public string Word { get; set; }

        public DateTime SendTime { get; set; }

        public RepeatInfo(long memberId, string word)
        {
            this.MemberId = memberId;
            this.Word = word;
            this.SendTime = DateTime.Now;
        }

    }
}
