using LinnworksTest.Features.services.linnworks.Pages;
using LinnworksTest.Features.steps.@base;
using TechTalk.SpecFlow;

namespace LinnworksTest.Features.steps.pages
{
    [Binding]
    public class AddCategoryPageSteps: LinnworksPageStepsBase<AddCategoryPage>
    {
        [Then(@"Error message '(.*)' is displayed for Name field")]
        public void ThenErrorMessageIsDisplayedForNameField(string messageText) => Page.NameValidationError.AssertMatch(expectedPattern: messageText);

        [When(@"I click Save button")]
        public void WhenIClickSaveButton() => Page.Save();

    }
}
