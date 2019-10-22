using System;

namespace selenium.core.Framework.PageElements
{
    public enum WaitCondition
    {
        None,
        Sleep,
        Ajax,
        PageInProgress,
        Alert,
        Redirect,
        NewWindow
    }
}