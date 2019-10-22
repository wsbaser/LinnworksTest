using System;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace LinnworksTest.Features.services.linnworks.Pages
{
    public class CategoriesList : ListBase<CategoryListItem>
    {
        public CategoriesList(IPage parent, string rootScss = null) : base(parent, rootScss)
        {
        }

        public override string ItemIdScss => InnerScss("tbody>tr>td[1]");
    }

    public class CategoryListItem : ItemBase
    {
        [WebComponent("root: a['Edit']")]
        public WebText EditLink;

        [WebComponent("root: a['Delete']")]
        public WebText DeleteLink;

        public CategoryListItem(IContainer container, string id) : base(container, id)
        {
        }

        public override string ItemScss => Container.InnerScss($"tbody>tr[>td[1]['{ID}']]");

        internal EditCategoryPage Edit() => EditLink.ClickAndWaitForRedirect<EditCategoryPage>();
    }
}