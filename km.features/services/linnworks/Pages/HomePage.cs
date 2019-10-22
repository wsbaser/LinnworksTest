using selenium.core.Framework.PageElements;

namespace LinnworksTest.Features.services.linnworks.Pages
{
    public class HomePage : LinnworksPageBase
    {
        public override string AbsolutePath => "/";

        [WebComponent("a['Home']")]
        public WebLink HomeLink;

        [WebComponent("a['Login']")]
        public WebLink LoginLink;

        [WebComponent("a['Logout']")]
        public WebLink LogoutLink;

        [WebComponent("a['Categories']")]
        public WebLink CategoriesLink;
    }
}