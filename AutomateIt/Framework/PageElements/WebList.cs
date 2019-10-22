using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public class WebListUnordered : ListBase<WebListItem>
    {
        public WebListUnordered(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        public override string ItemIdScss => InnerScss(">li");
    }
}
