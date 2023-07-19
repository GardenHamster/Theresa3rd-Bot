using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Cache
{
    public class GroupCoolingInfo
    {
        public long GroupId { get; set; }

        public DateTime? LastSetuTime { get; set; }

        public DateTime? LastWordCloudTime { get; set; }

        public bool IsWordCloudHanding { get; set; }

        public Dictionary<PixivRankingType, DateTime?> LastPixivRankingTime { get; set; }

        public GroupCoolingInfo(long groupId)
        {
            this.GroupId = groupId;
            this.LastPixivRankingTime = new();
        }

    }
}
