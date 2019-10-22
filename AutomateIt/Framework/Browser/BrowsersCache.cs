using System;
using System.Collections.Generic;
using System.IO;
using selenium.core.Framework.Service;
using selenium.core.Logging;

namespace selenium.core.Framework.Browser
{
    public class BrowsersCache
    {
        private readonly Dictionary<BrowserType, Browser> _browsers;
        private readonly ITestLogger _log;
        private readonly Web _web;

        public BrowsersCache(Web web, ITestLogger log)
        {
            _web = web;
            _log = log;
            _browsers = new Dictionary<BrowserType, Browser>();
        }

        public Browser GetBrowser(BrowserType browserType)
        {
            if (_browsers.ContainsKey(browserType))
                return _browsers[browserType];
            var browser = CreateBrowser(browserType);
            _browsers.Add(browserType, browser);
            return browser;
        }

        private Browser CreateBrowser(BrowserType browserType)
        {
            var browserSettings = new BrowserSettings();
            var driverManager = getDriverFactory(browserType, browserSettings);
            return new Browser(_web, _log, driverManager);
        }

        private DriverManager getDriverFactory(BrowserType browserType, BrowserSettings browserSettings)
        {
            switch (browserType)
            {
                case BrowserType.FIREFOX:
                    return new FirefoxDriverManager(browserSettings);
                case BrowserType.CHROME:
                    return new ChromeDriverFacrory(browserSettings);
                default:
                    return null;
            }
        }

        public bool BrowserIsCreated(BrowserType browserType) => _browsers.ContainsKey(browserType);
    }

    public class BrowserSettings
    {
        public const string DOWNLOAD_DIRECTORY_NAME = "Downloads";
        public string DownloadDirectory { get; }

        public BrowserSettings()
        {
            DownloadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DOWNLOAD_DIRECTORY_NAME);
        }
    }
}
