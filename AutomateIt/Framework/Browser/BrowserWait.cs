using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using automateit.SCSS;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;
using selenium.core.SCSS;

namespace selenium.core.Framework.Browser
{
    public class BrowserWait : DriverFacade
    {
        public BrowserWait(Browser browser)
            : base(browser)
        {
        }

        public T Until<T>(Func<T> condition, int timeout = 3, int sleepInterval = 100, string conditionName = null)
        {
            var wait = new WebDriverWait(new SystemClock(), Driver, TimeSpan.FromSeconds(timeout), TimeSpan.FromMilliseconds(sleepInterval));
            wait.IgnoreExceptionTypes(typeof(WebDriverTimeoutException));
            if (conditionName != null)
            {
                wait.Message = $"{conditionName}. Time is out. {timeout} seconds.";
            }

            var sw = new Stopwatch();
            if (conditionName != null)
            {
                sw.Start();
            }

            var iterationIndex = 0;
            T result = wait.Until(
                driver =>
                    {
                        iterationIndex++;
                        return condition.Invoke();
                    });
            sw.Stop();
            if (conditionName != null)
            {
                Log.Debug($"Wait condition: {conditionName}, time: {sw.Elapsed}, iterations: {iterationIndex}.");
            }

            return result;
        }

        /// <summary>
        /// Wait for condition. Do not throw TimeoutException if condition is not satisfied.
        /// </summary>
        public T UntilSoftly<T>(Func<T> condition, int timeout = 3, int sleepInterval = 100, string conditionName = null)
        {
            var wait = new WebDriverWait(new SystemClock(), Driver, TimeSpan.FromSeconds(timeout), TimeSpan.FromMilliseconds(sleepInterval));
            wait.IgnoreExceptionTypes(typeof(WebDriverTimeoutException));
            var sw = new Stopwatch();
            var iterationIndex = 0;
            try
            {
                if (conditionName != null)
                {
                    sw.Start();
                }

                return wait.Until(driver => condition.Invoke());
            }
            catch (WebDriverTimeoutException)
            {
                return default(T);
            }
            finally
            {
                if (conditionName != null)
                {
                    sw.Stop();
                    Log.Debug($"Wait condition: {conditionName}, time: {sw.Elapsed}, iterations: {iterationIndex}.");
                }
            }
        }

        internal void Condition(Action action, WaitCondition waitCondition, int waitTimeout)
        {
            var oldUrl = Browser.Window.Url;
            action.Invoke();
            switch (waitCondition)
            {
                case WaitCondition.None:
                    break;
                case WaitCondition.Sleep:
                    Thread.Sleep(waitTimeout);
                    break;
                case WaitCondition.Ajax:
                    if (waitTimeout == 0)
                    {
                        WhileAjax(ajaxInevitable: true);
                    }
                    else
                    {
                        WhileAjax(waitTimeout, ajaxInevitable: true);
                    }
                    break;
                case WaitCondition.PageInProgress:
                    WhilePageInProgress(waitTimeout);
                    break;
                case WaitCondition.Alert:
                    ForHtmlAlert(waitTimeout);
                    break;
                case WaitCondition.Redirect:
                    ForRedirect(oldUrl);
                    break;
                case WaitCondition.NewWindow:
                    ForWindow(waitTimeout);
                    break;
            }
        }

        public void ForElementNotVisible(Selector selector, int timeout = BrowserTimeouts.FIND) => Until(() => !Browser.Is.Visible(selector.By, selector.FrameBy), timeout);

        /// <summary>
        ///     Wait until the element is visible
        /// </summary>
        /// <param name="by">Visible element selector</param>
        /// <param name="timeout">Maximum waiting period</param>
        public void WhileElementVisible(string scssSelector, int timeout = BrowserTimeouts.AJAX) => WhileElementVisible(Scss.GetBy(scssSelector), timeout);

        public void WhileElementVisible(Selector selector, int timeout = BrowserTimeouts.AJAX) => WhileElementVisible(selector.By, timeout, selector.FrameBy);

        public void WhileElementVisible(By by, int timeout = BrowserTimeouts.AJAX, By frameBy = null)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
            wait.Until(driver => !Browser.Is.Visible(by, frameBy));
        }

        /// <summary>
        ///     Wait until element is not visible
        /// </summary>
        public void ForElementVisible(string scssSelector, int timeout = BrowserTimeouts.FIND) => ForElementVisible(Scss.GetBy(scssSelector), null, timeout);

        public void ForElementVisible(string scss, string frameSelector, int timeout = 3) => ForElementVisible(ScssBuilder.CreateBy(scss), frameSelector == null ? null : ScssBuilder.CreateBy(frameSelector), timeout);

        public void ForElementVisible(Selector selector, int timeout = 3) => ForElementVisible(selector.By,selector.FrameBy,timeout);

        public void ForElementVisible(By by, By frameBy = null, int timeout = 3) {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
            wait.Until(driver => Browser.Is.Visible(by, frameBy));
        }

        /// <summary>
        ///     Wait until all the progress registered on the page disappears
        /// </summary>
        public void WhilePageInProgress(int timeout = BrowserTimeouts.AJAX) {
            if (Browser.State.Page == null)
                return;
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
            wait.Until(driver => Browser.State.Page.ProgressBars.All(p => !p.IsVisible()));
        }

