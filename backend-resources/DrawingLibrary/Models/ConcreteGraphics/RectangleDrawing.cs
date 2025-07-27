using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using static DrawingLibrary.Models.ConcreteGraphics.DimensionLineDrawing;

namespace DrawingLibrary.Models.ConcreteGraphics
{
    public class RectangleDrawing : ShapeDrawing<RectangleInfo, RectangleDimensionsPresentationOptions>,IPolygonSimulatableDrawing, IDeepClonable<RectangleDrawing>
    {
        public RectangleDrawing(RectangleInfo rectagleInfo) : base(rectagleInfo) { }
        public RectangleDrawing(RectangleInfo rectangleInfo, DrawingPresentationOptions? presentationOptions = null) : base(rectangleInfo, presentationOptions) { }
        public RectangleDrawing(RectangleInfo rectangleInfo, DrawingPresentationOptions? presentationOptions, RectangleDimensionsPresentationOptions? dimensionsOptions)
            : base(rectangleInfo, presentationOptions)
        {
            if (dimensionsOptions is not null) DimensionsPresentationOptions = dimensionsOptions;
        }

        public override RectangleDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<RectangleDimensionsPresentationOptions>();
        public int MinSimulationSides => RectangleInfo.MINSIMULATIONSIDES;

        protected override void BuildPathData()
        {
            pathDataBuilder.AddRectangle(Shape);
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.AddRectangleHole(Shape);
        }

