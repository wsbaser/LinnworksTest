using LinnworksTest.Features.services;
using LinnworksTest.Features.services.linnworks;
using selenium.core.Framework.Browser;
using TechTalk.SpecFlow;

namespace LinnworksTest.Features
{
    [Binding]
    public partial class SpecFlowHooks
    {
        private static LinnworksService Linnworks => LinnworksSeleniumContext.Inst.Linnworks;
        private static Browser Browser => LinnworksSeleniumContext.Inst.Browser;


        [BeforeTestRun(Order = -30000)]
        public static void BeforeTestRun()
        {
            LinnworksSeleniumContext.Inst.Init();
        }

        [AfterTestRun(Order = -30000)]
        public static void AfterTestRun()
        {
            LinnworksSeleniumContext.Inst.Destroy();
        }

        [AfterScenario]
        public static void AfterScenario()
        {
            if (Browser.State.GetActiveAlert() != null)
            {
                Browser.Go.Refresh();
            }
        }

        [BeforeFeature("login")]
        public static void Login() => Linnworks.Scenarios.Login(Linnworks.AuthToken);

        [BeforeFeature("delete_categories")]
        public static void DeleteCategories()
        {
            using (var context = Linnworks.GetLinnworksContext())
            {
                context.Categories.RemoveRange(context.Categories);
                context.SaveChanges();
            }
            Browser.Go.Refresh();
        }
    }
}