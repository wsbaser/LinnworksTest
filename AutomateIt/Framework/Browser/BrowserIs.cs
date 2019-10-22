﻿using System.Linq;
using automateit.SCSS;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace selenium.core.Framework.Browser {
    public class BrowserIs : DriverFacade {
        public BrowserIs(Browser browser)
            : base(browser) {
        }

        public bool Visible(Selector selector) => Visible(selector.By, selector.FrameBy);

        public bool Visible(string scssSelector, By frameBy = null) => Visible(ScssBuilder.CreateBy(scssSelector), frameBy);

        public bool Visible(By by, By frameBy = null) => Visible(Browser.Driver, by, frameBy);

        public bool Visible(ISearchContext context, By by, By frameBy) => RepeatAfterStale(
            () => Browser.Find.ElementFastOrNull(context, @by, frameBy) != null);

        public bool NotVisible(By by) => NotVisible(Browser.Driver, by, null);

        public bool NotVisible(By by, By frameBy) => NotVisible(Browser.Driver, by, frameBy);

        public bool NotVisible(ISearchContext context, By by, By frameBy) => RepeatAfterStale(
            () =>
                {
                    var element = Browser.Find.ElementFastOrNull(context, by, frameBy, false);
                    return element == null || !element.Displayed;
                });

        /// <summary>
        ///     Checks if the element has the specified class
        /// </summary>
        public bool HasClass(Selector selector, string className) => HasClass(selector.By, selector.FrameBy, className);

        public bool HasClass(string scssSelector, string className) => HasClass(ScssBuilder.CreateBy(scssSelector), null, className);

        public bool HasClass(By by, By frameBy, string className) => RepeatAfterStale(
            () =>
                {
                    var element = Browser.Find.ElementFastOrNull(@by, frameBy);
                    if (element == null)
                        return false;
                    return HasClass(element, className);
                });

        public bool HasClass(IWebElement element, string className) => element.GetAttribute("class").Split(' ').Select(c => c.Trim()).Contains(className);

        /// <summary>
		/// Is element exist
		/// </summary>
		public bool Exists(string scssSelector, string frameScss = null) => Exists(ScssBuilder.CreateBy(scssSelector), ScssBuilder.CreateBy(frameScss));
        public bool Exists(string scssSelector, By frameBy = null) => Exists(ScssBuilder.CreateBy(scssSelector), frameBy);

        public bool Exists(By by, By frameBy) => Browser.Find.ElementFastOrNull(@by, frameBy, false) != null;

        public bool Exists(By by) => Browser.Find.ElementFastOrNull(by, null, false) != null;

        public bool Exists(ISearchContext context, string scssSelector) => Browser.Find.ElementFastOrNull(context, ScssBuilder.CreateBy(scssSelector), null, false) != null;

        /// <summary>
        ///     if true - at least one ajax request is executed
        /// </summary>
        public bool AjaxActive() => !Browser.Js.Excecute<bool>(
            @"
                        var isJqueryComplete = typeof(jQuery) != 'function' || !jQuery.active;
                        var isPrototypeComplete = typeof(Ajax) != 'function' || !Ajax.activeRequestCount;
                        var isDojoComplete = typeof(dojo) != 'function' || !dojo.io.XMLHTTPTransport.inFlight.length;
                        return isJqueryComplete && isPrototypeComplete && isDojoComplete;");

        /// <summary>
        ///     Is checkbox checked
        /// </summary>

        public bool Checked(Selector selector) => Checked(selector.By, selector.FrameBy);

        public bool Checked(string scssSelector) => Checked(ScssBuilder.CreateBy(scssSelector));

        public bool Checked(By by, By frameBy = null) => RepeatAfterStale(() => Checked(Browser.Find.Element(by, frameBy, false)));

        public bool Checked(IWebElement element) => element.GetAttribute("checked") == "true";

        public bool Expanded(IWebElement element) => element.GetAttribute("aria-expanded") == "true";

        public string GetUrl(By by) => Browser.Find.Element(by).GetAttribute("src");

        //public bool Active(string scssSelector) => Active(ScssBuilder.CreateBy(scssSelector));

        //public bool Active(By by, By frameBy = null) => RepeatAfterStale(() => Active(Browser.Find.Element(by, frameBy, false)));

        //public bool Active(IWebElement element) => !element.GetAttribute("class").Contains("disabled");

        public bool HasJsErrors() => Browser.Get.JsErrors().Count > 0;

        public bool Selected(Selector selector) => RepeatAfterStale(() => Browser.Find.Element(selector.By, selector.FrameBy).Selected);

        public bool Disabled(Selector selector) => RepeatAfterStale(() => Browser.Get.Attr(selector.By, selector.FrameBy, "disabled")?.ToLower()=="true");

        public bool Enabled(Selector selector) => !Disabled(selector);

        public bool Stale(IWebElement element) {
            try {
                var elementEnabled = element.Enabled;
                return false;
            }
            catch (StaleElementReferenceException) {
                return true;
            }
        }
    }
}