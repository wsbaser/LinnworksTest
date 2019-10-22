using OpenQA.Selenium;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public class WebImage : SimpleWebComponent
    {
        public WebImage(IPage parent, By by)
            : base(parent, by)
        {
        }

        public WebImage(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

	    public string GetClass() => Get.Attr(By, FrameBy, "class");

        public string GetFileName() => Get.ImgFileName(By);

        public void Click(int sleepTimeout = 0)
        {
            Log.Action($"Click by icon '{ComponentName}'");
            Action.Click(By, FrameBy, sleepTimeout);
        }

        public void ClickAndWaitForRedirect(bool waitForAjax = false, bool ajaxInevitable = false)
        {
            Log.Action($"Click '{ComponentName} button'");
            Action.ClickAndWaitForRedirect(By, FrameBy, waitForAjax, ajaxInevitable);
        }

        public void ClickAndWaitWhileAjax(int sleepTimeout = 0, bool ajaxInevitable = false)
        {
            Log.Action($"Click '{ComponentName} button'");
            Action.ClickAndWaitWhileAjax(By, FrameBy, sleepTimeout, ajaxInevitable);
        }

        public string GetSrc() => Get.ImgSrc(Selector);
    }
}
