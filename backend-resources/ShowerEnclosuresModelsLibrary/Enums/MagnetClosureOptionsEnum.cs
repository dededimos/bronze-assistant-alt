using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerEnclosuresModelsLibrary.Enums
{
    public enum MagnetClosureOptionsEnum
    {// DO NOT CHANGE NUMBERING IT GETS SAVED TO THE DATABASE
        [Description("Αλουμίνιο Μαγνήτη")]
        WithMagnet = 0,
        [Description("Πολυκαρβονικό")]
        WithPolycarbonic = 1,
        [Description("Χωρίς Λάστιχο")]
        WithoutStrip = 2

    }
}
