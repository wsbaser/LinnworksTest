using System;
using System.Collections.Generic;
using automateit.SCSS;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;
using selenium.core.SCSS;

namespace automateit.Framework.PageElements.Table2
{
	public class WebBody<T> : ListBase<T> where T:IWebRowBase<IContainer>
	{
		public string RowScss => InnerScss("tr");
		public override string ItemIdScss{ get { throw new NotImplementedException(); }}
		public WebBody(IPage parent)
			: base(parent)
		{
		}

		public WebBody(IPage parent, string rootScss)
			: base(parent, rootScss)
		{
			
		}

		//****     ACTION     *********************************************************************************************************************
		//****     IS     *************************************************************************************************************************
		//****     GET     ************************************************************************************************************************
		public int GetRowsCount() => Get.CountOfElements(RowScss, FrameBy);

	    public T GetRandomItem() => GetItem(new Random().Next(GetRowsCount()).ToString());

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
	}

    public class WebTableBody<T> : ListBase<T>
        where T : IWebRowBase<IContainer> {
        private readonly string _idRelativeScss;

        public WebTableBody(IPage parent, string rootScss = null, string idRelativeScss = null)
            : base(parent, rootScss) {
            if (string.IsNullOrWhiteSpace(idRelativeScss)) {
                throw new ArgumentException("Invalid value for idRelativeScss.");
            }
            _idRelativeScss = idRelativeScss;
        }

        public override string ItemIdScss => InnerScss(_idRelativeScss);

        //****     ACTION     *********************************************************************************************************************
        //****     IS     *************************************************************************************************************************
        //****     GET     ************************************************************************************************************************
        //****     ASSERT     *********************************************************************************************************************
    }
}