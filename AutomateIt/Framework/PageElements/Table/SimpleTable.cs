using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using automateit.Framework.PageElements.Table2;
using selenium.core.Framework.Page;

namespace automateit.Framework.PageElements.Table
{
    public class SimpleTable<TRow> : WebTableIndexed<TRow>
        where TRow : IWebRowBase<IContainer>
    {
        public SimpleTable(IPage parent)
            : base(parent)
        {
        }

        public SimpleTable(IPage parent, string rootScss)
            : base(parent, rootScss)
        {
        }
    }
}
