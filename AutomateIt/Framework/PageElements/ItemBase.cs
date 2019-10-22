using NUnit.Framework;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.PageElements
{
    public abstract class ItemBase : ContainerBase, IItem
    {
        protected readonly IContainer Container;

        protected ItemBase(IContainer container, string id)
            : base(container.ParentPage)
        {
            Container = container;
            ID = id;
        }

        public override string RootScss => ItemScss;

        #region IItem Members

        public string ID { get; set; }
        public abstract string ItemScss { get; }

        #endregion

        protected string ContainerInnerScss(string relativeXpath, params object[] args) => Container.InnerScss(relativeXpath, args);

        public IItem Clone() => (IItem)MemberwiseClone();

        public override void AssertVisible() => Assert.IsTrue(IsVisible(), $"'{ComponentName}({ID})' is not displayed", ComponentName);
    }
}
