using LinnworksTest.Features.services;
using LinnworksTest.Features.services.linnworks;
using selenium.core.Framework.Browser;
using selenium.core.Framework.Page;
using selenium.core.Logging;

namespace LinnworksTest.Features.steps.@base
{
    public abstract class LinnworksPageStepsBase<P> : PageStepsBase<P>
        where P : class, IPage
    {
        protected override Browser Browser => LinnworksSeleniumContext.Inst.Browser;

        protected override ITestLogger Log => LinnworksSeleniumContext.Inst.Log;

        protected LinnworksNavigate Navigate => Linnworks.Navigate;
        protected LinnworksService Linnworks => LinnworksSeleniumContext.Inst.Linnworks;
        protected LinnworksScenarios Scenarios => Linnworks.Scenarios;
    }
}
