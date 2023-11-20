namespace TheresaBot.Main.Model.Content
{
    public class ForwardContent : BaseContent
    {
        public long MemberId { get; protected set; }

        public string MemberName { get; protected set; }

        public BaseContent[] Contents { get; protected set; }

        public ForwardContent(long memberId, string memberName, string message)
        {
            MemberId = memberId;
            MemberName = memberName;
            Contents = new BaseContent[]
            {
                new PlainContent(message)
            };
        }

        public ForwardContent(long memberId, string memberName, BaseContent[] contents)
        {
            MemberId = memberId;
            MemberName = memberName;
            Contents = contents ?? new BaseContent[0];
        }

    }
}
