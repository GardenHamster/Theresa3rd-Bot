using System.Collections.Generic;
using System.Linq;
using Theresa3rd_Bot.Model.Base;
using Theresa3rd_Bot.Model.Lolicon;
using Theresa3rd_Bot.Type;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Lolisuki
{
    public class LolisukiResult
    {
        public string error { get; set; }

        public List<LolisukiData> data { get; set; }
    }

    public class LolisukiData : BaseWorkInfo
    {
        public long pid { get; set; }
        public int p { get; set; }
        public int total { get; set; }
        public string uid { get; set; }
        public string author { get; set; }
        public int level { get; set; }
        public LolisukiTaste taste { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool r18 { get; set; }
        public bool gif { get; set; }
        public int aiType { get; set; }
        public bool original { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string ext { get; set; }
        public long uploadDate { get; set; }
        public LolisukiUrls urls { get; set; }
        public List<LolisukiUrls> fullUrls { get; set; }
        public List<string> tags { get; set; }
        public List<string> extags { get; set; }

        public override bool IsGif
        {
            get { return gif; }
        }

        public override bool IsAI
        {
            get { return aiType > 1; }
        }

        public override string PixivId
        {
            get { return pid.ToString(); }
        }

        public override bool IsR18
        {
            get
            {
                //xRestrict=1为R18,xRestrict=2为R18G
                if (r18) return true;
                if (tags != null && tags.IsR18()) return true;
                if (extags != null && tags.IsR18()) return true;
                return false;
            }
        }

        public override bool IsImproper
        {
            get
            {
                if (tags != null && tags.IsImproper()) return true;
                if (extags != null && tags.IsImproper()) return true;
                return false;
            }
        }

        public override List<string> getTags()
        {
            List<string> tagList = new List<string>();
            tagList.AddRange(tags);
            tagList.AddRange(extags);
            return tagList;
        }

        public override string hasBanTag()
        {
            return tags?.hasBanTags() ?? extags?.hasBanTags();
        }

        public override List<string> getOriginalUrls()
        {
            if (urls == null) return new List<string>();
            return new List<string>() { urls.original };
        }
    }

    public class LolisukiUrls
    {
        public string thumb { get; set; }
        public string small { get; set; }
        public string regular { get; set; }
        public string original { get; set; }
    }
}
