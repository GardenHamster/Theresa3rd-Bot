using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Game
{
    /// <summary>
    /// 组队游戏，需要自愿报名参加的游戏
    /// </summary>
    public abstract class BaseGroupGame : BaseGame
    {
        /// <summary>
        /// 加入游戏中的成员
        /// </summary>
        public List<long> MemberIds { get; protected set; }

        /// <summary>
        /// 最小加入人数
        /// </summary>
        public int MinMember { get; protected set; }

        /// <summary>
        /// 最多加入人数
        /// </summary>
        public int MaxMember { get; protected set; }

        /// <summary>
        /// 组队时间
        /// </summary>
        public int MatchSecond { get; protected set; }

    }
}
