using automateit.Framework.PageElements.Table2;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

public class TextTypeHeaderRow : OneTypeWebHeaderRowBase<WebText>
{
	public TextTypeHeaderRow(IContainer container, string id)
		: base(container, id)
	{
	}
	
}