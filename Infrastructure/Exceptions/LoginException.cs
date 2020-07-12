using System;

namespace Infrastructure.Exceptions
{
    public class LoginException : Exception
    {
        public int AccessFailedCount { get; set; }
        public LoginException(string message) : base(message)
        { }
    }
}
