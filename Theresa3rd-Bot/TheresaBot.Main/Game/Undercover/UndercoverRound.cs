using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Game.Undercover
{
    public class UndercoverRound
    {
        /// <summary>
        /// 发言记录
        /// </summary>
        public List<UndercoverRecord> Records { get; set; } = new();

        /// <summary>
        /// 投票记录
        /// </summary>
        public List<UndercoverVote> Votes { get; set; } = new();

        /// <summary>
        /// 出局成员
        /// </summary>
        public List<UndercoverPlayer> OutPlayers { get; set; } = new();

    }
}
