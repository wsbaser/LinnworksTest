using System;
using System.Collections.Generic;
using System.Linq;
using automateit.Configs.Models;
using selenium.core.Framework.Page;

namespace selenium.core.Framework.Service
{
    public class Web
    {
        // Список зарегистрированных сервисов
        public readonly List<IService> Services;

        public Web()
        {
            Services = new List<IService>();
        }

        // Определение сервиса, который должен обработать запрос(DNS маршрутизация и маршрутизация внутри домена)
        public ServiceMatchResult MatchService(RequestData request)
        {
            ServiceMatchResult baseDomainMatch = null;
            foreach (var service in Services)
            {
                var baseUrlPattern = service.BaseUrlPattern;
                var result = baseUrlPattern.Match(request.Url.OriginalString);
                if (result.Level == BaseUrlMatchLevel.FullDomain)
                    return new ServiceMatchResult(service, result.GetBaseUrlInfo());
                if (result.Level == BaseUrlMatchLevel.BaseDomain)
                {
                    if (baseDomainMatch != null)
                        throw new Exception($"Two BaseDomain matches for url {request.Url}");
                    baseDomainMatch = new ServiceMatchResult(service, result.GetBaseUrlInfo());
                }
            }
            return baseDomainMatch;
        }

        // Поиск страницы в зарегистрированных сервисах
        // и получение ее Url
        public RequestData GetRequestData(IPage page)
        {
            var service = Services.FirstOrDefault(s => s.Router.HasPage(page));
            if (service == null)
                throw new PageNotRegisteredException(page);
            return service.Router.GetRequest(page, service.DefaultBaseUrlInfo);
        }

        // Зарегистрировать сервис
        public T RegisterService<T>(ServiceFactory serviceFactory) where T:IService
        {
            var service = serviceFactory.CreateService();
            Services.Add(service);
            return (T)service;
        }

        public IPage GetEmailPage(Uri uri)
        {
            foreach (var service in Services)
            {
                var emailPage = service.GetEmailPage(uri);
                if (emailPage != null)
                    return emailPage;
            }
            return null;
        }
    }
}
