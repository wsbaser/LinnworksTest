using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements
{
    public abstract class WebAccordionBase : SimpleWebComponent
    {
        public virtual void Expand()
        {
            if (!Expanded())
            {
                Log.Action($"Expand Accordion {ComponentName}");
                Action.ClickAndWaitWhileAjax(By, FrameBy);
            }
        }

        public virtual void Collapse()
        {
            if (Expanded())
            {
                Log.Action($"Collapse Accordion {ComponentName}");
                Action.Click(By, FrameBy);
            }
        }

        public virtual bool Expanded() => Get.Attr(By, FrameBy, "aria-expanded") == "true";

        public WebAccordionBase(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }
    }
}
