using System;
using OpenQA.Selenium;
using selenium.core.Framework.Page;
using selenium.core.Framework.Service;
using selenium.core.Logging;

namespace selenium.core.Framework.Browser {
    public class Browser {
        public BrowserOptions Options { get; private set; }
        private readonly DriverManager _driverManager;

        public BrowserSettings Settings => _driverManager.Settings;

        public readonly BrowserAction Action;
        public readonly BrowserAlert Alert;
        public readonly BrowserFind Find;
        public readonly BrowserGet Get;
        public readonly BrowserGo Go;
        public readonly BrowserIs Is;
        public readonly BrowserState State;
        public readonly BrowserWait Wait;
        public readonly BrowserJs Js;
        public BrowserWindow Window;
        public BrowserCookies Cookies;

        public Browser(Web web, ITestLogger log, DriverManager driverManager) {
            Web = web;
            Log = log;
            _driverManager = driverManager;
            _driverManager.InitDriver();
            Driver = _driverManager.GetDriver();
            Find = new BrowserFind(this);
            Get = new BrowserGet(this);
            Is = new BrowserIs(this);
            Alert = new BrowserAlert(this);
            State = new BrowserState(this);
            Action = new BrowserAction(this);
            Window = new BrowserWindow(this);
            Go = new BrowserGo(this);
            Wait = new BrowserWait(this);
            Js = new BrowserJs(this);
            Cookies = new BrowserCookies(this);
            Options = new BrowserOptions();
        }

        public ITestLogger Log { get; private set; }
        public Web Web { get; private set; }
        public IWebDriver Driver { get; private set; }
        public bool TimeoutDisabled { get; set; }

        // Уничтожить драйвер(закрывает все открытые окна браузер)
        public void Destroy() => _driverManager.DestroyDriver();

        // Пересоздать драйвер
        public void Recreate() {
            Log.Action("Close browser and open again");
            _driverManager.DestroyDriver();
            _driverManager.InitDriver();
            Driver = _driverManager.GetDriver();
        }

        public void DisableTimeout() {
            Driver.Manage().Timeouts().ImplicitWait =TimeSpan.FromSeconds(value: 0);
            TimeoutDisabled = true;
        }

        public void EnableTimeout() {
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(BrowserTimeouts.FIND);
            TimeoutDisabled = false;
        }

        public void WithOptions(Action action, bool findSingle = BrowserOptions.FINDSINGLE_DEFAULT, bool useJsClick=BrowserOptions.USE_JS_CLICK_DEFAULT) {
            var memento = (BrowserOptions)Options.Clone();
            Options.FindSingle = findSingle;
            Options.UseJsClick = useJsClick;
            action.Invoke();
            Options = memento;
        }

        public void ApplyPageOptions(IPage page) => Options = page.BrowserOptions;

        public void Cleanup() => Action.CleanDownloadsFolder();
    }

    public class BrowserOptions : ICloneable {
        public const bool FINDSINGLE_DEFAULT = true;

        public const bool WAIT_WHILE_AJAX_BEFORE_CLICK_DEFAULT = true;

        public const TypeInStyle TYPE_IN_STYLE_DEFAULT = TypeInStyle.FullValue;
        /// <summary>
        ///     Если при поиске элемента по селектору найдено несколько кидать исключение
        /// </summary>
        public bool FindSingle;

        /// <summary>
        ///     Ожидать завершения Ajax запросов перед выполнением клика
        /// </summary>
        public bool WaitWhileAjaxBeforeClick;

        public const bool USE_JS_CLICK_DEFAULT=false;

        public BrowserOptions() {
            FindSingle = FINDSINGLE_DEFAULT;
            WaitWhileAjaxBeforeClick = WAIT_WHILE_AJAX_BEFORE_CLICK_DEFAULT;
            UseJsClick = USE_JS_CLICK_DEFAULT;
            TypeInStyle = TYPE_IN_STYLE_DEFAULT;
        }

        public bool UseJsClick { get; set; }
        public TypeInStyle TypeInStyle { get; set; }

        public object Clone() {
            var options = new BrowserOptions
            {
                FindSingle = FindSingle,
                WaitWhileAjaxBeforeClick = WaitWhileAjaxBeforeClick
            };
            return options;
        }
    }

    public enum TypeInStyle {
        FullValue,
        Chars,
        Js
    }

    public static class BrowserTimeouts {
        /// <summary>
        ///     Иногда после выполнения некоторого действия страница подвисает и просто ничего не происходит.
        ///     Данное значение определяет максимальное время ожидания пока отрабатывает Java Script
        /// </summary>
        public const int JS = 10;

        /// <summary>
        ///     Таймаут ожидания при поиске элемента по умолчанию
        /// </summary>
        public const int FIND = 10;

        /// <summary>
        ///     Таймаут ожидания пока отображается прогресс
        /// </summary>
        public const int AJAX = 45;

        public const int REDIRECT = 60;
        public const int WINDOW = 45;

        /// <summary>
        ///     Таймаут ожидания пока подгружаются компоненты страницы
        /// </summary>
        public const int PAGE_LOAD = 60;

        /// <summary>
        ///     Таймаут ожидания пока загрузится файл
        /// </summary>
        public const int FILE_DOWNLOAD = 45;

        /// <summary>
        ///     Timeout of waiting when file starts downloading
        /// </summary>
        public const int FILE_DOWNLOAD_START = 45;
    }
}
