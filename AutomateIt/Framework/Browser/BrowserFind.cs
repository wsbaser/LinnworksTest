using System;
using System.Collections.Generic;
using System.Linq;
using automateit.SCSS;
using Natu.Utils.Exceptions;
using OpenQA.Selenium;
using selenium.core.Exceptions;

namespace selenium.core.Framework.Browser
{
    public class BrowserFind : DriverFacade
    {
        public BrowserFind(Browser browser)
            : base(browser)
        {
        }

        /// <summary>
        ///     Search for element. If not found - throw an exception
        /// </summary>
        public IWebElement Element(string scssSelector) => Element(ScssBuilder.CreateBy(scssSelector));

        public IWebElement Element(Selector selector, bool displayed = true) => Element(selector.By, selector.FrameBy, displayed);

        public IWebElement Element(By by, By frameBy = null, bool displayed = true) => Element(Driver, by, frameBy, displayed);

        public IWebElement Element(ISearchContext context, By by, bool displayed = true) => Element(context, by, null, displayed);

        public IWebElement Element(ISearchContext context, By by, By frameBy, bool displayed = true) {
            Browser.Driver.SwitchTo().DefaultContent();
            if (frameBy != null) {
                SwitchToFrame(context, frameBy);
            }
            var start = DateTime.Now;
            var elements = context.FindElements(by).ToList();
            if (elements.Count == 0) {
                Log.Selector(by);
                throw new NoSuchElementException($"Search time: {(DateTime.Now - start).TotalMilliseconds}");
            }
            if (displayed) {
                elements = elements.Where(e => e.Displayed).ToList();
                if (elements.Count == 0) {
                    if (Browser.TimeoutDisabled) {
                        Log.Selector(by);
                        throw new NoVisibleElementsException();
                    }
                    try {
                        Browser.Wait.ForElementVisible(by, frameBy);
                        elements = context.FindElements(by).Where(e => e.Displayed).ToList();
                    }
                    catch (WebDriverTimeoutException) {
                        Log.Selector(by);
                        throw new NoVisibleElementsException();
                    }
                }
            }
            if (Browser.Options.FindSingle && elements.Count > 1) {
                Log.Selector(by);
                Throw.TestException("Found more then 1 element. Count: ", elements.Count);
            }
            return elements.First();
        }

        private void SwitchToFrame(ISearchContext context, By frameBy) {
            var start = DateTime.Now;
            var frameElements = context.FindElements(frameBy);
            if (frameElements.Count == 0) {
                Log.Selector(frameBy);
                throw new NoSuchElementException($"Search time: {(DateTime.Now - start).TotalMilliseconds}");
            }
            if (frameElements.Count > 1) {
                Log.Selector(frameBy);
                Throw.TestException("Found more then 1 IFRAME element. Count: ", frameElements.Count);
            }
            else {
                Browser.Driver.SwitchTo().Frame(frameElements[0]);
            }
        }

        /// <summary>
        ///     Attempt to find element. If not found - doesn't throw an exception
        /// </summary>
        public IWebElement ElementFastOrNull(string scssSelector, bool displayed = true) => ElementFastOrNull(ScssBuilder.CreateBy(scssSelector), null, displayed);

        public IWebElement ElementFastOrNull(By by, By frameBy = null, bool displayed = true) => ElementFastOrNull(Driver, by, frameBy, displayed);

        public IWebElement ElementFastOrNull(ISearchContext context, By by, By frameBy, bool displayed = true)
        {
            try {
                Browser.DisableTimeout();
                Log.Disable();
                Browser.Options.FindSingle = false;
                return Element(context, by, frameBy, displayed);
            }
            catch (NoSuchElementException) {
                return null;
            }
            catch (NoVisibleElementsException) {
                return null;
            }
            finally {
                Browser.Options.FindSingle = true;
                Browser.EnableTimeout();
                Log.Enable();
            }
        }

        /// <summary>
        ///     Find element without waiting
        /// </summary>
        public IWebElement ElementFast(string scssSelector) => ElementFast(ScssBuilder.CreateBy(scssSelector));

        public IWebElement ElementFast(By by)
        {
            try
            {
                Browser.DisableTimeout();
                Log.Selector(by);
                return Driver.FindElement(by);
            }
            finally
            {
                Browser.EnableTimeout();
            }
        }

        /// <summary>
        ///     Find the elements by the selector without waiting. Do not fall down if nothing is found
        /// </summary>
        public List<IWebElement> Elements(string scssSelector) => Elements(ScssBuilder.CreateBy(scssSelector));

        public List<IWebElement> Elements(By by, By frameBy = null) => Elements(Driver, by, frameBy);

        public List<IWebElement> Elements(Selector selector) => Elements(Driver, selector.By, selector.FrameBy);

        public List<IWebElement> ElementsFast(string scss, string frameScss=null) => ElementsFast(new Selector(scss,frameScss));
        public List<IWebElement> ElementsFast(Selector selector) => ElementsFast(Driver, selector.By, selector.FrameBy);

        public List<IWebElement> ElementsFast(By by, By frameBy = null) => ElementsFast(Driver, by, frameBy);

        public List<IWebElement> ElementsFast(ISearchContext context, By by, By frameBy = null) {
            try {
                Browser.DisableTimeout();
                return Elements(context, by, frameBy);
            }
            finally {
                Browser.EnableTimeout();
            }
        }

        public List<IWebElement> Elements(ISearchContext context, By by, By frameBy = null) {
            try {
                Browser.Driver.SwitchTo().DefaultContent();
                if (frameBy != null) {
                    SwitchToFrame(context,frameBy);
                }
                return new List<IWebElement>(context.FindElements(by));
            }
            catch (NoSuchElementException) {
                return new List<IWebElement>();
            }
        }

        /// <summary>
        ///     Find the elements by the specified selector without waiting. Do not fall down if nothing is found
        ///     Return only visible elements
        /// </summary>
        public List<IWebElement> VisibleElements(string scssSelector) => VisibleElements(ScssBuilder.CreateBy(scssSelector));

        public List<IWebElement> VisibleElements(By by) => RepeatAfterStale(() => Elements(by).Where(e => e.Displayed).ToList());
    }
}
