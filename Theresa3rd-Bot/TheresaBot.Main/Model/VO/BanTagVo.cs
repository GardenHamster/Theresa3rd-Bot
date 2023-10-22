namespace TheresaBot.Main.Model.VO
{
    public record BanTagVo
    {
        public int Id { get; set; }

        public string KeyWord { get; set; }

        public bool FullMatch { get; set; }

        public bool IsRegular { get; set; }

        public long CreateAt { get; set; }

        public string CreateDate { get; set; }

    }
}
