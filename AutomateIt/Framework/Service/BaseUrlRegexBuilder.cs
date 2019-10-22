using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace selenium.core.Framework.Service
{
    public class BaseUrlRegexBuilder
    {
        private readonly string _schemesPattern;
        private string _domainPattern = "";
        private string _absolutePathPattern = "";
        private string _subDomainPattern = "(?<optionalsubdomain>([^\\.]+\\.)*)";

        public BaseUrlRegexBuilder()
            : this("http", "https")
        {
        }

        public BaseUrlRegexBuilder(params string[] schemes)
        {
            _schemesPattern = GenerateSchemasPattern(schemes);
        }

        private string GenerateSchemasPattern(IEnumerable<string> schemas) =>
            $"(?<scheme>({string.Join("|", schemas)}))";

        public BaseUrlRegexBuilder SetDomains(params string[] domains) => SetDomains(domains.ToList());

        public BaseUrlRegexBuilder SetDomains(List<string> domains)
        {
            _domainPattern = GenerateDomainsPattern(domains);
            return this;
        }

        private string GenerateDomainsPattern(IEnumerable<string> domains)
        {
            var s = domains.Aggregate("", (current, domain) => current + domain + "|");
            s = s.Substring(0, s.Length - 1);
            s = s.Replace(".", "\\.");
            return $"(?<domain>({s}))";
        }

        public BaseUrlRegexBuilder SetSubDomain(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _subDomainPattern = $"(?<subdomain>{value})\\.";
            }

            return this;
        }

        public BaseUrlRegexBuilder SetAbsolutePathPattern(string pattern)
        {
            if (!string.IsNullOrWhiteSpace(pattern))
            {
                _absolutePathPattern = $"(?<abspath>{pattern})";
            }

            return this;
        }

        // Сформировать Regex паттерн для BaseUrl сервиса
        public string Build()
        {
            var result = $"{_schemesPattern}://";
            if (string.IsNullOrEmpty(_domainPattern))
            {
                return result + "(.*)";
            }

            result += "(www.|)" +
                      _subDomainPattern +
                      _domainPattern;
            return result + _absolutePathPattern + "(/.*|$)";
        }
    }

    [TestFixture]
    public class BaseUrlRegexBuilderTests
    {
        [TestCase("schema://www.subdomain.domain.com/absolutepath/someurl?parameter1=value1&parameter2=value2#hashtag", "schema", "subdomain", "domain.com", "absolutepath")]
        [TestCase("schema://www.domain.com/absolutepath/someurl?parameter1=value1&parameter2=value2#hashtag", "schema", null, "domain.com", "absolutepath")]
        [TestCase("schema://www.domain.com/someurl?parameter1=value1&parameter2=value2#hashtag", "schema", null, "domain.com", null)]
        [TestCase("schema://someurl?parameter1=value1&parameter2=value2#hashtag", "schema", null, null, null)]
        public void BuildRegex(string urlToMatch, string schema, string subdomain, string domain, string absolutepath)
        {
            // .Arrange
            var builder = new BaseUrlRegexBuilder(schema);

            if (!string.IsNullOrEmpty(domain))
            {
                builder.SetDomains(domain);
            }

            if (!string.IsNullOrEmpty(subdomain))
            {
                builder.SetSubDomain(subdomain);
            }

            if (!string.IsNullOrEmpty(absolutepath))
            {
                builder.SetAbsolutePathPattern(absolutepath);
            }

            // .Act
            var pattern = builder.Build();
            var regex = new Regex(pattern);

            // .Assert
            var match = regex.Match(urlToMatch);
            Assert.IsTrue(match.Success);
        }
    }
}
