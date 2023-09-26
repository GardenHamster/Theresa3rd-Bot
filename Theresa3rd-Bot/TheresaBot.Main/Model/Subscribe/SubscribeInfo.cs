using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Subscribe
{
    public record SubscribeInfo
    {
        public int SubscribeId { get; set; }
        public string SubscribeCode { get; set; }
        public SubscribeType SubscribeType { get; set; }
        public int SubscribeSubType { get; set; }
        public string SubscribeName { get; set; }
        public DateTime SubscribeDate { get; set; }
        public long GroupId { get; set; }
    }

}
