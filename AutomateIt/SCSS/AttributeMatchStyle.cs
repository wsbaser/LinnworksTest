using Natu.Utils.Extensions;

namespace selenium.core.SCSS
{
    internal enum AttributeMatchStyle
    {
        [StringValue("=")]
        Equal,

        [StringValue("~")]
        Contains
    }
}
