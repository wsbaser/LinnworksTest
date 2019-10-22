using System;
using automateit.SCSS;
using NUnit.Framework;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public class WebCheckbox : ContainerBase, IWebCheckbox
    {
        protected string LabelScss;

        public Selector LabelSelector => new Selector(LabelScss, FrameScss);


        public WebCheckbox(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        public WebCheckbox(IPage parent, string rootScss, string labelScss)
            : base(parent, rootScss)
        {
            LabelScss = labelScss;
        }

        public override void Click(int sleepTimeout = 0)
        {
            if (!string.IsNullOrWhiteSpace(LabelScss))
            {
                Action.Click(LabelSelector);
            }
            else
            {
                Action.Click(Selector);
            }
        }

        public virtual bool Check()
        {
            if (!Checked())
            {
                Log.Action($"Check '{ComponentName}' checkbox ");
                Click();
                return true;
            }
            return false;
        }

        public void CheckAndWaitWhileAjax()
        {
            if (!Checked())
            {
                Log.Action($"Check '{ComponentName}' checkbox ");
                Click();
                Wait.WhileAjax(ajaxInevitable: true);
            }
        }

        public virtual bool Uncheck()
        {
            if (Checked())
            {
                Log.Action($"Uncheck '{ComponentName}' checkbox");
                Click();
                return true;
            }
            return false;
        }

        public void UncheckAndWaitWhileAjax()
        {
            if (Checked())
            {
                Log.Action($"Uncheck '{ComponentName}' checkbox ");
                Click();
                Wait.WhileAjax(ajaxInevitable: true);
            }
        }

        public virtual bool Checked() => Is.Checked(Selector);

        public void AssertIsUnchecked() => Assert.IsFalse(Checked(), "Checkbox is checked");

        public void AssertIsChecked() => Assert.IsTrue(Checked(), "Checkbox is unchecked");

        public bool Disabled() => Is.Disabled(RootSelectorNew);

        public void AssertIsEnabled() => Assert.IsFalse(Disabled(), "Checkbox is disabled");

        public void SetValue(string fieldValue)
        {
            bool state;
            bool.TryParse(fieldValue, out state);
            if (state)
            {
                Check();
            }
            else
            {
                Uncheck();
            }
        }

        public override string GetValue() => Checked().ToString();

        public override void AssertEqual(string expected, bool ignoreRegister = false)
        {
            bool isCheckedExpected;
            switch (expected.Trim().ToLower())
            {
                case "1":
                case "true":
                    isCheckedExpected = true;
                    break;
                case "0":
                case "false":
                    isCheckedExpected = false;
                    break;
                default:
                    throw new ArgumentException(expected);
            }

            Assert.AreEqual(isCheckedExpected, Checked(), $"Checkbox '{ComponentName}' has invalid state.");
        }
    }

    public interface IWebCheckbox
    {
        bool Check();
        bool Uncheck();
        bool Checked();
    }
}
