using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table2
{
	public class HeaderRowWithCheckbox : WebRowBase<WebLink>
	{
		[WebComponent("root: th[1]>input")]
		public WebCheckbox SelectAllCheckbox;
		public HeaderRowWithCheckbox(IContainer container, string id)
			: base(container, id)
		{
		}
		public string CellScss => InnerScss(">th[>a]");

		//****     ACTION     *********************************************************************************************************************
		//****     IS     *************************************************************************************************************************
		//****     GET     ************************************************************************************************************************
		public int GetCellsCount() => Get.CountOfElements(CellScss, FrameBy);
		public override List<WebLink> GetItems()
		{
			var cellsList = new List<WebLink>();
			var cellsCount = GetCellsCount();
			for (int i = 1; i <= cellsCount; i++)
			{
				cellsList.Add(WebPageBuilder.CreateComponent<WebLink>(this, $"{CellScss}[{i}]"));
			}
			return cellsList;
		}
		public override List<string> GetTexts() => GetItems().Select(r => r.Text.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();

		//****     ASSERT     *********************************************************************************************************************
		
		public void AssertColumnNamesAre(IList<string> columns) => CollectionAssert.AreEqual(columns, GetTexts(), "Invalid column names");
	}
}
