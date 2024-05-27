namespace TheresaBot.Core.Model.Pixiv
{
    public class PixivSearchTask
    {
        public int Page { get; set; }

        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public PixivSearchTask(int page, int startIndex, int endIndex)
        {
            this.Page = page;
            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
        }

    }
}
