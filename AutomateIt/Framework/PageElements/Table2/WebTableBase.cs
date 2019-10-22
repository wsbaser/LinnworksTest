using System.Collections.Generic;
using NUnit.Framework;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;
using selenium.core.SCSS;

namespace automateit.Framework.PageElements.Table2
{
    public abstract class WebTableBase<TRow> : ContainerBase
        where TRow : IWebRowBase<IContainer>
    {
        protected WebTableBase(IPage parent)
            : base(parent)
        {
        }

        protected WebTableBase(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        public override void AssertIsNotEmpty() => Assert.Greater(GetRowsCount(), 0, $"No rows in {ComponentName}");

        public string RowScss => InnerScss(">tbody>tr");
        public string RowInnerScss(string rowRelativeScss) => Scss.Concat(RowScss, rowRelativeScss);
        public int GetRowsCount() => Get.CountOfElements(RowScss, FrameBy);
        public abstract List<TRow> GetRows();
        public override bool IsEmpty() => GetRowsCount() == 0;
    }

    public abstract class WebTableBase<THeaderRow, TRow> : WebTableBase<TRow>
        where THeaderRow : IWebRowBase<IContainer>
        where TRow : IWebRowBase<IContainer>
    {
        protected virtual bool CacheRows => true;
        protected List<TRow> Rows;

        protected WebTableBase(IPage parent)
            : base(parent)
        {
        }

        protected WebTableBase(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        public string HeaderRowScss => InnerScss(">thead>tr");
        public string HeaderRowInnerScss(string headerRowRelativeScss) => Scss.Concat(HeaderRowScss, headerRowRelativeScss);

        public int GetHeaderRowsCount() => Get.CountOfElements(HeaderRowScss, FrameBy);
        public virtual List<THeaderRow> GetHeaderRows() => WebPageBuilder.CreateItems<THeaderRow>(this, GetHeaderRowsCount());
        public virtual THeaderRow GetHeaderRow(int index = 0) => WebPageBuilder.CreateComponent<THeaderRow>(this, (index + 1).ToString());
    }
}