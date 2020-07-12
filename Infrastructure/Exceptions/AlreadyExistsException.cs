using System;

namespace Infrastructure.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string message) : base(message) 
        { }
    }
}
