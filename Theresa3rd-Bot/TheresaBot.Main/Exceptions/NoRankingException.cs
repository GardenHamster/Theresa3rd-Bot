using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheresaBot.Main.Exceptions
{
    public class NoRankingException : ApiException
    {
        public NoRankingException(string message) : base(message) { }
    }
}
