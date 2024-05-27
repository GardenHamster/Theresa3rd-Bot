namespace TheresaBot.Core.Model.Bot
{
    public class BotInfos
    {
        public long QQ { get; init; }
        public string NickName { get; init; }
        public BotInfos(long qq, string nickName)
        {
            QQ = qq;
            NickName = nickName;
        }
    }
}
