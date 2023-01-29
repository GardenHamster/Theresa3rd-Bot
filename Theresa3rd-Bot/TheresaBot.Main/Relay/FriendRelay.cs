using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Relay
{
    public abstract class FriendRelay : BaseRelay
    {
        public FriendRelay(int msgId, string message, long memberId) : base(msgId, message, memberId)
        {
        }

    }
}
