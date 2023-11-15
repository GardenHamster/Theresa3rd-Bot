namespace TheresaBot.Main.Game.Undercover
{
    public class UndercoverPlayer : BasePlayer
    {
        /// <summary>
        /// 玩家身份
        /// </summary>
        public UndercoverPlayerType PlayerType { get; private set; }

        /// <summary>
        /// 是否已经出局
        /// </summary>
        public bool IsOut { get; set; }

    }
}
