using System;
using System.Collections.Generic;
using System.Linq;
using Natu.Utils.Extensions;
using NUnit.Framework;
using selenium.core.Framework.Page;

namespace automateit.Framework.PageElements
{
//    public class WebDropdown : DropdownBase
//    {
//        public override string ItemNameScss { get; }
//        public override string ToggleScss { get; }
//        public override string ItemScssTemplate { get; }
//
//        public WebDropdown(IPage parent, string rootScss, string itemScssTemplate)
//            : this(parent, rootScss, itemScssTemplate, null) {
//        }
//
//        public WebDropdown(IPage parent, string rootScss, string itemRelativeScssTemplate, string itemNameRelativeScss = null, string toggleRelativeScss = null)
//            : base(parent, rootScss) {
//            ItemScssTemplate = itemRelativeScssTemplate;
//            if (!string.IsNullOrEmpty(toggleRelativeScss)) {
//                ToggleScss = InnerScss(toggleRelativeScss);
//            }
//            if (!string.IsNullOrWhiteSpace(itemNameRelativeScss)) {
//                ItemNameScss = InnerScss(itemNameRelativeScss);
//            }
//        }
//
//        public override string GetItemScss(string option) => InnerScss(ItemScssTemplate, option);
//
//        // TODO(**):  generic class should not contain concrete selectors
//        public override string Text => Get.Text(InnerSelector("//span[@class='ng-binding ng-scope']"), FrameBy);
//
//        /// <summary>
//        ///     Содержит ли список указанное значение
//        /// </summary>
//
//        public override bool IsExpanded() {
//            throw new NotImplementedException();
//        }
//
//        //*****     ACTION     **********************************************************************************************************************
//        //*****     IS     **************************************************************************************************************************
//        // TODO(**): generic class should not contain concrete classes
//        public bool IdDropdownDisabled() => Get.Attr(RootSelector, FrameBy, "class").Contains("disabled");
//
//        //*****     GET     *************************************************************************************************************************
//        //*****     ASSERT     **********************************************************************************************************************
//        public void AssertWebDropdownIsDisabled() => Assert.IsTrue(IdDropdownDisabled(), $"Error: {ComponentName} is not disabled.");
//
//        public void AssertWebDropdownEnabled() => Assert.IsFalse(IdDropdownDisabled(), $"Error: {ComponentName} is disabled.");
//		public void AssertAttrubuteMatch(string value, string attribute) => Assert.AreEqual(value, Get.Attr(RootSelector, attribute), $"Inccorect value in {attribute} attribute.");
//
//        public void AssertOptionsListMatch(List<string> options) => CollectionAssert.AreEqual(options, GetItems(), "Options list is incorrect");
//        public void AssertOptionSelected(string dropdownSelectedOption) => Assert.AreEqual(dropdownSelectedOption, GetSelectedOption(), $"'{ComponentName}' has invalid selected option.");
//    }
//
//    public class WebDropdown<T> : WebDropdown {
//        public WebDropdown(IPage parent, string rootScss, string itemScss)
//            : base(parent, rootScss, itemScss) {
//            if (!typeof(T).IsEnum)
//                throw new ArgumentException("Invalid generic type. T should be an enum.");
//        }
//
//        public new List<T> GetItems() => Get.Texts(ItemNameScss).Select(t => (T)Enum.Parse(typeof(T), t)).ToList();
//
//        //*****     ACTION     **********************************************************************************************************************
//        public virtual void SelectOption(T option) {
//            var optionEnum = option as Enum;
//            SelectOption(optionEnum.StringValue());
//        }
//
//        //*****     IS     **************************************************************************************************************************
//        //*****     GET     *************************************************************************************************************************
//        //*****     ASSERT     **********************************************************************************************************************
//    }
}
