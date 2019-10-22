using LinnworksTest.Features.services.linnworks.Pages;
using LinnworksTest.Features.steps.@base;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace LinnworksTest.Features.steps.pages
{
    [Binding]
    public class CategoriesPageSteps : LinnworksPageStepsBase<CategoriesPage>
    {
        [Then(@"Categories list contains category '(.*)'")]
        public void ThenCategoriesListContainsCategory(string categoryName) => Assert.IsTrue(Page.CategoriesList.GetItem(categoryName).IsVisible());

        [Then(@"Categories list does not contain category '(.*)'")]
        public void ThenCategoriesListDoesNotContainCategory(string categoryName) => Assert.IsFalse(Page.CategoriesList.GetItem(categoryName).IsVisible());


        [When(@"I click Edit link for category '(.*)'")]
        public void WhenIClickEditLinkForCategory(string categoryName) => Page.CategoriesList.GetItem(categoryName).EditLink.ClickAndWaitForRedirect<EditCategoryPage>();


        [When(@"I click Delete link for category '(.*)'")]
        public void WhenIClickDeleteLinkForCategory(string categoryName) => Page.CategoriesList.GetItem(categoryName).DeleteLink.ClickAndWaitForAlert();

    }
}
