using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table2
{
	public abstract class WebRows<T>: ListBase<WebRowBase<T>> where T: ContainerBase
	{
		public override string ItemIdScss { get; }
		public WebRows(IPage parent, string rootScss = null)
			: base(parent, rootScss)
		{
		}
	}
}