using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    public enum CoverDistanceEnum
    {// DO NOT CHANGE NUMBERING IT GETS SAVED TO THE DATABASE
        [Description("Μέγιστο Άνοιγμα")]
        MaxOpening = 0,
        [Description("Ίσα Γυαλιά")]
        SameGlasses = 1,
        [Description("Προσαρμοσμένο")]
        Customized = 2

    }
}
