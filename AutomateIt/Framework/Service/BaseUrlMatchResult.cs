namespace selenium.core.Framework.Service
{
    public class BaseUrlMatchResult
    {
        public BaseUrlMatchLevel Level;
        public string Scheme;
        public string SubDomain;
        public string Domain;
        public string AbsolutePath;

        public BaseUrlMatchResult(BaseUrlMatchLevel level, string scheme, string subDomain, string domain, string absolutePath)
        {
            Level = level;
            Scheme = scheme;
            SubDomain = subDomain;
            Domain = domain;
            AbsolutePath = absolutePath;
        }

        public static BaseUrlMatchResult Unmatched()
        {
            return new BaseUrlMatchResult(BaseUrlMatchLevel.Unmatched, null, null, null, null);
        }

        public BaseUrlInfo GetBaseUrlInfo()
        {
            return new BaseUrlInfo(SubDomain, Domain, AbsolutePath,Scheme);
        }
    }
}
