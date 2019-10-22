using System;
using System.Collections.Generic;
using System.Linq;
using Natu.Utils.Exceptions;

namespace Natu.Utils.Extensions
{
    public static class IEnumerableExtensions
    {
        public static T RandomItemOrNull<T>(this IEnumerable<T> enumerable, params T[] exclude) {
            if (enumerable.ToList().Count == 0)
                return default(T);
            return enumerable.ToList().RandomItem(exclude.ToList());
        }

        public static T RandomItem<T>(this IEnumerable<T> enumerable, params T[] exclude) => enumerable.ToList().RandomItem(exclude.ToList());

        public static T RandomItem<T>(this IEnumerable<T> enumerable, Func<T, bool> excludeCondition)
        {
            var list = enumerable.ToList();
            T randomItem;
            var rnd = new Random();
            do
            {
                if (list.Count == 0)
                {
                    throw new NoAvaliableItemException("Unable to get random item. There are no available items in list.");
                }
                randomItem = list[rnd.Next(list.Count)];
                if (excludeCondition.Invoke(randomItem))
                {
                    list.Remove(randomItem);
                    continue;
                }
                break;
            }
            while (true);
            return randomItem;
        }

        public static T RandomItem<T>(this IEnumerable<T> list, IEnumerable<T> exclude) => list.RandomItem(exclude.Contains);
    }
}
