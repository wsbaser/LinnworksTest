using System;
using LinnworksTest.Features.services.linnworks.Pages;
using selenium.core.Framework.PageElements;

namespace LinnworksTest.Features.services.linnworks.Pages
{
    public class CategoriesPage : LinnworksPageBase
    {
        public override string AbsolutePath => "/fetch-category";

        [WebComponent("a['Create New']")]
        public WebLink CreateNewLink;

        [WebComponent("app-fetch-category table")]
        public CategoriesList CategoriesList { get; internal set; }

        public override void WaitLoaded() => Wait.WhileAjax(10);

        internal AddCategoryPage NavigateToAddCategoryPage() => CreateNewLink.ClickAndWaitForRedirect<AddCategoryPage>();
    }
}