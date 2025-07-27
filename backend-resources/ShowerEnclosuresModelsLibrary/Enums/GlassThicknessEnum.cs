using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    public enum GlassThicknessEnum
    {
        GlassThicknessNotSet = 0,
        [Description("5mm")]
        Thick5mm = 1,
        [Description("6mm")]
        Thick6mm = 2,
        [Description("8mm")]
        Thick8mm = 3,
        [Description("10mm")]
        Thick10mm = 4,
        [Description("TenPlex")]
        ThickTenplex10mm = 5
    }
}
