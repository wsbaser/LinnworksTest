using System;
using System.Collections.Generic;
using Natu.Utils.Extensions;
using NLog;
using selenium.core.Framework.Page;

namespace automateit.Framework.Page
{
    public class UriMatcher {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly string _pageAbsolutePath;
        private readonly Dictionary<string, string> _pageData;
        private readonly Dictionary<string, string> _pageParams;
        public UriMatcher(string pageAbsolutePath, Dictionary<string, string> pageData, Dictionary<string, string> pageParams)
        {
            _pageAbsolutePath = pageAbsolutePath;
            _pageData = pageData;
            _pageParams = pageParams;
        }

        public UriMatchResult Match(Uri uri, string siteAbsolutePath)
        {
            siteAbsolutePath = siteAbsolutePath ?? "";
            if (!uri.AbsolutePath.StartsWith(siteAbsolutePath, StringComparison.Ordinal)) {
                return UriMatchResult.Unmatched();
            }
            var actualPath = uri.AbsolutePath.Substring(siteAbsolutePath.Length);
            var pageArr = _pageAbsolutePath.Split('/');
            var realArr = actualPath.Split('/');
            if (pageArr.Length != realArr.Length)
                return UriMatchResult.Unmatched();

            var actualData = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            // Extract Data from url
            for (var i = 0; i < pageArr.Length; i++) {
                var pageArrItem = pageArr[i];
                var actualArrItem = realArr[i];
                if (pageArrItem.StartsWith("{")
                    && pageArrItem.EndsWith("}")) {
                    var paramName = pageArrItem.Substring(1, pageArrItem.Length - 2);
                    actualData[paramName] = actualArrItem;
                }
                else if (!string.Equals(pageArrItem, actualArrItem, StringComparison.OrdinalIgnoreCase))
                    return UriMatchResult.Unmatched();
            }

            //  Compare Data
            foreach (var key in _pageData.Keys) {
                if (!actualData.ContainsKey(key)
                    || !string.Equals(actualData[key], _pageData[key], StringComparison.OrdinalIgnoreCase))
                    return UriMatchResult.Unmatched();
            }


            var actualParams = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (_pageParams != null
                && _pageParams.Count > 0) {
                // Extract Parameters from url
                var queryParamsArr = uri.Query.CutFirst('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrWhiteSpace(uri.Fragment) && uri.Fragment.Split('=').Length>1)
                {
                    // TODO(***): Handle urls with # in a proper way
                    var keyvalue = uri.Fragment.Split('=');
                    actualParams.Add(keyvalue[0], keyvalue[1]);
                }
                else
                {
                    foreach (var queryParam in queryParamsArr)
                    {
                        var arr = queryParam.Split('=');
                        if (arr.Length != 2)
                        {
                            _log.Debug($"Invalid url query parameter: {queryParam}");
                            continue;
                        }
                        var paramName = arr[0];
                        var paramValue = arr[1];
                        if (actualParams.ContainsKey(paramName))
                        {
                            _log.Debug($"Url query contains duplicated parameter with different values. {paramName}={actualParams[paramName]}&{paramName}={paramValue}");
                            if (_pageParams.ContainsKey(paramName))
                            {
                                return UriMatchResult.Unmatched();
                            }
                        }
                        else
                        {
                            actualParams.Add(paramName, paramValue);
                        }
                    }
                }

                // Compare Parameters
                foreach (var key in _pageParams.Keys)
                {
                    if (!actualParams.ContainsKey(key)
                        || !string.Equals(actualParams[key], _pageParams[key], StringComparison.OrdinalIgnoreCase))
                        return UriMatchResult.Unmatched();
                }
            }
            return new UriMatchResult(true, actualData, actualParams);
        }
    }
}
