using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;

namespace DrawingLibrary.Models.ConcreteGraphics
{
    public class CircleDrawing : ShapeDrawing<CircleInfo,CircleDimensionsPresentationOptions>, IPolygonSimulatableDrawing, IDeepClonable<CircleDrawing>
    {
        public CircleDrawing(CircleInfo circleInfo) : base(circleInfo) { }
        public CircleDrawing(CircleInfo circleInfo,DrawingPresentationOptions? presentationOptions = null) : base(circleInfo, presentationOptions) { }
        public CircleDrawing(CircleInfo circleInfo,DrawingPresentationOptions? presentationOptions = null , CircleDimensionsPresentationOptions? dimensionsOptions = null) 
            : base(circleInfo, presentationOptions)
        {
            if (dimensionsOptions is not null) DimensionsPresentationOptions = dimensionsOptions;
        }

        public override CircleDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<CircleDimensionsPresentationOptions>();
        public int MinSimulationSides => CircleInfo.MINSIMULATIONSIDES;

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            List<DimensionLineDrawing> dims = [.. base.GetDimensionsDrawings()];
            var builder = DimensionLineDrawing.GetBuilder();

            #region Dimeter-Radius Dimension
            if (DimensionsPresentationOptions.ShowDiameter || DimensionsPresentationOptions.ShowRadius)
            {
                var lineOptions = DimensionsPresentationOptions.ShowRadius ? DimensionsPresentationOptions.RadiusLineOptions : DimensionsPresentationOptions.DiameterLineOptions;
                double startDistanceFromPerimeter = DimensionsPresentationOptions.DiameterRadiusMarginFromShape;
                double endDistanceFromPerimeter = startDistanceFromPerimeter + lineOptions.OneEndLineLength;
                string dimensionName = DimensionsPresentationOptions.ShowRadius ? "Radius" : "Diameter";
                double dimensionValue = DimensionsPresentationOptions.ShowRadius ? originalShape.Radius : originalShape.Diameter;
                if (DimensionsPresentationOptions.ShowDiameterRadiusInsideShape)
                {
                    //Put the dimension inside the circle
                    startDistanceFromPerimeter *= -1;
                    endDistanceFromPerimeter *= -1;
                }
                var start = MathCalculations.Circle.GetPointInDistanceFromCirclePerimeter(Shape, DimensionsPresentationOptions.RadiusDiameterPositionAngleRadians, startDistanceFromPerimeter);
                var end = MathCalculations.Circle.GetPointInDistanceFromCirclePerimeter(Shape, DimensionsPresentationOptions.RadiusDiameterPositionAngleRadians, endDistanceFromPerimeter);

                var diamRadius = builder.SetPresentationOptions(DimensionsPresentationOptions.DiameterRadiusPresentationOptions)
                    .SetDimensionLineOptions(lineOptions)
                    .SetDimensionValue(dimensionValue)
                    .SetName($"{dimensionName} ({this.Name})")
                    .SetStart(start)
                    .SetEnd(end)
                    .BuildDimensionLine();

                dims.Add(diamRadius);
            }
            #endregion

