using NUnit.Framework;
using OpenQA.Selenium;
using selenium.core.Framework.Browser;
using selenium.core.Framework.Page;
using selenium.core.TestData;

namespace selenium.core.Framework.PageElements
{
    public class WebInput : SimpleWebComponent
    {
        public WebInput(IPage parent, By by)
            : base(parent, by)
        {
        }

        public string Placeholder => Get.Attr(By, FrameBy, "placeholder");

        public void AssertAtributeEqual(string attribute, string value) => Assert.AreEqual(Get.Attr(By, FrameBy, attribute), value, $"{attribute} is wrong");

        public void AssertAtributeNotEqual(string attribute, string value) => Assert.AreNotEqual(Get.Attr(By, FrameBy, attribute), value, $"{attribute} is wrong");

        public WebInput(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        public override string Text => Get.InputValue(By, FrameBy);

        public override T GetValue<T>() => Get.InputValue<T>(RootSelector, FrameBy);

        public virtual WebInput TypeIn(object value, bool clear = true)
        {
            Log.Action($"Input '{value}' in '{ComponentName}' field");
            Log.WriteValue(ComponentName, value.ToString());
            Action.TypeIn(By, FrameBy, value, clear: clear);
            return this;
        }

        public WebInput TypeInAndWaitWhileAjax(object value, bool clear = true)
        {
            Log.Action($"Input '{value}' in '{ComponentName}' field");
            Action.TypeInAndWaitWhileAjax(By, FrameBy, value, clear);
            return this;
        }

        public void AssertIsEmpty() => Assert.IsTrue(IsEmpty(), "Field '{0}' is not empty", ComponentName);

        public void AssertPlaceholderEqual(string expected) => Assert.AreEqual(expected, Placeholder, $"Invalid placeholder in text field {ComponentName}");

        public bool IsEmpty() => string.IsNullOrWhiteSpace(Text);

        public bool IsNotEmpty() => !string.IsNullOrWhiteSpace(Text);

        public override void AssertIsNotEmpty() => Assert.IsFalse(string.IsNullOrWhiteSpace(Text), $"Field '{ComponentName}' is empty");

        public void AssertHasPositiveIntegerValueOrEmpty()
        {
            if (string.IsNullOrEmpty(Text))
            {
                AssertIsEmpty();
            }
            else
            {
                AssertHasPositiveIntegerValue();
            }
        }

	    public string TypeInRandomNumber(int length = 10)
        {
            var random = RandomDataHelper.Cifers(length);
            TypeIn(random);
            return random;
        }

        public void RemoveLast(bool changeFocus = false)
        {
            Action.PressKey(By, FrameBy, Keys.End);
            Action.PressKey(By, FrameBy, Keys.Backspace);
            if (changeFocus)
                Action.ChangeFocus();
        }

        public virtual void Clear() => Action.Clear(By, FrameBy);

        public void TypeInAndSubmit(string query)
        {
            TypeIn(query);
            Action.PressEnter(By, FrameBy);
            Wait.WhileAjax();
        }

        public T SubmitAndWaitForRedirect<T>(bool waitForAjax = false, bool ajaxInevitable = false) where T:class,IPage {
            SubmitAndWaitForRedirect(waitForAjax,ajaxInevitable);
            return State.PageAs<T>();
        }

        public void SubmitAndWaitForRedirect(bool waitForAjax = false, bool ajaxInevitable = false) {
            Log.Action($"Press enter in '{ComponentName}'");
            var oldUrl = Browser.Window.Url;
            Action.PressEnter(By, FrameBy);
            Browser.Wait.ForRedirect(oldUrl, waitForAjax, ajaxInevitable);
        }

        public void AssertHasPositiveIntegerValue(){
            int intValue;
            Assert.IsTrue(int.TryParse(Text, out intValue), $"Field '{ComponentName}' has not integer value");
            Assert.IsTrue(intValue > 0, $"Field '{ComponentName}' has not positive value");
        }

        private bool IsValidEmail(string email){
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }

        public void SubmitAndWaitWhileAjax(bool ajaxInevitable = false)
        {
            Log.Action($"Press enter in '{ComponentName}'");
            Action.PressEnter(By, FrameBy);
            Browser.Wait.WhileAjax(BrowserTimeouts.AJAX, ajaxInevitable);
        }

        public void Submit() {
            Log.Action($"Press enter in '{ComponentName}'");
            Action.PressEnter(By, FrameBy);
        }

		public void Click()
		{
			Log.Action($"Click in '{ComponentName}'");
			Action.ClickAndWaitWhileAjax(By, FrameBy);
		}
	}
}
