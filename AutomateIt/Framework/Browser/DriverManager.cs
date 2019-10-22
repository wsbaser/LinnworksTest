using OpenQA.Selenium;

namespace selenium.core.Framework.Browser
{
    public interface DriverManager
    {
        BrowserSettings Settings { get; }
        void InitDriver();
        IWebDriver GetDriver();
        void DestroyDriver();
    }
}
