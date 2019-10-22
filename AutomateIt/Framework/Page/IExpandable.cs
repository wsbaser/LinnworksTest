namespace automateit.Framework.Page {
    public interface IExpandable
    {
        bool Expand();
        bool IsExpanded();
        void AssertIsExpanded();
        void AssertIsCollapsed();
    }
}