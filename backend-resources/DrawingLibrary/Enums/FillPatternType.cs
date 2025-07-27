using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Enums
{
    /// <summary>
    /// The Pattern Type to use as Fill
    /// </summary>
    public enum FillPatternType
    {
        NoPattern = 0,
        DotPattern = 1,
        HatchLine45DegPattern = 2,
        HatchLine225DegPattern = 3,
        HatchLineHorizontalPattern = 4,
        HatchLineVerticalPattern = 5,
    }
}
