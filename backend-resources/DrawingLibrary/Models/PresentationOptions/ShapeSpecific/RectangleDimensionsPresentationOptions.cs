using CommonInterfacesBronze;
using DrawingLibrary.Enums;

namespace DrawingLibrary.Models.PresentationOptions.ShapeSpecific
{
    public class RectangleDimensionsPresentationOptions : ShapeDimensionsPresentationOptions , IDeepClonable<RectangleDimensionsPresentationOptions>
    {
        public bool ShowRadiuses { get; set; } = true;
        public RectangleRadiusDimensionShowOption RadiusOptionWhenAllZero { get; set; } = RectangleRadiusDimensionShowOption.ShowNone;
        public RectangleRadiusDimensionShowOption RadiusOptionWhenTotalRadius { get; set; } = RectangleRadiusDimensionShowOption.ShowTopLeftRadius;

        public double TopLeftRadiusMarginFromShape { get; set; }
        public DimensionLineOptions TopLeftRadiusLineOptions { get; set; } = new();
        public DrawingPresentationOptions TopLeftRadiusPresentationOptions { get; set; } = new();

        public double TopRightRadiusMarginFromShape { get; set; }
        public DimensionLineOptions TopRightRadiusLineOptions { get; set; } = new();
        public DrawingPresentationOptions TopRightRadiusPresentationOptions { get; set; } = new();

        public double BottomLeftRadiusMarginFromShape { get; set; }
        public DimensionLineOptions BottomLeftRadiusLineOptions { get; set; } = new();
        public DrawingPresentationOptions BottomLeftRadiusPresentationOptions { get; set; } = new();

        public double BottomRightRadiusMarginFromShape { get; set; }
        public DimensionLineOptions BottomRightRadiusLineOptions { get; set; } = new();
        public DrawingPresentationOptions BottomRightRadiusPresentationOptions { get; set; } = new();
       
        public override RectangleDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (RectangleDimensionsPresentationOptions)base.GetDeepClone();
            clone.TopRightRadiusLineOptions = this.TopRightRadiusLineOptions.GetDeepClone();
            clone.TopLeftRadiusLineOptions = this.TopLeftRadiusLineOptions.GetDeepClone();
            clone.BottomLeftRadiusLineOptions = this.BottomLeftRadiusLineOptions.GetDeepClone();
            clone.BottomRightRadiusLineOptions = this.BottomRightRadiusLineOptions.GetDeepClone();

            clone.TopRightRadiusPresentationOptions = this.TopRightRadiusPresentationOptions.GetDeepClone();
            clone.TopLeftRadiusPresentationOptions = this.TopLeftRadiusPresentationOptions.GetDeepClone();
            clone.BottomLeftRadiusPresentationOptions = this.BottomLeftRadiusPresentationOptions.GetDeepClone();
            clone.BottomRightRadiusPresentationOptions = this.BottomRightRadiusPresentationOptions.GetDeepClone();
            return clone;
        }

        public void SetRadiusMarginFromShape(double margin)
        {
            TopLeftRadiusMarginFromShape = margin;
            TopRightRadiusMarginFromShape = margin;
            BottomLeftRadiusMarginFromShape = margin;
            BottomRightRadiusMarginFromShape = margin;
        }
    }

    public class RectangleDistancingOptions
    {
        //Vertical Distance , Left / Top / Mid / Center
    }
}
