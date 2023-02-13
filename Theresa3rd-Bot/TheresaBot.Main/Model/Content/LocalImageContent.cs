using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Content
{
    public class LocalImageContent : BaseContent
    {
        public FileInfo FileInfo { get; set; }
        public SendTarget SendTarget { get; set; }

        public LocalImageContent(SendTarget target, string fullFilePath)
        {
            this.SendTarget = target;
            this.FileInfo = new FileInfo(fullFilePath);
        }

        public LocalImageContent(SendTarget target, FileInfo fileInfo)
        {
            this.SendTarget = target;
            this.FileInfo = fileInfo;
        }

    }
}
