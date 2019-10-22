using System;
using selenium.core.Framework.Browser;
using selenium.core.Framework.Service;
using selenium.core.Logging;

namespace selenium.core {
    public class SeleniumContext<T> : ISeleniumContext
        where T : ISeleniumContext {
        private readonly BrowsersCache _browsersCache;

        protected SeleniumContext(ITestLogger log)
        {
            Log = log;
            Web = new Web();
            _browsersCache = new BrowsersCache(Web, Log);
        }

        public static T Inst => SingletonCreator<T>.CreatorInstance;

        #region ISeleniumContext Members

        public Web Web { get; }

        public ITestLogger Log { get; }

        public Browser Browser => _browsersCache.GetBrowser(BrowserType.CHROME);

        public bool BrowserIsCreated => _browsersCache.BrowserIsCreated(BrowserType.CHROME);

        #endregion

        protected virtual void InitWeb() {
        }

        public virtual void Init() {
            InitWeb();
        }

        public virtual void Destroy() {
            Inst.Browser.Destroy();
        }

        #region Nested type: SingletonCreator

        /// ‘абрика используетс€ дл€ отложенной инициализации экземпл€ра класса
        private sealed class SingletonCreator<S>
            where S : ISeleniumContext {
            //»спользуетс€ Reflection дл€ создани€ экземпл€ра класса без публичного конструктора
            //            (S).GetConstructor(
            //                BindingFlags.Instance | BindingFlags.NonPublic,
            //                null,
            //                new Type[0],
            //                new ParameterModifier[0]).Invoke(null);

            public static S CreatorInstance { get; } = (S)Activator.CreateInstance(typeof(S));
        }

        #endregion
    }
}