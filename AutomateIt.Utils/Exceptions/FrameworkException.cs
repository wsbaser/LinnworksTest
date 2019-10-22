using System;

namespace Natu.Utils.Exceptions
{
    public class FrameworkException : Exception
    {
        public FrameworkException(string message)
            : base(message)
        {
        }
    }
}
