using System;

namespace Theresa3rd_Bot.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }

    }
}
