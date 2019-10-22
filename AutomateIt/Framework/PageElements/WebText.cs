using System;
using System.Globalization;
using NUnit.Framework;
using OpenQA.Selenium;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public class WebText : SimpleWebComponent
    {
        public bool Displayed;

        public WebText(IPage parent, By by)
            : base(parent, by)
        {
        }

        public WebText(IPage parent, string rootScss) : this(parent, rootScss, false)
        {
        }

        public WebText(IPage parent, string rootScss, bool displayed=false)
            : base(parent, rootScss)
        {
            Displayed = displayed;
        }

        public override string Text => Get.Text(By, FrameBy, Displayed);

        public string TextOrNull => IsVisible() ? Get.Text(By, FrameBy, Displayed) : null;
        public object Placeholder => Get.Placeholder(Selector);

        public override string GetValue() => TextOrNull;

        public void AssertIsHidden() => Assert.IsFalse(IsVisible(), $"{ComponentName} is displayed");

        public void Click(int sleepTimeout = 0)
        {
            Log.Action($"Click the fake link '{ComponentName}'");
            Action.Click(By, FrameBy, sleepTimeout);
        }

        public void ClickAndWaitForAlert() => Action.ClickAndWaitForAlert(By, FrameBy);

        public void ClickAndWaitWhileAjax(int sleepTimeout = 0, bool ajaxInevitable = false) {
            Log.Action($"Click the fake link '{ComponentName}'");
            Action.ClickAndWaitWhileAjax(By, FrameBy, sleepTimeout, ajaxInevitable);
        }

        public T ClickAndWaitForRedirect<T>(bool waitForAjax = false, bool ajaxInevitable = false) where T : class, IPage {
            ClickAndWaitForRedirect(waitForAjax, ajaxInevitable);
            return State.PageAs<T>();
        }

        public void ClickAndWaitForRedirect(bool waitForAjax = false, bool ajaxInevitable = false)
        {
            Log.Action($"Click the fake link '{ComponentName}'");
            Action.ClickAndWaitForRedirect(By, FrameBy, waitForAjax, ajaxInevitable);
        }

        public void ClickAndWaitForState(Func<bool> checkState)
        {
            Log.Action($"Click the fake link '{ComponentName}'");
            Action.ClickAndWaitForState(By, checkState);
        }

        public T Value<T>() => Get.Value<T>(By);

        public void DragByHorizontal(int pixels)
        {
            Log.Action($"Drag '{ComponentName}' at {pixels} pixels");
            Action.DragHorizontally(By, pixels);
        }

        public void MouseOver() => Action.MouseOver(By, FrameBy);

        public DateTime GetDateTime(string format) => DateTime.ParseExact(Text, format, CultureInfo.InvariantCulture);
    }
}
