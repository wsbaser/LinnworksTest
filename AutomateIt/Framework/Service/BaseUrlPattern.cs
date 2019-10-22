using System.Text.RegularExpressions;

namespace selenium.core.Framework.Service
{
    public class BaseUrlPattern
    {
        private readonly string _regexPattern;
        private readonly bool _ignoreCase;

        public BaseUrlPattern(string regexPattern, bool ignoreCase)
        {
            _regexPattern = regexPattern;
            _ignoreCase = ignoreCase;
        }

        // Соответствует ли указанный Url шаблону
        public BaseUrlMatchResult Match(string url)
        {
            var regexOptions = RegexOptions.None;
            if (_ignoreCase)
            {
                regexOptions |= RegexOptions.IgnoreCase;
            }

            var regex = new Regex(_regexPattern, regexOptions);
            var match = regex.Match(url);
            if (!match.Success)
                return BaseUrlMatchResult.Unmatched();
            var scheme = match.Groups["scheme"].Value;
            var domain = hasGroup(match, "domain") ? match.Groups["domain"].Value : null;
            var abspath = hasGroup(match, "abspath") ? match.Groups["abspath"].Value : null;

            // У сервиса есть жестко заданный поддомен и он совпадает с поддоменом в Url
            if (hasGroup(match, "subdomain"))
            {
                return new BaseUrlMatchResult(BaseUrlMatchLevel.FullDomain, scheme, match.Groups["subdomain"].Value, domain, abspath);
            }

            var optionalsubdomain = match.Groups["optionalsubdomain"].Value;
            // У сервиса нет жестко заданного поддомена и в Url также нет поддомена
            if (string.IsNullOrEmpty(optionalsubdomain))
            {
                return new BaseUrlMatchResult(BaseUrlMatchLevel.FullDomain, scheme, null, domain, abspath);
            }

            // У сервиса нет жестко заданного поддомена, но в Url поддомен имеется
            return new BaseUrlMatchResult(BaseUrlMatchLevel.BaseDomain, scheme, optionalsubdomain, domain, abspath);
        }

        // Проверить, имеется ли группа в паттерне
        private bool hasGroup(Match match, string groupName)
        {
            return !string.IsNullOrEmpty(match.Groups[groupName].Value);
        }
    }
}
