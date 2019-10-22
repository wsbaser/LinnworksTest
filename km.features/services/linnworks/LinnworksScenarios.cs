using System;
using LinnworksTest.Features.services.linnworks.Pages;
using selenium.core.Framework.Browser;
using selenium.core.Framework.Page;
using selenium.core.Logging;

namespace LinnworksTest.Features.services.linnworks
{
    public class LinnworksScenarios
    {
        private readonly LinnworksService linnworks;

        internal CategoriesPage CreateCategory(string categoryName)
        {
            var categoriesPage = Navigate.ToCategoriesPage();
            if (!categoriesPage.CategoriesList.GetIds().Contains(categoryName))
            {
                var addCategory = categoriesPage.NavigateToAddCategoryPage();
                addCategory.FillAndSave(categoryName);
            }
            return Browser.State.PageAs<CategoriesPage>();
        }

        public LinnworksNavigate Navigate => linnworks.Navigate;

        public LinnworksScenarios(LinnworksService linnworks)
        {
            this.linnworks = linnworks;
        }

        private Browser Browser => linnworks.Context.Browser;
        private ITestLogger Log => Browser.Log;

        public T PreparePage<T>(bool fresh = false)
            where T : class, IPage => PreparePage<T>(() => Browser.Go.ToPage<T>(), fresh);

        public T PreparePage<T>(Action action, bool fresh = false)
            where T : class, IPage
        {
            if (!fresh)
            {
                if (Browser.State.PageIs<T>())
                {
                    var page = Browser.State.PageAs<T>();
                    page.CleanUp();
                    return page;
                }
            }
            action.Invoke();
            if (!Browser.State.PageIs<T>())
            {
                throw new Exception($"Could not open page '{typeof(T).Name}'");
            }
            return Browser.State.PageAs<T>();
        }

        internal void Login(string token)
        {
            var homePage = Navigate.ToHomePage();
            if (homePage.LoginLink.IsVisible())
            {
                var loginPage = homePage.LoginLink.ClickAndWaitForRedirect<LoginPage>();
                loginPage.Login(token);
            }
        }
    }
}