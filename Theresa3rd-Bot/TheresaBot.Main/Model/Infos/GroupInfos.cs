using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Model.Infos
{
    public record GroupInfos
    {
        public long GroupId { get; set; }

        public string GroupName { get; set; }

        public GroupInfos(long groupId, string groupName)
        {
            GroupId = groupId;
            GroupName = groupName;
        }

    }
}
