using CommonHelpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.Comparers
{
    /// <summary>
    /// An Equality Comparer for a <see cref="List{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListEqualityComparer<T> : IEqualityComparer<List<T>>
    {
        private IEqualityComparer<T> _itemsComparer = EqualityComparer<T>.Default;
        /// <summary>
        /// Sets the comparer for the Items of the List , if the comparer is not set the Default one is used
        /// </summary>
        /// <param name="itemsComparer"></param>
        public void SetItemsComparer(IEqualityComparer<T> itemsComparer) => _itemsComparer = itemsComparer;

        public bool Equals(List<T>? x, List<T>? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.SequenceEqual(y,_itemsComparer);
        }

        public int GetHashCode([DisallowNull] List<T> obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
