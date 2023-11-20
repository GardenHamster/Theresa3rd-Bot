namespace TheresaBot.Main.Relay
{
    public abstract class GroupRelay : BaseRelay
    {
        public long GroupId { get; init; }

        public bool IsAt { get; init; }

        public bool IsQuote { get; init; }

        public bool IsInstruct { get; init; }

        public abstract long QuoteMsgId { get; }

        public GroupRelay(long msgId, string message, long groupId, long memberId, bool isAt, bool isQuote, bool isInstruct) : base(msgId, message, memberId)
        {
            IsAt = isAt;
            IsQuote = isQuote;
            IsInstruct = isInstruct;
            GroupId = groupId;
        }

    }
}
