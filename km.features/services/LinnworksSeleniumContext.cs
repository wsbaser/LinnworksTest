using LinnworksTest.Features.services.linnworks;
using selenium.core;
using selenium.core.Framework.Service;
using selenium.core.Logging;

namespace LinnworksTest.Features.services
{
    public class LinnworksSeleniumContext : SeleniumContext<LinnworksSeleniumContext>
    {
        public LinnworksSeleniumContext() : base(new TestLogger()) { }

        protected override void InitWeb()
        {
            try
            {
                Linnworks = Web.RegisterService<LinnworksService>(new LinnworksServiceFactory("localhost:59509"));
            }
            catch (RouterInitializationException e)
            {
                Log.Exception(e, "Unable to initialize service");
            }
        }

        public LinnworksService Linnworks;
    }
}