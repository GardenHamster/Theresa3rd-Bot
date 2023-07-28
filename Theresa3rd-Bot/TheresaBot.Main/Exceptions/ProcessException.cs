using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Exceptions
{
    public class ProcessException : Exception
    {
        public string RemindMessage { get; init; }

        public ProcessException(string message) : base(message)
        {
            this.RemindMessage = message;
        }
    }
}
