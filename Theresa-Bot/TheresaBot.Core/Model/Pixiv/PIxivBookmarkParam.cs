namespace TheresaBot.Core.Model.Pixiv
{
    public class PIxivBookmarkParam
    {
        public string illust_id { get; set; }

        public int restrict { get; set; } = 0;

        public string comment { get; set; } = string.Empty;

        public string[] tags { get; set; } = new string[0];

        public PIxivBookmarkParam(string illustId)
        {
            this.illust_id = illustId;
        }

    }
}
