using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    public enum CabinThicknessEnum
    {//DO NOT CHANGE NUMBERING , THE NUMBERS ARE STORED IN THE DATABASE 
     //Have Changed the Numbering Unfortunately for Obvious Equality Reasons 16-10-2022
        [Description("N/A")]
        NotSet = 0,
        [Description("5mm")]
        Thick5mm = 1, //Was 0
        [Description("6mm")]
        Thick6mm = 2, // Was 1
        [Description("6-8mm")]
        Thick6mm8mm = 3, //Was 5
        [Description("8mm")]
        Thick8mm = 4, //Was 2
        [Description("8-10mm")]
        Thick8mm10mm = 5, //Was 6
        [Description("10mm")]
        Thick10mm = 6, //Was 3
        [Description("Tenplex")]
        ThickTenplex10mm = 7, //Was 4
    }
}
