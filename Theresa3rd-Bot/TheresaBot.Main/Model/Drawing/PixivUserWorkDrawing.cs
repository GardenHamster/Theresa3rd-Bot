using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Helper;
using TheresaBot.Main.Model.Pixiv;

namespace TheresaBot.Main.Model.Drawing
{
    public class PixivUserWorkDrawing : BasePixivDrawing
    {
        public PixivUserWorkInfo WorkInfo { get; set; }
        public override string PixivId => WorkInfo.id;
        public override string ImageHttpUrl => WorkInfo.url.ThumbToSmallUrl();
        public override string ImageSavePath { get; protected set; }

        public PixivUserWorkDrawing(PixivUserWorkInfo workInfo)
        {
            this.WorkInfo = workInfo;
            this.ImageSavePath = ImageHttpUrl.GetPreviewImgSaveName(PixivId);
        }

    }
}
