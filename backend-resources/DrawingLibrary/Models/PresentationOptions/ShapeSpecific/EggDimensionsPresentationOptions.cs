using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.PresentationOptions.ShapeSpecific
{
    public class EggDimensionsPresentationOptions : ShapeDimensionsPresentationOptions, IDeepClonable<EggDimensionsPresentationOptions>
    {
        public bool ShowCircleRadius { get; set; }
        public bool ShowEllipseRadius { get; set; }

        public EggCircleRadiusDimensionPosition CircleRadiusDimensionsPosition { get; set; }

        public DimensionLineOptions EllipseRadiusLineOptions { get; set; } = new();
        public DimensionLineOptions CircleRadiusLineOptions { get; set; } = new();

        public DrawingPresentationOptions EllipseRadiusPresentationOptions { get; set; } = new();
        public DrawingPresentationOptions CircleRadiusPresentationOptions { get; set; } = new();

        public override EggDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (EggDimensionsPresentationOptions)base.GetDeepClone();
            clone.EllipseRadiusLineOptions = this.EllipseRadiusLineOptions.GetDeepClone();
            clone.CircleRadiusLineOptions = this.CircleRadiusLineOptions.GetDeepClone();
            clone.EllipseRadiusPresentationOptions = this.EllipseRadiusPresentationOptions.GetDeepClone();
            clone.CircleRadiusPresentationOptions = this.CircleRadiusPresentationOptions.GetDeepClone();
            return clone;
        }
    }
}
