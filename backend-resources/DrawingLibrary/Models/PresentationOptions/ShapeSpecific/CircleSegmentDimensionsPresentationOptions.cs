using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.PresentationOptions.ShapeSpecific
{
    public class CircleSegmentDimensionsPresentationOptions : ShapeDimensionsPresentationOptions, IDeepClonable<CircleSegmentDimensionsPresentationOptions>
    {
        public bool ShowCircleRadius { get; set; }
        private bool showCircleDiameter;
        public bool ShowDiameter { get => ShowCircleRadius ? false : showCircleDiameter; set => showCircleDiameter = value; }
        public bool ShowChordLength { get; set; }
        public bool ShowHelpCircleExtension { get; set; }
        public double RadiusDiameterMarginFromShape { get; set; }
        public double ChordLengthMarginFromShape { get; set; }

        public DimensionLineOptions RadiusLineOptions { get; set; } = new();
        public DimensionLineOptions DiameterLineOptions { get; set; } = new();
        public DimensionLineOptions ChordLengthLineOptions { get; set; } = new();
        public DrawingPresentationOptions RadiusPresentationOptions { get; set; } = new();
        public DrawingPresentationOptions DiameterPresentationOptions { get; set; } = new();
        public DrawingPresentationOptions ChordLengthPresentationOptions { get; set; } = new();
        /// <summary>
        /// The Extension of the Circle to Show radius e.t.c.
        /// </summary>
        public DrawingPresentationOptions CircleExtensionPresentationOptions { get; set; } = new();

        public override CircleSegmentDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (CircleSegmentDimensionsPresentationOptions)base.GetDeepClone();
            clone.RadiusLineOptions = this.RadiusLineOptions.GetDeepClone();
            clone.DiameterLineOptions = this.DiameterLineOptions.GetDeepClone();
            clone.ChordLengthLineOptions = this.ChordLengthLineOptions.GetDeepClone();
            clone.RadiusPresentationOptions = this.RadiusPresentationOptions.GetDeepClone();
            clone.DiameterPresentationOptions = this.DiameterPresentationOptions.GetDeepClone();
            clone.ChordLengthPresentationOptions = this.ChordLengthPresentationOptions.GetDeepClone();
            clone.CircleExtensionPresentationOptions = this.CircleExtensionPresentationOptions.GetDeepClone();
            return clone;
        }
    }
}
