using System.Collections.Generic;
using System.Linq;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table
{
	public class Columns:ListBase<Column>
	{
		public string HeadScss { get; set; }
		public string ColumnScss { get; set; }
		public override string ItemIdScss => InnerScss(HeadScss != null ? $"{HeadScss}" : "th");

		public Columns(IPage parent, string rootScss, string headColumn, string columnScss)
			: base(parent, rootScss)
		{
			HeadScss = headColumn;
			ColumnScss = columnScss;
		}

		//****     GET     ************************************************************************************************************************
		public override List<Column> GetItems()
		{
			var columns = new List<Column>();
			columns = ColumnScss == null ? base.GetItems() : GetIds().Where(id => !id.Contains("'")).Select(id => (Column)WebPageBuilder.CreateComponent<Column>(this, id, ColumnScss)).ToList();
			return columns;
		}

		public List<string> GetTexts() => GetItems().Select(c => c.Text.Trim()).ToList();
	}
}