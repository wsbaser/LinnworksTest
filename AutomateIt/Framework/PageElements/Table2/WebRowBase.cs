using System.Collections.Generic;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

namespace automateit.Framework.PageElements.Table2
{
	public abstract class WebRowBase<T>:ItemBase, IWebRowBase<T> where T: IContainer
	{
	    public override string ItemScss => ContainerInnerScss($"tr[{ID}]");
		public WebRowBase(IContainer container, string id)
			: base(container, id)
		{
		}

		//****     ACTION     *********************************************************************************************************************
		//****     IS     *************************************************************************************************************************
		//****     GET     ************************************************************************************************************************
		public abstract List<T> GetItems();
		public abstract List<string> GetTexts();
		//****     ASSERT     *********************************************************************************************************************we
    }
}