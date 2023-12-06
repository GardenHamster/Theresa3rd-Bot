namespace TheresaBot.Main.Relay
{
    public abstract class FriendRelay : BaseRelay
    {
        public bool IsInstruct { get; init; }

        public FriendRelay(long msgId, string message, long memberId, bool isInstruct) : base(msgId, message, memberId)
        {
            IsInstruct = isInstruct;
        }

    }
}
