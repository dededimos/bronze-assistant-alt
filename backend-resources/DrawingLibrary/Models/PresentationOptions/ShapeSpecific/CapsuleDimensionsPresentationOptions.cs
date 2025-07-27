using CommonInterfacesBronze;
using DrawingLibrary.Enums;

namespace DrawingLibrary.Models.PresentationOptions.ShapeSpecific
{
    public class CapsuleDimensionsPresentationOptions : ShapeDimensionsPresentationOptions , IDeepClonable<CapsuleDimensionsPresentationOptions>
    {
        private bool showDiameter;
        /// <summary>
        /// Weather to show the Diameter of the Circles of the Capsule, if Radius is shown Diameter is ALWAYS hidden
        /// </summary>
        public bool ShowDiameter { get => ShowRadius ? false : showDiameter; set => showDiameter = value; }
        /// <summary>
        /// Weather to show the Radius of the Circles of the Capsule , if Radius is shown Diameter is ALWAYS hidden
        /// </summary>
        public bool ShowRadius { get; set; }
        public CapsuleRadiusDimensionPosition DiameterRadiusDimensionPosition { get; set; }
        public double DiameterRadiusMarginFromShape { get; set; }
        public DimensionLineOptions DiameterLineOptions { get; set; } = new();
        public DimensionLineOptions RadiusLineOptions { get; set; } = new();
        public DrawingPresentationOptions DiameterRadiusPresentationOptions { get; set; } = new();

        public bool ShowRectangleDimension { get; set; }
        public double RectangleDimensionMarginFromShape { get; set; }
        public CapsuleRectangleDimensionPosition RectangleDimensionPosition { get; set; }
        public DimensionLineOptions RectangleDimensionLineOptions { get; set; } = new();
        public DrawingPresentationOptions RectangleDimensionPresentationOptions { get; set; } = new();

        public bool ShowCircle1CenterHelpLines { get; set; }
        public bool ShowCircle2CenterHelpLines { get; set; }

        public override CapsuleDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (CapsuleDimensionsPresentationOptions)base.GetDeepClone();
            clone.DiameterLineOptions = this.DiameterLineOptions.GetDeepClone();
            clone.RadiusLineOptions = this.RadiusLineOptions.GetDeepClone();
            clone.DiameterRadiusPresentationOptions = this.DiameterRadiusPresentationOptions.GetDeepClone();
            clone.RectangleDimensionLineOptions = this.RectangleDimensionLineOptions.GetDeepClone();
            clone.RectangleDimensionPresentationOptions = this.RectangleDimensionPresentationOptions.GetDeepClone();
            return clone;
        }
    }
}
