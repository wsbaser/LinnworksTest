using Natu.Utils.Exceptions;
using selenium.core.Framework.Browser;
using selenium.core.Framework.Page;
using selenium.core.Logging;

namespace LinnworksTest.Features.steps.@base
{
    public abstract class StepsBase
    {
        protected abstract Browser Browser { get; }
        protected abstract ITestLogger Log { get; }

        public BrowserAction Action
        {
            get { return Browser.Action; }
        }

        public BrowserAlert Alert
        {
            get { return Browser.Alert; }
        }

        public BrowserFind Find
        {
            get { return Browser.Find; }
        }

        public BrowserGet Get
        {
            get { return Browser.Get; }
        }

        public BrowserGo Go
        {
            get { return Browser.Go; }
        }

        public BrowserIs Is
        {
            get { return Browser.Is; }
        }

        public BrowserState State
        {
            get { return Browser.State; }
        }

        public BrowserWait Wait
        {
            get { return Browser.Wait; }
        }

        public BrowserJs Js
        {
            get { return Browser.Js; }
        }

        public BrowserWindow Window
        {
            get { return Browser.Window; }
        }

        private BrowserCookies Cookies
        {
            get { return Browser.Cookies; }
        }

        public T GoTo<T>(bool update = true, bool waitForAjax = true, bool ajaxInevitable = false) where T : class, IPage
        {
            if (!Browser.State.PageIs<T>() || update)
                Browser.Go.ToPage<T>();
            if (!Browser.State.PageIs<T>())
                Throw.FrameworkException("Íå ïåðåøëè íà ñòðàíèöó '{0}'", typeof(T).Name);
            if (waitForAjax)
                Browser.Wait.WhileAjax(ajaxInevitable: ajaxInevitable);
            return Browser.State.PageAs<T>();
        }
    }
}
