using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using NUnit.Framework;

namespace Natu.Utils
{
    public class XPathBuilder
    {
        public const string DESCENDANT_AXIS = "descendant::";
        public const string ANCESTOR_AXIS = "ancestor::";
        public const string ANCESTORORSELF_AXIS = "ancestor-or-self::";
        public const string ATTRIBUTE_AXIS = "attribute::";
        public const string CHILD_AXIS = "child::";
        public const string DESCENDANTORSELF_AXIS = "descendant-or-self::";
        public const string FOLLOWING_AXIS = "following::";
        public const string FOLLOWINGSIBLING_AXIS = "following-sibling::";
        public const string NAMESPACE_AXIS = "namespace::";
        public const string PARENT_AXIS = "parent::";
        public const string PARENT_AXIS2 = "..";
        public const string PRECEDING_AXIS = "preceding::";
        public const string PRECEDINGSIBLING_AXIS = "preceding-sibling::";
        public const string SELF_AXIS = "self::";
        private const string XPATH_ROOT = "//";

        public static string Concat(string root, string relative) {
            if (string.IsNullOrWhiteSpace(relative)) {
                return root;
            }
            if (string.IsNullOrWhiteSpace(root)) {
                return CombineXpathParts(GetXpathParts(relative).Select(x => AddRoot(CutRoot(x))));
            }
            var rootXpaths = GetXpathParts(root);
            var relativeXpaths = GetXpathParts(relative).Select(CutRoot).ToList();
            if (rootXpaths.Count == 1) {
                return CombineXpathParts(relativeXpaths.Select(r => ConcatNormalizedXpathParts(rootXpaths[0], r)));
            }
            if (relativeXpaths.Count == 1) {
                return CombineXpathParts(rootXpaths.Select(r => ConcatNormalizedXpathParts(r, relativeXpaths[0])));
            }
            if (rootXpaths.Count == relativeXpaths.Count) {
                var xpathPartsConcatenated = new List<string>();
                for (var i = 0; i < rootXpaths.Count; i++) {
                    xpathPartsConcatenated.Add(ConcatNormalizedXpathParts(rootXpaths[i], relativeXpaths[i]));
                }
                return CombineXpathParts(xpathPartsConcatenated);
            }
            throw new InvalidOperationException($"There is no algorithm of concatenating xpaths with different amount of parts. root:{root}, relative: {relative}.");
        }

        /// <summary>
        /// Combines 
        /// </summary>
        /// <param name="root">Normalized root xpath</param>
        /// <param name="relative">Normalized relative xpath</param>
        /// <returns></returns>
        private static string ConcatNormalizedXpathParts(string root, string relative) {
            if (string.IsNullOrWhiteSpace(relative)) {
                if (string.IsNullOrWhiteSpace(root))
                    throw new InvalidOperationException("Invalid xpath: root and relative parts are empty");
                return root;
            }
            if (string.IsNullOrWhiteSpace(root)) {
                return AddRoot(relative);
            }
            var axis = HasAxis(relative) ? string.Empty : DESCENDANT_AXIS;
            return $"{root}/{axis}{relative}";
        }

        private static string CombineXpathParts(IEnumerable<string> xpaths) => string.Join("|", xpaths);

        private static string AddRoot(object relativeXpath) => XPATH_ROOT + relativeXpath;

        private static string CutRoot(string xpath) => xpath.StartsWith(XPATH_ROOT, StringComparison.Ordinal) ? xpath.Substring(2, xpath.Length - 2) : xpath;

        private static List<string> GetXpathParts(string fullXpath) => fullXpath.Split('|').Select(p => p.Trim()).ToList();

        private static bool HasAxis(string xpath)
        {
            var axises = new List<string>
            {
                ANCESTOR_AXIS,
                ANCESTORORSELF_AXIS,
                ATTRIBUTE_AXIS,
                CHILD_AXIS,
                DESCENDANT_AXIS,
                DESCENDANTORSELF_AXIS,
                FOLLOWING_AXIS,
                FOLLOWINGSIBLING_AXIS,
                NAMESPACE_AXIS,
                PARENT_AXIS,
                PARENT_AXIS2,
                PRECEDING_AXIS,
                PRECEDINGSIBLING_AXIS,
                SELF_AXIS
            };
            return axises.Any(xpath.StartsWith);
        }

        public static bool IsXPath(string value)
        {
            try
            {
                XPathExpression.Compile(value);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }

    [TestFixture]
    public class XpathBuilderTest
    {
        //*[@id='aaa1']/descendant::*[@id='bbb']|//*[@id='aaa2']/descendant::*[@id='bbb']

        [TestCase("div", true)]
        [TestCase("//div", true)]
        [TestCase("//div[@id='myId']", true)]
        [TestCase("//div[text()='mytext']", true)]
        [TestCase("//div[text()='mytext' and @class='myclass']", true)]
        [TestCase("//div[@id='myId']/descendant::span", true)]
        [TestCase("//div[@id='myId1']|//div[@id='myId2']", true)]
        [TestCase("#myId", false)]
        [TestCase(".myclass", false)]
        public void IsXpath(string xpath, bool isXpath)
        {
            Assert.AreEqual(isXpath, XPathBuilder.IsXPath(xpath));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("   ")]
        public void RootIsEmpty(string root)
        {
            var relative = "div";
            Assert.AreEqual("//div", XPathBuilder.Concat(root, relative));
        }

        [Test]
        public void RelativeIsEmpty()
        {
            var root = "//*[@id='aaa1']";
            var relative = "";
            Assert.AreEqual("//*[@id='aaa1']", XPathBuilder.Concat(root, relative));
        }

        [Test]
        public void MakeRelative()
        {
            var root = "//*[@id='aaa1']";
            var relative = "//*[@id='bbb']";
            Assert.AreEqual("//*[@id='aaa1']/descendant::*[@id='bbb']",
                XPathBuilder.Concat(root, relative));
        }

        [Test]
        public void MultipleRootXpath()
        {
            var root = "//*[@id='aaa1'] | //*[@id='aaa2']";
            var relative = "*[@id='bbb']";
            Assert.AreEqual("//*[@id='aaa1']/descendant::*[@id='bbb']|//*[@id='aaa2']/descendant::*[@id='bbb']",
                XPathBuilder.Concat(root, relative));
        }

//        [Test]
//        public void InsertArgsToRelative()
//        {
//            var root = "//div";
//            var relative = "*[@id='{0}']";
//            Assert.AreEqual("//div/descendant::*[@id='myid']", XPathBuilder.Concat(root, relative, "myid"));
//
//            root = "//div[@id='{0}']";
//            relative = "*[@id='{0}']";
//            Assert.AreEqual("//div[@id='{0}']/descendant::*[@id='myid']", XPathBuilder.Concat(root, relative, "myid"));
//        }

        [Test]
        public void ConcatAsDescendant()
        {
            var root = "//div";
            var relative = "*[@id='myid']";
            Assert.AreEqual("//div/descendant::*[@id='myid']", XPathBuilder.Concat(root, relative));
        }

        [Test]
        public void LeaveAxis()
        {
            var root = "//div";
            var relative = "self::*[@id='myid']";
            Assert.AreEqual("//div/self::*[@id='myid']", XPathBuilder.Concat(root, relative));
        }

        [TestCase("//div|//span", "a|i", "//div/descendant::a|//span/descendant:i")]
        public void ConcatTests(string root, string relative, string expectedResult) {
            // .Act
            var actualResult = XPathBuilder.Concat(root, relative);
            // .Assert
            Assert.AreEqual(expectedResult, actualResult, "Invalid concatenation result.");
        }
    }
}
