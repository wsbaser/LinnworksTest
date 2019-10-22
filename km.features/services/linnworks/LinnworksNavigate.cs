using System;
using LinnworksTest.Features.services.linnworks.Pages;
using selenium.core.Framework.Browser;

namespace LinnworksTest.Features.services.linnworks
{
    public class LinnworksNavigate
    {
        private readonly LinnworksService _westkm;
        private Browser Browser => _westkm.Context.Browser;
        private LinnworksScenarios Scenarios => _westkm.Scenarios;

        public LinnworksNavigate(LinnworksService westkm)
        {
            _westkm = westkm;
        }

        internal HomePage ToHomePage() => Scenarios.PreparePage<HomePage>();

        internal CategoriesPage ToCategoriesPage() => Scenarios.PreparePage<CategoriesPage>();

        internal void ToCreateCategoryPage() => ToCategoriesPage().NavigateToAddCategoryPage();
    }
}