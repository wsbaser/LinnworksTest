using LinnworksTest.Features.services.linnworks.Pages;
using LinnworksTest.Features.steps.@base;
using Natu.Utils.Extensions;
using NUnit.Framework;
using System;
using TechTalk.SpecFlow;

namespace LinnworksTest.Features.steps.givens
{
    [Binding]
    public class NavigationGivens : LinnworksStepsBase
    {
        [Given(@"I am on the '(.*)' page")]
        public void GivenIAmOnThePage(string pageName)
        {
            switch (pageName)
            {
                case "Home":
                    Navigate.ToHomePage();
                    break;
                case "Categories":
                    Navigate.ToCategoriesPage();
                    break;
                case "Create Category":
                    Navigate.ToCreateCategoryPage();
                    break;
            }
        }

        [Then(@"I am on the '(.*)' page")]
        public void ThenIAmOnThePage(string pageName)
        {
            switch (pageName)
            {
                case "Home":
                    Assert.IsTrue(State.PageIs<HomePage>());
                    break;
                case "Categories":
                    Assert.IsTrue(State.PageIs<CategoriesPage>());
                    break;
            }
        }

    }
}