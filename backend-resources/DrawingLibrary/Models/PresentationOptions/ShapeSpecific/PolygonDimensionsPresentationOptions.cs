using CommonInterfacesBronze;

namespace DrawingLibrary.Models.PresentationOptions.ShapeSpecific
{
    public class RegularPolygonDimensionsPresentationOptions : ShapeDimensionsPresentationOptions, IDeepClonable<RegularPolygonDimensionsPresentationOptions>
    {
        public bool ShowRadius { get; set; }
        public double RadiusMarginFromShape { get; set; }
        public DrawingPresentationOptions RadiusPresentationOptions { get; set; } = new();
        public DimensionLineOptions RadiusLineOptions { get; set; } = new();
        public DrawingPresentationOptions HelpCirclePresentationOptions { get; set; } = new();
        public bool ShowHelpCircle { get => ShowRadius; }

        public override RegularPolygonDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (RegularPolygonDimensionsPresentationOptions)base.GetDeepClone();
            clone.RadiusPresentationOptions = this.RadiusPresentationOptions.GetDeepClone();
            clone.RadiusLineOptions = this.RadiusLineOptions.GetDeepClone();
            clone.HelpCirclePresentationOptions = this.HelpCirclePresentationOptions.GetDeepClone();
            return clone;
        }
    }
}
