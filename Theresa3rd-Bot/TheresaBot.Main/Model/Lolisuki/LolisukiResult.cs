using TheresaBot.Main.Datas;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Base;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Lolisuki
{
    public class LolisukiResult
    {
        public int code { get; set; }
        public string error { get; set; }
        public List<LolisukiData> data { get; set; }
    }

    public class LolisukiData : BaseWorkInfo
    {
        public int pid { get; set; }
        public int p { get; set; }
        public int total { get; set; }
        public int uid { get; set; }
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

        public override bool IsGif => gif;
        public override bool IsAI => aiType > 1;
        public override int PixivId => pid;
        public override int UserId => uid;
        public override string Title => title;
        public override string UserName => author;

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

        public override List<string> GetTags()
        {
            List<string> tagList = new List<string>();
            tagList.AddRange(tags);
            tagList.AddRange(extags);
            return tagList;
        }

        public override List<string> HavingBanTags()
        {
            return GetTags().HavingBanTags();
        }

        public override List<string> GetOriginalUrls()
        {
            if (urls is null) return new List<string>();
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
