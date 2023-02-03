namespace TheresaBot.Main.Model.Base
{
    public abstract class BaseWorkInfo
    {
        public abstract bool IsR18 { get; }
        public abstract bool IsGif { get; }
        public abstract bool IsAI { get; }
        public abstract bool IsImproper { get; }
        public abstract string PixivId { get; }
        public abstract string hasBanTag();
        public abstract List<string> getTags();
        public abstract List<string> getOriginalUrls();
        public List<string> Tags
        {
            get
            {
                List<string> tags = new List<string>();
                if (IsR18) tags.Add("R-18");
                if (IsAI) tags.Add("AI绘图");
                if (IsGif) tags.Add("动图");
                tags.AddRange(getTags());
                return tags.Distinct().ToList();
            }
        }
        
    }
}
