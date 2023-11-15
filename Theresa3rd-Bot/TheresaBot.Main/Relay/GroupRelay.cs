namespace TheresaBot.Main.Relay
{
    public abstract class GroupRelay : BaseRelay
    {
        public long GroupId { get; set; }

        public GroupRelay(long msgId, string message, long groupId, long memberId) : base(msgId, message, memberId)
        {
            this.GroupId = groupId;
        }

    }
}
