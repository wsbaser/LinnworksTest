using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using automateit.SCSS;
using Natu.Utils.Extensions;
using OpenQA.Selenium;
using selenium.core.Framework.PageElements;

namespace selenium.core.Framework.Browser
{
    public class BrowserGet : DriverFacade
    {
        public BrowserGet(Browser browser)
            : base(browser)
        {
        }

        /// <summary>
        ///     Get initial page source
        /// </summary>
        public string PageSource => Browser.Driver.PageSource;

        // Get element content
        public string TextFastOrNull(string scssSelector) => TextFastOrNull(ScssBuilder.CreateBy(scssSelector));

        public string TextFastOrNull(By by) => RepeatAfterStale(
            () =>
                {
                    var element = Browser.Find.ElementFastOrNull(by);
                    if (element == null)
                        return null;
                    return element.Text;
                });

        // Get element content
        public string Text(string scssSelector, By frameBy = null, bool displayed = false) => Text(ScssBuilder.CreateBy(scssSelector), frameBy, displayed);

        public string Text(By by, By frameBy, bool displayed = false) => Text(Driver, by, frameBy, displayed);

        /// <summary>
        ///     Get element content
        /// </summary>
        public string Text(ISearchContext context, By by, By frameBy, bool displayed = false)
        {
            if (displayed && !Browser.Is.Visible(context, by, frameBy))
                return null;
            return RepeatAfterStale(() => Browser.Find.Element(context, by, frameBy, displayed).Text);
        }

        /// <summary>
        ///     Find the elements by the specified selector and get their texts
        /// </summary>
        public List<string> Texts(Selector selector, bool displayed = false) => Texts(selector.By, selector.FrameBy);

        public List<string> Texts(string scssSelector, By frameBy = null, bool displayed = false) => Texts(ScssBuilder.CreateBy(scssSelector), frameBy, displayed);

        public List<string> Texts(By by, By frameBy = null, bool displayed = false) => Texts(Driver, by, frameBy, displayed);

        public List<string> Texts(ISearchContext context, string scssSelector, By frameBy = null, bool displayed = false) => Texts(context, ScssBuilder.CreateBy(scssSelector), frameBy, displayed);

        public List<string> Texts(ISearchContext context, By by, By frameBy = null, bool displayed = false) => RepeatAfterStale(
            () =>
                {
                    var elements = Browser.Find.Elements(context, by, frameBy);
                    if (displayed)
                        elements = elements.Where(e => e.Displayed).ToList();
                    return elements.Select(e => e.Text).ToList();
                });

        public List<string> TextsFast(Selector selector, bool displayed = false) => TextsFast(selector.By, selector.FrameBy, displayed);

        public List<string> TextsFast(string scss, By frameBy = null, bool displayed = false) => TextsFast(ScssBuilder.CreateBy(scss), frameBy, displayed);

        public List<string> TextsFast(By by, By frameBy = null, bool displayed = false) => TextsFast(Driver, by, frameBy, displayed);

        public List<string> TextsFast(ISearchContext context, By by, By frameBy = null, bool displayed = false) => RepeatAfterStale(
            () =>
                {
                    var elements = Browser.Find.ElementsFast(context, by, frameBy);
                    if (displayed)
                        elements = elements.Where(e => e.Displayed).ToList();
                    return elements.Select(e => e.Text).ToList();
                });

        public List<string> Ids(string scssSelector, bool displayed = false) => Ids(Driver, ScssBuilder.CreateBy(scssSelector), displayed);

        public List<string> Ids(By by, bool displayed = false) => Ids(Driver, by, displayed);

        public List<string> Ids(ISearchContext context, By by, bool displayed = false) => RepeatAfterStale(
            () =>
                {
                    var elements = Browser.Find.Elements(context, by);
                    if (displayed)
                        elements = elements.Where(e => e.Displayed).ToList();
                    return elements.Select(e => e.Text).ToList();
                });

        /// <summary>
        ///     Get attribute src from tag img
        /// </summary>
        public string ImgFileName(string scssSelector) => ImgFileName(ScssBuilder.CreateBy(scssSelector));

        public string ImgFileName(By by) => ImgSrc(by).Split('/').Last();

