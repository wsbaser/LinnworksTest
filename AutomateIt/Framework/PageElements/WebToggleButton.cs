using NUnit.Framework;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public class WebToggleButton : WebButton
    {
        private readonly string _selectedClass;

        public WebToggleButton(IPage parent, string scssSelector, string selectedClass)
            : base(parent, scssSelector)
        {
            _selectedClass = selectedClass;
        }

        public bool IsActive() => Is.HasClass(By, FrameBy, _selectedClass);

        public void SelectAndWaitWhileAjax(int sleepTimeout = 0, bool ajaxInevitable = false)
        {
            if (!IsActive())
                ClickAndWaitWhileAjax(sleepTimeout, ajaxInevitable);
        }

        public void Select(int sleepTimeout = 0)
        {
            if (!IsActive())
                Click(sleepTimeout);
        }

        public void AssertIsSelected() => Assert.IsTrue(IsActive(), "'{0}' is not selected", ComponentName);
    }
}
