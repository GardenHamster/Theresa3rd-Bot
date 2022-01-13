using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Subscribe
{
    public class SubscribeMember
    {
        public long GroupId { get; set; }

        public List<long> MemberIdList { get; set; }

        public SubscribeMember(long groupId, long memberId)
        {
            this.GroupId = groupId;
            this.MemberIdList = new List<long>() { memberId };
        }

        public void addMember(long memberId)
        {
            if (MemberIdList.Contains(memberId)) return;
            MemberIdList.Add(memberId);
        }

    }
}
