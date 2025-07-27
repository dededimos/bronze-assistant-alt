using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.Enums
{
    public enum MirrorShape
    {
        [Description("Ορθογωνικός")]
        Rectangular = 0,
        [Description("Κυκλικός")]
        Circular = 1,
        [Description("Κάψουλα")]
        Capsule = 2,
        [Description("Έλειψη")]
        Ellipse = 3,
        [Description("Ειδικό-Σχήμα")]
        Special = 4,

        [Description("Πέτρα Σειρά NS")]
        StoneNS = 5 ,
        [Description("Βότσαλο Σειρά ND")]
        PebbleND = 6,
        [Description("Τμήμα Κύκλου Κομμένο με μία χορδή")]
        CircleSegment = 7,
        [Description("Τμήμα Κύκλου Κομμένο με δύο χορδές")]
        CircleSegment2 = 8,
    }
}
