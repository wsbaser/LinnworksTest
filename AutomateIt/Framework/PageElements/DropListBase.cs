using System;
using System.Collections.Generic;
using System.Linq;
using automateit.Framework.Page;
using Natu.Utils.Exceptions;
using Natu.Utils.Extensions;
using NUnit.Framework;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements {
    public abstract class DropdownBase<TList, TToggle> : ContainerBase, IExpandable,IOverlay, IDropdown where TList : IWebList where TToggle : IContainer
    {
        public abstract TToggle Toggle { get; set; }
        public abstract TList Menu { get; set; }

        protected DropdownBase(IPage parent, string rootScss)
            : base(parent, rootScss) {
        }

        //*****     ACTION     **********************************************************************************************************************
        public virtual void SelectRandomOption()
        {
            Expand();
            var options = Menu.GetIds().ToList();
            if (options.Count == 0)
            {
                throw Throw.FrameworkException(
                    $"Dropdown {ComponentName} is empty and does not have an option to select.");
            }

            Menu.GetItem<IItem>(options.RandomItem()).Click();
        }

        /// <summary>
        ///     Select option if it not selected yet
        /// </summary>
        /// <param name="name">Option text</param>
        /// <returns>true if selection performed</returns>
        public virtual bool SelectOption(string name) {
            Expand();
            var item = Menu.GetItem<IItem>(name);
            item.Click();
            return true;
        }

        public bool SelectOptionAndWaitWhileAjax(string name)
        {
            SelectOption(name);
            Wait.WhileAjax(ajaxInevitable:true);
            return true;
        }

        public virtual bool Expand() {
            if (!IsExpanded()) {
                Log.Action($"Expand {ComponentName}");
                Toggle.Click();
                Wait.Until(Menu.IsVisible); 
                return true;
            }
            return false;
        }

        public virtual bool Collapse() {
            if (IsExpanded()) {
                Toggle.Click();
                Wait.Until(()=>!Menu.IsVisible());
                return true;
            }
            return false;
        }

        //*****     IS     **************************************************************************************************************************
        public virtual bool IsExpanded() => Menu.IsVisible();

        public bool HasSelectedOption() => !string.IsNullOrWhiteSpace(Toggle.Text);

        public bool IsOptionSelected(string name) => Toggle.Text.Trim().Equals(name, StringComparison.OrdinalIgnoreCase);

        public virtual bool Contains(string option) => GetOptions().Contains(option);

        //*****     GET     *************************************************************************************************************************

        public virtual List<string> GetOptions() {
            var expanded = Expand();
            var ids = Menu.GetIds();
            if (expanded) {
                Toggle.Click();
            }
            return ids;
        }

        public virtual string GetSelectedOption() => Toggle.GetValue();

        public override string GetValue() => GetSelectedOption();

        //*****     ASSERT     **********************************************************************************************************************
        public virtual void AssertIsExpanded() => Assert.IsTrue(IsExpanded(), $"{ComponentName} is not expanded.");
        public virtual void AssertIsCollapsed() => Assert.IsFalse(IsExpanded(), $"{ComponentName} is not collapsed.");

        public virtual void AssertNotExpanded() => Assert.IsFalse(IsExpanded(), $"{ComponentName} is expanded.");

        public virtual void AssertContains(string option) => CollectionAssert.Contains(GetOptions(), option, $"{ComponentName} does not contain option '{option}'");

        public virtual void AssertContains(params string[] optionNames) => CollectionAssert.IsSupersetOf(GetOptions(), optionNames, $"{ComponentName} does not contain the list of expected options.");

        public void AssertDoesNotContain(params string[] expectedOptions) => CollectionAssert.IsNotSubsetOf(expectedOptions, GetOptions(), $"List of options in {ComponentName} doesn't contain expected options.");

        public void AssertOptionsAreEquivalent(List<string> optionNames) => CollectionAssert.AreEquivalent(GetOptions(),optionNames, $"{ComponentName} has invalid list of options.");

        public void AssertOptionSelected(string option) => Assert.IsTrue(IsOptionSelected(option), $"Option '{option}' is not selected in {ComponentName}");

        public virtual void AssertDoesNotContain(string option) => CollectionAssert.DoesNotContain(GetOptions(), option, $"{ComponentName} contains invalid option '{option}'");

        public virtual void AssertOptionIsSelected(string name) => Assert.IsTrue(IsOptionSelected(name), $"'{name}' option is not selected");

        #region Interface IOverlay
        public bool IsMatch(Exception e) {
            throw new NotImplementedException();
        }

        public void Close() => Collapse();

        public bool IsOpened() => IsExpanded();

        #endregion

    }
}