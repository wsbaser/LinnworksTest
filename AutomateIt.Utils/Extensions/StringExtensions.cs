using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Natu.Utils.Extensions
{
    public static class StringExtensions {
        public static string CutFirst(this string s) => s.Substring(1, s.Length - 1);

        public static string CutFirst(this string s, char symbol) => s.StartsWith(symbol.ToString()) ? s.Substring(1, s.Length - 1) : s;

        public static string CutLast(this string s) => s.Substring(0, s.Length - 1);

        public static string CutLast(this string s, char symbol) => s.EndsWith(symbol.ToString()) ? s.Substring(0, s.Length - 1) : s;

        public static int AsInt(this string s) => int.Parse(FindInt(s));

        public static decimal AsDecimal(this string s) => decimal.Parse(s.FindNumber());

        public static string FindNumber(this string s) => RegexHelper.GetString(s, "((?:-|)\\d+(?:(?:\\.|,)\\d+)?)");

        public static string FindInt(this string s) => RegexHelper.GetString(s, "((?:-|)\\d+)");

        public static string FindUInt(this string s) => RegexHelper.GetString(s, "(\\d+)");

        public static string RemoveExtension(this string s) {
            return Path.GetFileNameWithoutExtension(s);
//            var list = s.Split('.').ToList();
//            if (list.Count > 1
//                && !string.IsNullOrEmpty(list.Last())) {
//                list.RemoveAt(list.Count - 1);
//                return string.Join(".", list);
//            }
//            return s;
        }

	    public static string RemoveMultipleSpaces(this string s) => System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ");

	    public static string InvertCase(this string value) => string.Join("", value.Select(c => char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c)));

        public static string AddSpaces(this string value) {
            var sb = new StringBuilder(value.Length*2);
            for (var i = 0; i < value.Length; i++) {
                var cur = value[i];
                if (cur == ' ')
                {
                    sb.Append(cur);
                    continue;
                }
                var next = i + 1 < value.Length ? value[i+1] : (char?)null;
                var nextplus1 = i + 2 < value.Length ? value[i + 2] : (char?)null;
                sb.Append(i == 0 ? cur.ToString().ToUpper() : cur.ToString());
                if (!char.IsLetter(cur))
                {
                    continue;
                }
                if (next.HasValue && !char.IsUpper(cur) && (char.IsUpper((char)next) || char.IsDigit((char)next))
                    || nextplus1.HasValue && char.IsUpper(cur) && char.IsUpper((char)next) && !char.IsUpper((char)nextplus1)) {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        public static string RemovePunctuation(this string value) => new string(value.Where(c => !char.IsPunctuation(c)).ToArray());

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }
    }

    [TestFixture]
    public class StringExtensionsTest
    {
        [TestCase("MyString", "My String")]
        [TestCase("mystring", "Mystring")]
        [TestCase("myString", "My String")]
        [TestCase("MYSTRING", "MYSTRING")]
        [TestCase("MYString", "MY String")]
        [TestCase("MyString1", "My String 1")]
        [TestCase("", "")]
        [TestCase("Email/Print dropdown", "Email/Print dropdown")]
        public void AddSpaces(string before, string after) {
            // .Arrange
            // .Act
            var result = before.AddSpaces();
            // .Assert
            Assert.AreEqual(after, result);
        }

        [TestCase("1.txt", "1")]
        [TestCase(".txt", "")]
        [TestCase("1.1", "1")]
        [TestCase("1.", "1.")]
        [TestCase("1", "1")]
        [TestCase(".", ".")]
        public void RemoveExtension(string text, string expected)
        {
            Assert.AreEqual(expected, text.RemoveExtension());
        }

        [TestCase("-5", "-5")]
        [TestCase("text -5 text", "-5")]
        [TestCase("text 1.2 text", "1.2")]
        [TestCase("text 1,2 text", "1,2")]
        public void FindNumber(string text, string expected)
        {
            Assert.AreEqual(expected, text.FindNumber());
        }

        [TestCase("-5", -5)]
        public void AsDecimal(string text, decimal expected)
        {
            Assert.AreEqual(expected, text.AsDecimal());
        }

        [TestCase("1", "1")]
        [TestCase("text 1 text", "1")]
        [TestCase("text 1.2 text", "1")]
        [TestCase("-1", "-1")]
        [TestCase("text -1 text", "-1")]
        [TestCase("text -1.2 text", "-1")]
        public void FindInt(string text, string expected)
        {
            Assert.AreEqual(expected, text.FindInt());
        }
    }
}
