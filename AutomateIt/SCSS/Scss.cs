using System;
using System.Collections.Generic;
using System.Linq;
using automateit.SCSS;
using Natu.Utils;
using NUnit.Framework;
using OpenQA.Selenium;

namespace selenium.core.SCSS
{
    public class Scss
    {
        public readonly string Css;
        public readonly string Xpath;
        public bool CombineWithRoot;

        public Scss(string xpath, string css, bool combineWithRoot = false) {
            Css = css;
            Xpath = xpath;
            CombineWithRoot = combineWithRoot;
        }

        public By By
        {
            get { return string.IsNullOrEmpty(Css) ? By.XPath(Xpath) : By.CssSelector(Css); }
        }

        public string Value
        {
            get { return string.IsNullOrEmpty(Css) ? Xpath : Css; }
        }

        public static string Concat(string scssSelector1, string scssSelector2)
        {
            return ScssBuilder.Concat(scssSelector1, scssSelector2).Value;
        }

        public Scss Concat(Scss scss2)
        {
            var resultXpath = XPathBuilder.Concat(Xpath, scss2?.Xpath);
            var resultCss = string.IsNullOrEmpty(Css) || string.IsNullOrEmpty(scss2.Css)
                ? string.Empty
                : CssBuilder.Concat(Css, scss2.Css);
            return new Scss(resultXpath, resultCss);
        }

        public static By GetBy(string scssSelector1, string scssSelector2)
        {
            return ScssBuilder.Concat(scssSelector1, scssSelector2).By;
        }

        public static By GetBy(string scssSelector)
        {
            return ScssBuilder.CreateBy(scssSelector);
        }
    }

    public class CssBuilder
    {
        private const char CSS_PARTS_DELIMITER = ',';

        public static string Concat(string rootCss, string relativeCss) {
            if (string.IsNullOrWhiteSpace(relativeCss))
                return rootCss;
            if (string.IsNullOrEmpty(rootCss))
                return relativeCss;
            var rootCssParts = GetCssParts(rootCss);
            var relativeCssParts = GetCssParts(relativeCss);
            if (rootCssParts.Count == 1) {
                return CombineCssParts(relativeCssParts.Select(r => ConcatCssParts(rootCssParts[0], r)));
            }
            if (relativeCssParts.Count == 1) {
                return CombineCssParts(rootCssParts.Select(r => ConcatCssParts(r, relativeCssParts[0])));
            }
            if (rootCssParts.Count == relativeCssParts.Count) {
                var xpathPartsConcatenated = new List<string>();
                for (var i = 0; i < rootCssParts.Count; i++) {
                    xpathPartsConcatenated.Add(ConcatCssParts(rootCssParts[i], relativeCssParts[i]));
                }
                return CombineCssParts(xpathPartsConcatenated);
            }
            throw new InvalidOperationException($"There is no algorithm for concatenating css selectors with different amount of parts. root:{rootCss}, relative: {relativeCss}.");
        }

        private static string CombineCssParts(IEnumerable<string> cssParts) => string.Join(",", cssParts);

        private static List<string> GetCssParts(string rootCss) => rootCss.Split(CSS_PARTS_DELIMITER).Select(p=>p.Trim()).ToList();

        private static string ConcatCssParts(string rootCssPart, string relativeCssPart)
	    {
		    var cssAxisList = new List<string> { " ", ">", "+" };
		    return cssAxisList.Any(axis => relativeCssPart.StartsWith(axis, StringComparison.Ordinal)) ? rootCssPart + relativeCssPart : $"{rootCssPart} {relativeCssPart}";
	    }
    }

    [TestFixture]
    public class CssBuilderTests {
        [TestCase("div", "", "div")]
        [TestCase("", "a", "a")]
        [TestCase("div", "a", "div a")]
        [TestCase("div", ">a", "div>a")]
        [TestCase("div", "a,i", "div a,div i")]
        [TestCase("div", ">a,>i", "div>a,div>i")]
        [TestCase("div,span", "a", "div a,span a")]
        [TestCase("div,span", "a,i", "div a,span i")]
        public void ConcatTests(string root, string relative, string expectedResult) {
            // .Act
            var actualResult = CssBuilder.Concat(root, relative);
            // .Assert
            Assert.AreEqual(expectedResult, actualResult, "Invalid concatenation result.");
        }
    }

    [TestFixture]
    public class ScssTests {
        [TestCase("div", "div", "//div/descendant::div", "div div")]
        [TestCase("div", ">div", "//div/child::div", "div>div")]
        [TestCase("div", ">div", "//div/child::div", "div>div")]
        [TestCase("#LitJurisdiction>ul>li", "../../p/strong", "//*[@id='LitJurisdiction']/ul/li/../../p/strong", "")]
        [TestCase(".content-section.results, .km-selection", "li span,li span[2]", "//*[contains(@class,'content-section')][contains(@class,'results')]/descendant::li/descendant::span|//*[contains(@class,'km-selection')]/descendant::li/descendant::span[2]", "")]
        public void Run(string scssSelector1, string scssSelector2, string resultXpath, string resultCss) {
            var scss1 = ScssBuilder.Create(scssSelector1);
            var scss2 = ScssBuilder.Create(scssSelector2);
            var resultScss = scss1.Concat(scss2);
            Assert.AreEqual(resultXpath, resultScss.Xpath);
            Assert.AreEqual(resultCss, resultScss.Css);
        }
    }
}
