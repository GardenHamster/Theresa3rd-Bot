namespace TheresaBot.Core.Model.Drawing
{
    public abstract class BasePixivDrawing
    {
        public abstract string PixivId { get; }
        public abstract string ImageHttpUrl { get; }
        public abstract string ImageSavePath { get; protected set; }
    }
}
