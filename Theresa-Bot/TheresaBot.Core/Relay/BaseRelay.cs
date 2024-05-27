namespace TheresaBot.Core.Relay
{
    public abstract class BaseRelay
    {
        public string Message { get; init; }

        public abstract long MsgId { get; }

        public abstract long MemberId { get; }

        public abstract List<string> GetImageUrls();

        public BaseRelay(string message)
        {
            Message = message?.Trim() ?? string.Empty;
        }

    }
}
