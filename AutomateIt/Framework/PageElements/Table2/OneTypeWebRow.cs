using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table2
{
	public abstract class OneTypeWebRow<T> : WebRowBase<T> where T: ContainerBase
	{

		public abstract string CellScss{get;}

		public OneTypeWebRow(IContainer container, string id)
			: base(container, id)
		{
		}

		//****     ACTION     *********************************************************************************************************************
		//****     IS     *************************************************************************************************************************
		//****     GET     ************************************************************************************************************************
		public int GetCellsCount() => Get.CountOfElements(CellScss, FrameBy);
		public override List<T> GetItems()
		{
			var cellsList = new List<T>();
			var cellsCount = GetCellsCount();
			for (int i = 1; i <= cellsCount; i++)
			{
				cellsList.Add((T)WebPageBuilder.CreateComponent<T>(this, $"{CellScss}[{i}]"));
			}
			return cellsList;
		}

		public override List<string> GetTexts() => GetItems().Select(r => r.Text.Trim().Replace(Environment.NewLine, " ")).Where(x => !string.IsNullOrEmpty(x)).ToList();

		//****     ASSERT     *********************************************************************************************************************
		public void AssertTextsMatch(IList<string> texts) => CollectionAssert.AreEqual(texts, GetTexts());
	}
}