using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.PresentationOptions.ShapeSpecific
{
    public class CircleRingDimensionsPresentationOptions : CircleDimensionsPresentationOptions, IDeepClonable<CircleRingDimensionsPresentationOptions>
    {
        public bool ShowThickness { get; set; }
        public DimensionLineOptions ThicknessLineOptions { get; set; } = new();
        public DrawingPresentationOptions ThicknessPresentationOptions { get; set; } = new();
        public RectangleRingThicknessDimensionPosition ThicknessPosition { get; set; }

        public override CircleRingDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (CircleRingDimensionsPresentationOptions)base.GetDeepClone();
            clone.ThicknessLineOptions = this.ThicknessLineOptions.GetDeepClone();
            clone.ThicknessPresentationOptions = this.ThicknessPresentationOptions.GetDeepClone();
            return clone;
        }
    }
}