        public override RectangleDrawing GetDeepClone()
        {
            var clone = (RectangleDrawing)base.GetDeepClone();
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        public override RectangleDrawing GetDeepClone(bool generateUniqueId)
        {
            var clone = (RectangleDrawing)base.GetDeepClone(generateUniqueId);
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        public override RectangleDrawing GetCloneCenteredToContainer(RectangleInfo container)
        {
            var clone = (RectangleDrawing)base.GetCloneCenteredToContainer(container);
            return clone;
        }

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            List<DimensionLineDrawing> dims = [.. base.GetDimensionsDrawings()];

            var bBox = GetBoundingBox();
            var builder = DimensionLineDrawing.GetBuilder();

            bool showTopLeft = false;
            bool showTopRight = false;
            bool showBottomLeft = false;
            bool showBottomRight = false;
            if (DimensionsPresentationOptions.ShowRadiuses)
            {
                if (originalShape.HasDifferentRadiuses)
                {
                    // Show all corners when radii are different
                    showTopLeft = true;
                    showTopRight = true;
                    showBottomLeft = true;
                    showBottomRight = true;
                }
                else
                {
                    // Determine the appropriate radius option based on the shape
                    var radiusOption = originalShape.HasTotalNonZeroRadius
                        ? DimensionsPresentationOptions.RadiusOptionWhenTotalRadius
                        : DimensionsPresentationOptions.RadiusOptionWhenAllZero;

                    showTopLeft = ShouldShowCornerRadius(radiusOption, RectangleRadiusDimensionShowOption.ShowTopLeftRadius);
                    showTopRight = ShouldShowCornerRadius(radiusOption, RectangleRadiusDimensionShowOption.ShowTopRightRadius);
                    showBottomLeft = ShouldShowCornerRadius(radiusOption, RectangleRadiusDimensionShowOption.ShowBottomLeftRadius);
                    showBottomRight = ShouldShowCornerRadius(radiusOption, RectangleRadiusDimensionShowOption.ShowBottomRightRadius);
                } 
            }

            #region TopLeftRadius
            if (showTopLeft)
            {
                var topLeftRadius = GetTopLeftRadiusDimension(bBox, builder);
                dims.Add(topLeftRadius);
            }
            #endregion

            #region TopRightRadius
            if (showTopRight)
            {
                var topRightRadius = GetTopRightRadiusDimension(bBox, builder);
                dims.Add(topRightRadius);
            }
            #endregion

            #region BottomRightRadius
            if (showBottomRight)
            {
                var bottomRightRadius = GetBottomRightRadiusDimension(bBox,builder);
                dims.Add(bottomRightRadius);
            }
            #endregion

            #region BottomLeftRadius
            if (showBottomLeft)
            {
                var bottomLeftRadius = GetBottomLeftRadiusDimension(bBox, builder);
                dims.Add(bottomLeftRadius);
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

        private DimensionLineDrawing GetTopLeftRadiusDimension(RectangleInfo bBox,DimensionLineBuilder builder)
        {
            //Get the point shift from the LeftTop
            var shift = MathCalculations.Rectangle.GetXYShiftFromRectangleEdgeToCornerRadiusArcMiddle(Shape.TopLeftRadius);
            var start = new PointXY(bBox.LeftX + shift, bBox.TopY + shift);
            var end = new PointXY(start.X + DimensionsPresentationOptions.TopLeftRadiusLineOptions.OneEndLineLength, start.Y);

            var topLeftRadius = builder.SetPresentationOptions(DimensionsPresentationOptions.TopLeftRadiusPresentationOptions)
                .SetDimensionLineOptions(DimensionsPresentationOptions.TopLeftRadiusLineOptions)
                .SetStart(start)
                .SetEnd(end)
                .SetDimensionValue(originalShape.TopLeftRadius)
                .SetName($"TopLeftRadius ({this.Name})")
                .BuildDimensionLine();
            var topLeftRadiusRotation = DimensionsPresentationOptions.TopLeftRadiusLineOptions.StartRotationAngle;

            //The StartPoint(SP) of the TopLeft Radius Dimension when connected with the leftTop(LT) of the Rectangle
            //Forms a -0 angle with X axis . (Assuming there is a Margin from the Dimension line with the TopLeft of the Rectangle)
            //If the Top Left has a Rotation instead of being straight then this angle closes when the rotation is positive and opens if the rotation is negative
            //So the Actual angle formed by the line of points SP-LT is : θ = -π + topLeftRadiusRotation
            //And with θ given and Margin taken from the options , we can find what translation to apply to the Dimension so that the
            //margin will be correct in both X and Y dimensions (by using the sin(θ) = apenanti/Ipotinousa , cos(θ)= proskimeni/Ipotinousa)

            //Find θ
            var thetta = +0 + topLeftRadiusRotation;
            //Assume LT of rectangle is 0,0 to find the x,y differences to apply as translation
            var xyPointTranslation = MathCalculations.Points.GetPointAtDistanceFromPoint(DimensionsPresentationOptions.TopLeftRadiusMarginFromShape, thetta);
            topLeftRadius.Shape.Translate(xyPointTranslation.X, xyPointTranslation.Y);
            return topLeftRadius;
        }
        private DimensionLineDrawing GetTopRightRadiusDimension(RectangleInfo bBox,DimensionLineBuilder builder)
        {
            //Get the point shift from the LeftTop
            var shift = MathCalculations.Rectangle.GetXYShiftFromRectangleEdgeToCornerRadiusArcMiddle(Shape.TopRightRadius);
            var start = new PointXY(bBox.RightX - shift, bBox.TopY + shift);
            var end = new PointXY(start.X + DimensionsPresentationOptions.TopRightRadiusLineOptions.OneEndLineLength, start.Y);

            var topRightRadius = builder.SetPresentationOptions(DimensionsPresentationOptions.TopRightRadiusPresentationOptions)
                .SetDimensionLineOptions(DimensionsPresentationOptions.TopRightRadiusLineOptions)
                .SetStart(start)
                .SetEnd(end)
                .SetDimensionValue(originalShape.TopRightRadius)
                .SetName($"TopRightRadius ({this.Name})")
                .BuildDimensionLine();
            var topRightRadiusRotation = DimensionsPresentationOptions.TopRightRadiusLineOptions.StartRotationAngle;

            //The StartPoint(SP) of the TopRight Radius Dimension when connected with the TopRight(TR) of the Rectangle
            //Forms a '0' angle with X axis . (Assuming there is a Margin from the Dimension line with the TopRight of the Rectangle)
            //If the Top Right has a Rotation instead of being straight then this angle increses when the rotation is positive and decreases if the rotation is negative
            //So the Actual angle formed by the line of points SP-TR is : θ = 0 + topRightRadiusRotation
            //And with θ given and Margin taken from the options , we can find what translation to apply to the Dimension so that the
            //margin will be correct in both X and Y dimensions (by using the sin(θ) = apenanti/Ipotinousa , cos(θ)= proskimeni/Ipotinousa)

            //Find θ
            var thetta = 0 + topRightRadiusRotation;
            //Assume TR of rectangle is 0,0 to find the x,y differences to apply as translation
            var xyPointTranslation = MathCalculations.Points.GetPointAtDistanceFromPoint(DimensionsPresentationOptions.TopRightRadiusMarginFromShape, thetta);
            topRightRadius.Shape.Translate(xyPointTranslation.X, xyPointTranslation.Y);
            return topRightRadius;
        }
        private DimensionLineDrawing GetBottomRightRadiusDimension(RectangleInfo bBox , DimensionLineBuilder builder)
        {
            //Get the point shift from the LeftTop
            var shift = MathCalculations.Rectangle.GetXYShiftFromRectangleEdgeToCornerRadiusArcMiddle(Shape.BottomRightRadius);
            var start = new PointXY(bBox.RightX - shift, bBox.BottomY - shift);
            var end = new PointXY(start.X + DimensionsPresentationOptions.BottomRightRadiusLineOptions.OneEndLineLength, start.Y);

            var bottomRightRadius = builder.SetPresentationOptions(DimensionsPresentationOptions.BottomRightRadiusPresentationOptions)
                .SetDimensionLineOptions(DimensionsPresentationOptions.BottomRightRadiusLineOptions)
                .SetStart(start)
                .SetEnd(end)
                .SetDimensionValue(originalShape.BottomRightRadius)
                .SetName($"BottomRightRadius ({this.Name})")
                .BuildDimensionLine();
            var bottomRightRadiusRotation = DimensionsPresentationOptions.BottomRightRadiusLineOptions.StartRotationAngle;

            //The StartPoint(SP) of the BottomRight Radius Dimension when connected with the BottomRight(BR) of the Rectangle
            //Forms a '0' angle with X axis . (Assuming there is a Margin from the Dimension line with the BottomRight of the Rectangle)
            //If the Bottom Right has a Rotation instead of being straight then this angle increses when the rotation is positive and decreases if the rotation is negative
            //So the Actual angle formed by the line of points SP-BR is : θ = 0 + bottomRightRadiusRotation
            //And with θ given and Margin taken from the options , we can find what translation to apply to the Dimension so that the
            //margin will be correct in both X and Y dimensions (by using the sin(θ) = apenanti/Ipotinousa , cos(θ)= proskimeni/Ipotinousa)

            //Find θ
            var thetta = 0 + bottomRightRadiusRotation;
            //Assume TR of rectangle is 0,0 to find the x,y differences to apply as translation
            var xyPointTranslation = MathCalculations.Points.GetPointAtDistanceFromPoint(DimensionsPresentationOptions.BottomRightRadiusMarginFromShape, thetta);
            bottomRightRadius.Shape.Translate(xyPointTranslation.X, xyPointTranslation.Y);
            return bottomRightRadius;
        }
        private DimensionLineDrawing GetBottomLeftRadiusDimension(RectangleInfo bBox , DimensionLineBuilder builder)
        {
            //Get the point shift from the LeftTop
            var shift = MathCalculations.Rectangle.GetXYShiftFromRectangleEdgeToCornerRadiusArcMiddle(Shape.BottomLeftRadius);
            var start = new PointXY(bBox.LeftX + shift, bBox.BottomY - shift);
            var end = new PointXY(start.X + DimensionsPresentationOptions.BottomLeftRadiusLineOptions.OneEndLineLength, start.Y);

            var bottomLeftRadius = builder.SetPresentationOptions(DimensionsPresentationOptions.BottomLeftRadiusPresentationOptions)
                .SetDimensionLineOptions(DimensionsPresentationOptions.BottomLeftRadiusLineOptions)
                .SetStart(start)
                .SetEnd(end)
                .SetDimensionValue(originalShape.BottomLeftRadius)
                .SetName($"BottomLeftRadius ({this.Name})")
                .BuildDimensionLine();
            var bottomLeftRadiusRotation = DimensionsPresentationOptions.BottomLeftRadiusLineOptions.StartRotationAngle;

            //The StartPoint(SP) of the BottomLeft Radius Dimension when connected with the bottomTop(BL) of the Rectangle
            //Forms a 0 angle with X axis . (Assuming there is a Margin from the Dimension line with the BOttomLeft of the Rectangle)
            //If the Bottom Left has a Rotation instead of being straight then this angle closes when the rotation is positive and opens if the rotation is negative
            //So the Actual angle formed by the line of points SP-BL is : θ = -π + bottomLeftRadiusRotation
            //And with θ given and Margin taken from the options , we can find what translation to apply to the Dimension so that the
            //margin will be correct in both X and Y dimensions (by using the sin(θ) = apenanti/Ipotinousa , cos(θ)= proskimeni/Ipotinousa)

            //Find θ
            var thetta = +0 + bottomLeftRadiusRotation;
            //Assume LT of rectangle is 0,0 to find the x,y differences to apply as translation
            var xyPointTranslation = MathCalculations.Points.GetPointAtDistanceFromPoint(DimensionsPresentationOptions.BottomLeftRadiusMarginFromShape, thetta);
            bottomLeftRadius.Shape.Translate(xyPointTranslation.X, xyPointTranslation.Y);
            return bottomLeftRadius;
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
        private static bool ShouldShowCornerRadius(RectangleRadiusDimensionShowOption option , RectangleRadiusDimensionShowOption cornerOption)
        {
            return option == RectangleRadiusDimensionShowOption.ShowAll || option == cornerOption;
        }

    }
    public class RectangleRingDrawing : ShapeDrawing<RectangleRingInfo, RectangleRingDimensionsPresentationOptions>, IDeepClonable<RectangleRingDrawing>
    {
        public override RectangleRingDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<RectangleRingDimensionsPresentationOptions>();

        public RectangleRingDrawing(RectangleRingInfo rectagleRingInfo) : base(rectagleRingInfo) { }
        public RectangleRingDrawing(RectangleRingInfo rectagleRingInfo, DrawingPresentationOptions? presentationOptions = null) : base(rectagleRingInfo, presentationOptions) { }
        public RectangleRingDrawing(RectangleRingInfo rectagleRingInfo, DrawingPresentationOptions? presentationOptions = null,RectangleRingDimensionsPresentationOptions? dimensionsOptions = null) 
            : base(rectagleRingInfo, presentationOptions) 
        {
            if (dimensionsOptions != null) DimensionsPresentationOptions = dimensionsOptions;
        }
        protected override void BuildPathData()
        {
            var innerRect = Shape.GetInnerRingWholeShape();
            pathDataBuilder.ResetBuilder().AddRectangle(Shape).AddRectangleHole(innerRect);
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.ResetBuilder().AddRectangleHole(Shape).AddRectangle(Shape.GetInnerRingWholeShape());
        }
        
        public override RectangleRingDrawing GetDeepClone()
        {
            return (RectangleRingDrawing)base.GetDeepClone();
        }
        public override RectangleRingDrawing GetDeepClone(bool generateUniqueId)
        {
            return (RectangleRingDrawing)base.GetDeepClone(generateUniqueId);
        }

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            List<DimensionLineDrawing> dims = [.. base.GetDimensionsDrawings()];

            var builder = DimensionLineDrawing.GetBuilder();


            #region Thickness Dimension

            if (DimensionsPresentationOptions.ShowThickness)
            {
                PointXY start;
                PointXY end;
                switch (DimensionsPresentationOptions.ThicknessPosition)
                {
                    case RectangleRingThicknessDimensionPosition.LeftMiddle:
                        start = new(Shape.LeftX, Shape.LocationY);
                        end = new(Shape.LeftX + Shape.Thickness, Shape.LocationY);
                        break;
                    case RectangleRingThicknessDimensionPosition.RightMiddle:
                        start = new(Shape.RightX - Shape.Thickness, Shape.LocationY);
                        end = new(Shape.RightX , Shape.LocationY);
                        break;
                    case RectangleRingThicknessDimensionPosition.TopMiddle:
                        start = new(Shape.LocationX, Shape.TopY + Shape.Thickness);
                        end = new(Shape.LocationX, Shape.TopY);
                        break;
                    case RectangleRingThicknessDimensionPosition.BottomMiddle:
                        start = new(Shape.LocationX, Shape.BottomY);
                        end = new(Shape.LocationX, Shape.BottomY - Shape.Thickness);
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

            #region Inner Length Dimension
            if (DimensionsPresentationOptions.ShowInnerLength)
            {
                var innerRect = Shape.GetInnerRingWholeShape();
                PointXY start = DimensionsPresentationOptions.InnerLengthPosition switch
                {
                    RectangleLengthDimensionPosition.Bottom => new(innerRect.LeftX, innerRect.BottomY - DimensionsPresentationOptions.InnerLengthMarginFromShape),
                    RectangleLengthDimensionPosition.Top => new(innerRect.LeftX, innerRect.TopY + DimensionsPresentationOptions.InnerLengthMarginFromShape),
                    _ => throw new EnumValueNotSupportedException(DimensionsPresentationOptions.InnerLengthPosition),
                };
                PointXY end = new(innerRect.RightX, start.Y);
                var innerLength = builder.SetStart(start)
                                         .SetEnd(end)
                                         .SetDimensionLineOptions(DimensionsPresentationOptions.InnerLengthLineOptions)
                                         .SetPresentationOptions(DimensionsPresentationOptions.InnerLengthPresentationOptions)
                                         .SetDimensionValue(originalShape.GetInnerRingWholeShape().Length)
                                         .SetName($"Inner Length ({this.Name})")
                                         .BuildDimensionLine();
                dims.Add(innerLength);
            }
            #endregion

            #region Inner Height Dimension
            if (DimensionsPresentationOptions.ShowInnerHeight)
            {
                var innerRect = Shape.GetInnerRingWholeShape();
                PointXY start;
                switch (DimensionsPresentationOptions.InnerHeightPosition)
                {
                    case RectangleHeightDimensionPosition.Right:
                        start = new(innerRect.RightX - DimensionsPresentationOptions.InnerHeightMarginFromShape, innerRect.BottomY);
                        break;
                    case RectangleHeightDimensionPosition.Left:
                        start = new(innerRect.LeftX + DimensionsPresentationOptions.InnerLengthMarginFromShape, innerRect.BottomY);
                        break;
                    default:
                        throw new EnumValueNotSupportedException(DimensionsPresentationOptions.InnerLengthPosition);
                }
                PointXY end = new(start.X, innerRect.TopY);
                var innerHeight = builder.SetStart(start)
                                         .SetEnd(end)
                                         .SetDimensionLineOptions(DimensionsPresentationOptions.InnerHeightLineOptions)
                                         .SetPresentationOptions(DimensionsPresentationOptions.InnerHeightPresentationOptions)
                                         .SetDimensionValue(originalShape.GetInnerRingWholeShape().Height)
                                         .SetName($"Inner Height ({this.Name})")
                                         .BuildDimensionLine();
                dims.Add(innerHeight);
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
