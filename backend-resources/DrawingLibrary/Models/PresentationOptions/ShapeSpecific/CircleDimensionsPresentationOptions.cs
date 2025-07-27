using CommonInterfacesBronze;

namespace DrawingLibrary.Models.PresentationOptions.ShapeSpecific
{
    public class CircleDimensionsPresentationOptions : ShapeDimensionsPresentationOptions, IDeepClonable<CircleDimensionsPresentationOptions>
    {
        private bool showDiameter;
        /// <summary>
        /// When Radius or Length or Height is show diameter is always hidden . Otherwise diameter is shown based on the Selected option
        /// </summary>
        public bool ShowDiameter { get => (!ShowRadius && !ShowLength && !ShowHeight) ? showDiameter : false; set => showDiameter = value; }
        
        private bool showRadiusDimension;
        /// <summary>
        /// When Length or Height is shown Radius is always hidden. Otherwise Radius is shown if not hidden (Diameter is hidden when Radius is Shown)
        /// </summary>
        public bool ShowRadius { get => (ShowHeight || ShowLength) ? false : showRadiusDimension; set => showRadiusDimension = value; }
        
        /// <summary>
        /// Weather to show the Dimension Inside the Circle instead of outside
        /// </summary>
        public bool ShowDiameterRadiusInsideShape { get; set; }
        /// <summary>
        /// The starting Position of the Radius Diameter Dimension
        /// </summary>
        public double RadiusDiameterPositionAngleRadians { get; set; }

        public double DiameterRadiusMarginFromShape { get; set; }
        public DimensionLineOptions DiameterLineOptions { get; set; } = new();
        public DimensionLineOptions RadiusLineOptions { get; set; } = new();
        public DrawingPresentationOptions DiameterRadiusPresentationOptions { get; set; } = new();

        public override CircleDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (CircleDimensionsPresentationOptions)base.GetDeepClone();
            clone.DiameterLineOptions = this.DiameterLineOptions.GetDeepClone();
            clone.RadiusLineOptions = this.RadiusLineOptions.GetDeepClone();
            clone.DiameterRadiusPresentationOptions = this.DiameterRadiusPresentationOptions.GetDeepClone();
            return clone;
        }
    }
}
