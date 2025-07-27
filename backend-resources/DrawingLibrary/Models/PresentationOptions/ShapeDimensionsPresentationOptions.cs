using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Interfaces;

namespace DrawingLibrary.Models.PresentationOptions
{
    public class ShapeDimensionsPresentationOptions : IDeepClonable<ShapeDimensionsPresentationOptions>
    {
        public bool ShowDimensions { get; set; }

        /// <summary>
        /// Weather to show the Dimensions of the Clips
        /// </summary>
        public bool ShowClipsDimensions { get; set; }

        /// <summary>
        /// Weather to Show the Height Dimension of the Shape
        /// </summary>
        public bool ShowHeight { get; set; }
        public RectangleHeightDimensionPosition HeightPosition { get; set; } = RectangleHeightDimensionPosition.Right;
        public double HeightMarginFromShape { get; set; }
        public DimensionLineOptions HeightLineOptions { get; set; } = new();
        public DrawingPresentationOptions HeightPresentationOptions { get; set; } = new();

        /// <summary>
        /// Weather to Show the Length Dimension of the Shape
        /// </summary>
        public bool ShowLength { get; set; }
        public double LengthMarginFromShape { get; set; }
        public RectangleLengthDimensionPosition LengthPosition { get; set; } = RectangleLengthDimensionPosition.Bottom;
        public DimensionLineOptions LengthLineOptions { get; set; } = new();
        public DrawingPresentationOptions LengthPresentationOptions { get; set; } = new();

        public DrawingPresentationOptions HelpLinesPresentationOptions { get; set; } = new();
        public bool ShowCenterHelpLines { get; set; }

        public virtual ShapeDimensionsPresentationOptions GetDeepClone()
        {
            var clone = (ShapeDimensionsPresentationOptions)this.MemberwiseClone();
            clone.HeightLineOptions = this.HeightLineOptions.GetDeepClone();
            clone.LengthLineOptions = this.LengthLineOptions.GetDeepClone();
            clone.HeightPresentationOptions = this.HeightPresentationOptions.GetDeepClone();
            clone.LengthPresentationOptions = this.LengthPresentationOptions.GetDeepClone();
            clone.HelpLinesPresentationOptions = this.HelpLinesPresentationOptions.GetDeepClone();
            return clone;
        }
        /// <summary>
        /// Returns Empty Hidden Options
        /// </summary>
        /// <typeparam name="TShapeDimensionOptions"></typeparam>
        /// <returns></returns>
        public static TShapeDimensionOptions GetEmptyDimensionOptions<TShapeDimensionOptions>()
            where TShapeDimensionOptions: ShapeDimensionsPresentationOptions , new()
        {
            return  new()
            {
                ShowDimensions = false
            };
        }
    }
}