            return dims;
        }
        public override IEnumerable<IDrawing> GetHelpLinesDrawings()
        {
            List<IDrawing> helplines = [];
            if (DimensionsPresentationOptions.ShowCenterHelpLines)
            {
                helplines.Add(GetVerticalCenterHelpline());
                helplines.Add(GetHorizontalCenterHelpline());
            }
            return helplines;
        }
        protected override void BuildPathData()
        {
            pathDataBuilder.AddCircle(Shape);
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.AddCircleHole(Shape);
        }
        public override CircleDrawing GetDeepClone()
        {
            var clone = (CircleDrawing)base.GetDeepClone();
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        public override CircleDrawing GetDeepClone(bool generateUniqueId)
        {
            var clone  = (CircleDrawing)base.GetDeepClone(generateUniqueId);
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        
        public string GetPolygonSimulationPathData(int sides)
        {
            var polygon = Shape.GetPolygonSimulation(sides);
            pathDataBuilder.ResetBuilder().AddPolygon(polygon);
            return pathDataBuilder.GetPathData();
        }
        public string GetNormalAndSimulatedPathData(int sides)
        {
            pathDataBuilder.ResetBuilder();
            BuildPathData();
            AddClipPathData();
            return pathDataBuilder.AddPolygon(Shape.GetPolygonSimulation(sides)).GetPathData();
        }
        private LineDrawing GetVerticalCenterHelpline()
        {
            var line = GetUnpositionedVerticalCenterHelpline();
            line.SetLocation(this.LocationX, this.Shape.TopY);
            return line;
        }
        private LineDrawing GetHorizontalCenterHelpline()
        {
            var line = GetUnpositionedHorizontalCenterHelpLine();
            line.SetLocation(this.Shape.LeftX, this.LocationY);
            return line;
        }
    }
    public class CircleRingDrawing : ShapeDrawing<CircleRingInfo,CircleRingDimensionsPresentationOptions>, IDeepClonable<CircleRingDrawing>
    {
        public override CircleRingDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<CircleRingDimensionsPresentationOptions>();

        public CircleRingDrawing(CircleRingInfo circleRingInfo) : base(circleRingInfo) { }
        public CircleRingDrawing(CircleRingInfo circleRingInfo,DrawingPresentationOptions? presentationOptions = null) : base(circleRingInfo, presentationOptions) { }
        public CircleRingDrawing(CircleRingInfo circleRingInfo, DrawingPresentationOptions? presentationOptions = null,CircleRingDimensionsPresentationOptions? dimensionsOptions = null) : base(circleRingInfo, presentationOptions) 
        {
            if (dimensionsOptions is not null) DimensionsPresentationOptions = dimensionsOptions;
        }
        protected override void BuildPathData()
        {
            pathDataBuilder.ResetBuilder().AddCircle(Shape).AddCircleHole(Shape.GetInnerRingWholeShape());
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.ResetBuilder().AddCircleHole(Shape).AddCircle(Shape.GetInnerRingWholeShape());
        }

        public override CircleRingDrawing GetDeepClone()
        {
            var clone = (CircleRingDrawing)base.GetDeepClone();
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        public override CircleRingDrawing GetDeepClone(bool generateUniqueId)
        {
            var clone =  (CircleRingDrawing)base.GetDeepClone(generateUniqueId);
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            List<DimensionLineDrawing> dims = [.. base.GetDimensionsDrawings()];
            var builder = DimensionLineDrawing.GetBuilder();

            #region Dimeter-Radius Dimension
            if (DimensionsPresentationOptions.ShowDiameter || DimensionsPresentationOptions.ShowRadius)
            {
                var lineOptions = DimensionsPresentationOptions.ShowRadius ? DimensionsPresentationOptions.RadiusLineOptions : DimensionsPresentationOptions.DiameterLineOptions;
                double startDistanceFromPerimeter = DimensionsPresentationOptions.DiameterRadiusMarginFromShape;
                double endDistanceFromPerimeter = startDistanceFromPerimeter + lineOptions.OneEndLineLength;
                string dimensionName = DimensionsPresentationOptions.ShowRadius ? "Radius" : "Diameter";
                double dimensionValue = DimensionsPresentationOptions.ShowRadius ? originalShape.Radius : originalShape.Diameter;
                if (DimensionsPresentationOptions.ShowDiameterRadiusInsideShape)
                {
                    //Put the dimension inside the circle
                    startDistanceFromPerimeter *= -1;
                    endDistanceFromPerimeter *= -1;
                }
                var start = MathCalculations.Circle.GetPointInDistanceFromCirclePerimeter(Shape, DimensionsPresentationOptions.RadiusDiameterPositionAngleRadians, startDistanceFromPerimeter);
                var end = MathCalculations.Circle.GetPointInDistanceFromCirclePerimeter(Shape, DimensionsPresentationOptions.RadiusDiameterPositionAngleRadians, endDistanceFromPerimeter);

                var diamRadius = builder.SetPresentationOptions(DimensionsPresentationOptions.DiameterRadiusPresentationOptions)
                    .SetDimensionLineOptions(lineOptions)
                    .SetDimensionValue(dimensionValue)
                    .SetName($"{dimensionName} ({this.Name})")
                    .SetStart(start)
                    .SetEnd(end)
                    .BuildDimensionLine();

                dims.Add(diamRadius);
            }
            #endregion

            #region Thickness Dimension

            if (DimensionsPresentationOptions.ShowThickness)
            {
                PointXY start;
                PointXY end;
                switch (DimensionsPresentationOptions.ThicknessPosition)
                {
                    case RectangleRingThicknessDimensionPosition.LeftMiddle:
                        start = new(Shape.LocationX - Shape.Radius, Shape.LocationY);
                        end = new(start.X + Shape.Thickness, Shape.LocationY);
                        break;
                    case RectangleRingThicknessDimensionPosition.RightMiddle:
                        start = new(Shape.LocationX - Shape.Thickness, Shape.LocationY);
                        end = new(Shape.LocationX, Shape.LocationY);
                        break;
                    case RectangleRingThicknessDimensionPosition.TopMiddle:
                        start = new(Shape.LocationX, Shape.LocationY - Shape.Radius + Shape.Thickness);
                        end = new(Shape.LocationX, Shape.LocationY - Shape.Radius);
                        break;
                    case RectangleRingThicknessDimensionPosition.BottomMiddle:
                        start = new(Shape.LocationX, Shape.LocationY + Shape.Radius);
                        end = new(Shape.LocationX, Shape.LocationY + Shape.Radius - Shape.Thickness);
                        break;
                    default:
                        throw new EnumValueNotSupportedException(DimensionsPresentationOptions.ThicknessPosition);
                }
                var thickness = builder.SetStart(start)
                    .SetEnd(end)
                    .SetPresentationOptions(DimensionsPresentationOptions.ThicknessPresentationOptions)
                    .SetDimensionLineOptions(DimensionsPresentationOptions.ThicknessLineOptions)
                    .SetDimensionValue(originalShape.Thickness)
                    .SetName($"Thickness ({this.Name})")
                    .BuildDimensionLine();
                dims.Add(thickness);
            }

            #endregion

            return dims;
        }
        public override IEnumerable<IDrawing> GetHelpLinesDrawings()
        {
            List<IDrawing> helplines = [];
            if (DimensionsPresentationOptions.ShowCenterHelpLines)
            {
                helplines.Add(GetVerticalCenterHelpline());
                helplines.Add(GetHorizontalCenterHelpline());
            }
            return helplines;
        }
        private LineDrawing GetVerticalCenterHelpline()
        {
            var line = GetUnpositionedVerticalCenterHelpline();
            line.SetLocation(this.LocationX, this.Shape.TopY);
            return line;
        }
        private LineDrawing GetHorizontalCenterHelpline()
        {
            var line = GetUnpositionedHorizontalCenterHelpLine();
            line.SetLocation(this.Shape.LeftX, this.LocationY);
            return line;
        }
    }
}
