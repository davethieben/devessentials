﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Essentials
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? list)
        {
            return list == null || !list.Any();
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T>? target)
        {
            list.IsRequired();

            if (target.IsNullOrEmpty())
                return;

            foreach (T item in target)
                list.Add(item);
        }

        /// <summary>
        /// removes an element from the list based on a predicate delegate
        /// </summary>
        public static void Remove<T>(this ICollection<T> list, Predicate<T> test)
        {
            foreach (var item in list.ToList())
                if (test(item))
                    list.Remove(item);
        }

        /// <summary>
        /// returns a new List<T> if the input is null
        /// </summary>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? list)
        {
            if (list == null)
                return new List<T>();
            return list;
        }

        /// <summary>
        /// evaluates two lists and compares their contents to each other
        /// if any one of the items in the lists don't match, returns false
        /// if items are the same but are in different positions, returns false
        /// if null at same position in both lists, that is okay
        /// </summary>
        public static bool IsEqualTo<T>(this IEnumerable<T>? x, IEnumerable<T>? y)
        {
            if ((x == null && y != null) || (x != null && y == null))
                return false;

            if (x != null && y != null)
            {
                if (x.Count() != y.Count())
                    return false;

                for (int i = 0; i < x.Count(); i++)
                {
                    T xi = x.ElementAt(i);
                    T yi = y.ElementAt(i);

                    if (xi == null && yi == null)
                        continue;

                    if (xi != null && !xi.Equals(yi))
                        return false;
                }
            }

            return true;
        }

        public static bool IsEqualTo<T>(this IEnumerable<T>? x, params T[]? y) =>
            IsEqualTo<T>(x, (IEnumerable<T>?)y);

        /// <summary>
        /// selects only the Distinct elements in the list by comparing values in the provided expression.
        ///     <see cref="KeyComparer{T}"/>
        /// </summary>
        /// <example>myList.Distinct(x => x.Id);</example>
        /// <param name="source">list to select elements from</param>
        /// <param name="keySelector">expression to select values from the elements to compare</param>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Expression<Func<T, object>> keySelector)
            where T : class
        {
            source.IsRequired();

            return source.Distinct(new KeyComparer<T>(keySelector));
        }

        /// <summary>
        /// inserts the given item into the list ONCE when the predicate is satisfied
        /// </summary>
        public static void InsertBefore<T>(this IList<T> list, Predicate<T> test, T item)
        {
            for (int index = 0; index < list.Count; index++)
            {
                if (test(list[index]))
                {
                    list.Insert(index, item);
                    break;
                }
            }
        }

        public static T FindOne<T>(this IEnumerable<T> list, Func<T, bool> test)
        {
            var candidates = list.Where(test);
            if (candidates.IsNullOrEmpty())
                throw new ArgumentException($"Cannot find matching '{typeof(T)}' in the list");

            if (candidates.Count() > 1)
                throw new ArgumentException($"Found more than one matching '{typeof(T)}' in the list");

            return candidates.Single();
        }

        public static T FindOne<T>(this IEnumerable<T> list) => list.FindOne(_ => true);

        public static bool TryGetFirst<T>(this IEnumerable<T> list, Func<T, bool> test, out T? found)
            where T : class
        {
            found = default;

            if (!list.IsNullOrEmpty())
            {
                T item = list.FirstOrDefault(test.IsRequired());
                if (item != null)
                {
                    found = item;
                    return true;
                }
            }

            return false;
        }

        public static int IndexOf<T>(this IEnumerable<T> list, T item, int start = 0)
        {
            item.IsRequired();
            if (!list.IsNullOrEmpty())
            {
                for (int index = start; index < list.Count(); index++)
                {
                    if (list.ElementAt(index)?.Equals(item) == true)
                        return index;
                }
            }

            return -1;
        }

        public static int NullSafeCount<T>(this IEnumerable<T> list)
        {
            if (list.IsNullOrEmpty())
                return 0;
            else
                return list.Count();
        }

        /// <summary>
        /// merges two lists of the same type based on a key value in the object, and uses a merging function
        /// to copy values into a single result object. Note: source lists are unaffected. a new IEnumerable<TSource> 
        /// is returned.
        /// </summary>
        /// <typeparam name="TSource">Type of the items in the list</typeparam>
        /// <typeparam name="TKey">Type of the key value used for comparing objects</typeparam>
        /// <param name="first">first list to merge into the output</param>
        /// <param name="second">second list to merge into the output</param>
        /// <param name="keySelector">function to select the key value from a given TSource object</param>
        /// <param name="merge">function to merge two TSource objects into one</param>
        /// <returns>new IEnumerable containing the merged output of the 2 input sources</returns>
        public static IEnumerable<TSource> Merge<TSource, TKey>(this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            Func<TSource, TKey> keySelector,
            Func<TSource, TSource, TSource> merge)
            where TSource : class, new()
        {
            var groups = first.EmptyIfNull()
                          .Concat(second.EmptyIfNull())
                          .GroupBy(keySelector);
            foreach (var group in groups)
                yield return group.Aggregate(new TSource(), merge);
        }

        public static bool ContainsAll<T>(this IEnumerable<T> list, IEnumerable<T> second)
        {
            if (list.IsNullOrEmpty() && second.IsNullOrEmpty())
                return true;

            if (list.IsNullOrEmpty() && !second.IsNullOrEmpty())
                return false;

            var intersection = list.Intersect(second.EmptyIfNull());
            return intersection.Count() == second.NullSafeCount();
        }

        public static bool ContainsAny<T>(this IEnumerable<T> list, IEnumerable<T> second)
        {
            if (list.IsNullOrEmpty() || second.IsNullOrEmpty())
                return false;

            return list.Any(t => second.Contains(t));
        }

        public static void RemoveOutliers(IList<long> input, double percentile)
        {
            if (percentile < 0 || percentile > 1)
                throw new ArgumentOutOfRangeException("Percentile must be greater than 0 and less than 1");

            List<long> set = input.OrderBy(x => x).ToList();

            int topIndex = (int)Math.Round((1 - percentile) * set.Count, MidpointRounding.AwayFromZero);
            int bottomIndex = (int)Math.Round(percentile * set.Count, MidpointRounding.AwayFromZero);

            long topThreshold = set[topIndex - 1];
            long bottomThreshold = set[bottomIndex];

            input.Remove(x => x < bottomThreshold || x > topThreshold);
        }

        public static IEnumerable<string> ToStrings<T>(this IEnumerable<T> list)
        {
            foreach (T item in list.EmptyIfNull())
                yield return item?.ToString() ?? "<null>";
        }

        public static string ToDisplayString<T>(this IEnumerable<T> list, string separator = ",") => string.Join(separator, list.ToStrings());

        public static IEnumerable<T> Append<T>(this IEnumerable<T> input, T item)
        {
            if (input.IsNullOrEmpty())
            {
                input = new List<T>();
            }

            if (input is ICollection<T> collection)
            {
                collection.Add(item);
                return collection;
            }
            else
            {
                return input.EmptyIfNull().Concat(new[] { item });
            }
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            source.IsRequired();

            while (source.Any())
            {
                yield return source.Take(chunkSize);

                source = source.Skip(chunkSize);
            }
        }

        public static int GetContentsHashCode<T>(this IEnumerable<T>? input)
        {
            // 13 and 19 are arbitrary prime numbers to seed the hash
            return input?.Aggregate(13, (hash, item) =>
            {
                return hash * 19 + (item?.GetHashCode() ?? 0);
            }) ?? 0;
        }

        /// <summary>
        /// given an object that has a child property of the same type, converts the hierarchy into a flat list
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this T start, Func<T, T?> getChild)
            where T : class
        {
            var output = new List<T>();

            if (start != null)
            {
                output.Add(start);

                T? child = getChild(start);
                if (child != null)
                    output.AddRange(Flatten(child, getChild));
            }

            return output;
        }

        public static IEnumerable<Exception> FlattenAggregate(this AggregateException aggEx)
        {
            foreach (var ex in aggEx.InnerExceptions)
                foreach (var childEx in ex.Flatten(x => x?.InnerException))
                    yield return childEx;
        }

    }

    public class KeyComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private readonly Func<T, object> _keySelector;

        public KeyComparer(Expression<Func<T, object>> keySelector)
        {
            _keySelector = keySelector.IsRequired().Compile();
        }

        public bool Equals([AllowNull] T x, [AllowNull] T y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            var xKey = _keySelector(x);
            var yKey = _keySelector(y);

            return xKey?.Equals(yKey) == true;
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            if (obj == null)
                return 0;

            return _keySelector(obj).GetHashCode();
        }

    }
}

namespace Essentials.Collections
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs a natural sort on the collection.
        /// </summary>
        /// <param name="items">Collection to be sorted.</param>
        /// <param name="selector">Selector that returns the string to be sorted.</param>
        /// <param name="stringComparer">Optional string comparer.</param>
        public static IOrderedEnumerable<T> OrderByNatural<T>(this IEnumerable<T> items, Func<T, string> selector, StringComparer? stringComparer = null)
        {
            var regex = new Regex(@"\d+", RegexOptions.Compiled);

            int maxDigits = items
                          .SelectMany(i => regex.Matches(selector(i)).Cast<Match>().Select(digitChunk => (int?)digitChunk.Value.Length))
                          .Max() ?? 0;

            return items.OrderBy(i => regex.Replace(selector(i), match => match.Value.PadLeft(maxDigits, '0')), stringComparer ?? StringComparer.CurrentCulture);
        }

    }
}
