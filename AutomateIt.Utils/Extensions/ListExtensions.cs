using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Natu.Utils.Extensions
{
    public static class ListExtensions
    {
        public static List<string> TrimAll(this List<string> list)
        {
            return list.Select(i => i.Trim()).ToList();
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            var random = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static string AsString(this IEnumerable<string> list, string delimiter = ",")
        {
            if (!list.Any())
                return string.Empty;
            var s = list.Aggregate(string.Empty, (current, item) => current + item + delimiter);
            return s.Substring(0, s.Length - 1);
        }
    }

    [TestFixture]
    public class ListExtensionsTest
    {
        [Test]
        public void RandomItemTest()
        {
            var list = new List<ClassA> { new ClassA("111"), new ClassA("222"), new ClassA("111") };
            var randomItem = list.RandomItem(new ClassA("111"));
            Assert.AreNotEqual("111", randomItem.Field);
        }

        private class ClassA
        {
            public readonly string Field;

            public ClassA(string field)
            {
                Field = field;
            }

            public override bool Equals(object obj)
            {
                return obj is ClassA && (obj as ClassA).Field == Field;
            }
        }
    }
}
