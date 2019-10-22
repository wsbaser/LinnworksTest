using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using automateit.SCSS;
using OpenQA.Selenium;

namespace selenium.core.Framework.Browser
{
    public class BrowserJs : DriverFacade {
        public BrowserJs(Browser browser)
            : base(browser) {
        }

        /// <summary>
        ///     Выполнить Java Script
        /// </summary>
        public T Excecute<T>(string js, params object[] args) => (T)Convert.ChangeType(Excecute(js, args), typeof(T));

        /// <summary>
        ///     Выполнить Java Script
        /// </summary>
        public object Excecute(string js, params object[] args) {
            var excecutor = Driver as IJavaScriptExecutor;
            return excecutor.ExecuteScript(js, args);
        }

        /// <summary>
        ///     Recommended as the most reliable way here https://stackoverflow.com/questions/3401343/scroll-element-into-view-with-selenium
        /// </summary>
        public void ScrollIntoView(IWebElement element)
        {
            Excecute("arguments[0].scrollIntoView(true);", element);
            Thread.Sleep(100);  // Probably would need to increase
        }

        /// <summary>
        ///     Получить обработчики события для указанного элемента
        /// </summary>
        /// <remarks>только для страниц с JQuery</remarks>
        public string GetEventHandlers(string css, JsEventType eventType) {
            var js = string.Format($@"var handlers= $._data($('{css}').get(0),'events').{eventType};
                          var s='';
                          for(var i=0;i<handlers.length;i++)
                            s+=handlers[i].handler.toString();
                          return s;");
            return Excecute<string>(js);
        }

        /// <summary>
        ///     Находимся ли внизу страницы
        /// </summary>
        public bool IsPageBottom() => Excecute<bool>("return document.body.scrollHeight===" +
                                                     "document.body.scrollTop+document.documentElement.clientHeight");

        /// <summary>
        ///     Прокрутить скроллбар до низа страницы
        /// </summary>
        public void ScrollToBottom() => Excecute(@"window.scrollTo(0,
                                       Math.max(document.documentElement.scrollHeight,
                                                document.body.scrollHeight,
                                                document.documentElement.clientHeight));");

        /// <summary>
        ///     Прокрутить скроллбар до верха страницы
        /// </summary>
        public void ScrollToTop() => Excecute(@"window.scrollTo(0,0);");

        public void SetCssValue(By by, By frameBy, ECssProperty cssProperty, int value) => Excecute($"arguments[0].style.{cssProperty}={value};", Browser.Find.Element(by, frameBy));

        public void Click(Selector selector) => Click(Browser.Find.Element(selector.By, selector.FrameBy));

        public void Click(By by, By frameBy) => Click(Browser.Find.Element(by, frameBy));

        public void Click(IWebElement element) {
            //Excecute($"window.scrollTo(0,{element.Location.X})");
            Excecute("arguments[0].click();", element);
        }

        public void SetValue(IWebElement element, string value) => Excecute($"arguments[0].setAttribute('value', '{value}')", element);

        public string GetComputedStyle(IWebElement element, string cssPropertyName) => Excecute<string>($"return window.getComputedStyle(arguments[0])['{cssPropertyName}'];", element);

        public string GetFirstLevelText(Selector selector) => GetFirstLevelText(Browser.Find.Element(selector.By, selector.FrameBy));
        public string GetFirstLevelText(string css) => GetFirstLevelText(Browser.Find.Element(css));


	    public string GetFirstLevelText(IWebElement element) => Excecute<string>(
            @"var iter = document.evaluate('text()', arguments[0], null, XPathResult.ORDERED_NODE_ITERATOR_TYPE);
                var want = '';
                var node;
                while (node = iter.iterateNext()){
                    want += node.data;
                }
                return want;", element);

        public List<string> GetFirstLevelTexts(Selector selector) => GetFirstLevelTexts(Browser.Find.Element(selector.By, selector.FrameBy));

        public List<string> GetFirstLevelTexts(IWebElement element)
        {
            var texts = Excecute(
                @"var iter = document.evaluate('text()', arguments[0], null, XPathResult.ORDERED_NODE_ITERATOR_TYPE);
                var want = [];
                var node;
                while (node = iter.iterateNext()){
                    want.push(node.data);
                }
                return want;", element);
            return (texts as ReadOnlyCollection<object>).Select(t => t.ToString()).ToList();
        }

        public string GetSelectedTextFor(Selector selector) =>
            Excecute<string>(
                $"return arguments[0].value.slice(arguments[0].selectionStart, arguments[0].selectionEnd);",
                Browser.Find.Element(selector));
    }

    /// <summary>
    ///     типы стандартных js событий
    /// </summary>
    public enum JsEventType
    {
        click
    }
}
