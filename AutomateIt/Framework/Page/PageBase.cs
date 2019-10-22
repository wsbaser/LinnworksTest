using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenQA.Selenium;
using selenium.core.Framework.Browser;
using selenium.core.Framework.Service;
using selenium.core.Logging;

namespace selenium.core.Framework.Page
{
    public abstract class PageBase : IPage
    {
        protected PageBase()
        {
            Data = new Dictionary<string, string>();
            Params = new Dictionary<string, string>();
            Components = new ConcurrentDictionary<string, IComponent>(StringComparer.OrdinalIgnoreCase);
        }

        public Browser.Browser Browser { get; private set; }

        public ITestLogger Log { get; private set; }

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

        #region IPage Members

        /// <summary>
        ///     Активизировать страницу
        /// </summary>
        /// <remarks>
        ///     Если страница активна, значит через нее можно работать с браузером
        /// </remarks>
        public virtual void Activate(Browser.Browser browser, ITestLogger log) {
            Browser = browser;
            Log = log;
        }

        public virtual void InitializeComponents() {
            Alerts = new List<IHtmlAlert>();
            Overlays = new List<IOverlay>();
            ProgressBars = new List<IProgressBar>();
            WebPageBuilder.InitPage(this);
        }

        public virtual void CleanUp() {
            if (State.PageInvalidated
                || Is.AjaxActive())
            {
                Refresh();
            }
            var alert = State.GetActiveAlert();
            if (alert != null)
            {
                alert.Dismiss();
                State.ActualizeAlerts();
                if (State.GetActiveAlert() != null)
                {
                    Refresh();
                }
            }
            try {
                foreach (var overlay in State.GetOverlays()) {
                    overlay.Close();
                }
            }
            catch (Exception e) {
                Log.Error("Error occured while closing overlay.");
                Log.Error(e.Message);
                Refresh();
            }
        }

        public virtual void Refresh() => Go.Refresh();

        public List<IProgressBar> ProgressBars { get; private set; }

        /// <summary>
        /// Browser Options to use on the page
        /// </summary>
        public virtual BrowserOptions BrowserOptions => new BrowserOptions();

	    public bool Invalidated { get; set; }

	    public List<IHtmlAlert> Alerts { get; private set; }

        public List<IOverlay> Overlays{ get; private set; }

        public BaseUrlInfo BaseUrlInfo { get; set; }

        public List<Cookie> Cookies { get; set; }

        public Dictionary<string, string> Params { get; set; }

        public Dictionary<string, string> Data { get; set; }

        protected ConcurrentDictionary<string, IComponent> Components;

        public void RegisterComponent(IComponent component)
        {
            if (!Components.TryAdd(component.ComponentName, component))
            {
                // TODO: what should I do in this case?
                //Console.WriteLine($"Unable to register component '{component.ComponentName}' in page '{GetType().Name}'");
            }

            if (component is IHtmlAlert)
                Alerts.Add(component as IHtmlAlert);
            else if (component is IOverlay)
            {
                Overlays.Add(component as IOverlay);
            }
            else if (component is IProgressBar)
                ProgressBars.Add(component as IProgressBar);
        }

        public virtual void WaitLoaded()
        {
            try
            {
                Wait.Until(IsLoaded, LoadPageTimeout);
            }
            catch (WebDriverTimeoutException e)
            {
                Log.Error($"Could not load page after {LoadPageTimeout} of waiting.");
                Go.Refresh();
                Wait.Until(IsLoaded, LoadPageTimeout);
            }
        }

        protected int LoadPageTimeout => BrowserTimeouts.PAGE_LOAD;

        public virtual bool IsLoaded() => true;

        public T RegisterComponent<T>(string componentName, params object[] args) where T : IComponent
        {
            var component = CreateComponent<T>(args);
            RegisterComponent(component);
            component.ComponentName = componentName;
            return component;
        }

        public T CreateComponent<T>(params object[] args) where T : IComponent
        {
            return (T)WebPageBuilder.CreateComponent<T>(this, args);
        }

        #endregion
    }
}
