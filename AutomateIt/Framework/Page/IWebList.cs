using System.Collections.Generic;

namespace selenium.core.Framework.Page
{
    public interface IWebList<T> : IWebList
        where T : IItem {
        List<T> GetItems();
        T GetItem(string id);
    }

    public interface IWebList : IContainer {
        string ItemIdScss { get; }
        List<string> GetIds();

        List<T> GetItems<T>()
            where T : IItem;

        T GetItem<T>(string id)
            where T : IItem;

        int GetCount();
    }

    public interface IContainer : IComponent
    {
        string InnerScss(string relativeScss, params object[] args);
    }

    public interface IDropdown
    {
        List<string> GetOptions();
        bool SelectOption(string name);
        bool IsOptionSelected(string name);
        void AssertContains(string optionName);
        void AssertContains(params string[] optionNames);
        void AssertOptionsAreEquivalent(List<string> optionNames);
        void AssertOptionSelected(string option);
    }
}
