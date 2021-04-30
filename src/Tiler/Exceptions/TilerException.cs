using System;
using System.Collections.Generic;
using System.Text;

namespace Tiler.Exceptions
{
    public class TilerException : Exception
    {
        public TilerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
