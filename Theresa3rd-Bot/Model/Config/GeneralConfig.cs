using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class GeneralConfig
    {
        public string Prefix { get; set; }

        public string DownloadPath { get; set; }

        public List<long> ErrorGroups { get; set; }

        public string ErrorMsg { get; set; }

        public string DownErrorImg { get; set; }

        public string PixivProxy { get; set; }

        public bool DownWithProxy { get; set; }

        public bool PixivFreeProxy { get; set; }

        public string DisableMsg { get; set; }

        public string NoPermissionsMsg { get; set; }

        public string ManagersRequiredMsg { get; set; }

        public string SetuCustomDisableMsg { get; set; }
    }
}
