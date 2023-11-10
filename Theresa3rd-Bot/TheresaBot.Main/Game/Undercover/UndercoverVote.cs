using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
    }
}
