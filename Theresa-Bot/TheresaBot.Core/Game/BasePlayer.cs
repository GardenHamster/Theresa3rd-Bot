namespace TheresaBot.Core.Game
{
    public abstract class BasePlayer
    {
        public int PlayerId { get; private set; }

        public long MemberId { get; private set; }

        public string MemberName { get; private set; }

        public string NameAndQQ => $"{MemberName}({MemberId})";

        public BasePlayer(long memberId, string memberName)
        {
            MemberId = memberId;
            MemberName = memberName;
        }

        public void SetPlayerId(int id)
        {
            PlayerId = id;
        }

    }
}
