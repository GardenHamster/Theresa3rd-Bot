using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class PermissionsConfig
    {
        public List<long> AcceptGroups { get; set; }

        public List<long> SuperManagers { get; set; }

        public List<long> LimitlessMembers { get; set; }

        public List<long> SetuGroups { get; set; }

        public List<long> SetuShowImgGroups { get; set; }

        public List<long> SetuShowAIGroups { get; set; }

        public List<long> SetuShowR18Groups { get; set; }

        public List<long> SetuCustomGroups { get; set; }

        public List<long> SetuNoneCDGroups { get; set; }

        public List<long> SetuLimitlessGroups { get; set; }

        public List<long> SaucenaoGroups { get; set; }

        public List<long> SaucenaoR18Groups { get; set; }

        public List<long> SubscribeGroups { get; set; }

    }
}
