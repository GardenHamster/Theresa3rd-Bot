namespace TheresaBot.Main.Model.Bot
{
    public record GroupInfos
    {
        public long GroupId { get; set; }

        public string GroupName { get; set; }

        public GroupInfos(long groupId, string groupName)
        {
            GroupId = groupId;
            GroupName = groupName;
        }

    }
}
