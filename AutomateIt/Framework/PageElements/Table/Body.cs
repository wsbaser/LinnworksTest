using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table
{
	public class Body: ContainerBase
	{
		public Rows Rows { get; set; }
		
		public Body(IPage parent, string rootScss, string uniqueColumn)
			: base(parent, rootScss)
		{
			Rows = new Rows(parent, InnerScss("tbody"), uniqueColumn);
		}
	}
}
