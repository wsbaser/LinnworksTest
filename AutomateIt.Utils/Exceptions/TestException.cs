using System;

namespace Natu.Utils.Exceptions
{
    public class TestException : Exception
    {

        public TestException(string message, Exception innerException)
            : base(message, innerException) {
        }

        public TestException(string message)
            : base(message)
        {
        }
    }
}
