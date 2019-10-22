using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public class WebListItem : ItemBase
    {
        [WebComponent("root: a")]
        public WebLink Link;

        public override string ItemScss => ContainerInnerScss("li[normalize-space(.)='{0}']", ID);

        public WebListItem(IContainer container, string id)
            : base(container, id)
        {
        }

        public void ClickAndWatiForRedirect(bool waitForAjax = false, bool ajaxInevitable = false)
        {
            Log.Action($"Click by list item '{ID}'");
            Action.ClickAndWaitForRedirect(ItemScss, FrameBy, waitForAjax, ajaxInevitable);
        }

        public override void Click(int sleepTimeout = 0)
        {
            Log.Action($"Click by list item '{ID}'");
            Action.Click(Selector);
        }

        public T ClickAndWatiForRedirect<T>(bool ajaxInevitable = false)
            where T : class
        {
            Log.Action($"Click by list item '{ID}'");
            return Action.ClickAndWaitForRedirect<T>(ItemScss, FrameBy, ajaxInevitable);
        }

        public void MouseOver()
        {
            Log.Action($"Click by list item '{ID}'");
            Action.MouseOver(ItemScss, FrameBy);
        }

        public bool ContainsLink() => Link.IsVisible();
    }
}