        /// <summary>
        ///     Get attribute src from tag img
        /// </summary>
        public string ImgSrc(string scssSelector) => ImgSrc(ScssBuilder.CreateBy(scssSelector));

        public string ImgSrc(Selector selector) => ImgSrc(selector.By, selector.FrameBy);

        public string ImgSrc(By by, By frameBy = null) => Attr(by, frameBy, "src");

        /// <summary>
        ///     Get list attributes src from tag img
        /// </summary>
        public List<string> ImgSrcs(string scssSelector) => ImgSrcs(ScssBuilder.CreateBy(scssSelector));

        public List<string> ImgSrcs(By by) => Attrs(by, "src");

        public string Src(Selector selector) => Attr( selector, "src");

        /// <summary>
        ///     Get element's attribute
        /// </summary>
        /// <param name="scssSelector">Element's selector</param>
        /// <param name="name">Attribute name</param>
        /// <param name="displayed">Seek only visible elements</param>
        public string Attr(string scssSelector, string name, bool displayed = true) => Attr(ScssBuilder.CreateBy(scssSelector), null, name, displayed);

        public string Attr(By by, string name, bool displayed = true) => Attr(by, null, name, displayed);

        public string Attr(Selector selector, string name, bool displayed = true) => Attr(selector.By, selector.FrameBy, name, displayed);

        public string Attr(By by, By frameBy, string name, bool displayed = true) => RepeatAfterStale(() => Attr(Browser.Find.Element(by, frameBy, displayed), name));

        public T Attr<T>(Selector selector, string name, bool displayed = true) => Attr<T>(selector.By, selector.FrameBy, name, displayed);

        public T Attr<T>(string scssSelector, string name, bool displayed = true) => Attr<T>(ScssBuilder.CreateBy(scssSelector), name, displayed);

        public T Attr<T>(By by, string name, bool displayed = true) => Attr<T>(by, null, name, displayed);

        public T Attr<T>(By by, By frameBy, string name, bool displayed = true) => Cast<T>(Attr(by, frameBy, name, displayed));

        public T Attr<T>(IWebElement element, string name) => Cast<T>(Attr(element, name));

        public string Attr(IWebElement element, string name) => element.GetAttribute(name);

        /// <summary>
        ///     Get elements attributes
        /// </summary>
        /// <param name="scssSelector">Element's selector</param>
        /// <param name="name">Attribute name</param>
        public List<string> Attrs(string scssSelector, string name) => Attrs(ScssBuilder.CreateBy(scssSelector), name);

        public List<string> Attrs(By by, string name) => RepeatAfterStale(
            () => Browser.Find.Elements(by).Select(e => Attr(e, name)).ToList());

        /// <summary>
        ///     Get attribute href from tag a
        /// </summary>
        public string Href(string scssSelector) => Href(ScssBuilder.CreateBy(scssSelector));

        public string Href(By by) => Attr(by, "href");

        public string Href(Selector selector) => Attr(selector, "href");

        /// <summary>
        ///     Get a list of attributes href from tags a
        /// </summary>
        public List<string> Hrefs(string scssSelector) => Hrefs(ScssBuilder.CreateBy(scssSelector));

        public List<string> Hrefs(By by) => Attrs(by, "href");

        /// <summary>
        ///     Get a list of attributes of the specified type
        /// </summary>
        public List<T> Attrs<T>(string scssSelector, string name) => Attrs<T>(scssSelector, null, name);

        public List<T> Attrs<T>(string scssSelector, string frameScss, string name) => Attrs<T>(new Selector(scssSelector, frameScss), name);

        public List<T> Attrs<T>(Selector selector, string name) => RepeatAfterStale(
            () => Browser.Find.Elements(selector).Select(e => Attr(e, name)).Select(Cast<T>).ToList());

