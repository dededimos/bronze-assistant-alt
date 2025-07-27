using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.DrawingElements.ModulesDrawingOptions
{
    public class MagnifierSandblastDimensionOptions : IDeepClonable<MagnifierSandblastDimensionOptions>
    {
        public CircleRingDimensionsPresentationOptions SandblastBodyDimensionOptions { get; set; } = new();
        /// <summary>
        /// Weather to show distances of the Magnifier's Center from the Parent Mirrors Boundaries/Center e.t.c.
        /// </summary>
        public bool ShowDistanceFromCenter { get; set; } = true;
        public DrawingPresentationOptions DistanceFromCenterPresentationOptions { get; set; } = new();
        public DimensionLineOptions DistanceFromCenterLineOptions { get; set; } = new();

        public static MagnifierSandblastDimensionOptions Defaults(bool isSketch)
        {
            var bodyOptions = MirrorDrawOptions.GetDefaultDimensionOptions<CircleRingDimensionsPresentationOptions>(isSketch);
            bodyOptions.ShowDiameter = isSketch;
            bodyOptions.ThicknessPosition = DrawingLibrary.Enums.RectangleRingThicknessDimensionPosition.BottomMiddle;
            bodyOptions.ThicknessPresentationOptions.TextAnchorLineOption = DrawingLibrary.Enums.AnchorLinePreferenceOption.PreferSmallerXAnchorline;
            bodyOptions.ShowRadius = false;

            var defaults = new MagnifierSandblastDimensionOptions()
            {
                SandblastBodyDimensionOptions = bodyOptions,
                ShowDistanceFromCenter = isSketch,
                DistanceFromCenterPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(),
                DistanceFromCenterLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions(),
            };
            return defaults;
        }

        public MagnifierSandblastDimensionOptions GetDeepClone()
        {
            var clone = (MagnifierSandblastDimensionOptions)this.MemberwiseClone();
            clone.SandblastBodyDimensionOptions = this.SandblastBodyDimensionOptions.GetDeepClone();
            clone.DistanceFromCenterPresentationOptions = this.DistanceFromCenterPresentationOptions.GetDeepClone();
            clone.DistanceFromCenterLineOptions = this.DistanceFromCenterLineOptions.GetDeepClone();
            return clone;
        }

        [Flags]
        public enum DistanceFromCenterOptionRectangle
        {
            VerticalDistanceFromClosestEdge,
            VerticalDistanceFromFurthestEdge,
            VerticalDistanceFromCenterY,
            HorizontalDistanceFromClosestEdge,
            HorizontalDistanceFromFurthestEdge,
            HorizontalDistanceFromCenterX,
        }
    }
}
