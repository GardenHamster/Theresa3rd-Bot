using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theresa3rd_Bot.Util;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class SubscribeTask
    {
        public int SubscribeId { get; set; }
        public string SubscribeCode { get; set; }
        public int SubscribeType { get; set; }
        public int SubscribeSubType { get; set; }
        public string SubscribeName { get; set; }
        public string SubscribeDescription { get; set; }
        public List<SubscribeMember> SubscribeMembers { get; set; }

        public SubscribeTask(SubscribeInfo subscribeInfo)
        {
            this.SubscribeId = subscribeInfo.SubscribeId;
            this.SubscribeCode = subscribeInfo.SubscribeCode;
            this.SubscribeType = subscribeInfo.SubscribeType;
            this.SubscribeSubType = subscribeInfo.SubscribeSubType;
            this.SubscribeName = subscribeInfo.SubscribeName;
            this.SubscribeDescription = subscribeInfo.SubscribeDescription;
            this.SubscribeMembers = new List<SubscribeMember>();
        }

        public void addSubscribeMember(long groupId,long memberId)
        {
            SubscribeMember subscribeMember = SubscribeMembers.Where(o => o.GroupId == groupId).FirstOrDefault();
            if (subscribeMember == null)
            {
                subscribeMember = new SubscribeMember(groupId, memberId);
                SubscribeMembers.Add(subscribeMember);
            }
            else
            {
                subscribeMember.addMember(memberId);
            }
        }

        public void addSubscribeMember(long groupId, string memberIds)
        {
            List<long> memberIdList = MathHelper.splitListFromStr(memberIds);
            foreach (long memberId in memberIdList) addSubscribeMember(groupId, memberId);
        }

    }
}
