using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace km.tests.selenium.services.kmNewUI.Pages.EndUser.DocumentPage {
    public class WebiFrame:ContainerBase {
        public WebiFrame(IPage parent)
            : base(parent) {
        }

        public WebiFrame(IPage parent, string rootScss)
            : base(parent, rootScss) {
        }
    }
}