using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table
{
	public class Row : ItemBase
	{
		public Cells Cells { get; set; }
		public override string ItemScss => ContainerInnerScss($"tr[ [~'{ID}']]");

		public Row(IContainer container, string id)
			: base(container, id)
		{
			Cells = new Cells(ParentPage, InnerScss(""));
		}
	}
}
