using TheresaBot.Main.Model.Base;

namespace TheresaBot.Main.Model.Content
{
    public record PixivSetuContent : SetuContent
    {
        public BaseWorkInfo WorkInfo { get; private set; }

        protected PixivSetuContent() { }

        public PixivSetuContent(List<BaseContent> setuInfos, List<FileInfo> images, BaseWorkInfo workInfo)
        {
            this.SetuInfos = setuInfos;
            this.SetuImages = images;
            this.WorkInfo = workInfo;
        }

    }
}
