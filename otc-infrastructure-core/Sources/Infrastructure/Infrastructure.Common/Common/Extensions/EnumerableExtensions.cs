using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable, T item)
        {
            var tmp = new List<T> {item};
            tmp.AddRange(enumerable.Select(x => x));
            return tmp;
        }

        public static void Add<T>(this List<T> @enumerable, T item, bool isNeedAdd)
        {
            if (isNeedAdd)
            {
                @enumerable.Add(item);
            }
        }

        public static void SafeAddRange<T>(this List<T> @enumerable, IEnumerable<T> source)
        {
            if (source.IsNotEmpty())
            {
                @enumerable.AddRange(source);
            }
        }

        public static T SafeGetItem<T>(this T[] @array, int index)
        {
            var item = default(T);
            if (@array == null || @array.IsEmpty())
            {
                return default(T);
            }

            if (index < @array.Count())
            {
                item = @array[index];
            }

            return item;
        }

        public static ICollection<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || enumerable.Any() == false;
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable != null && enumerable.Any();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TSource> DistinctBySome<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var result = source.GroupBy(keySelector).Select(x => x.First()).ToList();
            return result;
        }

        public static string ToStringWithSeparator(this IEnumerable<string> enumerable, string separator = ",")
        {
            if (enumerable.IsEmpty())
            {
                return string.Empty;
            }

            return string.Join(separator, enumerable.ToArray());
        }

        public static string[] SplitString(this string enumerable, string separator = ",")
        {
            if (enumerable.IsEmpty())
            {
                return new List<string>().ToArray();
            }

            return enumerable.Split(separator.ToArray());
        }

        public static int[] SplitStringToIntArray(this string enumerable, string separator = ",")
        {
            return
                enumerable.SplitString(separator)
                    .Select(x => x.ToInt())
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .ToArray();
        }

        public static IEnumerable<int> SplitStringToIntegers(this string enumerable, string separator = ",")
        {
            return
                enumerable.SplitString(separator)
                    .Select(x => x.ToInt())
                    .Where(x => x.HasValue)
                    .Select(x => x.Value);
        }

        /// <summary>
        /// Разбивает массив на N массивов.
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int count)
        {
            return source
                .Select((x, y) => new { Index = y, Value = x })
                .GroupBy(x => x.Index / count)
                .Select(x => x.Select(y => y.Value).ToList())
                .ToList();
        }
    }
}