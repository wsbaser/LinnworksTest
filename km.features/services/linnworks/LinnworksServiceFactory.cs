using System.Collections.Generic;
using LinnworksTest.Features.services;
using LinnworksTest.Features.services.linnworks;
using LinnworksTest.Features.services.linnworks.Pages;
using selenium.core.Framework.Service;

namespace LinnworksTest.Features.services.linnworks
{
    public class LinnworksServiceFactory : ServiceFactory
    {
        private readonly string _defaultServer;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public LinnworksServiceFactory(string defaultServer)
        {
            _defaultServer = defaultServer;
        }

        public Router CreateRouter()
        {
            var router = new SelfMatchingPagesRouter();
            router.RegisterDerivedPages<LinnworksPageBase>();
            return router;
        }

        public IService CreateService()
        {
            return new LinnworksService(
                LinnworksSeleniumContext.Inst,
                GetDefaultBaseUrlInfo(_defaultServer),
                CreateBaseUrlPattern(new List<string> { _defaultServer }),
                CreateRouter());
        }

        public BaseUrlPattern CreateBaseUrlPattern(List<string> serverHosts)
        {
            var urlRegexBuilder = new BaseUrlRegexBuilder()
                .SetDomains(serverHosts);
            return new BaseUrlPattern(urlRegexBuilder.Build(), true);
        }

        public BaseUrlInfo GetDefaultBaseUrlInfo(string defaultServer) => new BaseUrlInfo(defaultServer, "");
    }
}