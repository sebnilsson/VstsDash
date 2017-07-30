using System;
using System.Collections.Generic;
using System.Linq;

namespace VstsDash
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            return source.GroupBy(keySelector).Select(x => x.First());
        }
    }
}