using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    public enum GlassTypeEnum
    {
        GlassTypeNotSet = 0,
        [Description("Σταθερό Γυαλί")]
        FixedGlass = 1,
        [Description("Κινητό Γυαλί")]
        DoorGlass = 2,
        [Description("Ειδικό Γυαλί")]
        SpecialGlass = 3,
        [Description("Ημικυκλικό Γυαλί")]
        DoorGlassSemicircle = 4,
    }
}
