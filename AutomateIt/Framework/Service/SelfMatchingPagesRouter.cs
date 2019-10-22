
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Internal;
using selenium.core.Framework.Page;
using selenium.core.Framework.Service;

//using WestKmFiveTests.Utils.Pages.WestKm.Setup.Vetting.VettingGroups;

namespace selenium.core.Framework.Service
{
    public class SelfMatchingPagesRouter : RouterBase
    {
        private readonly Dictionary<Type, ISelfMatchingPage> _pages;
        private readonly List<IEmailPage> _savedPages;

	    private List<ISelfMatchingPage> _sortedPages;

	    public SelfMatchingPagesRouter()
        {
            _pages = new Dictionary<Type, ISelfMatchingPage>();
            _savedPages = new List<IEmailPage>();
        }

		private void InvalidateSortedPages() => _sortedPages = null;

	    public List<ISelfMatchingPage> GetSortedPages()
	    {
		    if (_sortedPages == null)
		    {
				_sortedPages = _pages.Values.ToList();
				_sortedPages.Sort(ComparePages);
			    _sortedPages.Reverse();
		    }
		    return _sortedPages;
	    }

	    private int ComparePages(ISelfMatchingPage page1, ISelfMatchingPage page2) => page1.Compare(page2);

	    public override RequestData GetRequest(IPage page, BaseUrlInfo defaultBaseUrlInfo)
        {
            var selfMatchingPage = page as SelfMatchingPageBase;
            if (selfMatchingPage == null)
                return null;
            return selfMatchingPage.GetRequest(defaultBaseUrlInfo);
        }

        public override IPage GetPage(RequestData requestData, BaseUrlInfo baseUrlInfo)
        {
			foreach (var dummyPage in GetSortedPages())
            {
	            
	           var match = dummyPage.Match(requestData, baseUrlInfo);
                if (match.Success)
                {
                    var instance = (SelfMatchingPageBase)Activator.CreateInstance(dummyPage.GetType());
                    instance.BaseUrlInfo = baseUrlInfo;
                    instance.Data = match.Data;
                    instance.Params = match.Params;
                    instance.Cookies = match.Cookies;
                    return instance;
                }
            }
            return null;
        }

        public override IPage GetEmailPage(Uri uri)
        {
            return (IPage)_savedPages.FirstOrDefault(p => p.Match(uri));
        }

        public override bool HasPage(IPage page)
        {
            return _pages.ContainsKey(page.GetType());
        }

        public override IEnumerable<Type> GetAllPageTypes() => _pages.Keys;

        public void RegisterDerivedPages<T>() where T : SelfMatchingPageBase
        {
            var superType = typeof(T);
            var assembly = Assembly.GetAssembly(superType);
            var derivedTypes =
                assembly.GetTypes().AsEnumerable().Where(t => !t.IsAbstract && superType.IsAssignableFrom(t));
            foreach (var derivedType in derivedTypes)
                RegisterPage(derivedType);
        }

        public void RegisterPage<T>()
        {
            RegisterPage(typeof(T));
        }

        private void RegisterPage(Type pageType)
        {
            var pageInstance = (ISelfMatchingPage)Activator.CreateInstance(pageType);
            _pages.Add(pageType, pageInstance);
	        InvalidateSortedPages();
        }

	    public void RegisterEmailPage<T>() where T : IEmailPage
        {
            var pageInstance = (IEmailPage)Activator.CreateInstance(typeof(T));
            _savedPages.Add(pageInstance);
        }
    }
}


[TestFixture]
public class SelfMatchingRouterTests
{
	[Test]
	public void CompareTest()
	{
		// .Arrange
		var router = new SelfMatchingPagesRouter();

		// .Act
		router.RegisterPage<TestPage1>();
		router.RegisterPage<TestPage2>();

		// .Assert
		var sortedPages = router.GetSortedPages();

		Assert.AreEqual(typeof(TestPage2), sortedPages[0].GetType());
		Assert.AreEqual(typeof(TestPage1), sortedPages[1].GetType());
	}
}

public class TestPage1 : SelfMatchingPageBase
{
	public override string AbsolutePath => "/somepath";

	public TestPage1()
	{
		Params["param1"] = "value1";
	}
}
public class TestPage2 : SelfMatchingPageBase
{
	public override string AbsolutePath => "/somepath";
	public TestPage2()
	{
		Params["param1"] = "value1";
		Params["param2"] = "value2";
	}
}