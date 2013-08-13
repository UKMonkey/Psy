using System.Collections.Generic;

namespace Psy.Core
{
    public static class EnumerableExtentions
    {
        public class IndexedEnumeratorValue<T>
        {
            public readonly int Index;
            public readonly T Value;

            public IndexedEnumeratorValue(int index, T value)
            {
                Index = index;
                Value = value;
            }
        }

        public static IEnumerable<IndexedEnumeratorValue<T>> IndexOver<T>(this IEnumerable<T> enumerable)
        {
            var i = 0;
            foreach (var element in enumerable)
            {
                yield return new IndexedEnumeratorValue<T>(i, element);
                i++;
            }
        }

        public static IEnumerable<T> ReverseInPlace<T>(this IList<T> items)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }
    }
}