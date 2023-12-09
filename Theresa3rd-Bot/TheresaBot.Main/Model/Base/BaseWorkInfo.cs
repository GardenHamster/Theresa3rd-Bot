namespace TheresaBot.Main.Model.Base
{
    public abstract class BaseWorkInfo
    {
        public abstract bool IsR18 { get; }
        public abstract bool IsGif { get; }
        public abstract bool IsAI { get; }
        public abstract bool IsImproper { get; }
        public abstract int PixivId { get; }
        public abstract int UserId { get; }
        public abstract string Title { get; }
        public abstract string UserName { get; }
        public abstract List<string> HavingBanTags();
        public abstract List<string> GetTags();
        public abstract List<string> GetOriginalUrls();
        public List<string> Tags
        {
            get
            {
                List<string> tags = new List<string>();
                if (IsR18) tags.Add("R-18");
                if (IsAI) tags.Add("AI绘图");
                if (IsGif) tags.Add("动图");
                tags.AddRange(GetTags());
                return tags.Distinct().ToList();
            }
        }

    }
}
