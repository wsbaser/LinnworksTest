using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace automateit.Framework.Page
{
	public interface ISelectable
	{
		bool Select();
		bool IsSelected();
	}
}
