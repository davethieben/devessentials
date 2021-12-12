using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Essentials.Helpers
{
    public class Compare
    {
        /// <summary>
        /// compares the contents of two lists, and creates change reports based on
        /// elements that have been changed (indicated by the `equals` delegate parameter), added or deleted,
        /// based on the `getHashCode` delegate parameter. 
        /// note - in order for an element to be considered "changed", it must have the same hash code but
        /// fail the `equals` test. i.e., { Id = 1, Name = "old" } vs. { Id = 1, Name = "new" } 
        /// refer to <see cref="KeyComparer{T, TResult}"/> for lists of <see cref="Models.IEntity"/> entities.
        /// entities can override `Equals` to determine if an entity is "changed".
        /// </summary>
        /// <example>
        /// most cases for this method will be against child collections on a domain entity:
        /// ```
        ///     office.OfficeLeadRoleUsers.CompareAndAct(oldOffice.OfficeLeadRoleUsers,
        ///         // compare vital info that must be updated:
        ///         equalityComparer: (x,y) => (x.LeadRoleId, x.UserId).Equals((y.LeadRoleId, y.UserId)), 
        ///         onChanged: (x, y) => logger.Log($"{x} was changed to {y}"),
        ///         onAdded: (x) => logger.Log($"{x} was added"),
        ///         onRemoved: (x) => logger.Log($"{x} was removed"));
        /// ```
        /// </example>
        /// <typeparam name="T">Type of the elements in both lists</typeparam>
        /// <param name="xList">original list of elements</param>
        /// <param name="yList">modified list of elements</param>
        /// <param name="getHashCode">delegate to be used to find the identity of an element. if null, the default 
        ///     `Object.GetHashCode()` is used.</param>
        /// <param name="equals">delegate to be used to test if the contents of an element have
        ///     changed, by comparing it to he original element. if null, the default `Object.Equals(object)` is used.</param>
        /// <returns>a reporting object containing the details of the differences in the two lists. the report can 
        ///     iterated to perform actions based on the results.</returns>
        public static Compare<T> Collection<T>(
            IEnumerable<T> xList,
            IEnumerable<T> yList,
            Func<T, int>? getHashCode = null,
            Func<T, T, bool>? equals = null)
        {
            if (getHashCode == null) getHashCode = (x) => x?.GetHashCode() ?? 0;
            if (equals == null)
            {
                if (typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
                    equals = (x, y) => x is IEquatable<T> eqx && eqx.Equals((T)y);
                else
                    equals = (x, y) => x != null && x.Equals(y);
            }

            return new Compare<T>(xList, yList, getHashCode, equals);
        }
    }

    public class Compare<T> : IEnumerable<CollectionComparison<T>>
    {
        private IEnumerable<CollectionComparison<T>> _changes;

        internal Compare(IEnumerable<T> xList, IEnumerable<T> yList, Func<T, int> getHashCode, Func<T, T, bool> equals)
        {
            _changes = CreateComparison(xList, yList, getHashCode, equals);
        }

        /// <summary>
        /// generates the actual differences report. note this uses a yield enumerator for lazy evaluation
        /// </summary>
        private static IEnumerable<CollectionComparison<T>> CreateComparison(
            IEnumerable<T> xList,
            IEnumerable<T> yList,
            Func<T, int> getHashCode,
            Func<T, T, bool> equals)
        {
            getHashCode.IsRequired();
            equals.IsRequired();

            if (xList == null) xList = new List<T>();
            if (yList == null) yList = new List<T>();

            List<T> xRemaining = xList.ToList(); // make a copy to be able to manipulate

            int yCount = yList.Count();
            for (int index = 0; index < yCount; index++)
            {
                T yElement = yList.ElementAt(index);
                int yHash = getHashCode(yElement);

                T xElement = xRemaining.FirstOrDefault(x => getHashCode(x) == yHash);
                if (xElement != null)
                {
                    if (!equals.Invoke(xElement, yElement))
                    {
                        yield return new CollectionComparison<T>
                        {
                            Type = ChangeType.Changed,
                            OldValue = xElement,
                            NewValue = yElement
                        };
                    }

                    xRemaining.Remove(xElement);
                }
                else
                {
                    yield return new CollectionComparison<T>
                    {
                        Type = ChangeType.Added,
                        NewValue = yElement
                    };
                }
            }

            foreach (T xElement in xRemaining)
            {
                yield return new CollectionComparison<T>
                {
                    Type = ChangeType.Removed,
                    OldValue = xElement,
                };
            }
        }

        public IEnumerator<CollectionComparison<T>> GetEnumerator() => _changes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _changes.GetEnumerator();

        /// <summary>
        /// perform some action on all elements that have been added
        /// </summary>
        /// <param name="action">callback with the added element</param>
        /// <returns>itself</returns>
        public Compare<T> OnAdded(Action<T> action)
        {
            foreach (var change in _changes.Where(x => x.Type == ChangeType.Added))
                if (change.NewValue != null)
                    action?.Invoke(change.NewValue);
            return this;
        }

        /// <summary>
        /// perform some action on all elements that have been changed
        /// </summary>
        /// <param name="action">callback with the old element and the new element</param>
        /// <returns>itself</returns>
        public Compare<T> OnChanged(Action<T, T> action)
        {
            foreach (var change in _changes.Where(x => x.Type == ChangeType.Changed))
                if (change.OldValue != null && change.NewValue != null)
                    action?.Invoke(change.OldValue, change.NewValue);
            return this;
        }

        /// <summary>
        /// perform some action on all elements that have been added OR changed 
        /// </summary>
        /// <param name="action">callback with the old element and the new (or added) element.</param>
        /// <returns>itself</returns>
        public Compare<T> OnAddedOrChanged(Action<T?, T> action)
        {
            foreach (var change in _changes.Where(x => x.Type == ChangeType.Added || x.Type == ChangeType.Changed))
                if (change.NewValue != null)
                    action?.Invoke(change.OldValue, change.NewValue);
            return this;
        }

        /// <summary>
        /// perform some action on all elements that have been removed
        /// </summary>
        /// <param name="action">callback with the removed element</param>
        /// <returns>itself</returns>
        public Compare<T> OnRemoved(Action<T> action)
        {
            foreach (var change in _changes.Where(x => x.Type == ChangeType.Removed))
                if (change.OldValue != null)
                    action?.Invoke(change.OldValue);
            return this;
        }

    }

    public enum ChangeType { Added, Changed, Removed };

    public class CollectionComparison<T>
    {
        public ChangeType Type { get; set; }

        public T? OldValue { get; set; }

        public T? NewValue { get; set; }

    }

}
