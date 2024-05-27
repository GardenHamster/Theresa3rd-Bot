namespace TheresaBot.Core.Relay
{
    public abstract class PrivateRelay : BaseRelay
    {
        public bool IsInstruct { get; init; }

        public PrivateRelay(string message, bool isInstruct) : base(message)
        {
            IsInstruct = isInstruct;
        }

    }
}
