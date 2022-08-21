using System;
using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Saucenao
{
    public class Ascii2dResult
    {
        public int MatchCount { get; set; }

        public DateTime StartDateTime { get; set; }

        public List<Ascii2dItem> Items { get; set; }

        public Ascii2dResult(List<Ascii2dItem> items, DateTime startDateTime, int matchCount)
        {
            this.Items = items;
            this.StartDateTime = startDateTime;
            this.MatchCount = matchCount;
        }

    }
}
