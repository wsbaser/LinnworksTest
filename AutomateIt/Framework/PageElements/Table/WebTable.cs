using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table
{
	public class WebTable : ContainerBase
	{
		public Header Header { get; set; }
		public Body Body { get; set; }

		public WebTable(IPage parent, string rootScss, string uniqueColumn, string headColumn, string columnScss)
			: base(parent, rootScss)
		{
			Header = new Header(parent, rootScss, headColumn, columnScss);
			Body = new Body(parent, rootScss, uniqueColumn);
		}

		public WebTable(IPage parent, string rootScss, string uniqueColumn)
			: base(parent, rootScss)
		{
			Header = new Header(parent, rootScss, "th", "th[~'{0}']");
			Body = new Body(parent, rootScss, uniqueColumn);
		}

		//****     ACTION     *********************************************************************************************************************
		public void CheckCheckboxInCell(string row, int cellNumber) => GetCell(row, cellNumber).Checkbox.CheckAndWaitWhileAjax();
		public void ClickLink(string row, int cellNumber) => GetCell(row, cellNumber).Link.ClickAndWaitWhileAjax();
		public void ClickRedirectLink(int row, int cellNumber) => GetCell(row, cellNumber).Link.ClickAndWaitForRedirect();
		public void CheckAll() => Header.CheckAll();
		public void UncheckAll() => Header.UncheckAll();
		public void ChechRandomItem() => GetRandomRow().Cells.GetItems()[0].Checkbox.Check();

		//****     GET     ************************************************************************************************************************
		public Cell GetCell(string row, int cellNumber) => Body.Rows.GetItem(row).Cells.GetItems()[cellNumber];
		public Cell GetCell(int rowNumber, int cellNumber) => Body.Rows.GetItems()[rowNumber].Cells.GetItems()[cellNumber];
		public Row GetRandomRow() => Body.Rows.RandomItem();

		//****     ASSERT     *********************************************************************************************************************
		public void AssetrColumnsMatch(string expectedColumns)
		{
			var columnsList = expectedColumns.Split(',').Select(c=>c.Trim()).ToList();
			AssetrColumnsMatch(columnsList);
		}
		public void AssetrColumnsMatch(List<string> expectedColumns) => CollectionAssert.AreEqual(expectedColumns, Header.GetColumnsNames());
		public void AssertCellValueMatch(string row, int cellNumber, string expectedValue) => GetCell(row, cellNumber).Text.AssertMatch(expectedValue);
	}
}
