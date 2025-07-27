using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesBronze
{
    public static class CommonExtensions2
    {
        public static List<T> GetDeepClonedList<T>(this List<T> list)
            where T : IDeepClonable<T>
        {
            return list.Select(item => item.GetDeepClone()).ToList();
        }
    }
}