        public void ForPageInProgress(int milliseconds = 1000) {
            if (Browser.State.Page == null)
                return;
            const int POLLING_INTERVAL = 200;
            var count = (int)Math.Ceiling(milliseconds / (decimal)POLLING_INTERVAL);
            for (var i = 0; i < count; i++) {
                if (Browser.State.Page.ProgressBars.Any(p => p.IsVisible())) {
                    return;
                }
                Thread.Sleep(POLLING_INTERVAL);
            }
        }

        /// <summary>
        ///     Wait until Ajax queries are completed
        /// </summary>
        /// <param name="timeout">maximum wait time until all ajax queries are completed</param>
        /// <param name="ajaxInevitable">
        ///     if true - ajax queries 100% should be completed
        ///     If no, wait 3 seconds and check again
        /// </param>
        public void WhileAjax(int timeout = BrowserTimeouts.AJAX, bool ajaxInevitable = false)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
            var waited = false;
            wait.Until(
                driver =>
                    {
                        var ajaxActive = Browser.Is.AjaxActive();
                        if (ajaxActive)
                        {
                            waited = true;
                            return false;
                        }
                        return true;
                    });
            if (!waited && ajaxInevitable)
            {
                Browser.Wait.ForAjax(3000);
                wait.Until(driver => !Browser.Is.AjaxActive());
            }
        }

        /// <summary>
        ///     Wait until ajax queries are executed
        /// </summary>
        public void ForAjax(int miliseconds = 1000)
        {
            const int POLLING_INTERVAL = 200;
            var count = (int)Math.Ceiling(miliseconds / (decimal)POLLING_INTERVAL);
            for (var i = 0; i < count; i++)
            {
                if (Browser.Is.AjaxActive())
                    return;
                Thread.Sleep(POLLING_INTERVAL);
            }
        }

        public void ForWindow(int timeout = BrowserTimeouts.REDIRECT) => Browser.Wait.Until(
            () =>
                {
                    if (Browser.State.ActualizeWindow()) {
                        Browser.State.ActualizePage();
                        return true;
                    }
                    return false;
                }, timeout);

        public T ForRedirectTo<T>(int timeout = BrowserTimeouts.REDIRECT)
            where T : class {
            Browser.Wait.UntilSoftly(
                () =>
                    {
                        if (Browser.State.PageIs<T>())
                            return true;
                        Browser.State.ActualizePage();
                        return false;
                    }, timeout);
            return Browser.State.PageAs<T>();
        }

        public void ForRedirect(string oldUrl = null, bool waitWhileAjax = false, bool ajaxInevitable = false, int timeout = BrowserTimeouts.REDIRECT) {
            oldUrl = oldUrl ?? Browser.Window.Url;
            Browser.Wait.Until(() => oldUrl != Browser.Window.Url, timeout);
            if (waitWhileAjax || ajaxInevitable) {
                Browser.Wait.WhileAjax(ajaxInevitable: ajaxInevitable);
            }
            Browser.State.Actualize();
            if (Browser.State.PageIs<PageBase>())
            {
                Browser.State.PageAs<PageBase>().WaitLoaded();
            }
        }

        public IAlert ForAlert(int timeout = BrowserTimeouts.AJAX)
        {
            Browser.Wait.Until(
                () =>
                {
                    Browser.State.ActualizeAlerts();
                    if (Browser.State.HtmlAlert != null || Browser.State.SystemAlert != null)
                    {
                        Log.Trace($"Alert of type '{Browser.State.GetActiveAlert().GetType().Name}' was found on the page.");
                        return true;
                    }

                    return false;
                }, timeout);
            return Browser.State.GetActiveAlert();
        }

        public T ForHtmlAlert<T>(int timeout = BrowserTimeouts.AJAX)
            where T : class, IHtmlAlert
        {
            Browser.Wait.Until(
                () =>
                    {
                        Browser.State.ActualizeHtmlAlert();
                        if (Browser.State.HtmlAlert != null)
                        {
                            Log.Trace($"Html alert of type '{Browser.State.HtmlAlert.GetType().Name}' was found on the page.");
                            return true;
                        }

                        return false;
                    }, timeout);
            return Browser.State.HtmlAlertAs<T>();
        }

        public void ForHtmlAlert(int timeout = BrowserTimeouts.AJAX) => ForHtmlAlert<IHtmlAlert>(timeout);

        public void ForAlertOrRedirect(string oldUrl = null, bool waitForAjax = false, bool ajaxInevitable = false)
        {
            const int POLLING_INTERVAL = 200;
            oldUrl = oldUrl ?? Browser.Window.Url;
            for (var i = 0; i < 10; i++)
            {
                Browser.State.ActualizeHtmlAlert();
                // Browser.State.SystemAlert != null || 
                if (Browser.State.HtmlAlert != null)
                {
                    // . alert displayed
                    return;
                }
                if (oldUrl != Browser.Window.Url)
                {
                    // . redirect occured
                    if (waitForAjax || ajaxInevitable)
                        Browser.Wait.WhileAjax(ajaxInevitable: ajaxInevitable);
                    Browser.State.Actualize();
                    return;
                }
                Thread.Sleep(POLLING_INTERVAL);
            }
        }

        public T ForOverlay<T>(int timeout = BrowserTimeouts.AJAX)
            where T : class, IOverlay => Browser.Wait.Until(() => Browser.State.GetOverlay<T>(), timeout);
    }
}
