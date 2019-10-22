using System;
using System.Collections.Generic;
using System.Linq;
using Natu.Utils.Extensions;
using NUnit.Framework;
using selenium.core.Framework.Page;

namespace automateit.Framework.PageElements.Table2 {
    public abstract class WebTableWithIdBase<TRow> : WebTableBase<TRow>
        where TRow : class, IWebRowBase<IContainer> {
        protected WebTableWithIdBase(IPage parent)
            : base(parent) {
        }

        protected WebTableWithIdBase(IPage parent, string rootScss)
            : base(parent, rootScss) {
        }

        public abstract string RowIdScss { get; }

        public virtual List<string> GetRowIds() => Get.Texts(RowIdScss, FrameBy).Select(t => t.Trim()).ToList();
        public override List<TRow> GetRows() => WebPageBuilder.CreateItems<TRow>(this, GetRowIds());
        public TRow GetRow(string id) => WebPageBuilder.CreateComponent<TRow>(this, id);

        public TRow GetRandomRow() => GetRow(GetRowIds().RandomItem());

        public TRow FindRow(Func<TRow, bool> filterFunc)
        {
            var rowIds = this.GetRowIds();
            foreach (var rowId in rowIds)
            {
                var row = GetRow(rowId);
                if (filterFunc.Invoke(row))
                {
                    return row;
                }
            }

            return null;
        }
    }

    public abstract class WebTableWithIdBase<THeaderRow, TRow> : WebTableBase<THeaderRow, TRow>
        where THeaderRow : IWebRowBase<IContainer>
        where TRow : IWebRowBase<IContainer>
    {
        protected WebTableWithIdBase(IPage parent)
            : base(parent)
        {
        }

        protected WebTableWithIdBase(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }

        public abstract string RowIdScss { get; }

        public virtual List<string> GetRowIds() => Get.Texts(RowIdScss, FrameBy).Select(t => t.Trim()).ToList();

        public override List<TRow> GetRows()
        {
            if (Rows == null
                || !CacheRows)
            {
                Rows = WebPageBuilder.CreateItems<TRow>(this, GetRowIds());
            }

            return Rows;
        }

        public TRow GetRow(string id) => WebPageBuilder.CreateComponent<TRow>(this, id);
        public THeaderRow HeaderRow => GetHeaderRow(0);

        public void AssertColumnsMatch(List<string> columns, int headerRowNumber = 0) => CollectionAssert.AreEqual(
            columns, GetHeaderRow(headerRowNumber).GetTexts(),
            "Columns list is incorrect");

        public void AssertRowsIdsMatch(List<string> rows) => CollectionAssert.AreEquivalent(
            rows, GetRowIds(),
            "Rows ids list is incorrect");

        public TRow GetRandomRow() => GetRow(GetRowIds().RandomItem());
    }
}