namespace TheresaBot.Main.Model.Content
{
    public class LocalImageContent : ImageContent
    {
        public FileInfo FileInfo { get; set; }

        public LocalImageContent(string fullFilePath)
        {
            this.FileInfo = new FileInfo(fullFilePath);
        }

        public LocalImageContent(FileInfo fileInfo)
        {
            this.FileInfo = fileInfo;
        }

    }
}
