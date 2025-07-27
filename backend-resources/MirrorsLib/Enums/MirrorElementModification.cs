using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Enums
{
    public enum MirrorElementModification
    {
        AnyElementModification,
        BluetoothModification = 1,
        MagnifierSandblastedModification = 2,
        MagnifierModification = 3,
        MirrorBackLidModification = 4,
        MirrorLampModification = 5,
        ResistancePadModification = 6,
        ScreenModuleModification = 7,
        TouchButtonModification = 8,
        TransformerModification = 9,
        ProcessModification = 10,
        LightModification = 11,
        CustomElementModification = 12,
        FinishModification = 14,
    }
    public enum MirrorModificationType
    {
        AnyModificationType = 0,
        Removal = 1,
        Addition = 2,
        Change = 3,
    }
}
