using System.Numerics;
using System.Text.RegularExpressions;
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
        /// 玩家词条
        /// </summary>
        public string PlayerWord { get; set; }
        /// <summary>
        /// 是否已经出局
        /// </summary>
        public bool IsOut { get; set; }

        public UndercoverPlayer(long memberId, string memberName) : base(memberId, memberName) { }

        public string GetWordMessage()
        {
            if (PlayerType == UndercoverPlayerType.Whiteboard)
            {
                return $"本轮游戏中，你是白板，词条为空";
            }
            else
            {
                return $"本轮游戏中，你的词条是：{PlayerWord}";
            }
        }

    }
}
