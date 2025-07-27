using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.Enums
{
    public enum MirrorLight
    {
        [Description("Χωρίς")]
        WithoutLight = 0,
        [Description("Θερμό")]
        Warm = 1,
        [Description("Ψυχρό")]
        Cold = 2,
        [Description("Ημέρας")]
        Daylight = 3,
        [Description("Θερμό-Ψυχρό")]
        Warm_Cold = 4,
        [Description("Θερ-Ψυχ-Ημέρ")]
        Warm_Cold_Day = 5,
        [Description("COB Ημέρας")]
        Day_COB = 6,
        [Description("COB Θερμό")]
        Warm_COB = 7,
        [Description("COB Ψυχρό")]
        Cold_COB = 8,
        [Description("COB Θερ-Ψυχ-Ημέρ")]
        Warm_Cold_Day_COB = 9,
        [Description("Ημέρας 16Watt")]
        Day_16W = 10,
        [Description("Θερμό 16Watt")]
        Warm_16W = 11,
        [Description("Ψυχρό 16Watt")]
        Cold_16W = 12,
        [Description("Θερ-Ψυχ-Ημέρ 16Watt")]
        Warm_Cold_Day_16W = 14,
    }
}
