namespace TheresaBot.Main.Relay
{
    public abstract class BaseRelay
    {
        public long MsgId { get; init; }

        public string Answer { get; init; }

        public long MemberId { get; init; }

        public BaseRelay(long msgId, string answer, long memberId)
        {
            this.MsgId = msgId;
            this.Answer = answer;
            this.MemberId = memberId;
        }

    }
}
