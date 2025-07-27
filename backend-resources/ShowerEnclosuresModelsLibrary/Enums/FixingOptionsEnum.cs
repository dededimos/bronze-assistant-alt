using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    public enum FixingOptionsEnum
    {// DO NOT CHANGE NUMBERING IT GETS SAVED TO THE DATABASE
        [Description("Στοπάκι Δαπέδου")]
        FloorStopper = 0,
        [Description("Αλουμινάκι Δαπέδου")]
        FloorAluminium = 1,
        [Description("Περιμετρικό Πλαίσιο")]
        PerimetricalFrame = 2 ,
        [Description("ΓωνίεςΤοίχου & Στοπάκι")]
        WallSupportsFloorStopper = 3
    }
}
