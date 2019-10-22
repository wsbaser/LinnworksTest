using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using selenium.core.Framework.Page;

namespace automateit.Framework.PageElements.Table2
{
	public interface IWebRowBase<out T> : IItem, IContainer
		where T : IContainer
	{
		List<string> GetTexts();
	}
}