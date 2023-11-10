using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheresaBot.Main.Relay;

namespace TheresaBot.Main.Game.Undercover
{
    /// <summary>
    /// 发言记录
    /// </summary>
    public class UndercoverRecord
    {
        /// <summary>
        /// 发言玩家
        /// </summary>
        public UndercoverPlayer Player { get; private set; }

        /// <summary>
        /// 发言消息
        /// </summary>
        public GroupRelay Relay { get; private set; }

        /// <summary>
        /// 发言内容
        /// </summary>
        public string Content => Relay.Message;

        /// <summary>
        /// 构建一个发言记录
        /// </summary>
        /// <param name="player"></param>
        /// <param name="relay"></param>
        public UndercoverRecord(UndercoverPlayer player, GroupRelay relay)
        {
            Player = player;
            Relay = relay;
        }

    }

}
