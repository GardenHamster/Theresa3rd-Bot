namespace TheresaBot.Main.Model.Content
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
