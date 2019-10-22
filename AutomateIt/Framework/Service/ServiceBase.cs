using System;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.Service
{
    public abstract class ServiceBase : IService
    {
        public ServiceBase(BaseUrlInfo defaultBaseUrlInfo, BaseUrlPattern baseUrlPattern, Router router)
        {
            DefaultBaseUrlInfo = defaultBaseUrlInfo;
            BaseUrlPattern = baseUrlPattern;
            Router = router;
        }

        public IPage GetPage(RequestData requestData, BaseUrlInfo baseUrlInfo)
        {
            return Router.GetPage(requestData, baseUrlInfo);
        }

        public IPage GetEmailPage(Uri uri)
        {
            return Router.GetEmailPage(uri);
        }

        public RequestData GetRequestData(IPage page)
        {
            return Router.GetRequest(page, DefaultBaseUrlInfo);
        }

        #region Service Members

        public Router Router { get; }
        public BaseUrlPattern BaseUrlPattern { get; }
        public BaseUrlInfo DefaultBaseUrlInfo { get; }

        #endregion
    }
}
