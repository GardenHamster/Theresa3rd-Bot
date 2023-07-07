namespace TheresaBot.Main.Model.Pixiv
{
    public record PixivProfileDetail
    {
        public int No { get; set; }
        public PixivUserWorkInfo WorkInfo { get; set; }

        public PixivProfileDetail(PixivUserWorkInfo workInfo, int no)
        {
            this.No = no;
            this.WorkInfo = workInfo;
        }
    }
}
