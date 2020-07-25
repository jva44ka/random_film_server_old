using System;

namespace Infrastructure.Exceptions
{
    public class NotExistsException : Exception
    {
        public NotExistsException(string message) : base(message)
        { }
    }
}
