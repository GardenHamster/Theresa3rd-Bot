namespace Theresa3rd_Bot.Model.Content
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
