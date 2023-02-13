namespace TheresaBot.Main.Model.Content
{
    public class PlainContent : BaseContent
    {
        public string Content { get; set; }

        public PlainContent(string content)
        {
            Content = content;
        }

    }
}
