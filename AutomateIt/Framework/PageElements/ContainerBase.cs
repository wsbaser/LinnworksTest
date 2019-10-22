using automateit.SCSS;
using NUnit.Framework;
using OpenQA.Selenium;
using selenium.core.Framework.Page;
using selenium.core.SCSS;

namespace selenium.core.Framework.PageElements
{
    public interface IHasDefaultAction
    {
        WaitCondition DefaultActionWaitCondition { get; set; }
        int DefaultActionWaitTimeout { get; set; }
    }

    public abstract class ContainerBase : ComponentBase, IContainer, IHasIsEmpty
    {
        private string _rootScss;

        public virtual string RootScss => _rootScss ?? (_rootScss = "html");

        public By RootSelector => ScssBuilder.CreateBy(RootScss);

        public Selector RootSelectorNew => new Selector(RootScss, FrameScss);

        public Selector FrameSelector => new Selector(FrameScss);

        protected ContainerBase(IPage parent)
            : this(parent, null)
        {
        }

        protected ContainerBase(IPage parent, string rootScss)
            : base(parent)
        {
            _rootScss = rootScss;
        }

        public override Selector Selector => new Selector(RootScss, FrameScss);

        public override bool IsDisabled() => Is.Disabled(RootSelectorNew);

        public override bool IsVisible() => Is.Visible(RootSelector, FrameBy);

        public override bool IsExist() => Is.Exists(RootSelector, FrameBy);

        public override bool IsNotVisible() => !IsVisible();

        public override bool HasClass(string className) => Is.HasClass(RootSelector, FrameBy, className);

        /// <summary>
        ///     Get Scss for the nested element
        /// </summary>
        public string InnerScss(string relativeScss, params object[] args)
        {
            relativeScss = string.Format(relativeScss, args);
            return Scss.Concat(RootScss, relativeScss);
        }

        /// <summary>
        ///     Get selector for the nested element
        /// </summary>
        public By InnerSelector(string relativeScss, params object[] args)
        {
            relativeScss = string.Format(relativeScss, args);
            return ScssBuilder.Concat(RootScss, relativeScss).By;
        }

        public Selector InnerSelectorNew(string relativeScss, params object[] args) {
            relativeScss = string.Format(relativeScss, args);
            return new Selector(ScssBuilder.Concat(RootScss, relativeScss), ScssBuilder.Create(FrameScss));
        }

        public override T GetValue<T>() => Get.Value<T>(RootSelector, FrameBy);

        /// <summary>
        ///     Get selector for the nested element
        /// </summary>
        public By InnerSelector(Scss innerScss)
        {
            var rootScss = ScssBuilder.Create(RootScss);
            return rootScss.Concat(innerScss).By;
        }

        public override string Text => Get.Text(RootSelector, FrameBy);

        public virtual void AssertIsVisible() => Assert.IsTrue(IsVisible(), $"{ComponentName} is not displayed.");

        public virtual bool IsEmpty() => string.IsNullOrWhiteSpace(GetValue());

        public virtual void AssertIsEmpty() => Assert.True(IsEmpty(), $"{ComponentName} is not empty.");

        public virtual void AssertIsNotEmpty() => Assert.False(IsEmpty(), $"{ComponentName} is empty.");

        public virtual IWebElement GetElement(bool displayed = true) => Find.Element(RootSelectorNew, displayed);

        public virtual bool IsEnabled() => Is.Enabled(RootSelectorNew);

        public override void Click(int sleepTimeout = 0) {
            Log.Action($"Click by {ComponentName}.");
            Action.Click(RootSelectorNew);
        }

        public void ClickAndWaitForNewWindow(int sleepTimeout=0)
        {
            Log.Action($"Click by '{ComponentName}' and wait new browser window");
            Action.ClickAndWaitNewWindow(RootSelector, FrameBy, sleepTimeout);
        }

        public string GetAttr(string attrName) => Get.Attr(Selector, attrName, false);

        public virtual void AssertIsReadonly()
        {
            var readonlyValue = GetAttr("readonly")?.ToLower();
            Assert.IsTrue(readonlyValue == "true", $"Component '{ComponentName} is not readonly.'");
        }
    }

    public interface IHasIsEmpty
    {
        bool IsEmpty();
        void AssertIsEmpty();
        void AssertIsNotEmpty();
    }
}
