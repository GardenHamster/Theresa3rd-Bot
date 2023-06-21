namespace TheresaBot.Main.Relay
{
    public abstract class GroupRelay : BaseRelay
    {
        public int GroupId { get; set; }

        public GroupRelay(int msgId, string message, long groupId, long memberId) : base(msgId, message, memberId)
        {
            this.GroupId = GroupId;
        }

        public abstract List<string> GetImageUrls();

    }
}
