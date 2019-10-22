using OpenQA.Selenium;

namespace selenium.core.Framework.Browser
{
    public class BrowserAlert : DriverFacade
    {
        public BrowserAlert(Browser browser)
            : base(browser)
        {
        }

        /// <summary>
        ///     Получение системного алерта
        /// </summary>
        public IAlert GetSystemAlert()
        {
            try
            {
                return Driver.SwitchTo().Alert();
            }
            catch (NoAlertPresentException)
            {
                return null;
            }
        }
    }
}
