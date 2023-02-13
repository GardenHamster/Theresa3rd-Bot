namespace TheresaBot.Main.Relay
{
    public abstract class FriendRelay : BaseRelay
    {
        public FriendRelay(int msgId, string message, long memberId) : base(msgId, message, memberId)
        {
        }

    }
}
