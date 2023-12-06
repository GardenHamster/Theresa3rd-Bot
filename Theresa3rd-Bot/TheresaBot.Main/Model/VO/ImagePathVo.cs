namespace TheresaBot.Main.Model.VO
{
    public class ImagePathVo
    {
        public string HttpPath { get; set; }

        public string ServerPath { get; set; }

        public ImagePathVo(string httpPath, string serverPath)
        {
            HttpPath = httpPath;
            ServerPath = serverPath;
        }

    }
}
