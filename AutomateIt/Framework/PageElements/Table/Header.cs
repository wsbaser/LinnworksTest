using System.Collections.Generic;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table
{
	public class Header: ContainerBase
	{
		//[WebComponent("root: th[1]>input", ComponentName = "Check All")]
		public WebCheckbox CheckAllCheckbox;
		public Columns  Columns { get; set; }
		public string ColumnName { get; set; }
		public Header(IPage parent)
			: base(parent)
		{
		}

		public Header(IPage parent, string rootScss, string headColumn, string columnScss)
			: base(parent, rootScss)
		{
			Columns = new Columns(parent, InnerScss("thead"), headColumn, columnScss);
			CheckAllCheckbox = new WebCheckbox(parent, InnerScss("th[1]>input"));
		}

		//****     ACTION     *********************************************************************************************************************
		public void CheckAll()
		{
			if (!CheckAllCheckbox.Checked())
			{
				CheckAllCheckbox.Check();
			}
		}

		public void UncheckAll()
		{
			if (CheckAllCheckbox.Checked())
			{
				CheckAllCheckbox.Check();
			}
		}

		//****     IS     *************************************************************************************************************************
		//****     GET     ************************************************************************************************************************
		public List<string> GetColumnsNames() => Columns.GetTexts();

		//****     ASSERT     *********************************************************************************************************************
	}
}
