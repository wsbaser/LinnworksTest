using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace selenium.core.Framework.Browser
{
    public class BrowserCookies : DriverFacade
    {
        public BrowserCookies(Browser browser)
            : base(browser)
        {
        }

        /// <summary>
        ///     Clear all Cookie
        /// </summary>
        public void Clear() => Driver.Manage().Cookies.DeleteAllCookies();

        public void Set(string name, string value) => Driver.Manage().Cookies.AddCookie(new Cookie(name, value));

        public void Set(List<Cookie> cookies)
        {
            foreach (var cookie in cookies)
            {
                Driver.Manage().Cookies.AddCookie(cookie);
            }
        }

        public Cookie Get(string name) => Driver.Manage().Cookies.GetCookieNamed(name);

        public List<Cookie> GetAll() => Driver.Manage().Cookies.AllCookies.ToList();
    }
}
