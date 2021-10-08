using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Essentials
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(
#if NETSTANDARD2_1_OR_GREATER
            [NotNullWhen(false)] 
#endif
            this IEnumerable<T>? list)
        {
            return list == null || !list.Any();
        }

        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T>? target)
        {
            list.IsRequired();

            if (target.IsNullOrEmpty())
                return;

            foreach (T item in target.EmptyIfNull())
                list.Add(item);
        }

        /// <summary>
        /// removes an element from the list based on a predicate delegate
        /// </summary>
        public static void Remove<T>(this ICollection<T>? list, Predicate<T> test)
        {
            if (list == null) return;

            test.IsRequired();

            foreach (var item in list.ToList())
                if (test(item))
                    list.Remove(item);
        }

        public static void RemoveEmpty(this ICollection<string>? list) => list.Remove(string.IsNullOrWhiteSpace);

        /// <summary>
        /// returns a new List<T> if the input is null
        /// </summary>
#if NETSTANDARD2_1_OR_GREATER
        [return: NotNull]
#endif
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? list)
        {
            return list ?? new List<T>();
        }

        /// <summary>
        /// evaluates two lists and compares their contents to each other
        /// if any one of the items in the lists don't match, returns false
        /// if items are the same but are in different positions, returns false
        /// if null at same position in both lists, returns true
        /// </summary>
        public static bool IsEqualTo<T>(this IEnumerable<T>? first, IEnumerable<T>? second, IEqualityComparer<T>? comparer = null)
        {
            if (first == null && second == null)
                return true;

            if ((first == null && second != null) || (first != null && second == null))
                return false;

            if (first.Count() != second.Count())
                return false;

            for (int index = 0; index < first.Count(); index++)
            {
                T firstItem = first.ElementAt(index);
                T secondItem = second.ElementAt(index);

                if (firstItem == null && secondItem == null)
                    continue;
                else
                {
                    if (comparer != null)
                    {
                        return comparer.Equals(firstItem, secondItem);
                    }
                    else
                    {
                        if (firstItem != null && !firstItem.Equals(secondItem))
                            return false;
                    }
                }
            }

            return true;
        }

        public static bool IsEqualTo<T>(this IEnumerable<T>? x, params T[]? y) => IsEqualTo(x, y, comparer: null);

        public static bool IsEqualTo<T>(this IEnumerable<T>? x, IEqualityComparer<T>? comparer, params T[]? y) => IsEqualTo(x, y, comparer);

        /// <summary>
        /// selects only the Distinct elements in the list by comparing values in the provided expression.
        ///     <see cref="KeyComparer{T, TResult}"/>
        /// </summary>
        /// <example>myList.Distinct(x => x.Id);</example>
        /// <param name="source">list to select elements from</param>
        /// <param name="keySelector">expression to select values from the elements to compare</param>
        public static IEnumerable<T> Distinct<T, TResult>(this IEnumerable<T>? source, Func<T, TResult> keySelector)
            where T : class
            where TResult : IEquatable<TResult>
        {
            return source.EmptyIfNull().Distinct(new KeyComparer<T, TResult>(keySelector));
        }

        /// <summary>
        /// inserts the given item into the list ONCE when the predicate is satisfied, only on the first match.
        /// </summary>
        public static void InsertBefore<T>(this IList<T> list, Func<T, bool> test, T item)
        {
            list.IsRequired();
            test.IsRequired();

            for (int index = 0; index < list.Count; index++)
            {
                if (test(list[index]))
                {
                    list.Insert(index, item);
                    break;
                }
            }
        }

        /// <summary>
        /// reducer similar to System.Linq.Enumerable.Single() but with better error messages
        /// </summary>
        public static T FindOne<T>(this IEnumerable<T>? list, Func<T, bool> test)
        {
            var candidates = list.EmptyIfNull().Where(test);
            if (candidates.IsNullOrEmpty())
                throw new ArgumentException($"Cannot find matching '{typeof(T)}' in the list");

            if (candidates.Count() > 1)
                throw new ArgumentException($"Found more than one matching '{typeof(T)}' in the list");

            return candidates.Single();
        }

        public static T FindOne<T>(this IEnumerable<T>? list) => list.FindOne(_ => true);

        public static bool TryGetFirst<T>(this IEnumerable<T>? list, Func<T, bool> test, out T? found)
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

        public static int IndexOf<T>(this IEnumerable<T>? list, Func<T?, bool> predicate, int start = 0)
        {
            predicate.IsRequired();

            if (!list.IsNullOrEmpty())
            {
                int count = list.Count();
                for (int index = start; index < count; index++)
                {
                    if (predicate.Invoke(list.ElementAt(index)))
                        return index;
                }
            }

            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T>? list, T item, int start = 0)
        {
            item.IsRequired();

            return list.IndexOf(x => x?.Equals(item) == true, start);
        }


        public static int NullSafeCount<T>(this IEnumerable<T>? list)
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
        public static IEnumerable<TSource> Merge<TSource, TKey>(this IEnumerable<TSource>? first,
            IEnumerable<TSource>? second,
            Func<TSource, TKey> keySelector,
            Func<TSource, TSource, TSource> merge)
            where TSource : class, new()
        {
            keySelector.IsRequired();
            merge.IsRequired();

            var groups = first.EmptyIfNull()
                          .Concat(second.EmptyIfNull())
                          .GroupBy(keySelector);
            foreach (var group in groups)
                yield return group.Aggregate(new TSource(), merge);
        }

        public static bool ContainsAll<T>(this IEnumerable<T>? list, IEnumerable<T>? second)
        {
            if (list.IsNullOrEmpty() && second.IsNullOrEmpty())
                return true;

            if (list.IsNullOrEmpty() && !second.IsNullOrEmpty())
                return false;

            var intersection = list.Intersect(second.EmptyIfNull());
            return intersection.Count() == second.NullSafeCount();
        }

        public static bool ContainsAny<T>(this IEnumerable<T>? list, IEnumerable<T>? second)
        {
            if (list.IsNullOrEmpty() || second.IsNullOrEmpty())
                return false;

            return list.Any(t => second.Contains(t));
        }

        public static bool ContainsEquivalent(this IEnumerable<string>? input, string? target)
        {
            return input != null && input.Any(str => target.IsEquivalent(str));
        }

        /// <summary>
        /// compares 2 enumerables to see if they both contain the same number and equivalent elements, using a custom
        /// selector to compare keys. returns false if any element is different. defaults to verifying the order is also
        /// equivalent; pass `maintainOrder:false` to allow a different order of elements.
        /// </summary>
        public static bool ContainsEquivalent<T, TResult>(this IEnumerable<T>? x, IEnumerable<T>? y, Func<T, TResult> keySelector, bool maintainOrder = true)
            where T : class
            where TResult : IEquatable<TResult>
        {
            return ContainsEquivalent(x, y, new KeyComparer<T, TResult>(keySelector), maintainOrder);
        }

        /// <summary>
        /// compares 2 enumerables to see if they both contain the same number and equivalent entities. returns false if any element is different. 
        /// defaults to verifying the order is also equivalent; pass `maintainOrder:false` to allow a different order of elements.
        /// </summary>
        public static bool ContainsEquivalent<T>(this IEnumerable<T>? x, IEnumerable<T>? y, IEqualityComparer<T>? comparer = null, bool maintainOrder = true)
        {
            if (x == null || y == null)
                throw new ArgumentNullException("collection is required");

            if (ReferenceEquals(x, y))
                return true;

            if (x.Count() != y.Count())
                return false;

            comparer ??= EqualityComparer<T>.Default;

            int xCount = x.Count();
            for (int index = 0; index < xCount; index++)
            {
                if (maintainOrder)
                {
                    if (!comparer.Equals(x.ElementAt(index), y.ElementAt(index)))
                        return false;
                }
                else
                {
                    var xVal = x.ElementAt(index);
                    if (!y.Any(yVal => comparer.Equals(xVal, yVal)))
                        return false;
                }
            }

            return true;
        }

        public static void RemoveOutliers(ICollection<long> input, double percentile)
        {
            input.IsRequired();

            if (percentile < 0 || percentile > 1)
                throw new ArgumentOutOfRangeException("Percentile must be greater than 0 and less than 1");

            List<long> set = input.OrderBy(x => x).ToList();

            int topIndex = (int)Math.Round((1 - percentile) * set.Count, MidpointRounding.AwayFromZero);
            int bottomIndex = (int)Math.Round(percentile * set.Count, MidpointRounding.AwayFromZero);

            long topThreshold = set[topIndex - 1];
            long bottomThreshold = set[bottomIndex];

            input.Remove(x => x < bottomThreshold || x > topThreshold);
        }

        public static IEnumerable<string> ToStrings<T>(this IEnumerable<T>? list)
        {
            foreach (T item in list.EmptyIfNull())
                yield return item?.ToString() ?? "<null>";
        }

        public static string ToDisplayString<T>(this IEnumerable<T>? list, string separator = ",") => list != null ? string.Join(separator, list.ToStrings()) : string.Empty;

        public static IEnumerable<T> Append<T>(this IEnumerable<T>? input, T item)
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
        public static IEnumerable<T> Flatten<T>(this T? start, Func<T, T?> getChild)
            where T : class
        {
            getChild.IsRequired();
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

        public static IEnumerable<Exception> FlattenAggregate(this AggregateException? aggEx)
        {
            if (aggEx != null)
                foreach (var ex in aggEx.InnerExceptions)
                    foreach (var childEx in ex.Flatten(x => x?.InnerException))
                        yield return childEx;
        }

        /// <summary>
        /// converts a <see cref="System.Collections.Specialized.NameValueCollection"/> into a 
        /// <see cref="System.Collections.Generic.IDictionary{string,string}"/>
        /// </summary>
        public static IDictionary<string, string> ToDictionary(this NameValueCollection? pairs)
        {
            var output = new Dictionary<string, string>();
            if (pairs != null && pairs.Count > 0)
            {
                for (int index = 0; index < pairs.Count; index++)
                {
                    output[pairs.GetKey(index)] = pairs.Get(index);
                }
            }
            return output;
        }

        /// <summary>
        /// adds the properties of a POCO as key/value pairs to the target collection. can also
        /// be called on Dictionary{TKey, TValue}.
        /// </summary>
        public static void AddValues(this ICollection<KeyValuePair<string, object>> list, object? values)
        {
            list.IsRequired();

            if (values != null)
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(values);
                foreach (PropertyDescriptor item in properties)
                {
                    object value = item.GetValue(values);
                    list.Add(new KeyValuePair<string, object>(item.Name, value));
                }
            }
        }

        /// <summary>
        /// converts a POCO into a list of key/value pairs
        /// </summary>
        public static IReadOnlyList<KeyValuePair<string, object>> CreateList(object? values)
        {
            var list = new List<KeyValuePair<string, object>>();
            if (values != null)
                list.AddValues(values);
            return list.AsReadOnly();
        }

        /// <summary>
        /// helper method to combine possible duplicate elements in a list, and combine a children collection. useful when using
        /// Dapper's "map" parameter to fetch child collections.
        /// </summary>
        /// <typeparam name="T">Type of the primary entity</typeparam>
        /// <typeparam name="TKey">Type the primary entity uses as a Key (usually `int`)</typeparam>
        /// <typeparam name="TChild">Type of the entities used in a child collection</typeparam>
        /// <param name="list">target list that may contain duplicate entries</param>
        /// <param name="groupingSelector">selector fn to fetch the key for the entity</param>
        /// <param name="childSelector">selector fn to fetch the child collection to combine</param>
        /// <returns>a normalized list of entities with no duplicates</returns>
        public static List<T> GroupAndCollect<T, TKey, TChild>(this IEnumerable<T>? list, Func<T, TKey> groupingSelector, Expression<Func<T, List<TChild>>> childSelector)
        {
            if (list == null) return new List<T>();

            groupingSelector.IsRequired();
            childSelector.IsRequired();

            PropertyInfo childProperty = (PropertyInfo)((MemberExpression)childSelector.Body).Member;
            return list.GroupBy(groupingSelector)
                .Where(group => !group.IsNullOrEmpty())
                .Select(group =>
                {
                    var single = group.First();
                    var children = group.SelectMany(childSelector.Compile()).ToList();
                    childProperty.SetValue(single, children);
                    return single;
                }).ToList();
        }

        public static IEnumerable<T>? Shuffle<T>(this IEnumerable<T>? list)
        {
            return list?.OrderBy(_ => Guid.NewGuid());
        }

        public static T? RandomOrDefault<T>(this IEnumerable<T>? list)
        {
            return list != null ? list.Shuffle().FirstOrDefault() : default;
        }

        public static void ActOnDifferences<T>(this IEnumerable<T>? originalList, IEnumerable<T>? newList,
            Action<T>? newAction = null,
            Action<T, T>? updateAction = null,
            Action<T>? deleteAction = null,
            IEqualityComparer<T>? comparer = null)
        {
            comparer ??= EqualityComparer<T>.Default;
            var toRemove = originalList.EmptyIfNull().ToList();

            foreach (T newElement in newList.EmptyIfNull())
            {
                var originalElement = originalList.FirstOrDefault(x => comparer.Equals(x, newElement));
                if (originalElement != null)
                {
                    toRemove.Remove(originalElement);
                    updateAction?.Invoke(newElement, originalElement);
                }
                else // element is not in the original list
                {
                    newAction?.Invoke(newElement);
                }
            }

            foreach (var deletedElement in toRemove)
            {
                deleteAction?.Invoke(deletedElement);
            }
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
        public static IOrderedEnumerable<T> OrderByNatural<T>(this IEnumerable<T>? items, Func<T, string> selector, StringComparer? stringComparer = null)
        {
            if (items == null) items = new T[0];
            selector.IsRequired();

            var regex = new Regex(@"\d+", RegexOptions.Compiled);

            int maxDigits = items
                          .SelectMany(i => regex.Matches(selector(i)).Cast<Match>().Select(digitChunk => (int?)digitChunk.Value.Length))
                          .Max() ?? 0;

            return items.OrderBy(i => regex.Replace(selector(i), match => match.Value.PadLeft(maxDigits, '0')), stringComparer ?? StringComparer.CurrentCulture);
        }

    }
}
