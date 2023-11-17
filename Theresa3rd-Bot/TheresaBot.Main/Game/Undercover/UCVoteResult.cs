using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Game.Undercover
{
    public class UCVoteResult
    {
        /// <summary>
        /// 玩家
        /// </summary>
        public UCPlayer Player { get; private set; }

        /// <summary>
        /// 票数
        /// </summary>
        public int VoteNum { get; set; }

        public UCVoteResult(UCPlayer player, int voteNum)
        {
            Player = player;
            VoteNum = voteNum;
        }

    }
}
