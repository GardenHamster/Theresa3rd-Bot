using System;

namespace TheresaBot.Main.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }

    }
}
