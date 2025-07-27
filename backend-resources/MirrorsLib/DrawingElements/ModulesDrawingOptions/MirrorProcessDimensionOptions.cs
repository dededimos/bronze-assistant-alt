using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.DrawingElements.ModulesDrawingOptions
{
    public class MirrorProcessDimensionOptions : IDeepClonable<MirrorProcessDimensionOptions>
    {
        public ShapeDimensionsPresentationOptions BodyDimensionOptions { get; set; } = new();
        public bool ShowDistanceFromCenter { get; set; }

        public MirrorProcessDimensionOptions GetDeepClone()
        {
            throw new NotImplementedException();
        }
    }
}
