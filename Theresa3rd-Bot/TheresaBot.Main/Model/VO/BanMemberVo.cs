namespace TheresaBot.Main.Model.VO
{
    public record BanMemberVo
    {
        public int Id { get; set; }

        public long MemberId { get; set; }

        public long CreateAt { get; set; }

        public string CreateDate { get; set; }

    }
}
