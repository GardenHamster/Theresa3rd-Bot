namespace TheresaBot.Main.Relay
{
    public abstract class FriendRelay : BaseRelay
    {
        public bool IsInstruct { get; init; }

        public FriendRelay(string message, bool isInstruct) : base(message)
        {
            IsInstruct = isInstruct;
        }

    }
}
