using Natu.Utils.Exceptions;
using OpenQA.Selenium;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public class WebLink : SimpleWebComponent, IClickable
    {
        public WebLink(IPage parent, By by)
            : base(parent, by)
        {
        }

        public WebLink(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        public override string Text
        {
            get { return Get.Text(By, FrameBy); }
        }

        public string Href => Get.Href(Selector);

        #region IClickable Members

        public override void Click(int sleepTimeout = 0)
        {
            Log.Action($"Click '{ComponentName}' link");
            Action.ClickAndWaitForRedirect(By, FrameBy);
        }

        #endregion

        public void ClickAndWaitWhileAjax(bool ajaxInevitable = false)
        {
            Log.Action($"Click '{ComponentName}' link");
            Action.ClickAndWaitWhileAjax(By, FrameBy, ajaxInevitable: ajaxInevitable);
        }

        public T ClickAndWaitForRedirect<T>(bool waitForAjax = false, bool ajaxInevitable = false)
            where T : class
        {
            Log.Action($"Click '{ComponentName}' button");
            Action.ClickAndWaitForRedirect(By, FrameBy, waitForAjax, ajaxInevitable);
            return State.PageAs<T>();
        }

        public void ClickAndWaitForRedirect(bool waitForAjax = false, bool ajaxInevitable = false)
        {
            Log.Action($"Click '{ComponentName}' link");
            Action.ClickAndWaitForRedirect(By, FrameBy, waitForAjax, ajaxInevitable);
        }

        public T ClickAndWaitForAlert<T>(bool repeat = false)
            where T : class, IAlert {
            Log.Action($"Click '{ComponentName}' button");
            var alert = Action.ClickAndWaitForAlert<T>(By, FrameBy, repeat: repeat);
            if (alert == null) {
                throw new TestException($"Alert of type '{typeof(T).Name}' did not appear.");
            }
            return alert;
        }

        public void MouseOver(int sleepTimeout = 0)
        {
            Log.Action($"Hover cursor on '{ComponentName}' link");
            Action.MouseOver(By, FrameBy, sleepTimeout);
        }
    }
}
