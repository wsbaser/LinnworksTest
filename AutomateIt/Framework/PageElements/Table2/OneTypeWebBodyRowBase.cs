using automateit.Framework.PageElements.Table2;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

public class OneTypeWebBodyRowBase<T> : OneTypeWebRow<T>
	where T : ContainerBase
{
    public override string ItemScss => ContainerInnerScss($"tbody>tr[{ID}]");
	public override string CellScss => InnerScss(">td");

	public OneTypeWebBodyRowBase(IContainer container, string id)
		: base(container, id)
	{
	}
}