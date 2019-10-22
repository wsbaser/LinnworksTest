using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using automateit.SCSS;
using Natu.Utils.Exceptions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using selenium.core.Exceptions;
using selenium.core.Framework.Page;
using selenium.core.SCSS;

namespace selenium.core.Framework.Browser
{
    public class BrowserAction : DriverFacade
    {
        private readonly BrowserFind _find;

        public BrowserAction(Browser browser)
            : base(browser)
        {
            _find = browser.Find;
        }

        /// <summary>
        ///     Select option in html tag Select
        /// </summary>
        public void Select(string scssSelector, string value) => Select(ScssBuilder.CreateBy(scssSelector), value);

        public void Select(By by, string value) => RepeatAfterStale(
            () =>
                {
                    var select = _find.Element(by);
                    var dropDown = new SelectElement(select);
                    dropDown.SelectByValue(value);
                });

        /// <summary>
        ///     Enter value into input field
        /// </summary>
        public void TypeIn(Selector selector, object value, bool clear = true) => TypeIn(selector.By, selector.FrameBy, value, clear);

        public void TypeIn(string scssSelector, By frameBy, object value, bool clear = true) => TypeIn(ScssBuilder.CreateBy(scssSelector), frameBy, value, clear);

        public void ClickBackButton()
        {
            var oldUrl = Browser.Window.Url;
            Driver.Navigate().Back();
            for (var i = 0; i < 10; i++)
            {
                Browser.State.Actualize();
                if (Browser.State.SystemAlert != null
                    || oldUrl != Browser.Window.Url)
                {
                    Browser.Wait.WhileAjax();
                    return;
                }
                Thread.Sleep(200);
            }
        }

        public void ClickNextButton()
        {
            var oldUrl = Browser.Window.Url;
            Driver.Navigate().Forward();
            for (var i = 0; i < 10; i++)
            {
                Browser.State.Actualize();
                if (Browser.State.SystemAlert != null
                    || oldUrl != Browser.Window.Url)
                {
                    Browser.Wait.WhileAjax();
                    return;
                }
                Thread.Sleep(200);
            }
        }

        public void TypeIn(By by, By frameBy, object value, bool clear = true) => RepeatAfterStale(
            () =>
                {
                    var element = _find.Element(by, frameBy);
                    if (clear)
                        Clear(element);
                    var valueString = value.ToString();
                    switch (Browser.Options.TypeInStyle) {
                        case TypeInStyle.FullValue:
                            element.SendKeys(valueString);
                            break;
                        case TypeInStyle.Chars:
                            foreach (var c in valueString)
                                element.SendKeys(c.ToString());
                            break;
                        case TypeInStyle.Js:
                            Browser.Js.SetValue(element, valueString);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Browser.Options.TypeInStyle");
                    }
                });

        public void TypeInAndWaitWhileAjax(By by, By frameBy, object value, bool clear = true, bool ajaxInevitable = false) {
            TypeIn(by, frameBy, value, clear);
            Browser.Wait.WhileAjax(ajaxInevitable: ajaxInevitable);
        }


        public void Click(Selector selector, int sleepTimeout = 0) => Click(selector.By, selector.FrameBy, sleepTimeout);

        public void Click(string scssSelector, By frameBy = null, int sleepTimeout = 0) => Click(ScssBuilder.CreateBy(scssSelector), frameBy, sleepTimeout);

        public void Click(By by, By frameBy = null, int sleepTimeout = 0) => Click(Driver, by, frameBy, sleepTimeout);

        public void Click(ISearchContext context, By by, By frameBy, int sleepTimeout = 0) => RepeatAfterStale(() =>
            {
                try {
                    if (Browser.Options.WaitWhileAjaxBeforeClick)
                        Browser.Wait.WhileAjax();
                    Click(_find.Element(context, by, frameBy), sleepTimeout);
                }
                catch (WebDriverException e) {
                    // TODO: !!find out how to fix this problem in a proper way!!
                    if (e.Message.Contains("Other element would receive the click: <div id=\"home-toast-wrapper\"")) {
                        try {
                            Browser.Js.SetCssValue(By.CssSelector("#home-toast-wrapper"), null, ECssProperty.zIndex, -1);
                            Click(_find.Element(context, by, frameBy), sleepTimeout);
                        }
                        finally {
                            Browser.Js.SetCssValue(By.CssSelector("#home-toast-wrapper"), null, ECssProperty.zIndex, 999999);
                        }
                    }
                    else if (e.Message.Contains("toast-success"))
                    {
                        Browser.Wait.ForElementNotVisible(new Selector(".toast-success"));
                        Click(_find.Element(context, by, frameBy), sleepTimeout);
                    }
                    else {
                        Log.Selector(by);
                        Log.Exception(e);
                        throw;
                    }
                }
            });

