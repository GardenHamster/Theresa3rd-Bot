namespace TheresaBot.Main.Game
{
    public abstract class BasePlayer
    {
        public long MemberId { get; private set; }

        public string MemberName { get; private set; }

        public BasePlayer(long memberId, string memberName)
        {
            MemberId = memberId;
            MemberName = memberName;
        }
    }
}
