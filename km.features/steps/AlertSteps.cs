using LinnworksTest.Features.steps.@base;
using System.Threading;
using TechTalk.SpecFlow;

namespace LinnworksTest.Features.steps
{
    [Binding]
    public class AlertSteps : LinnworksStepsBase
    {
        [When(@"I click Ok in confirmation alert")]
        public void WhenIClickOkInConfirmationAlert()
        {
            State.SystemAlert.Accept();
            Thread.Sleep(3);
        }

        [When(@"I click Cancel in confirmation alert")]
        public void WhenIClickCancelInConfirmationAlert()
        {
            State.SystemAlert.Dismiss();
            Thread.Sleep(3);
        }
    }
}
