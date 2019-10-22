using Natu.Utils.Extensions;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table
{
	public class Rows: ListBase<Row>
	{
		public string UniqueColumn;
		public override string ItemIdScss => InnerScss($"{UniqueColumn}");

		public Rows(IPage parent, string rootScss, string uniqueColumn)
			: base(parent, rootScss)
		{
			UniqueColumn = uniqueColumn;
		}

		//****     GET     ************************************************************************************************************************
		public int GetRandomRowNumber()
		{
			var rows = GetItems();
			var randomRow = rows.RandomItem();
			return rows.FindIndex(r => r == randomRow);
		}
	}
}