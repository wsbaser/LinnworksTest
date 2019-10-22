using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table
{
	public class Column : ItemBase
	{
		[WebComponent("root:>a")]
		public WebLink Link;

		[WebComponent("root:>input")]
		public WebCheckbox CheckBox;
		public string Scss { get; set; }
		public override string ItemScss => ContainerInnerScss(Scss);
		public Column(IContainer container, string id)
			: base(container, id)
		{
			Scss = $"th['{ID}']";
		}
		public Column(IContainer container, string id, string scss)
			: base(container, id)
		{
			Scss = string.Format(scss, id);
		}
	}
}
