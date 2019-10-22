using System;
using OpenQA.Selenium;
using selenium.core.Framework.Service;
using selenium.core.Logging;

namespace selenium.core.Framework.Browser
{
    public abstract class DriverFacade {
        protected DriverFacade(Browser browser) {
            Browser = browser;
        }

        protected Browser Browser { get; }

        protected Web Web => Browser.Web;

        protected ITestLogger Log => Browser.Log;

        protected IWebDriver Driver => Browser.Driver;

        /// <summary>
        ///     Повторять действие если возникло StaleReferenceException
        /// </summary>
        /// <param name="func">Действие</param>
        public T RepeatAfterStale<T>(Func<T> func) {
            const int TRY_COUNT = 3;
            var result = default(T);
            for (var i = 0; i < TRY_COUNT; i++) {
                try {
                    result = func.Invoke();
                    break;
                }
                catch (StaleElementReferenceException e) {
                    Log.Exception(e);
                    if (i == TRY_COUNT - 1)
                        throw;
                }
                catch (InvalidOperationException e) {
                    if (e.Message.Contains("element is not attached to the page document")) {
                        // Chrome sometimes throws InvalidOperationException instead of StaleElementReferenceException
                        Log.Exception(e, "Unable to perform action using stale element reference.");
                        if (i == TRY_COUNT - 1)
                            throw;
                    }
                    else {
                        // it is a regular InvalidOperationException
                        throw;
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///     Повторять действие если возникло StaleReferenceException
        /// </summary>
        /// <param name="action">действие</param>
        public void RepeatAfterStale(Action action) =>
            RepeatAfterStale(() =>
                {
                    action.Invoke();
                    return true;
                });
    }
}
