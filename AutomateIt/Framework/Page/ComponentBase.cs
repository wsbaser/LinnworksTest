using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using automateit.SCSS;
using Natu.Utils.Exceptions;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using selenium.core.Framework.Browser;
using selenium.core.Logging;
using Assert = NUnit.Framework.Assert;

namespace selenium.core.Framework.Page {
    public abstract class ComponentBase : IComponent {
        private string _componentName;

        protected ComponentBase(IPage parent) {
            ParentPage = parent;
        }

        #region IComponent Members

        public virtual string ComponentName {
            get {
                _componentName = _componentName ?? (_componentName = GetType().Name);
                return _componentName;
            }
            set { _componentName = value; }
        }

        public string FrameScss { get; set; }

        public By FrameBy => FrameScss != null ? ScssBuilder.CreateBy(FrameScss) : null;

        public abstract Selector Selector { get; }

        public IPage ParentPage { get; }
        public abstract bool IsVisible();
        public abstract bool IsExist();
        public abstract bool IsNotVisible();
        public abstract bool HasClass(string className);
        public abstract bool IsDisabled();


        public virtual string Text { get; }

        public Browser.Browser Browser => ParentPage.Browser;

        public ITestLogger Log => ParentPage.Log;

        public BrowserAction Action => Browser.Action;

        public BrowserAlert Alert => Browser.Alert;

        public BrowserFind Find => Browser.Find;

        public BrowserGet Get => Browser.Get;

        public BrowserGo Go => Browser.Go;

        public BrowserIs Is => Browser.Is;

        public BrowserState State => Browser.State;

        public BrowserWait Wait => Browser.Wait;

        public BrowserJs Js => Browser.Js;

        public BrowserWindow Window => Browser.Window;

        BrowserCookies IPageObject.Cookies => Browser.Cookies;

        #endregion

        //*****     ACTION     **********************************************************************************************************************
        public T Open<T>(Action action)
            where T : ComponentBase {
            return (T)Open(action);
        }

        public virtual ComponentBase Open(Action action) {
            if (!IsVisible()) {
                Log.Action($"Open component '{ComponentName}'");
                action.Invoke();
                try {
                    Wait.Until(IsVisible);
                }
                catch (WebDriverTimeoutException) {
                    Throw.FrameworkException("Component '{0}' is not opened", ComponentName);
                }
            }
            return this;
        }

        public void WaitForVisible(int timeout = 3) => Wait.Until(IsVisible, timeout);

        public void WaitForNotVisible(int timeout = 3) => Wait.Until(IsNotVisible, timeout);

        //*****     IS     **************************************************************************************************************************
        //*****     GET     *************************************************************************************************************************
        public virtual string GetValue() => Text;

        public abstract T GetValue<T>();

        //*****     ASSERT     **********************************************************************************************************************
        public virtual void AssertContains(string expected, bool ignoreRegister = false) {
            var text = ignoreRegister ? Text.ToLower().Trim() : Text;
            expected = ignoreRegister ? expected.ToLower().Trim() : expected;
            StringAssert.Contains(expected, text, $"Text in component {ComponentName} doesn't contain value '{expected}'");
        }

        public virtual Match AssertMatch(string expectedPattern) {
            var regex = new Regex(expectedPattern);
            var text = Text;
            Log.Info(text);
            var match = regex.Match(text);
            Assert.IsTrue(
                match.Success, $"Text in component {ComponentName} doesn't match pattern '{expectedPattern}'. " +
                                     $"Actual value is '{text}'");
            return match;
        }

        public virtual void AssertEnabled() => Assert.IsFalse(IsDisabled(), $"'{ComponentName}' is disabled.");

        public virtual void AssertDisabled() => Assert.IsTrue(IsDisabled(), $"'{ComponentName}' is enabled.");

        public virtual void AssertVisible() => Assert.IsTrue(IsVisible(), "'{0}' is not displayed", ComponentName);

        public virtual void AssertNotVisible() => Assert.IsFalse(IsVisible(), "'{0}' is displayed", ComponentName);

        public virtual void AssertExist() => Assert.IsTrue(IsExist(), "'{0}' does not exist.", ComponentName);

        public virtual void AssertNotExist() => Assert.IsFalse(IsExist(), "'{0}' is exist.", ComponentName);

        public virtual void AssertNotEqual(string expected, bool ignoreRegister = false) {
            var text = ignoreRegister ? Text?.ToLower() : Text;
            expected = ignoreRegister ? expected?.ToLower() : expected;
            Assert.AreNotEqual(expected, text?.Replace("'", ""), $"Component text '{ComponentName}' are not equal to expected value");
        }

        public virtual void AssertEqual(object expected)
        {
            if (expected is string)
            {
                AssertEqual((string)expected);
            }
            else
            {
                AssertEqual(JsonConvert.SerializeObject(expected));
            }
        }

        public virtual void AssertEqual(string expected, bool ignoreRegister = false) {
            var value = GetValue();
            if (ignoreRegister) {
                value = value?.ToLower();
                expected = expected?.ToLower();
            }
            Assert.AreEqual(expected?.Trim(), value?.Trim(), "Incorrect value in '{0}'", ComponentName);
        }

        public abstract void Click(int sleepTimeout = 0);
    }
}