using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesBronze
{
    /// <summary>
    /// An Object That Can Be Deep Cloned
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeepClonable<out T>
    {

        /// <summary>
        /// Get A Deep Clone of the Requested Object
        /// </summary>
        /// <returns></returns>
        public T GetDeepClone();
    }
}
