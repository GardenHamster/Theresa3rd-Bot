using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Relay
{
    public abstract class BaseRelay
    {
        public int MsgId { get; set; }

        public string Message { get; set; }

        public long MemberId { get; init; }

        public BaseRelay(int msgId, string message, long memberId)
        {
            this.MsgId = msgId;
            this.Message = message;
            this.MemberId = memberId;
        }

    }
}
