using LinnworksTest.Features.steps.@base;
using TechTalk.SpecFlow;

namespace LinnworksTest.Features.steps
{
    [Binding]
    public class ScenarioSteps:LinnworksStepsBase
    {
        [Given(@"I created category '(.*)'")]
        public void GivenICreatedCategory(string categoryName) => Scenarios.CreateCategory(categoryName);
    }
}
