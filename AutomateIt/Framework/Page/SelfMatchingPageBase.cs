using System;
using automateit.Framework.Page;
using selenium.core.Framework.Service;

namespace selenium.core.Framework.Page
{
	public interface ISelfMatchingPage
	{
		UriMatchResult Match(RequestData requestData, BaseUrlInfo baseUrlInfo);

		int Compare(object page);
	}

	public abstract class SelfMatchingPageBase : PageBase, ISelfMatchingPage
    {
        public abstract string AbsolutePath { get; }

        #region ISelfMatchingPage Members

        public virtual UriMatchResult Match(RequestData requestData, BaseUrlInfo baseUrlInfo)
        {
            return new UriMatcher(AbsolutePath, Data, Params).Match(requestData.Url, baseUrlInfo.AbsolutePath);
        }

		public int Compare(object page)
		{
			var selfMatchingPage = page as SelfMatchingPageBase;
			if (selfMatchingPage == null)
			{
				throw new ArgumentException("Invalid page type");
			}
			var compareResult =string.CompareOrdinal(AbsolutePath, selfMatchingPage.AbsolutePath);
			if (compareResult != 0)
			{
				return compareResult;
			}
			return Params.Count.CompareTo(selfMatchingPage.Params.Count);
		}

		#endregion

	public virtual RequestData GetRequest(BaseUrlInfo defaultBaseUrlInfo)
        {
            var url = new UriAssembler(BaseUrlInfo, AbsolutePath, Data, Params).Assemble(defaultBaseUrlInfo);
            return new RequestData(url);
        }
    }
}
