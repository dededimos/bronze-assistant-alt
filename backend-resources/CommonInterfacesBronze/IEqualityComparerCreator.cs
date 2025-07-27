using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesBronze
{
    /// <summary>
    /// An Object which can generate its Comparer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEqualityComparerCreator<T>
    {
        /// <summary>
        /// Generates a new instance of a Comparer for <typeparamref name="T"/> 
        /// </summary>
        /// <returns></returns>
        public abstract static IEqualityComparer<T> GetComparer();
    }
}
