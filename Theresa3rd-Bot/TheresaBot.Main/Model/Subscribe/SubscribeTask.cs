using TheresaBot.Main.Common;
using TheresaBot.Main.Type;

namespace TheresaBot.Main.Model.Subscribe
{
    public class SubscribeTask
    {
        public int SubscribeId { get; init; }
        public string SubscribeCode { get; init; }
        public SubscribeType SubscribeType { get; init; }
        public int SubscribeSubType { get; init; }
        public string SubscribeName { get; init; }
        public List<long> SubscribeGroups => GetSubscribeGroups();
        private List<long> GroupIds { get; init; } = new();

        public SubscribeTask(SubscribeInfo subscribeInfo)
        {
            SubscribeId = subscribeInfo.SubscribeId;
            SubscribeCode = subscribeInfo.SubscribeCode;
            SubscribeType = subscribeInfo.SubscribeType;
            SubscribeSubType = subscribeInfo.SubscribeSubType;
            SubscribeName = subscribeInfo.SubscribeName;
        }

        public void AddGroups(List<long> groupIds)
        {
            if (groupIds is null) return;
            foreach (long groupId in groupIds)
            {
                AddGroup(groupId);
            }
        }

        public void AddGroup(long groupId)
        {
            if (GroupIds.Contains(groupId) == false)
            {
                GroupIds.Add(groupId);
            }
        }

        private List<long> GetSubscribeGroups()
        {
            if (GroupIds.Contains(0))
            {
                return BotConfig.SubscribeGroups;
            }
            else
            {
                return GroupIds.Distinct().ToList();
            }
        }

    }
}
