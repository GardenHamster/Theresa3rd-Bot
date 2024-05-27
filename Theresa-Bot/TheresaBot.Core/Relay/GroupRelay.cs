namespace TheresaBot.Core.Relay
{
    public abstract class GroupRelay : BaseRelay
    {
        public bool IsAt { get; init; }

        public bool IsQuote { get; init; }

        public bool IsInstruct { get; init; }

        public abstract long GroupId { get; }

        public abstract long QuoteMsgId { get; }

        public GroupRelay(string message, bool isAt, bool isQuote, bool isInstruct) : base(message)
        {
            IsAt = isAt;
            IsQuote = isQuote;
            IsInstruct = isInstruct;
        }

    }
}
