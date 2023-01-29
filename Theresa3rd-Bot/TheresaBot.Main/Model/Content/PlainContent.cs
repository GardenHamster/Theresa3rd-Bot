namespace TheresaBot.Main.Model.Content
{
    public class PlainContent : ChatContent
    {
        public string Content { get; set; }

        public PlainContent(string content)
        {
            Content = content;
        }

    }
}
