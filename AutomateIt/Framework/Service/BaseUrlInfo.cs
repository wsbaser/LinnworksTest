namespace selenium.core.Framework.Service
{
    public class BaseUrlInfo
    {
        public BaseUrlInfo(string domain)
            : this(null, domain, null)
        {
        }

        public BaseUrlInfo(string domain, string absolutePath)
            : this(null, domain, absolutePath)
        {
        }

        public BaseUrlInfo(string subDomain, string domain, string absolutePath, string scheme = "http")
        {
            SubDomain = subDomain;
            Domain = domain;
            AbsolutePath = absolutePath;
            Scheme = scheme;
        }

        public string Scheme { get; }
        public string SubDomain { get; }
        public string Domain { get; set; }
        public string AbsolutePath { get; }

        public BaseUrlInfo ApplyActual(BaseUrlInfo baseUrlInfo)
        {
            var subDomain = baseUrlInfo == null || string.IsNullOrEmpty(baseUrlInfo.SubDomain)
                ? SubDomain
                : baseUrlInfo.SubDomain;
            var domain = baseUrlInfo == null || string.IsNullOrEmpty(baseUrlInfo.Domain)
                ? Domain
                : baseUrlInfo.Domain;
            var absolutePath = baseUrlInfo == null || string.IsNullOrEmpty(baseUrlInfo.AbsolutePath)
                ? AbsolutePath
                : baseUrlInfo.AbsolutePath;
            var scheme = baseUrlInfo == null || string.IsNullOrEmpty(baseUrlInfo.Scheme)
                ? Scheme
                : baseUrlInfo.Scheme;
            return new BaseUrlInfo(subDomain, domain, absolutePath, scheme);
        }

        // Сформировать BaseUrl
        public string GetBaseUrl()
        {
            var s = Domain;
            if (!string.IsNullOrEmpty(AbsolutePath))
                s += AbsolutePath;
            if (!string.IsNullOrEmpty(SubDomain))
                s = SubDomain + "." + s;
            return $"{Scheme}://{s}";
        }
    }
}
