using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.PresentationOptions.ShapeSpecific
{
    public class EllipseDimensionsPresentationOptions : ShapeDimensionsPresentationOptions, IDeepClonable<EllipseDimensionsPresentationOptions>
    {
        public bool ShowRadiusMajor { get; set; }
        public bool ShowRadiusMinor { get; set; }
        public EllipseRadiusDimensionPosition RadiusDimensionPosition { get; set; }
        public DimensionLineOptions RadiusMajorLineOptions { get; set; } = new();
        public DimensionLineOptions RadiusMinorLineOptions { get; set; } = new();
        public DrawingPresentationOptions RadiusMajorPresentationOptions { get; set; } = new();
        public DrawingPresentationOptions RadiusMinorPresentationOptions { get; set; } = new();

        public override EllipseDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (EllipseDimensionsPresentationOptions)base.GetDeepClone();
            clone.RadiusMajorLineOptions = this.RadiusMajorLineOptions.GetDeepClone();
            clone.RadiusMinorLineOptions = this.RadiusMinorLineOptions.GetDeepClone();
            clone.RadiusMajorPresentationOptions = this.RadiusMajorPresentationOptions.GetDeepClone();
            clone.RadiusMinorPresentationOptions = this.RadiusMinorPresentationOptions.GetDeepClone();
            return clone;
        }
    }
}
