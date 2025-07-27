using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    /// <summary>
    /// Defines a Direction for Cabins.
    /// Looking from Outside the Cabin where its Fixed Part is Positioned (If there is No Fixed Part then where the Door is Positioned)
    /// </summary>
    public enum CabinDirection
    {
        Undefined = 0,
        LeftSided = 1,
        RightSided = 2
    }
}
