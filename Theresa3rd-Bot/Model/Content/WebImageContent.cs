namespace Theresa3rd_Bot.Model.Content
{
    public class WebImageContent: ChatContent
    {
        public string Url { get; set; }

        public WebImageContent(string url)
        {
            this.Url = url;
        }

    }
}
