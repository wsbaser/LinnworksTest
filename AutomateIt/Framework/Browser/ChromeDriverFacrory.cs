using System;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace selenium.core.Framework.Browser
{
    public class ChromeDriverFacrory : DriverManager
    {
        private IWebDriver _driver;

        public ChromeDriverFacrory(BrowserSettings browserSettings)
        {
            Settings = browserSettings;
        }

        #region DriverManager Members

        public BrowserSettings Settings { get; }

        public void InitDriver()
        {
            // https://src.chromium.org/viewvc/chrome/trunk/src/chrome/common/pref_names.cc?view=markup

            var options = new ChromeOptions();
            options.AddArgument("--allow-running-insecure-content");
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-infobars");
            options.AddArgument("--test-type");
            options.AddArgument("no-sandbox");            
            options.AddUserProfilePreference("download.default_directory", Settings.DownloadDirectory);


            _driver = new ChromeDriver(options);
            _driver.Manage().Window.Size = new Size(1400, 900);
            _driver.Manage().Window.Maximize();
        }

        public IWebDriver GetDriver() => _driver;

        public void DestroyDriver() => _driver.Quit();

        #endregion
    }
}
