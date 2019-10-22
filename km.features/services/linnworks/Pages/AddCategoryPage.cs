using System;
using OpenQA.Selenium;
using selenium.core.Framework.PageElements;

namespace LinnworksTest.Features.services.linnworks.Pages
{
    public class AddCategoryPage : LinnworksPageBase
    {
        public override string AbsolutePath => "/add-category";

        [WebComponent("span.text-danger")]
        public WebText NameValidationError;

        [WebComponent("input[formcontrolname='categoryName']")]
        public WebInput NameTextbox;

        [WebComponent("button['Save']")]
        public WebButton SaveButton;

        [WebComponent("button['Cancel']",DefaultActionWaitCondition =WaitCondition.Redirect,DefaultActionWaitTimeout =30)]
        public WebButton CancelButton;

        internal void FillAndSave(string categoryName)
        {
            NameTextbox.TypeIn(categoryName);
            Save();
        }

        internal void Save()
        {
            Action.TypeIn(NameTextbox.Selector, Keys.Enter, false);
            Wait.Until(() =>
            {
                State.Actualize();
                return !State.PageIs<AddCategoryPage>() || NameTextbox.IsVisible();
            }, 10);
        }
    }
}