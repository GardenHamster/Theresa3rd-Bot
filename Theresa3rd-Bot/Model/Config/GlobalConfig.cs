using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class GlobalConfig
    {
        public static MiraiConfig MiraiConfig = new MiraiConfig();

        public static AuthConfig AuthConfig = new AuthConfig();

        public static RepeaterConfig RepeaterConfig = new RepeaterConfig();
    }
}
