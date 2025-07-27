using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    public enum CabinFinishEnum
    {//DO NOT CHANGE NUMBERING , THE ENUM IS STORED WITH THESE NUMBERS IN THE DATABASE
        [Description("Γυαλιστερό")]
        Polished = 0,
        [Description("Ματ")]
        Brushed = 1,
        [Description("Μαύρο Ματ")]
        BlackMat = 2,
        [Description("Λευκό Ματ")]
        WhiteMat = 3,
        [Description("Αντικέ")]
        Bronze = 4,
        [Description("Χρυσό Ματ")]
        BrushedGold = 5,
        [Description("Χρυσό")]
        Gold = 6,
        [Description("Χαλκού-Copper")]
        Copper = 7,
        [Description("Ειδικό-Άλλο")]
        Special = 8,
        [Description("Απροσδιόριστο")]
        NotSet = 9
    }
}
