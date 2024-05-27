using TheresaBot.Core.Relay;

namespace TheresaBot.Core.Game.Undercover
{
    /// <summary>
    /// 发言记录
    /// </summary>
    public class UCSpeech
    {
        /// <summary>
        /// 发言玩家
        /// </summary>
        public UCPlayer Player { get; private set; }

        /// <summary>
        /// 发言消息
        /// </summary>
        public GroupRelay Relay { get; private set; }

        /// <summary>
        /// 发言内容
        /// </summary>
        public string Content => Relay.Message;

        /// <summary>
        /// 构建一个发言记录
        /// </summary>
        /// <param name="player"></param>
        /// <param name="relay"></param>
        public UCSpeech(UCPlayer player, GroupRelay relay)
        {
            Player = player;
            Relay = relay;
        }

    }

}
