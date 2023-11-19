namespace TheresaBot.Main.Relay
{
    public abstract class GroupRelay : BaseRelay
    {
        public long GroupId { get; set; }

        public bool IsAt { get; set; }

        public bool IsQuote { get; set; }

        public bool IsInstruct { get; set; }

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
