using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.Enums
{
    [Flags]
    public enum MirrorDrawingView
    {
        None = 0,
        FrontView = 1,
        RearView = 2,
        SideView = 4,
    }
}
