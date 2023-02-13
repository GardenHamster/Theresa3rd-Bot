namespace TheresaBot.Main.Model.Pixiv
{
    public class PixivWorkPath
    {
        public string Host { get; set; }

        public string ImgPath { get; set; }

        public string Ext { get; set; }

        public PixivWorkPath(string host, string imgPath, string ext)
        {
            this.Host = host;
            this.ImgPath = imgPath;
            this.Ext = ext;
        }

    }
}
