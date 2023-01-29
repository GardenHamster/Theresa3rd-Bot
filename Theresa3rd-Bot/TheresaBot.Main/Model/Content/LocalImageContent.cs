using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Content
{
    public class LocalImageContent: ChatContent
    {
        public string FullFilePath { get; set; }
        public SendTarget SendTarget { get; set; }

        public LocalImageContent(SendTarget target, string fullFilePath)
        {
            this.SendTarget = target;
            this.FullFilePath = fullFilePath;
        }

    }
}
