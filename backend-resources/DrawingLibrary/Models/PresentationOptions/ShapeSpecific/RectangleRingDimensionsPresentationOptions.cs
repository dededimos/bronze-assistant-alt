using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.PresentationOptions.ShapeSpecific
{
    public class RectangleRingDimensionsPresentationOptions : ShapeDimensionsPresentationOptions, IDeepClonable<RectangleRingDimensionsPresentationOptions>
    {

        public bool ShowThickness { get; set; }
        public RectangleRingThicknessDimensionPosition ThicknessPosition { get; set; } = RectangleRingThicknessDimensionPosition.LeftMiddle;
        public DimensionLineOptions ThicknessLineOptions { get; set; } = new();
        public DrawingPresentationOptions ThicknessPresentationOptions { get; set; } = new();
        public double ThicknessMarginFromShape { get; set; }
        
        public bool ShowInnerLength { get; set; }
        public RectangleLengthDimensionPosition InnerLengthPosition { get; set; } = RectangleLengthDimensionPosition.Bottom;
        public DimensionLineOptions InnerLengthLineOptions { get; set; } = new();
        public DrawingPresentationOptions InnerLengthPresentationOptions { get; set; } = new();
        public double InnerLengthMarginFromShape { get; set; }

        public bool ShowInnerHeight { get; set; }
        public RectangleHeightDimensionPosition InnerHeightPosition { get; set; } = RectangleHeightDimensionPosition.Right;
        public DimensionLineOptions InnerHeightLineOptions { get; set; } = new();
        public DrawingPresentationOptions InnerHeightPresentationOptions { get; set; } = new();
        public double InnerHeightMarginFromShape { get; set; }

        public override RectangleRingDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (RectangleRingDimensionsPresentationOptions)base.GetDeepClone();
            clone.ThicknessLineOptions = this.ThicknessLineOptions.GetDeepClone();
            clone.ThicknessPresentationOptions = this.ThicknessPresentationOptions.GetDeepClone();
            clone.InnerLengthLineOptions = this.InnerLengthLineOptions.GetDeepClone();
            clone.InnerLengthPresentationOptions = this.InnerLengthPresentationOptions.GetDeepClone();
            clone.InnerHeightLineOptions = this.InnerHeightLineOptions.GetDeepClone();
            clone.InnerHeightPresentationOptions = this.InnerHeightPresentationOptions.GetDeepClone();
            return clone;
        }
    }
}
