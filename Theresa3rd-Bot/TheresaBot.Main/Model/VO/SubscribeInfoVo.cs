using TheresaBot.Main.Common;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.VO
{
    public record SubscribeInfoVo
    {
        public int SubscribeId { get; set; }
        public string SubscribeCode { get; set; }
        public SubscribeType SubscribeType { get; set; }
        public string SubscribeName { get; set; }
        public long GroupId { get; set; }
        public string GroupName => BotConfig.GroupInfos.FirstOrDefault(o => o.GroupId == GroupId)?.GroupName ?? "群名称加载失败";
    }

}
