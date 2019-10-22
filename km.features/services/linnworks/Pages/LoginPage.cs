using selenium.core.Framework.PageElements;

namespace LinnworksTest.Features.services.linnworks.Pages
{
    public class LoginPage : LinnworksPageBase
    {
        public override string AbsolutePath => "/login";

        [WebComponent("#token")]
        public WebInput Token;

        [WebComponent("button[type='submit']")]
        public WebButton LoginButton;

        public void Login(string token)
        {
            Token.TypeIn(token);
            LoginButton.ClickAndWaitForRedirect();
        }
    }
}