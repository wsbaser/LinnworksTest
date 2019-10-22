using automateit.SCSS;
using NUnit.Framework;
using OpenQA.Selenium;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public class WebRadioButton : SimpleWebComponent
    {
        private string _labelScss;

        public Selector LabelSelector => new Selector(_labelScss, FrameScss);

        public WebRadioButton(IPage parent, By by)
            : base(parent, by)
        {
        }

		public WebRadioButton(IPage parent, string rootScss) : base(parent, rootScss)
		{
		}

        public WebRadioButton(IPage parent, string rootScss, string labelScss) : base(parent, rootScss)
        {
            _labelScss = labelScss;
        }

        public override void Click(int sleepTimeout = 0)
        {
            if (!string.IsNullOrWhiteSpace(_labelScss))
            {
                Action.Click(LabelSelector);
            }
            else
            {
                Action.Click(Selector);
            }
        }

        public virtual void SelectAndWaitWhileAjax(int sleepTimeout = 0, bool ajaxInevitable = false)
        {
            Log.Action($"Select '{ComponentName}' radiobutton");
            Select(sleepTimeout);
            Wait.WhileAjax(ajaxInevitable: ajaxInevitable);
        }

        public bool IsSelected() => Is.Selected(RootSelectorNew);

        public virtual void Select(int sleepTimeout = 0)
        {
            Log.Action($"Select '{ComponentName}' radiobutton");
            Click(sleepTimeout);
        }

        public void AssertIsSelected() => Assert.IsTrue(IsSelected(), $"'{ComponentName}' is not selected.");

        public override bool IsVisible()
        {
            return string.IsNullOrWhiteSpace(_labelScss) ? base.IsVisible() : Is.Visible(LabelSelector);
        }
    }

}
