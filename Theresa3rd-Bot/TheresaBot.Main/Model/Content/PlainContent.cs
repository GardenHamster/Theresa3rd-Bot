namespace TheresaBot.Main.Model.Content
{
    public class PlainContent : BaseContent
    {
        public string Content { get; set; }

        public bool NewLine { get; set; }

        public PlainContent(string content, bool newLine = true)
        {
            this.Content = content;
            this.NewLine = newLine;
        }

    }
}
