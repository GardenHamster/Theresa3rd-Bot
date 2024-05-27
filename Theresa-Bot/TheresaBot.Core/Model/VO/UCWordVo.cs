namespace TheresaBot.Core.Model.VO
{
    public record UCWordVo
    {
        public int Id { get; set; }
        public string Word1 { get; set; }
        public string Word2 { get; set; }
        public bool IsAuthorized { get; set; }
        public long CreateMember { get; set; }
        public long CreateAt { get; set; }
    }
}
