using System;

namespace Infrastructure.Exceptions
{
    public class IdentityCreateException : Exception
    {
        public IdentityCreateException(string message = "") : base(message)
        { }
    }
}
