using automateit.SCSS;
using NUnit.Framework;
using OpenQA.Selenium;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public abstract class SimpleWebComponent : ContainerBase
    {
        public readonly By By;

        protected SimpleWebComponent(IPage parent, By by)
            : base(parent)
        {
            By = by;
        }

        protected SimpleWebComponent(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
            By = ScssBuilder.CreateBy(rootScss);
        }

        public override bool IsVisible() => Is.Visible(By, FrameBy);

        // TODO: we do not need By field. should always use RootSelector intead
        public override bool IsNotVisible() => Is.NotVisible(By, FrameBy);

		// TODO(***): Use Is.Disabled instead
		//public void AssertIsDisabled() => Assert.AreEqual("true", Get.Attr(By, "disabled"), "WebComponent is active");

    }

    //    public abstract class WebElementWrap : ComponentBase
    //    {
    //        public readonly IWebElement Element;
    //
    //        protected WebElementWrap(IPage parent, IWebElement element)
    //            : base(parent)
    //        {
    //            Element = element;
    //        }
    //
    //        public override bool IsVisible()
    //        {
    //            return Element.Displayed;
    //        }
    //
    //        public virtual void ClickAndWaitWhileAjax(bool ajaxInevitable = false)
    //        {
    //            Action.ClickAndWaitWhileAjax(Element, ajaxInevitable: ajaxInevitable);
    //        }
    //    }
}
