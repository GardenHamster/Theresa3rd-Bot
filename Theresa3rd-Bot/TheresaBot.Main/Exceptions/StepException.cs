using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Exceptions
{
    public class StepException : Exception
    {
        public string RemindMessage { get; init; }

        public StepException(string message) : base(message)
        {
            this.RemindMessage = message;
        }
    }
}