        private T Cast<T>(string value)
        {
            var type = typeof(T);
            if (type == typeof(short)
                || type == typeof(int)
                || type == typeof(long))
                return (T)Convert.ChangeType(value.FindInt(), typeof(T));
            if (type == typeof(ushort)
                || type == typeof(uint)
                || type == typeof(ulong))
                return (T)Convert.ChangeType(value.FindUInt(), typeof(T));
            if (type == typeof(decimal)
                || type == typeof(float)
                || type == typeof(double))
                return (T)Convert.ChangeType(value.FindNumber(), typeof(T));
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        ///     Get the content of the input field
        /// </summary>
        public string InputValue(By by, By frameBy, bool displayed = true) => InputValue<string>(by, frameBy, displayed);

        public T InputValue<T>(string scssSelector, bool displayed = true) => InputValue<T>(ScssBuilder.CreateBy(scssSelector), null, displayed);

        public T InputValue<T>(By by, By frameBy, bool displayed = true) {
            var value = Attr(by, frameBy, "value", displayed);
            return Cast<T>(value);
        }

        public T InputValue<T>(IWebElement element, bool displayed = true) {
            var value = Attr(element, "value");
            return Cast<T>(value);
        }

        /// <summary>
        ///     Get the integer value from the element found by the selector
        /// </summary>
        public int Int(Selector selector, bool displayed = false) => Int(selector.By, selector.FrameBy, displayed);

        public int Int(string scssSelector, string frameScss, bool displayed = false) => Int(ScssBuilder.CreateBy(scssSelector), ScssBuilder.CreateBy(frameScss), displayed);

        public int Int(string scssSelector, bool displayed = false) => Int(ScssBuilder.CreateBy(scssSelector), null, displayed);

        public int Int(By by, By frameBy, bool displayed = false) => Int(Driver, by, frameBy, displayed);

        public int Int(ISearchContext context, By by, By frameBy, bool displayed = false) => Text(context, by, frameBy, displayed).AsInt();

        /// <summary>
        ///     Get the integer value from the elements found by the selector
        /// </summary>
        public List<int> Ints(string scssSelector) => Ints(ScssBuilder.CreateBy(scssSelector));

        public List<int> Ints(By by) => Texts(by).Select(s => s.AsInt()).ToList();

        /// <summary>
        ///     Get the css tag parameter
        /// </summary>
        public T CssValue<T>(string scssSelector, ECssProperty property) => CssValue<T>(ScssBuilder.CreateBy(scssSelector), property);

        public T CssValue<T>(Selector selector, ECssProperty property) => RepeatAfterStale(() => CssValue<T>(Browser.Find.Element(selector.By, selector.FrameBy), property));

        public T CssValue<T>(By by, ECssProperty property) => RepeatAfterStale(() => CssValue<T>(Browser.Find.Element(by), property));

        public T CssValue<T>(IWebElement element, ECssProperty property)
        {
            var value = element.GetCssValue(property.StringValue());
            return Cast<T>(value);
        }

        /// <summary>
        /// Get computed value(using window.getComputedStyle()) of specified CSS property
        /// </summary>
        public T ComputedCssValue<T>(string scss, ECssProperty property) => RepeatAfterStale(() => ComputedCssValue<T>(Browser.Find.Element(ScssBuilder.CreateBy(scss)), property));

        public T ComputedCssValue<T>(By by,By frameBy, ECssProperty property) => RepeatAfterStale(() => ComputedCssValue<T>(Browser.Find.Element(by,frameBy), property));

        public T ComputedCssValue<T>(IWebElement element, ECssProperty property) {
            var value = Browser.Js.GetComputedStyle(element,property.StringValue());
            return Cast<T>(value);
        }

        public List<T> ComputedCssValues<T>(string scss, ECssProperty property) => RepeatAfterStale(
            () =>
                {
                    var webElements = Browser.Find.Elements(scss);
                    return webElements.Select(e => ComputedCssValue<T>(e, property)).ToList();
                });

        public int CountOfElements(string scss) => CountOfElements(new Selector(scss));

        public int CountOfElements(Selector selector) => CountOfElements(selector.By, selector.FrameBy);

        /// <summary>
        ///     Get the number of elements found by the specified selector
        /// </summary>
        public int CountOfElements(string scssSelector, By frameBy) => CountOfElements(ScssBuilder.CreateBy(scssSelector), frameBy);

        public int CountOfElements(By by, By frameBy) => RepeatAfterStale(() => Browser.Find.ElementsFast(by, frameBy).Count);

        public Bitmap TakeScreenshot() {
            var screenshotDriver = Driver as ITakesScreenshot;
            var screenshot = screenshotDriver.GetScreenshot();
            return new Bitmap(new MemoryStream(screenshot.AsByteArray));
        }

        /// <summary>
        ///     Make a screenshot of the specified area of the screen
        /// </summary>
        public Bitmap Screenshot()
        {
            Browser.Js.ScrollToTop();
            // Get the Total Size of the Document
            var totalWidth = (int)Browser.Js.Excecute<long>("return document.documentElement.scrollWidth");
            var totalHeight = (int)Browser.Js.Excecute<long>("return document.documentElement.scrollHeight");

            // Get the Size of the Viewport
            var viewportWidth = (int)Browser.Js.Excecute<long>("return document.documentElement.clientWidth");
            var viewportHeight = (int)Browser.Js.Excecute<long>("return document.documentElement.clientHeight");

            // Split the Screen in multiple Rectangles
            var rectangles = new List<Rectangle>();
            // Loop until the Total Height is reached
            for (var i = 0; i < totalHeight; i += viewportHeight)
            {
                var newHeight = viewportHeight;
                // Fix if the Height of the Element is too big
                if (i + viewportHeight > totalHeight)
                {
                    newHeight = totalHeight - i;
                }
                // Loop until the Total Width is reached
                for (var ii = 0; ii < totalWidth; ii += viewportWidth)
                {
                    var newWidth = viewportWidth;
                    // Fix if the Width of the Element is too big
                    if (ii + viewportWidth > totalWidth)
                    {
                        newWidth = totalWidth - ii;
                    }

                    // Create and add the Rectangle
                    var currRect = new Rectangle(ii, i, newWidth, newHeight);
                    rectangles.Add(currRect);
                }
            }

            // Build the Screenshot
            var stitchedImage = new Bitmap(totalWidth, totalHeight);
            // Get all Screenshots and stitch them together
            var previous = Rectangle.Empty;
            foreach (var rectangle in rectangles)
            {
                // Calculate the Scrolling (if needed)
                if (previous != Rectangle.Empty)
                {
                    var xDiff = rectangle.Right - previous.Right;
                    var yDiff = rectangle.Bottom - previous.Bottom;
                    // Scroll
                    Browser.Js.Excecute($"window.scrollBy({xDiff}, {yDiff})");
                    Thread.Sleep(200);
                }

                // Take Screenshot
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();

                // Build an Screenshot out of the Screenshot
                Image screenshotImage;
                using (var memStream = new MemoryStream(screenshot.AsByteArray))
                {
                    screenshotImage = Image.FromStream(memStream);
                }

                // Calculate the Source Rectangle
                var sourceRectangle = new Rectangle(viewportWidth - rectangle.Width,
                    viewportHeight - rectangle.Height, rectangle.Width,
                    rectangle.Height);

                // Copy the Screenshot
                using (var g = Graphics.FromImage(stitchedImage))
                {
                    g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                }

                // Set the Previous Rectangle
                previous = rectangle;
            }
            // The full Screenshot is now in the Variable "stitchedImage"
            return stitchedImage;
        }

        /// <summary>
        ///     Get the rectangle, specified by the Css properties of the element left, top, height, width
        /// </summary>
        public Rectangle Bounds(string scssSelector) => Bounds(ScssBuilder.CreateBy(scssSelector));

        public Rectangle Bounds(By by) => RepeatAfterStale(() => Bounds(Browser.Find.Element(by)));

        /// <summary>
        ///     Get element boundaries
        /// </summary>
        public Rectangle Bounds(IWebElement element) => new Rectangle(CssValue<int>(element, ECssProperty.left),
            CssValue<int>(element, ECssProperty.top),
            CssValue<int>(element, ECssProperty.width),
            CssValue<int>(element, ECssProperty.height));

        /// <summary>
        ///     Get the coordinates of an element
        /// </summary>
        public Point Point(IWebElement element) => new Point(CssValue<int>(element, ECssProperty.left),
            CssValue<int>(element, ECssProperty.top));

        /// <summary>
        ///     Get the value of the specified type from the element found by the selector
        /// </summary>
        public T Value<T>(string scssSelector) => Value<T>(ScssBuilder.CreateBy(scssSelector));

        public T Value<T>(By by, By frameBy=null) => RepeatAfterStale(() => Value<T>(Browser.Find.Element(by, frameBy)));

        public T Value<T>(IWebElement element) => Cast<T>(element.Text);

        public List<T> Values<T>(By by, By frameBy = null) => RepeatAfterStale(() => Values<T>(Browser.Find.Elements(by, frameBy)));

        public List<T> Values<T>(IEnumerable<IWebElement> elements) => elements.Select(e => Cast<T>(e.Text)).ToList();

        /// <summary>
        ///     Get a list of texts by the selector and combine them into a string
        /// </summary>
        public string TextsAsString(string scssSelector, string delimiter = " ") => TextsAsString(ScssBuilder.CreateBy(scssSelector), delimiter);

        public string TextsAsString(By by, string delimiter = " ") => Texts(by).AsString(delimiter);

        /// <summary>
        ///     Get the href attribute and get absolute Url from it (without a domain)
        /// </summary>
        public string AbsoluteHref(By by) => Href(by).CutBaseUrl();

        public string LastDownloadedFileName(bool waitUntilDownloaded = false)
        {
            var downloadedFile = Browser.Get.LastDownloadedFile(waitUntilDownloaded);
            return downloadedFile.Name.EndsWith(".crdownload", StringComparison.Ordinal) ? downloadedFile.Name.RemoveExtension() : downloadedFile.Name;
        }

        /// <summary>
        ///     Returns FileSystemInfo of the last file downloaded in Browser
        /// </summary>
        public FileInfo LastDownloadedFile(bool waitUntilDownloaded)
        {
            FileInfo lastDownloadedFile;
            if (waitUntilDownloaded)
            {
                lastDownloadedFile = LastDownloadedFile();
                const int NO_CHANGE_INTERVAL = 1000;
                long curLength = lastDownloadedFile.Length;
                Thread.Sleep(NO_CHANGE_INTERVAL);
                Browser.Wait.Until(() =>
                {
                    if (lastDownloadedFile.Length == curLength)
                    {
                        return true;
                    }
                    curLength = lastDownloadedFile.Length;
                    return false;
                }, BrowserTimeouts.FILE_DOWNLOAD, NO_CHANGE_INTERVAL);
            }
            return LastDownloadedFile();
        }

        public FileInfo LastDownloadedFile() =>
            Browser.Wait.Until(() =>
            {
                // Wait until file appear in the Downloads folder
                if (Directory.Exists(Browser.Settings.DownloadDirectory))
                {
                    var files = new DirectoryInfo(Browser.Settings.DownloadDirectory).GetFiles();
                    return files.OrderByDescending(f => f.CreationTime).FirstOrDefault();
                }
                return null;
            }, BrowserTimeouts.FILE_DOWNLOAD_START);

        //public string ChecksumOfLastDownloadedFile(bool waitUntilDownloaded = true)
        //{
        //    var lastDownloadedFile = LastDownloadedFile(waitUntilDownloaded);
        //    string checksum;
        //    using (var md5 = MD5.Create())
        //    {
        //        using (var stream = File.OpenRead(lastDownloadedFile.FullName))
        //        {
        //            var computeHash = md5.ComputeHash(stream);
        //            checksum = Convert.ToBase64String(computeHash);
        //        }
        //    }
        //    return checksum;
        //}

        public List<LogEntry> JsErrors()
        {
            var errorStrings = new List<string>
            {
                "SyntaxError",
                "EvalError",
                "ReferenceError",
                "RangeError",
                "TypeError",
                "URIError"
            };
            //            return Driver.Manage().Logs.GetLog(LogType.Browser).Where(x => errorStrings.Any(e => x.Message.Contains(e))).ToList();
            // Waiting for this to be released https://github.com/SeleniumHQ/selenium/commit/26d8b67a58e99fa4121376537ea434c761eea5e6
            return new List<LogEntry>();
        }

        public string Placeholder(Selector selector) => Attr(selector, "placeholder");

        private static string _clipboardValue;

        public string ClipboardValue()
        {
            var thread = new Thread(() => _clipboardValue = Clipboard.GetText());
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join(1000);
            return _clipboardValue;
        }
    }
}