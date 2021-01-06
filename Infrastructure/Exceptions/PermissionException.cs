using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Exceptions
{
    public class PermissionException : Exception
    {
        public PermissionException(string message) : base(message)
        { }
    }
}
