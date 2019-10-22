using System;

namespace selenium.core.Framework.PageElements
{
    public class WebComponentAttribute : Attribute, IComponentAttribute
    {
        public WebComponentAttribute()
        {
        }

        public WebComponentAttribute(params object[] args)
        {
            Args = args;
        }


        #region IComponentArgs Members

        public object[] Args { get; }
        public string ComponentName { get; set; }
        public string FrameScss { get; set; }

        public WaitCondition DefaultActionWaitCondition { get; set; }
        public int DefaultActionWaitTimeout { get; set; }

        #endregion
    }
}
