using System;

namespace selenium.core.SCSS
{
    public class InvalidScssException : Exception {
        public InvalidScssException(string message, Exception exception)
            : base(message, exception) {
        }

        public InvalidScssException(string message, params object[] args)
            : base(string.Format(message, args)) {
        }
    }
}
