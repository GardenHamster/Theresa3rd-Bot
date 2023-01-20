﻿using System.Collections.Generic;
using System.Linq;
using Theresa3rd_Bot.Model.Base;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Lolicon
{
    public class LoliconResultV2
    {
        public string error { get; set; }

        public List<LoliconDataV2> data { get; set; }
    }

    public class LoliconDataV2 : BaseWorkInfo
    {
        public long pid { get; set; }
        public int p { get; set; }
        public string uid { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public bool r18 { get; set; }
        public int aiType { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public List<string> tags { get; set; }
        public string ext { get; set; }
        public long uploadDate { get; set; }
        public LoliconUrlsV2 urls { get; set; }
        public override bool IsR18
        {
            get { return r18; }
        }
        public override bool IsGif
        {
            get { return tags != null && tags.IsGif(); }
        }
        public override bool IsAI
        {
            get { return aiType > 1; }
        }
        public override bool IsImproper
        {
            get { return tags != null && tags.IsImproper(); }
        }
        public override string PixivId
        {
            get { return pid.ToString(); }
        }

        public override List<string> getTags() => tags ?? new List<string>();

        public override string hasBanTag() => tags?.hasBanTags();

        public override List<string> getOriginalUrls()
        {
            if (urls == null) return new List<string>();
            return new List<string>() { urls.original };
        }
    }

    public class LoliconUrlsV2
    {
        public string original { get; set; }
    }
}
