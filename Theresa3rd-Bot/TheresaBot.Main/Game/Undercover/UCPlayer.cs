namespace TheresaBot.Main.Game.Undercover
{
    public class UCPlayer : BasePlayer
    {
        /// <summary>
        /// 玩家身份
        /// </summary>
        public UCCamp PlayerCamp { get; set; } = UCCamp.None;

        /// <summary>
        /// 玩家词条
        /// </summary>
        public string PlayerWord { get; set; }

        /// <summary>
        /// 是否已经出局
        /// </summary>
        public bool IsOut { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberName"></param>
        public UCPlayer(long memberId, string memberName) : base(memberId, memberName) { }

        /// <summary>
        /// 将玩家标记位已出局
        /// </summary>
        /// <returns></returns>
        public UCPlayer SetOut()
        {
            IsOut = true;
            return this;
        }

        /// <summary>
        /// 获取词条信息
        /// </summary>
        /// <returns></returns>
        public string GetWordMessage(bool withId)
        {
            if (PlayerCamp == UCCamp.Whiteboard)
            {
                return $"本轮游戏中，你是白板，词条为空";
            }
            if (withId)
            {
                return $"本轮游戏中，你的词条是：{PlayerWord}，身份是：{PlayerCamp.CampName}";
            }
            else
            {
                return $"本轮游戏中，你的词条是：{PlayerWord}";
            }
        }

    }
}
