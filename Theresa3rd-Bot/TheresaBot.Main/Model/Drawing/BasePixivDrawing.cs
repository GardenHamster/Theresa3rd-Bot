using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Drawing
{
    public abstract class BasePixivDrawing
    {
        public abstract string PixivId { get; }
        public abstract string ImageHttpUrl { get; }
        public abstract string ImageSavePath { get; protected set; }
    }
}
