using System;
using System.Collections.Generic;
using System.Linq;
using Natu.Utils.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements {
    public class WebTabs : SimpleWebComponent {
        public string ItemScss { get; }
        public string SelectedClass { get; }

        public WebTabs(IPage parent, string rootScss, string itemScss, string selectedClass)
            : base(parent, rootScss) {
            ItemScss = itemScss;
            SelectedClass = selectedClass;
        }

        public virtual bool SelectTab(string tabName) {
            if (!IsActive(tabName)) {
                Log.Action($"Select tab '{tabName}'");
                ClickTab(tabName);
                return true;
            }
            return false;
        }

        public virtual void ClickTab(string tabName) => Action.ClickAndWaitWhileProgress(TabSelector(tabName), FrameBy);

        public bool IsActive(string tabName) => Is.HasClass(TabSelector(tabName), FrameBy, SelectedClass);

        public bool IsVisible(string tabName) => Is.Visible(TabSelector(tabName), FrameBy);

        public By TabSelector(string tabName) => InnerSelector(ItemScss, tabName);

        public void SelectTabAndWaitWhileAjax(string tabName, int sleepTimeout = 0, bool ajaxInevitable = false) {
            if (!IsActive(tabName))
                Action.ClickAndWaitWhileAjax(TabSelector(tabName), FrameBy, sleepTimeout, ajaxInevitable);
        }

        public void AssertTabIsVisible(string tabName) {
            Wait.UntilSoftly(() => IsVisible(tabName), 10);
            Assert.IsTrue(IsVisible(tabName), "{0} '{1}' tab is not displayed", ComponentName, tabName);
        }

        public void AssertTabIsNotVisible(string tabName) => Assert.IsFalse(IsVisible(tabName), "{0} '{1}' tab is displayed", ComponentName, tabName);

        public void AssertTabIsActive(string tabName) => Assert.IsTrue(IsActive(tabName), "{0} '{1}' tab is not active", ComponentName, tabName);

        public int GetTabsCount() => Find.Elements(ItemScss).Count;

        public override string Text { get; }

        public string GetActiveTab() => GetTabs().FirstOrDefault(IsActive);

        public List<string> GetTabs() => Get.Texts(InnerSelector("li>a"), FrameBy);
    }

    public class WebTabs<T> : WebTabs where T: Enum{
        public WebTabs(IPage parent, string rootScss, string itemScss, string selectedClass)
            : base(parent, rootScss, itemScss, selectedClass) {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Invalid generic type. T should be an enum.");
        }

        //*****     ACTION     **********************************************************************************************************************
        public virtual bool SelectTab(T tab) {
            var optionEnum = tab as Enum;
            return SelectTab(optionEnum.StringValue());
        }

        //*****     IS     **************************************************************************************************************************
        public bool IsActive(T tab) => IsActive(GetTabName(tab));

        private string GetTabName(T tab) {
            var optionEnum = tab as Enum;
            return optionEnum.StringValue();
        }

        //*****     GET     *************************************************************************************************************************
        //*****     ASSERT     **********************************************************************************************************************
        public void AssertIsActive(T tab) {
            var tabName = GetTabName(tab);
            Assert.IsTrue(IsActive(tab), $"Tab {tabName} is not active.");
        }
    }

    public interface IHasTabs
    {
        WebTabs GetTabs();
        void SelectTab(string tab);
    }
}