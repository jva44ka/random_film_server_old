using System;

namespace Infrastructure.Exceptions
{
    public class MissingParametersException : Exception
    {
        public MissingParametersException(string message) : base(message)
        { }
    }
}
