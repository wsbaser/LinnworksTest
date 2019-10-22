using selenium.core.Framework.Page;

namespace selenium.core.Framework.Service
{
    public interface RequestAction
    {
        IPage getPage(RequestData requestData, BaseUrlInfo baseUrlInfo);
        RequestData getRequest(IPage page, BaseUrlInfo defaultBaseUrlInfo);
    }
}
