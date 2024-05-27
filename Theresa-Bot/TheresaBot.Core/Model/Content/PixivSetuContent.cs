using TheresaBot.Core.Model.Base;

namespace TheresaBot.Core.Model.Content
{
    public record PixivSetuContent : SetuContent
    {
        public BaseWorkInfo WorkInfo { get; private set; }

        protected PixivSetuContent() { }

        public PixivSetuContent(BaseContent setuInfo, List<FileInfo> images, BaseWorkInfo workInfo)
        {
            this.SetuInfos = new() { setuInfo };
            this.SetuImages = images;
            this.WorkInfo = workInfo;
        }

        public PixivSetuContent(List<BaseContent> setuInfos, List<FileInfo> images, BaseWorkInfo workInfo)
        {
            this.SetuInfos = setuInfos;
            this.SetuImages = images;
            this.WorkInfo = workInfo;
        }



    }
}
