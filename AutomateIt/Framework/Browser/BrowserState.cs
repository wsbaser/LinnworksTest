using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Natu.Utils.Extensions;
using OpenQA.Selenium;
using selenium.core.Framework.Page;
using selenium.core.Framework.Service;

namespace selenium.core.Framework.Browser {
    public class BrowserState : DriverFacade {
        // Object for working with a page in the active browser window
        public IPage Page;

        // Object for working with html imitation of an alert displayed in the active browser page
        public IHtmlAlert HtmlAlert;

        // Object for working with the system alert displayed in the active browser page
        public IAlert SystemAlert;

        /// <summary>
        ///     Current Window Id is
        /// </summary>
        public string CurrentWindowHandle;

        private readonly PagesPool _pagesPool;

        public BrowserState(Browser browser)
            : base(browser) {
            var pageTypes = Web.Services.SelectMany(s => s.Router.GetAllPageTypes());
            _pagesPool = new PagesPool(pageTypes);
            _pagesPool.Run();
        }

        public bool PageInvalidated => Page?.Invalidated ?? false;

        public IPageObject ActivePageObject => (IPageObject)HtmlAlert ?? Page;

        public bool ActivePageObjectIs<T>() => ActivePageObject is T;

        public List<IOverlay> GetOverlays() => PageAs<IPage>()?.Overlays.Where(o => o.IsOpened()).ToList() ?? new List<IOverlay>();

        public T GetOverlay<T>()
            where T : class, IOverlay
        {
            var page = PageAs<IPage>();
            return page == null ? null : (T)Page.Overlays.FirstOrDefault(o => o is T && o.IsOpened());
        }

        public IAlert GetActiveAlert() => Browser.Alert.GetSystemAlert() ?? (HtmlAlert != null && HtmlAlert.IsVisible() ? HtmlAlert : null);

        /// <summary>
        ///     Bringing the current html alert to the specified type
        /// </summary>
        public T HtmlAlertAs<T>()
            where T : class => HtmlAlert as T;

        /// <summary>
        ///     Bringing the current html alert to the specified type
        /// </summary>
        public bool HtmlAlertIs<T>() {
            if (HtmlAlert == null)
                return false;
            return HtmlAlert is T;
        }

        // Bringing the current page to the correct type
        public T PageAs<T>()
            where T : class => Page as T;

        // Verifying that the current page class matches the specified type
        public bool PageIs<T>() {
            if (Page == null)
                return false;
            return Page is T;
        }

        // Determine the current state of the browser
        public void Actualize()
        {
            var sw = new Stopwatch();
            sw.Start();
            ActualizeSystemAlert();
            if (SystemAlert != null)
                return;
            ActualizeWindow();
            ActualizePage();
            ActualizeHtmlAlert();
            sw.Stop();
            Log.Trace(
                $"Actualize. Time(ms): {sw.ElapsedMilliseconds}, " +
                $"Page: {Page?.GetType().Name}, " +
                $"Alert: {HtmlAlert?.GetType().Name}");
        }

        /// <summary>
        ///     Updating the current window
        /// </summary>
        public bool ActualizeWindow() {
            if (Driver.WindowHandles.Last() != CurrentWindowHandle) {
                Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                CurrentWindowHandle = Driver.CurrentWindowHandle;
                //Driver.Manage().Window.Maximize();
                return true;
            }
            return false;
        }

        public int GetWindowsCount() => Driver.WindowHandles.Count;

        /// <summary>
        ///     Updating the Html Alert
        /// </summary>
        public void ActualizeHtmlAlert() {
            HtmlAlert = null;
            if (Page == null)
                return;
            HtmlAlert = Page.Alerts.FirstOrDefault(a => a.IsVisible());
        }

        /// <summary>
        ///     Updating the system alert
        /// </summary>
        public void ActualizeSystemAlert() => SystemAlert = Browser.Alert.GetSystemAlert();

        public void ActualizeAlerts()
        {
            ActualizeSystemAlert();
            if (SystemAlert == null)
            {
                ActualizeHtmlAlert();
            }
        }

        public void ActualizePage() => ActualizePage(new RequestData(Driver.Url, new List<Cookie>(Driver.Manage().Cookies.AllCookies.AsEnumerable())));

        /// <summary>
        ///     Class definition for working with the current active browser page
        /// </summary>
        public void ActualizePage(RequestData requestData) {
            // . empty
            Page = null;

            // . match
            var result = Web.MatchService(requestData);
            if (result != null) {
                Page = result.Service.GetPage(requestData, result.BaseUrlInfo);
            }

            // . initialize and activate
            if (Page == null) {
                //Log.Debug("We are on unknown page.");
            }
            else {
                var initializedPage = _pagesPool.GetInitializedPage(Page);
                if (initializedPage != null) {
                    Page = initializedPage;
                }
                else {
                    Page.InitializeComponents();
                }
                Page.Activate(Browser, Log);
                Browser.ApplyPageOptions(Page);
                //Log.Debug($"We are on the {Page.GetType().Name}.");
            }
        }

        public bool HasOverlay() => PageAs<IPage>()?.Overlays.Any(o => o.IsOpened()) ?? false;

        public void InvalidatePage() {
            if (Page != null) {
                Page.Invalidated = true;
            }
        }

        public T GetHtmlAlert<T>()
            where T : class {
            ActualizeHtmlAlert();
            return HtmlAlertAs<T>();
        }
    }

    internal class PagesPool {
        private readonly ConcurrentBag<Type> _pageTypes;
        private readonly ConcurrentDictionary<Type, IPage> _initializedPages;
        private Thread _thread;

        public PagesPool(IEnumerable<Type> pageTypes) {
            _pageTypes = new ConcurrentBag<Type>(pageTypes);
            _initializedPages = new ConcurrentDictionary<Type, IPage>();
        }

        public void Run() {
            _thread = new Thread(GeneratePages);
            _thread.Start();
        }

        public void GeneratePages() {
            while (true) {
                if (_initializedPages.Count < _pageTypes.Count()) {
                    foreach (var pageType in _pageTypes) {
                        if (!_initializedPages.ContainsKey(pageType)) {
                            _initializedPages.TryAdd(pageType, GeneratePage(pageType));
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private IPage GeneratePage(Type pageType) {
            var instance = (IPage)Activator.CreateInstance(pageType);
            instance.InitializeComponents();
            return instance;
        }

        public IPage GetInitializedPage(IPage page) {
            IPage initializedPage;
            if (_initializedPages.TryRemove(page.GetType(), out initializedPage)) {
                initializedPage.BaseUrlInfo = page.BaseUrlInfo;
                initializedPage.Data = page.Data;
                initializedPage.Params = page.Params;
                initializedPage.Cookies = page.Cookies;
            }
            return initializedPage;
        }
    }
}