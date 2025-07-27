using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.Enums
{
    public enum MirrorSandblast
    {
        [Description("Χωρίς Αμμοβολή")]
        H7 = 0,
        [Description("Σχέδιο Η8")]
        H8 = 1,
        [Description("Σχέδιο Χ6")]
        X6 = 2,
        [Description("Σχέδιο Χ4")]
        X4 = 3,
        [Description("Σχέδιο 6000")]
        _6000 = 4,
        [Description("Σχέδιο Μ3")]
        M3 = 5,
        [Description("Χωρίς Αμμοβολή")]
        N9 = 6,
        [Description("Σχέδιο Ν6")]
        N6 = 7,
        [Description("Ειδικό")]
        Special = 8
    }
}
