using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Exceptions
{
    public class HandlerException:Exception
    {
        public string RemindMessage { get; private set; }

        public HandlerException(string message):base(message)
        {
            RemindMessage = message;
        }

    }
}
