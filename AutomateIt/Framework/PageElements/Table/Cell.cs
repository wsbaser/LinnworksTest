using selenium.core.Framework.PageElements;
using IContainer = selenium.core.Framework.Page.IContainer;

namespace automateit.Framework.PageElements.Table
{
	public class Cell : ItemBase
	{
		[WebComponent("root: a")]
		public WebLink Link;

		[WebComponent("root: input")]
		public WebCheckbox Checkbox;

		[WebComponent("root: textarea")]
		public WebInput Input;

		[WebComponent("root:")]
		public WebText Text;

		[WebComponent("root: select")]
		public WebSelect Dropdown;
		public override string ItemScss => ContainerInnerScss($"td[{ID}]");

		public Cell(IContainer container, string id)
			: base(container, id)
		{
			
		}
	}
}
