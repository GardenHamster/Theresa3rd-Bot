using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class WelcomeConfig : BaseConfig
    {
        public string Global { get; set; }

        public List<WelcomeSpecial> Special { get; set; }
    }

    public class WelcomeSpecial
    {
        public long GroupId { get; set; }

        public string Template { get; set; }
    }




}
