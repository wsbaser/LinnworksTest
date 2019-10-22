using System.Collections.Generic;
using selenium.core.Framework.Page;
using selenium.core.Framework.PageElements;

public class TextTypeBodyRow : OneTypeWebBodyRowBase<WebText>
{
	public TextTypeBodyRow(IContainer container, string id)
		: base(container, id)
	{
	}

	public override List<string> GetTexts()
	{
		throw new System.NotImplementedException();
	}
}