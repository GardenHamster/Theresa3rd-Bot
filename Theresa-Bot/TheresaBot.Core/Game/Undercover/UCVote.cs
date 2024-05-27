namespace TheresaBot.Core.Game.Undercover
{
    public class UCVote
    {
        /// <summary>
        /// 投票人
        /// </summary>
        public UCPlayer Voter { get; private set; }

        /// <summary>
        /// 投票对象
        /// </summary>
        public UCPlayer Target { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="voter"></param>
        /// <param name="target"></param>
        public UCVote(UCPlayer voter, UCPlayer target)
        {
            Voter = voter;
            Target = target;
        }

    }
}
