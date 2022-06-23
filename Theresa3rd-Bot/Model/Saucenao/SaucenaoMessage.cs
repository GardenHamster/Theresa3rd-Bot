using Mirai.CSharp.HttpApi.Models.ChatMessages;
using System.Collections.Generic;

namespace Theresa3rd_Bot.Model.Saucenao
{
    public class SaucenaoMessage
    {
        public decimal Similar { get; set; }

        public List<IChatMessage> GroupMsgs { get; set; }

        public List<IChatMessage> TempMsgs { get; set; }

        public SaucenaoMessage(List<IChatMessage> groupMsgs, List<IChatMessage> tempMsgs,decimal similar)
        {
            this.GroupMsgs = groupMsgs;
            this.TempMsgs = tempMsgs;
            this.Similar = similar;
        }


    }
}
