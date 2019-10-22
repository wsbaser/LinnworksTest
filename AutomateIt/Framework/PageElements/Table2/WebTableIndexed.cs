using System;
using System.Collections.Generic;
using selenium.core.Framework.Page;

namespace automateit.Framework.PageElements.Table2
{
    public class WebTableIndexed<TRow> : WebTableBase<TRow>
        where TRow : IWebRowBase<IContainer>
    {
        public WebTableIndexed(IPage parent)
            : base(parent)
        {
        }

        public WebTableIndexed(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        public override List<TRow> GetRows() => WebPageBuilder.CreateItems<TRow>(this, GetRowsCount());
        public TRow GetRow(int index) => WebPageBuilder.CreateComponent<TRow>(this, index.ToString());
        public TRow GetRandomRow() => GetRow(new Random().Next(GetRowsCount()));
    }

    public class WebTableIndexed<THeaderRow, TRow> : WebTableBase<THeaderRow, TRow>
        where THeaderRow : IWebRowBase<IContainer>
        where TRow : IWebRowBase<IContainer>
    {
        public WebTableIndexed(IPage parent)
            : base(parent)
        {
        }

        public WebTableIndexed(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        public override List<TRow> GetRows() => WebPageBuilder.CreateItems<TRow>(this, GetRowsCount());

        public TRow GetRow(int index) => WebPageBuilder.CreateComponent<TRow>(this, index.ToString());

        public TRow GetRandomRow() => GetRow(new Random().Next(GetRowsCount()));
    }
}