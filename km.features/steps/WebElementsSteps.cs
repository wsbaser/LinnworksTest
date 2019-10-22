using LinnworksTest.Features.services.linnworks.Pages;
using LinnworksTest.Features.steps.@base;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;
using TechTalk.SpecFlow;

namespace LinnworksTest.Features.steps
{
    [Binding]
    public class WebElementsSteps : LinnworksPageStepsBase<LinnworksPageBase>
    {
        [When(@"I type '(.*)' to (.*)")]
        [Given(@"I typed '(.*)' to (.*)")]
        public void WhenITypeTo(string text, string elementName) => Page.GetElement<WebInput>(elementName).TypeIn(text);

        [Then(@"([^']*) state is '(.*)'")]
        [Then(@"([^']*) value is '(.*)'")]
        [Then(@"([^']*) has text '(.*)'")]
        public void ThenSecurityLumpSizeValueIs(string elementName, string value) => Page.GetElement<ComponentBase>(elementName).AssertEqual(value);

        [Then(@"([^']*) is displayed")]
        public void ThenIsDisplayed(string elementName) => Page.GetElement<ComponentBase>(elementName).AssertVisible();

        [Then(@"([^']*) is not displayed")]
        public void ThenIsNotDisplayed(string elementName) => Page.GetElement<ComponentBase>(elementName).AssertNotVisible();

        [When(@"I click '(.*)'")]
        public void WhenIClick(string elementName) => Page.GetElement<ContainerBase>(elementName).Click();
    }
}