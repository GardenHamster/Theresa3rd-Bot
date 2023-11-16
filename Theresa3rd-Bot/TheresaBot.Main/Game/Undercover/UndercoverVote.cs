namespace TheresaBot.Main.Game.Undercover
{
    public class UndercoverVote
    {
        /// <summary>
        /// 投票人
        /// </summary>
        public UndercoverPlayer Voter { get; private set; }

        /// <summary>
        /// 投票对象
        /// </summary>
        public UndercoverPlayer Target { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="voter"></param>
        /// <param name="target"></param>
        public UndercoverVote(UndercoverPlayer voter, UndercoverPlayer target)
        {
            Voter = voter;
            Target = target;
        }

    }
}
