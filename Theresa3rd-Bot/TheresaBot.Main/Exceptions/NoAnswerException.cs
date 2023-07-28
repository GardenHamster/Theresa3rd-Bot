using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Exceptions
{
    public class NoAnswerException : ProcessException
    {
        public NoAnswerException() : base(string.Empty)
        {
        }

    }
}
