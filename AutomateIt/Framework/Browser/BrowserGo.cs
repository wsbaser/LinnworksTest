using System;
using System.IO;
using System.Web;
using OpenQA.Selenium;
using selenium.core.Framework.Page;
using selenium.core.Framework.Service;

namespace selenium.core.Framework.Browser
{
    public class BrowserGo : DriverFacade
    {
        public BrowserGo(Browser browser)
            : base(browser)
        {
        }

        // The definition of Url, which corresponds to the class of the page and the transition to it
        public T ToPage<T>() where T :class, IPage
        {
            var pageInstance = (T)Activator.CreateInstance(typeof(T));
            ToPage(pageInstance);
            return Browser.State.PageAs<T>();
        }

        // The definition of Url, which corresponds to the class of the page and the transition to it
        public void ToPage(IPage page)
        {
            var requestData = Web.GetRequestData(page);
            ToUrl(requestData);
            var logEntries = Browser.Get.JsErrors();
            foreach (var logEntry in logEntries) {
                Log.Error($"{logEntry.Timestamp}: {logEntry.Level} - {logEntry.Message}");
            }
        }

        public void ToUrl(string url) => ToUrl(new RequestData(url));

        // Go to the specified Url in the current browser window
        public void ToUrl(RequestData requestData)
        {
            if (requestData.HasBasicAuth())
            {
                Log.Action($"Navigating to url: {requestData.Url} with Basic Authentication");
                string basicAuthDomain =
                    $"{HttpUtility.UrlEncode(requestData.BasicAuthLogin)}:{HttpUtility.UrlEncode(requestData.BasicAuthPassword)}@{requestData.Url.Host}";
                string url = $"{requestData.Url.Scheme}://{basicAuthDomain}{requestData.Url.PathAndQuery}";
                Driver.Navigate().GoToUrl(url);
            }
            else
            {
                Log.Action($"Navigating to url: {requestData.Url}");
                Driver.Navigate().GoToUrl(requestData.Url);
            }

            Browser.State.Actualize();
            if (Browser.State.PageIs<PageBase>())
            {
                Browser.State.PageAs<PageBase>().WaitLoaded();
            }
        }

        /// <summary>
        ///     Save the initial code of the page to disk and open the page in the browser
        /// </summary>
        /// <typeparam name="T">Class of the page</typeparam>
        /// <param name="html">Initial code of the page</param>
        public T ToHtml<T>(string html) where T : IPage {
            // Save to the disk
            var type = typeof(T);
            var fileName = type.Name + ".html";
            var pagesFolder = Path.Combine(Environment.CurrentDirectory, "SavedPages");
            if (!Directory.Exists(pagesFolder))
                Directory.CreateDirectory(pagesFolder);
            var filePath = Path.Combine(pagesFolder, fileName);
            File.WriteAllText(filePath, html);

            // Open on browser
            ToUrl("file://" + filePath);

            // Create the appropriate page class
            var page = (T)Activator.CreateInstance(type);
            page.Activate(Browser, Log);
            Browser.ApplyPageOptions(page);
            return page;
        }

        /// <summary>
        ///     Find the letter with the specified header on the specified mailbox.
        ///     Open the text of the message in the browser
        /// </summary>
        /// <summary>
        ///     Back to previous page
        /// </summary>
        public void Back()
        {
            Driver.Navigate().Back();
            Log.Action($"Go.Back(). Result Url: {Driver.Url}");
            Browser.State.Actualize();
        }

        public void Refresh()
        {
            Log.Action($"Refresh page {Driver.Url}");
            Driver.Navigate().Refresh();
            Browser.State.Actualize();
        }
    }
}
