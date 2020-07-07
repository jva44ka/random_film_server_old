using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Exceptions
{
    public class NotExistsException : Exception
    {
        public NotExistsException(string? message) : base(message)
        { }
    }
}
