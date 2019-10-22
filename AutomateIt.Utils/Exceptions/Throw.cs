namespace Natu.Utils.Exceptions
{
    public static class Throw
    {
        public static FrameworkException FrameworkException(string msg, params object[] args)
        {
            throw new FrameworkException(string.Format(msg, args));
        }

        public static TestException TestException(string msg, params object[] args)
        {
             throw new TestException(string.Format(msg, args));
        }
    }
}
