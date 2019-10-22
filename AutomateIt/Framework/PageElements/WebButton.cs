using Natu.Utils.Exceptions;
using NUnit.Framework;
using OpenQA.Selenium;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public class WebButton : SimpleWebComponent, IClickable, IHasDefaultAction
    {
        public WaitCondition DefaultActionWaitCondition { get; set; }
        public int DefaultActionWaitTimeout { get; set; }

        public WebButton(IPage parent, By by)
            : base(parent, by)
        {
        }

        public WebButton(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        #region IClickable Members

        public override void Click(int sleepTimeout = 0)
        {
            Log.Action($"Click '{ComponentName} button'");
            Wait.Condition(() => Action.Click(By, FrameBy, sleepTimeout),
                DefaultActionWaitCondition,
                DefaultActionWaitTimeout);
        }

        #endregion

        public IAlert ClickAndWaitForAlert(bool repeat = false) => ClickAndWaitForAlert<AlertBase>(repeat);

        public T ClickAndWaitForAlert<T>(bool repeat = false, bool ajaxInevitable = false) where T : class, IAlert {
            Log.Action($"Click '{ComponentName}' button");
            var alert = Action.ClickAndWaitForAlert<T>(By, FrameBy, repeat: repeat);
            if (alert == null) {
                throw new TestException($"Alert of type '{typeof(T).Name}' did not appear.");
            }
            return alert;
        }

        public void ClickAndWaitWhileAjax(int sleepTimeout = 0, bool ajaxInevitable = false)
        {
            Log.Action($"Click '{ComponentName}' button");
            Action.ClickAndWaitWhileAjax(By, FrameBy, sleepTimeout, ajaxInevitable);
        }

        public void ClickAndWaitForRedirect(bool waitForAjax = false, bool ajaxInevitable = false) => ClickAndWaitForRedirect<PageBase>(waitForAjax, ajaxInevitable);

        public T ClickAndWaitForRedirect<T>(bool waitForAjax = false, bool ajaxInevitable = false)
            where T : class {
            Log.Action($"Click '{ComponentName}' button");
            Action.ClickAndWaitForRedirect(By, FrameBy, waitForAjax, ajaxInevitable);
            return State.PageAs<T>();
        }

        public void ClickAndWaitForAlertOrRedirect(bool waitForAjax, bool ajaxInevitable)
        {
            Log.Action($"Click '{ComponentName}' button");
            Action.ClickAndWaitForAlertOrRedirect(By, FrameBy, waitForAjax, ajaxInevitable);
        }

        public void ClickAndWaitWhileProgress()
        {
            Log.Action($"Click '{ComponentName}' button");
            Action.ClickAndWaitWhileProgress(By, FrameBy, 1000);
        }
		

        public override string Text
        {
            get
            {
                Log.Action($"Get '{ComponentName}' title");
                return Get.Text(By, FrameBy);
            }
        }

        public void AssertIsVisible() => Assert.IsTrue(IsVisible(), "{0} is not displayed", ComponentName);

        public void MouseOver(int sleepTimeout = 0)
		{
			Log.Action($"Hover cursor on '{ComponentName}' button");
			Action.MouseOver(By, FrameBy, sleepTimeout);
		}

		public void AssertTooltipEqual(string tooltip) => Assert.AreEqual(tooltip, Get.Attr(By,FrameBy, "title"), @"Invalid tooltip {0} button", ComponentName);
    }
}
