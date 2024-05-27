using TheresaBot.Core.Helper;
using TheresaBot.Core.Model.Pixiv;

namespace TheresaBot.Core.Model.Drawing
{
    public class PixivUserWorkDrawing : BasePixivDrawing
    {
        public PixivProfileDetail ProfileDetail { get; set; }
        public override string PixivId => ProfileDetail.WorkInfo.id;
        public override string ImageHttpUrl => ProfileDetail.WorkInfo.url.ThumbToSmallUrl();
        public override string ImageSavePath { get; protected set; }

        public PixivUserWorkDrawing(PixivProfileDetail detail)
        {
            this.ProfileDetail = detail;
            this.ImageSavePath = ImageHttpUrl.GetPreviewImgSaveName(PixivId);
        }

    }
}
