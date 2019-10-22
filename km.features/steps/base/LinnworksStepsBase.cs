using LinnworksTest.Features.services;
using LinnworksTest.Features.services.linnworks;
using selenium.core.Framework.Browser;
using selenium.core.Logging;

namespace LinnworksTest.Features.steps.@base
{
    public class LinnworksStepsBase : StepsBase
    {
        protected override Browser Browser => LinnworksSeleniumContext.Inst.Browser;

        protected override ITestLogger Log => LinnworksSeleniumContext.Inst.Log;

        protected LinnworksNavigate Navigate => LinnworksSeleniumContext.Inst.Linnworks.Navigate;

        protected LinnworksScenarios Scenarios => LinnworksSeleniumContext.Inst.Linnworks.Scenarios;

        protected LinnworksService Km => LinnworksSeleniumContext.Inst.Linnworks;
    }
}
