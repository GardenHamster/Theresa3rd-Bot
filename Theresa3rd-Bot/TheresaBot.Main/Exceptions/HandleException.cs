using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Exceptions
{
    public class HandleException : Exception
    {
        public string RemindMessage { get; init; }

        public HandleException(string message) : base(message)
        {
            this.RemindMessage = message;
        }
    }
}
