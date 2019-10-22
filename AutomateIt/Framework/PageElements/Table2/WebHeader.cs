using System;
using System.Collections.Generic;
using automateit.SCSS;
using NUnit.Framework;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table2
{
	public class WebHeader<T> : ListBase<T> where T : IWebRowBase<IContainer>
	{
		public string HeaderScss => InnerScss("tr");
		public override string ItemIdScss { get { throw new NotImplementedException(); } }

		public WebHeader(IPage parent)
			: base(parent)
		{
		}

		public WebHeader(IPage parent, string rootScss)
			: base(parent, rootScss)
		{
		}

		//****     ACTION     *********************************************************************************************************************
		//****     IS     *************************************************************************************************************************
		//****     GET     ************************************************************************************************************************
		public int GetRowsCount() => Get.CountOfElements(HeaderScss, FrameBy);
		public override List<T> GetItems()
		{
			var cellsList = new List<T>();
			var cellsCount = GetRowsCount();
			for (var i = 1; i <= cellsCount; i++)
			{
				cellsList.Add((T)WebPageBuilder.CreateComponent<T>(this, i.ToString()));
			}
			return cellsList;
		}

		//****     ASSERT     *********************************************************************************************************************
		public void AssertColumnsMatch(List<string> expectedColumns, int rowNumber = 0) => CollectionAssert.AreEqual(expectedColumns, GetItems<IWebRowBase<ContainerBase>>()[rowNumber].GetTexts(), "Table contains incorrect columns list");
	}
}