using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theresa3rd_Bot.Model.Config
{
    public class BotConfigDto
    {
        public GeneralConfig General { get; set; }

        public RepeaterConfig Repeater { get; set; }

        public WelcomeConfig Welcome { get; set; }

        public ReminderConfig Reminder { get; set; }

        public SetuConfig Setu { get; set; }

        public SubscribeConfig Subscribe { get; set; }
    }

}