        public void Click(IWebElement element, int sleepTimeout = 0)
        {
            try
            {
                ScrollIntoView(element);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (Browser.Options.UseJsClick)
            {
                try
                {
                    Browser.Js.Click(element);
                }
                catch (InvalidOperationException)
                {
                    element.Submit();
                }
            }
            else
            {
                element.Click();
            }

            if (sleepTimeout != 0)
                Thread.Sleep(sleepTimeout);
        }

        private void ScrollIntoView(IWebElement element)
        {
            new Actions(Driver).MoveToElement(element).Perform();
            Thread.Sleep(300);
        }

        public T ClickAndWaitForAlert<T>(Selector selector, int timeout = BrowserTimeouts.AJAX)
            where T : AlertBase
        {
            return ClickAndWaitForAlert<T>(selector.By, selector.FrameBy, timeout);
        }

        public T ClickAndWaitForAlert<T>(By by, By frameBy, int timeout = BrowserTimeouts.AJAX) where T : AlertBase {
            ClickAndWaitForAlert(by, frameBy, timeout);
            if (!Browser.State.HtmlAlertIs<T>()) {
                Throw.TestException($"{typeof(T).Name} did not appear after click.");
            }
            return Browser.State.HtmlAlertAs<T>();
        }

        /// <summary>
        ///     Click on element with waiting for Alert
        /// </summary>
        public void ClickAndWaitForAlert(string scssSelector, int timeout = BrowserTimeouts.AJAX, bool ajaxInevitable = false) => ClickAndWaitForAlert(ScssBuilder.CreateBy(scssSelector), null, timeout, ajaxInevitable);

        public void ClickAndWaitNewWindow(By by, By frameBy,int sleepTimeout=0, int timeout = BrowserTimeouts.WINDOW) => RepeatAfterStale(() => ClickAndWaitNewWindow(Browser.Find.Element(by, frameBy),sleepTimeout, timeout));

        public void ClickAndWaitNewWindow(IWebElement element, int sleepTimeout, int timeout) {
            Click(element, sleepTimeout);
            Browser.Wait.ForWindow(timeout);
        }

        public IAlert ClickAndWaitForAlert(By by, By frameBy, int timeout = BrowserTimeouts.AJAX, bool repeat = false, bool ajaxInevitable = false) => ClickAndWaitForAlert<IAlert>(by, frameBy, timeout, repeat, ajaxInevitable);

        public T ClickAndWaitForAlert<T>(By by, By frameBy, int timeout = BrowserTimeouts.AJAX, bool repeat = false, bool ajaxInevitable = false)
            where T : class, IAlert
        {
            if (repeat)
            {
                // .probably this is not necessary
                Browser.Wait.Until(
                    () =>
                        {
                            Click(by, frameBy);
                            return Browser.Wait.ForAlert(10) != null;
                        }, timeout);
            }
            else
            {
                Click(by, frameBy);
                Browser.Wait.ForAlert(timeout);
            }

            return Browser.State.GetActiveAlert() as T;
        }

        /// <summary>
        ///     Click and wait for redirect
        /// </summary>
        public void ClickAndWaitForRedirect(string scssSelector, By frameBy, bool waitForAjax = false, bool ajaxInevitable = false) => ClickAndWaitForRedirect(ScssBuilder.CreateBy(scssSelector), frameBy, waitForAjax, ajaxInevitable);

        public void ClickAndWaitForRedirect(By by, By frameBy, bool waitForAjax = false, bool ajaxInevitable = false) => RepeatAfterStale(() => ClickAndWaitForRedirect(Browser.Find.Element(by, frameBy), waitForAjax, ajaxInevitable));

        public T ClickAndWaitForRedirect<T>(string itemScss, By frameBy = null, bool ajaxInevitable = false)
            where T : class
        {
            ClickAndWaitForRedirect(itemScss, frameBy, ajaxInevitable, ajaxInevitable);
            return Browser.State.PageAs<T>();
        }

        public void ClickAndWaitForRedirect(IWebElement element, bool waitForAjax = false, bool ajaxInevitable = false)
        {
            var oldUrl = Browser.Window.Url;
            Log.Trace($"URL before click: {oldUrl}");
            Click(element, 1000);
            Browser.Wait.ForRedirect(oldUrl, waitForAjax, ajaxInevitable);
            Log.Trace($"URL after redirect click: {Browser.Window.Url}");
        }

        public void ClickAndWaitForAlertOrRedirect(By by, By frameBy, bool waitForAjax = false, bool ajaxInevitable = false) => RepeatAfterStale(() => ClickAndWaitForAlertOrRedirect(Browser.Find.Element(by, frameBy), waitForAjax, ajaxInevitable));

        public void ClickAndWaitForAlertOrRedirect(IWebElement element, bool waitForAjax = false, bool ajaxInevitable = false) {
            var oldUrl = Browser.Window.Url;
            Click(element, 1000);
            Browser.Wait.ForAlertOrRedirect(oldUrl, waitForAjax, ajaxInevitable);
        }

        public void ClickAndWaitForState(string scssSelector, Func<bool> checkState) => ClickAndWaitForState(ScssBuilder.CreateBy(scssSelector), checkState);

        public void ClickAndWaitForState(By by, Func<bool> checkState) => RepeatAfterStale(() => ClickAndWaitForState(Browser.Find.Element(by), checkState));

        public void ClickAndWaitForState(IWebElement element, Func<bool> checkState) {
            Click(element);
            try {
                Browser.Wait.Until(checkState);
            }
            catch (WebDriverTimeoutException) {
                Log.Info("Waited state not appeared");
            }
        }

        public void ClickAndWaitWhileAjax(Selector selector, int sleepTimeout = 0, bool ajaxInevitable = false) => ClickAndWaitWhileAjax(selector.By, selector.FrameBy, sleepTimeout, ajaxInevitable);

        public void ClickAndWaitWhileAjax(string scssSelector, int sleepTimeout = 0, bool ajaxInevitable = false) => ClickAndWaitWhileAjax(ScssBuilder.CreateBy(scssSelector), null, sleepTimeout, ajaxInevitable);

        public void ClickAndWaitWhileAjax(string scss, By frameBy, int sleepTimeout = 0, bool ajaxInevitable = false) => ClickAndWaitWhileAjax(ScssBuilder.CreateBy(scss), frameBy, sleepTimeout, ajaxInevitable);

        public void ClickAndWaitWhileAjax(By by, By frameBy, int sleepTimeout = 0, bool ajaxInevitable = false) => ClickAndWaitWhileAjax(Driver, by, frameBy, sleepTimeout, ajaxInevitable);

        public void ClickAndWaitWhileAjax(ISearchContext context, By by, By frameBy, int sleepTimeout = 0, bool ajaxInevitable = false) {
            Click(context, by, frameBy, sleepTimeout);
            Browser.Wait.WhileAjax(ajaxInevitable: ajaxInevitable);
        }

        public void ClickAndWaitWhileAjax(IWebElement element, int sleepTimeout = 0, bool ajaxInevitable = false) {
            // TODO: geti rid of this method
            Click(element, sleepTimeout);
            Browser.Wait.WhileAjax(ajaxInevitable: ajaxInevitable);
        }


        /// <summary>
        ///     Press Enter in field for found selector
        /// </summary>
        public void PressEnter(string scssSelector) => PressEnter(ScssBuilder.CreateBy(scssSelector));

        public void PressEnter(By by) => PressEnter(by, null);

        public void PressEnter(Selector selector) => PressEnter(selector.By, selector.FrameBy);

        public void PressEnter(By by, By frameBy) => PressKey(by, frameBy, Keys.Enter);

        /// <summary>
        ///     Press Key in input field
        /// </summary>
        public void PressKey(string scssSelector, string key) => PressKey(ScssBuilder.CreateBy(scssSelector), key);

        public void PressKey(By by, string key) => PressKey(by, null, key);

        public void PressKey(Selector selector, string key) =>
            RepeatAfterStale(() => PressKey(Browser.Find.Element(selector.By, selector.FrameBy), key));

        public void PressKey(By by, By frameBy, string key) => RepeatAfterStale(() => PressKey(Browser.Find.Element(by, frameBy), key));

        public void PressKey(IWebElement element, string key) => element.SendKeys(key);

        /// <summary>
        ///     Clear text field
        /// </summary>
        public void Clear(string scssSelector) => Clear(ScssBuilder.CreateBy(scssSelector));

        public void Clear(By by, By frameBy = null) => RepeatAfterStale(() => Clear(Browser.Find.Element(by, frameBy)));

        public void Clear(IWebElement element) => element.Clear();

        /// <summary>
        ///     Move focus from current component
        /// </summary>
        public void ChangeFocus() => PressKey(Driver.SwitchTo().ActiveElement(), Keys.Tab);

        /// <summary>
        ///     Switch to frame
        /// </summary>
        public void SwitchToFrame(By by, By frameBy) {
            var frame = _find.Element(by, frameBy);
            Driver.SwitchTo().Frame(frame);
        }

        /// <summary>
        ///     Switch to default content
        /// </summary>
        public void SwitchToDefaultContent() => Driver.SwitchTo().DefaultContent();

        /// <summary>
        ///     click and wait while While the page shows progress
        /// </summary>
        /// <param name="sleepTimeout">forced wait after a click</param>
        /// <param name="progressInevitable">
        ///     true means that after the click the progress should exactly appear
        ///     so first we expect it to appear, then wait until it disappears
        /// </param>
        public void ClickAndWaitWhileProgress(string scssSelector, int sleepTimeout = 0, bool progressInevitable = false) => ClickAndWaitWhileProgress(ScssBuilder.CreateBy(scssSelector), null, sleepTimeout, progressInevitable);

        public void ClickAndWaitWhileProgress(By by, By frameBy, int sleepTimeout = 0, bool progressInevitable = false) {
            Click(by, frameBy, sleepTimeout);
            if (progressInevitable)
                Browser.Wait.ForPageInProgress();
            Browser.Wait.WhilePageInProgress();
        }

        /// <summary>
        ///     Click on all elements found by selector
        /// </summary>
        public void ClickByAll(string scssSelector) => ClickByAll(ScssBuilder.CreateBy(scssSelector));

        public void ClickByAll(By by) {
            var elements = Browser.Find.Elements(by);
            foreach (var element in elements)
                Browser.Action.Click(element);
        }

        /// <summary>
        ///     Scroll page down
        /// </summary>
        public void ScrollToBottom() {
            var start = DateTime.Now;
            do {
                Browser.Js.ScrollToBottom();
                Browser.Wait.WhileAjax(ajaxInevitable: true);
            }
            while (!Browser.Js.IsPageBottom()
                   && (DateTime.Now - start).TotalSeconds < 300);
        }

//        /// <summary>
//        ///     Save
//        /// </summary>
//        /// <param name="marker">the file name of the screenshot</param>
//        /// <param name="folder">folder for screenshots</param>
//        public void SaveScreenshot(string marker = null, string folder = "d:\\") {
//            var screenshot = Browser.Get.Screenshot();
//            var filename = string.IsNullOrEmpty(marker) ? new Random().Next(100000).ToString() : marker;
//            var screenshotFilePath = Path.Combine(folder, filename + ".png");
//            screenshot.Save(screenshotFilePath, ImageFormat.Png);
//            Console.WriteLine("Screenshot: {0}", new Uri(screenshotFilePath));
//        }

        public void MouseOver(Selector selector, int sleepTimeout = 0) => MouseOver(selector.By, selector.FrameBy, sleepTimeout);

        /// <summary>
        ///     Move the cursor over an element
        /// </summary>
        public void MouseOver(string scssSelector, By frameBy, int sleepTimeout = 0) => MouseOver(ScssBuilder.CreateBy(scssSelector), frameBy, sleepTimeout);

        public void MouseOver(By by, By frameBy, int sleepTimeout = 0) {
            var action = new Actions(Driver);
            RepeatAfterStale(() =>
                {
                    var element = Browser.Find.Element(by, frameBy);
                    Browser.Js.ScrollIntoView(element);
                    action.MoveToElement(element).Build().Perform();
                    if (sleepTimeout != 0)
                        Thread.Sleep(sleepTimeout);
                });
        }

        public void SetFocus(IWebElement element) {
            if (element.TagName == "input")
                element.SendKeys("");
            else
                new Actions(Driver).MoveToElement(element).Build().Perform();
        }

        /// <summary>
        ///     Drag the item to the specified number of pixels horizontally
        /// </summary>
        public void DragHorizontally(string scssSelector, int pixels) => DragHorizontally(ScssBuilder.CreateBy(scssSelector), pixels);

        public void DragHorizontally(By by, int pixels)
        {
            var builder = new Actions(Driver);
            builder.DragAndDropToOffset(Browser.Find.Element(by), pixels, 0).Build().Perform();
        }

        public void DragVertically(string scssSelector, int pixels) => DragHorizontally(ScssBuilder.CreateBy(scssSelector), pixels);

        public void DragVertically(Selector selector, int pixels) => DragVertically(selector.By, pixels);

        public void DragVertically(By by, int pixels)
        {
            var builder = new Actions(Driver);
            builder.DragAndDropToOffset(Browser.Find.Element(by), 0, pixels).Build().Perform();
        }

        public void CleanDownloadsFolder()
        {
            if (Directory.Exists(Browser.Settings.DownloadDirectory))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Directory.Delete(Browser.Settings.DownloadDirectory, true);
            }
        }

        public void ContextClick(Selector selector, int sleepTimeout)
        {
            var action = new Actions(Driver);
            RepeatAfterStale(
                () =>
                    {
                        var element = Browser.Find.Element(selector);
                        action.ContextClick(element).Build().Perform();
                        if (sleepTimeout != 0)
                            Thread.Sleep(sleepTimeout);
                    });
        }
    }
}
