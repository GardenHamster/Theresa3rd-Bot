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
    }
}
