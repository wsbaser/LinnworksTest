using System;

namespace selenium.core.Framework.PageElements
{
    [Obsolete]
    public sealed class SimpleWebComponentAttribute : FindsByAttribute, IComponentAttribute
    {
        private string _componentName;

        public string ComponentName
        {
            get
            {
                return string.IsNullOrEmpty(_componentName) && !string.IsNullOrEmpty(LinkText)
                    ? LinkText
                    : _componentName;
            }
            set { _componentName = value; }
        }

        public string FrameScss { get; set; }

        #region IComponentArgs Members

        public object[] Args
        {
            get { return new object[] { Finder }; }
        }

        public WaitCondition DefaultActionWaitCondition { get; set; }
        public int DefaultActionWaitTimeout { get; set; }

        #endregion
    }
}
