using System;
using System.Drawing;
using System.Resources;
using OpenQA.Selenium;

namespace selenium.core.Logging {
    public interface ITestLogger {
        /// <summary>
        ///     Залогировать действие
        /// </summary>
        void Action(string msg);

        /// <summary>
        ///     Залогировать информационное сообщение
        /// </summary>
        void Info(string msg);

        void Debug(string message);

        void Trace(string message);

        /// <summary>
        ///     Сохранить значение в лог
        /// </summary>
        void WriteValue(string key, object value);

        /// <summary>
        ///     Прочитать сохраненное ранее значение из логи
        /// </summary>
        T GetValue<T>(string key);

        T GetValue<T>(object key);

        /// <summary>
        ///     Залогировать селектор
        /// </summary>
        void Selector(By by);

        /// <summary>
        ///     Залогировать исключение
        /// </summary>
        void Exception(Exception exception, string message = null);

        void Warning(string message);

        /// <summary>
        /// Ignore all the log data passed to logger
        /// </summary>
        void Disable();

        /// <summary>
        /// Save the log data passed to logger
        /// </summary>
        void Enable();

        void Error(string error);

        void Reset();

        T Milliseconds<T>(string actionName, Func<T> func);

        void Milliseconds(string actionName, Action action);

        void Screenshot(Bitmap image);
    }
}