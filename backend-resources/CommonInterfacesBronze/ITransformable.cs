using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesBronze
{
    /// <summary>
    /// Can Transform to a <typeparamref name="T"/> Object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITransformable<T>
    {
        /// <summary>
        /// Returns the <typeparamref name="T"/> Transformation of the object
        /// </summary>
        /// <returns></returns>
        T GetTransformation();
    }
}
