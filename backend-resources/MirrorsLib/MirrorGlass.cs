using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements.Sandblasts;
using ShapesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib
{
    public class MirrorGlass : ICodeable
    {
        public string Code { get; set; } = string.Empty;
        public ShapeInfo DimensionsInformation { get; set; } = ShapeInfo.Undefined();
        public MirrorGlassThickness Thickness { get; set; }
        public MirrorGlassType GlassType { get; set; }
        public List<ShapeInfo> Sandblasts { get; set; } = [];
        public List<ShapeInfo> Processes { get; set; } = [];
        public static MirrorGlass Undefined() => new();
    }
    //All the Mirror Shapes are placed inside the Mirror Based on the Mirror's coordinates.The Mirror Glass is always is inside the Mirror
    //There is no need to change the coordinates of the items placed on the Mirror Glass , as they will actually be inside those of the Mirror , as long as the Mirror Glass also maintains its coordinates as inherited from the Mirrro Structure.
    //Though if we need any of the internal items to have positions relative to the center of the Mirror glass , we should transform the coordinates so that the glass's center for example is 0,0
    //But that is the job of the Technical drawing objects and not here. 
}
