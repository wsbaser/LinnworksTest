using automateit.SCSS;
using selenium.core.Framework.PageElements;

namespace selenium.core.Framework.Page
{
    public interface IComponent : IPageObject, IClickable, IGetValue
    {
        IPage ParentPage { get; }
        bool IsVisible();
        string ComponentName { get; set; }
        string FrameScss { get; set; }
        Selector Selector { get; }
        string Text { get; }
    }
}
