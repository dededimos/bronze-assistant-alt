using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    public enum CabinWFinalHeightEnum
    {// DO NOT CHANGE NUMBERING IT GETS SAVED TO THE DATABASE
        [Description("Με Αντιρήδα")]
        WithSupportBar = 0,
        [Description("Χωρίς Αντιρίδα")]
        NoSupportBar = 1,
    }
}
