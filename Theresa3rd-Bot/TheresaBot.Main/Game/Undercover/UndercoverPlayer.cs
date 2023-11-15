using TheresaBot.Main.Command;

namespace TheresaBot.Main.Game.Undercover
{
    public class UndercoverPlayer : BasePlayer
    {
        /// <summary>
        /// 玩家身份
        /// </summary>
        public UndercoverPlayerType PlayerType { get; set; }
        /// <summary>
        /// 是否已经出局
        /// </summary>
        public bool IsOut { get; set; }

        public UndercoverPlayer(long memberId, string memberName) : base(memberId, memberName)
        {

        }

    }
}
