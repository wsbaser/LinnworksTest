using System;
using System.Collections.Generic;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.Service
{
    public interface Router
    {
        RequestData GetRequest(IPage page, BaseUrlInfo defaultBaseUrlInfo);
        IPage GetPage(RequestData requestData, BaseUrlInfo baseUrlInfo);
        IPage GetEmailPage(Uri uri);
        bool HasPage(IPage page);
        IEnumerable<Type> GetAllPageTypes();
    }
}
