using System.Collections.Generic;

namespace selenium.core.Framework.Service
{
    public interface ServiceFactory
    {
        // Создать маршрутизатор страниц(сопоставление Url-->Страницы) для сервиса
        Router CreateRouter();
        // Создать сервис
        IService CreateService();
        // Паттерн для Url, которым соответствует сервис
        BaseUrlPattern CreateBaseUrlPattern(List<string> serverHosts);
        // Дефолтные параметры базового Url
        BaseUrlInfo GetDefaultBaseUrlInfo(string defaultServer);
    }
}
