using selenium.core.Framework.Page;

namespace LinnworksTest.Features.steps.@base
{
    public abstract class PageStepsBase<P> : StepsBase
        where P : class, IPage
    {
        protected P Page => Browser.State.PageAs<P>();
    }
}
