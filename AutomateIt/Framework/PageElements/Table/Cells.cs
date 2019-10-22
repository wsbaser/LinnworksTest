using System;
using System.Collections.Generic;
using NUnit.Framework;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table
{
	public class Cells : ListBase<Cell>
	{
		public Cells(IPage parent, string rootScss)
			: base(parent, rootScss)
		{
		}

		public override string ItemIdScss
		{
			get { throw new NotImplementedException(); }
		}
		public override List<string> GetIds()
		{
			var count = Get.CountOfElements(InnerScss("td"), FrameBy);
			var list = new List<string>();
			for (var i = 1; i < count + 1; i++)
			{
				list.Add(i.ToString());
			}
			return list;
		}
	}
}