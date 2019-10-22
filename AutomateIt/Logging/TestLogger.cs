namespace selenium.core.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Natu.Utils.Exceptions;
    using OpenQA.Selenium;

    public class TestLogger : ITestLogger
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        static TestLogger()
        {
        }

        #region TestLogger Members


        public void WriteValue(string key, object value)
        {
            if (!this._values.ContainsKey(key))
            {
                this._values.Add(key, value);
            }
            else
            {
                this._values[key] = value;
            }
        }

        public T GetValue<T>(string key)
        {
            if (!this._values.ContainsKey(key))
            {
                throw Throw.TestException(string.Format("Value with key '{0}' was not logged", key));
            }
            return (T)this._values[key];
        }

        public void Selector(By by)
        {
            Console.WriteLine("By: {0}", by);
        }

        public void Exception(Exception exception)
        {
            Console.WriteLine("Exception: {0}", exception.Message);
        }

        public void Action(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Info(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Debug(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Trace(string msg)
        {
            Console.WriteLine(msg);
        }

        public T GetValue<T>(object key)
        {
            if (!this._values.ContainsKey(key.ToString()))
            {
                throw Throw.TestException(string.Format("Value with key '{0}' was not logged", key));
            }
            return (T)this._values[key.ToString()];
        }

        public void Exception(Exception exception, string message = null)
        {
            Console.WriteLine("Exception: {0}", exception.Message);
        }

        public void Warning(string message)
        {
            Console.WriteLine(message);
        }

        public void Disable()
        {
        }

        public void Enable()
        {
        }

        public void Error(string error)
        {
            Console.WriteLine(error);
        }

        public void Reset()
        {
        }

        public T Milliseconds<T>(string actionName, Func<T> func)
        {
            return func.Invoke();
        }

        public void Milliseconds(string actionName, Action action)
        {
            throw new NotImplementedException();
        }

        public void Screenshot(Bitmap image)
        {

        }

        #endregion
    }
}