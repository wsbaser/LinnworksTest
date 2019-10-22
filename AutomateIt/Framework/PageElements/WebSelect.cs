using System;
using System.Collections.Generic;
using System.Linq;
using Natu.Utils.Extensions;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using selenium.core.Framework.Browser;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements
{
    /// <summary>
    /// TODO(***): Add possiblility to specify selection method: by text or by value.
    /// </summary>
    public class WebSelect : ContainerBase, IDropdown, IHasDefaultAction {
        public WaitCondition DefaultActionWaitCondition { get; set; }
        public int DefaultActionWaitTimeout { get; set; }

        public WebSelect(IPage parent, string rootScss)
            : base(parent, rootScss) {
        }

        private SelectElement _selectElement;

        protected SelectElement GetSelectElement() {
            if (_selectElement != null
                && !Is.Stale(_selectElement.WrappedElement)) {
                return _selectElement;
            }
            return _selectElement = new SelectElement(Find.Element(RootSelector, FrameBy));
        }

        //*****     ACTION     **********************************************************************************************************************
        /// <summary>
        ///     Select option if it not selected yet
        /// </summary>
        /// <param name="option">Option text</param>
        /// <returns>true if selection excecuted</returns>
        public bool SelectOption(string option) {
            if (!IsOptionSelected(option)) {
                Log.Action($"Select {option} in {ComponentName}.");
                Wait.Condition(
                    () => GetSelectElement().SelectByText(option),
                    DefaultActionWaitCondition,
                    DefaultActionWaitTimeout);
                return true;
            }
            return false;
        }

        public bool SelectOptionAndWaitWhileAjax(string option, int timeout = BrowserTimeouts.AJAX) {
            if (SelectOption(option)) {
                Wait.WhileAjax(timeout);
                return true;
            }
            return false;
        }

        public bool SelectOptionAndWaitWhileProgress(string option, int timeout = BrowserTimeouts.AJAX)
        {
            if (SelectOption(option))
            {
                Wait.WhilePageInProgress(timeout);
                return true;
            }
            return false;
        }

        public void SelectRandomOption() => SelectOption(GetRandomOption());

        //*****     IS     **************************************************************************************************************************
        public bool IsOptionSelected(string name) => GetSelectElement().AllSelectedOptions.Any(optionElement => optionElement.Text == name);
        public bool Contains(string option) => GetOptions().Contains(option);

        //*****     GET     *************************************************************************************************************************
        public List<string> GetOptions() => Get.Texts(InnerSelector("option")).ToList();

        public List<string> GetSelectedOptions() =>GetSelectElement().AllSelectedOptions.Select(o => o.Text).ToList();

        public string GetSelectedOption() => GetSelectedOptions().First();

        public string GetRandomOption() => GetOptions().RandomItem();

        //*****     ASSERT     **********************************************************************************************************************
        public void AssertIsDisabled() => Assert.True(Get.Attr<bool>(GetSelectElement().WrappedElement, "disabled"), $"'{ComponentName}' is not disabled.");
        public void AssertIsEnabled() => Assert.IsFalse(Get.Attr<bool>(GetSelectElement().WrappedElement, "disabled"), $"'{ComponentName}' is disabled.");
        public void AssertOptionsListMatch(List<string> optionsList) => CollectionAssert.AreEquivalent(optionsList, GetOptions());
        public void AssertOptionIsSelected(string name) => Assert.IsTrue(IsOptionSelected(name), $"'{name}' option is not selected");
        public void AssertContains(string optionName) => CollectionAssert.Contains(GetOptions(), optionName, $"{ComponentName} does not contain the list of expected options.");
        public void AssertContains(List<string> optionNames) => CollectionAssert.IsSupersetOf(GetOptions(), optionNames, $"{ComponentName} does not contain the list of expected options.");
        public void AssertContains(params string[] optionNames) => CollectionAssert.IsSupersetOf(GetOptions(), optionNames, $"{ComponentName} does not contain the list of expected options.");
        public void AssertOptionsAreEquivalent(List<string> optionNames) => CollectionAssert.AreEquivalent(optionNames, GetOptions(), $"{ComponentName} has invalid list of options.");

        public void AssertOptionSelected(string option) => Assert.IsTrue(IsOptionSelected(option), $"Option '{option}' is not selected in {ComponentName}");
    }

	public class WebSelect<T> : WebSelect
	{
		public WebSelect(IPage parent, string rootScss)
			: base(parent, rootScss)
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException("Invalid generic type. T should be an enum.");
		}

		public new List<T> GetOptions() => base.GetOptions().Select(t => (T)Enum.Parse(typeof(T), t)).ToList();

		public virtual bool SelectOption(T option)
		{
			var optionEnum = option as Enum;
			return SelectOption(optionEnum.StringValue());
		}

		public virtual bool SelectOptionAndWaitWhileAjax(T option)
		{
			var optionEnum = option as Enum;
			return SelectOptionAndWaitWhileAjax(optionEnum.StringValue());
		}

	    
	}
}
