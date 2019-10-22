using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table2
{
	public class OneTypeWebHeaderRowBase<T> : OneTypeWebRow<T>
		where T : ContainerBase
	{
	    public override string ItemScss => ContainerInnerScss($"theader>tr[{ID}]");
		public override string CellScss => InnerScss(">th");

		public OneTypeWebHeaderRowBase(IContainer container, string id)
			: base(container, id)
		{
		}
	}
}