using TheresaBot.Core.Type;

namespace TheresaBot.Core.Model.Subscribe
{
    public record SubscribeInfo
    {
        public int Id { get; set; }
        public int SubscribeId { get; set; }
        public string SubscribeCode { get; set; }
        public SubscribeType SubscribeType { get; set; }
        public int SubscribeSubType { get; set; }
        public string SubscribeName { get; set; }
        public DateTime SubscribeDate { get; set; }
        public long GroupId { get; set; }
    }

}
