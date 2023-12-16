namespace TheresaBot.Main.Model.VO
{
    public record BanPixiverVo
    {
        public int Id { get; set; }

        public long PixiverId { get; set; }

        public long CreateAt { get; set; }

        public string CreateDate { get; set; }

    }
}
