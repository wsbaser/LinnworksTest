using System;
using OpenQA.Selenium;
using selenium.core.Framework.PageElements;

namespace selenium.core.Framework.Page
{
    public interface IProgressBar : IComponent
    {
        void WaitWhileVisible();
    }

    public class WebLoader : ContainerBase, IProgressBar
    {

        public WebLoader(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        public override bool HasClass(string className)
        {
            throw new NotImplementedException();
        }

        public override bool IsDisabled()
        {
            throw new NotImplementedException();
        }

        public override T GetValue<T>()
        {
            throw new NotImplementedException();
        }

        public void WaitWhileVisible()
        {
            try
            {
                Wait.ForElementVisible(Selector);
                Wait.WhileElementVisible(Selector);
            }
            catch (WebDriverTimeoutException)
            {
                // Did not show up in 10 seconds and it is ok
                // TODO: need to think how to improve this code. need to get rid from exception handling.
            }
        }
    }
}