using System.Collections.Generic;
using automateit.SCSS;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;
using selenium.core.SCSS;

namespace automateit.Framework.PageElements.Table2 {
    public abstract class WebTable2<TBodyRow> : ContainerBase
        where TBodyRow : IWebRowBase<IContainer> {
        public WebBody<TBodyRow> Body { get; }

        public WebTable2(IPage parent)
            : base(parent) {

        }

        public WebTable2(IPage parent, string rootScss)
            : base(parent, rootScss) {
            Body = new WebBody<TBodyRow>(parent, $"{rootScss}>tbody");
        }
    }

    public class WebTable2<THeaderRow, TBodyRow> : WebTable2<TBodyRow>
        where THeaderRow : IWebRowBase<IContainer>
        where TBodyRow : IWebRowBase<IContainer> {
        public WebHeader<THeaderRow> Header { get; }

        public WebTable2(IPage parent, string rootScss)
            : base(parent, rootScss) {
            Header = new WebHeader<THeaderRow>(parent, $"{rootScss}>thead");
        }

        //****     ACTION     *********************************************************************************************************************
        //****     IS     *************************************************************************************************************************
        //****     GET     ************************************************************************************************************************
        //****     ASSERT     *********************************************************************************************************************
        public virtual void AssertColumnsMatch(List<string> columns, int rowNumber = 0) => Header.AssertColumnsMatch(columns, rowNumber);
    }

    public abstract class WebTable2Id<TBodyRow> : ContainerBase
        where TBodyRow : IWebRowBase<IContainer> {
        public WebTableBody<TBodyRow> Body { get; }

        protected WebTable2Id(IPage parent, string rootScss, string idScss)
            : base(parent, rootScss)
        {
            Body = new WebTableBody<TBodyRow>(parent, $"{rootScss}>tbody", idScss);
        }
    }

    public abstract class WebTable2Id<THeaderRow,TBodyRow> : WebTable2Id<TBodyRow>
        where TBodyRow : IWebRowBase<IContainer>
        where THeaderRow : IWebRowBase<IContainer> {
        public WebHeader<THeaderRow> Header { get; }

        public WebTable2Id(IPage parent, string rootScss, string idScss)
            : base(parent, rootScss, idScss) {
            Header = new WebHeader<THeaderRow>(parent, $"{rootScss}>thead");
        }

        //****     ACTION     *********************************************************************************************************************
        //****     IS     *************************************************************************************************************************
        //****     GET     ************************************************************************************************************************
        //****     ASSERT     *********************************************************************************************************************
    }
}