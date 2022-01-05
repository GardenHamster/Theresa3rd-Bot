using System;

namespace Theresa3rd_Bot.Model.Cache
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
