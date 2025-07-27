using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    public enum CabinHBLengthCalculationEnum
    {// DO NOT CHANGE NUMBERING IT GETS SAVED TO THE DATABASE
        [Description("Μήκος Πόρτας")]
        ByDoorLength =0,
        [Description("Μήκος Σταθερού")]
        ByFixedLength = 1,
    }
}
