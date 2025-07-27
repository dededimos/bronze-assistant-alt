using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using ShapesLibrary.Enums;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Text;

namespace DrawingLibrary.Models.ConcreteGraphics
{
    public class CapsuleDrawing : ShapeDrawing<CapsuleInfo,CapsuleDimensionsPresentationOptions>,IPolygonSimulatableDrawing, IDeepClonable<CapsuleDrawing>
    {
        
        public CapsuleDrawing(CapsuleInfo capsuleInfo) : base(capsuleInfo) { }
        public CapsuleDrawing(CapsuleInfo capsuleInfo,DrawingPresentationOptions? presentationOptions = null) : base(capsuleInfo, presentationOptions) { }
        public CapsuleDrawing(CapsuleInfo capsuleInfo, DrawingPresentationOptions? presentationOptions = null,CapsuleDimensionsPresentationOptions? dimensionsOptions = null)
            :base(capsuleInfo, presentationOptions)
        {
            if (dimensionsOptions != null) DimensionsPresentationOptions = dimensionsOptions;
        }

        public override CapsuleDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<CapsuleDimensionsPresentationOptions>();
        public int MinSimulationSides => CapsuleInfo.MINSIMULATIONSIDES;

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            List<DimensionLineDrawing> dims = [.. base.GetDimensionsDrawings()];

            var builder = DimensionLineDrawing.GetBuilder();

            if (DimensionsPresentationOptions.ShowRadius || DimensionsPresentationOptions.ShowDiameter)
            {
                #region Find Dimension Start Point
                PointXY start;
                PointXY end;

                switch (Shape.Orientation)
                {
                    case ShapesLibrary.Enums.CapsuleOrientation.Horizontal:
                        start = DimensionsPresentationOptions.DiameterRadiusDimensionPosition switch
                        {
                            CapsuleRadiusDimensionPosition.TopLeft => MathCalculations.Circle.GetPointOnCirclePerimeter(Shape.GetCapsuleCircle(true), -3 * Math.PI / 4),
                            CapsuleRadiusDimensionPosition.TopRight => MathCalculations.Circle.GetPointOnCirclePerimeter(Shape.GetCapsuleCircle(false), -Math.PI / 4),
                            CapsuleRadiusDimensionPosition.BottomLeft => MathCalculations.Circle.GetPointOnCirclePerimeter(Shape.GetCapsuleCircle(true), 3 * Math.PI / 4),
                            CapsuleRadiusDimensionPosition.BottomRight => MathCalculations.Circle.GetPointOnCirclePerimeter(Shape.GetCapsuleCircle(false), Math.PI / 4),
                            _ => throw new EnumValueNotSupportedException(DimensionsPresentationOptions.DiameterRadiusDimensionPosition),
                        };
                        break;
                    case ShapesLibrary.Enums.CapsuleOrientation.Vertical:
                        start = DimensionsPresentationOptions.DiameterRadiusDimensionPosition switch
                        {
                            CapsuleRadiusDimensionPosition.TopLeft => MathCalculations.Circle.GetPointOnCirclePerimeter(Shape.GetCapsuleCircle(true), - 3 * Math.PI / 4),
                            CapsuleRadiusDimensionPosition.TopRight => MathCalculations.Circle.GetPointOnCirclePerimeter(Shape.GetCapsuleCircle(true), -Math.PI / 4),
                            CapsuleRadiusDimensionPosition.BottomLeft => MathCalculations.Circle.GetPointOnCirclePerimeter(Shape.GetCapsuleCircle(false), Math.PI / 4),
                            CapsuleRadiusDimensionPosition.BottomRight => MathCalculations.Circle.GetPointOnCirclePerimeter(Shape.GetCapsuleCircle(false), 3 * Math.PI / 4),
                            _ => throw new EnumValueNotSupportedException(DimensionsPresentationOptions.DiameterRadiusDimensionPosition),
                        };
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Shape.Orientation);
                }
                end = new(start.X + DimensionsPresentationOptions.DiameterLineOptions.OneEndLineLength, start.Y); 
                #endregion
                #region Diameter Dimension
                if (DimensionsPresentationOptions.ShowDiameter)
                {
                    var diam = builder.SetStart(start)
                        .SetEnd(end)
                        .SetPresentationOptions(DimensionsPresentationOptions.DiameterRadiusPresentationOptions)
                        .SetDimensionLineOptions(DimensionsPresentationOptions.DiameterLineOptions)
                        .SetDimensionValue(originalShape.CircleDiameter)
                        .SetName($"Diameter ({this.Name})")
                        .BuildDimensionLine();

                    //Rotate the Dimension and then according to the Applied Rotation Translate to apply Margin from the Shape
                    var diamRotation = DimensionsPresentationOptions.DiameterLineOptions.StartRotationAngle;
                    var xyPointTranslation = MathCalculations.Points.GetPointAtDistanceFromPoint(DimensionsPresentationOptions.DiameterRadiusMarginFromShape, diamRotation);
                    diam.Shape.Translate(xyPointTranslation.X, xyPointTranslation.Y);

                    dims.Add(diam);
                }
                #endregion
                #region Radius Dimension
                else if (DimensionsPresentationOptions.ShowRadius)
                {
                    var radius = builder.SetStart(start)
                        .SetEnd(end)
                        .SetPresentationOptions(DimensionsPresentationOptions.DiameterRadiusPresentationOptions)
                        .SetDimensionLineOptions(DimensionsPresentationOptions.RadiusLineOptions)
                        .SetDimensionValue(originalShape.CircleRadius)
                        .SetName($"Radius ({this.Name})")
                        .BuildDimensionLine();

                    //Rotate the Dimension and then according to the Applied Rotation Translate to apply Margin from the Shape
                    var radiusRotation = DimensionsPresentationOptions.RadiusLineOptions.StartRotationAngle;
                    var xyPointTranslation = MathCalculations.Points.GetPointAtDistanceFromPoint(DimensionsPresentationOptions.DiameterRadiusMarginFromShape, radiusRotation);
                    radius.Shape.Translate(xyPointTranslation.X, xyPointTranslation.Y);

                    dims.Add(radius);
                }
                #endregion
            }

            if (DimensionsPresentationOptions.ShowRectangleDimension)
            {
                var midRectangle = Shape.GetMiddleRectangle();
                PointXY dimStart;
                PointXY dimEnd;
                //According to Orientation Present the Horizontal or Vertical Rectangle Dimension of the Capsule
                switch (Shape.Orientation)
                {
                    case ShapesLibrary.Enums.CapsuleOrientation.Horizontal:
                        var rectLength = midRectangle.Length;
                        switch (DimensionsPresentationOptions.RectangleDimensionPosition)
                        {
                            case CapsuleRectangleDimensionPosition.LeftTop:
                            case CapsuleRectangleDimensionPosition.RightTop:
                                dimStart = new(midRectangle.LeftX, midRectangle.TopY - DimensionsPresentationOptions.RectangleDimensionMarginFromShape);
                                dimEnd = new(midRectangle.RightX, midRectangle.TopY - DimensionsPresentationOptions.RectangleDimensionMarginFromShape);
                                break;
                            case CapsuleRectangleDimensionPosition.LeftBottom:
                            case CapsuleRectangleDimensionPosition.RightBottom:
                                dimStart = new(midRectangle.LeftX, midRectangle.BottomY + DimensionsPresentationOptions.RectangleDimensionMarginFromShape);
                                dimEnd = new(midRectangle.RightX, midRectangle.BottomY + DimensionsPresentationOptions.RectangleDimensionMarginFromShape);
                                break;
                            default:
                                throw new EnumValueNotSupportedException(DimensionsPresentationOptions.RectangleDimensionPosition);
                        }
                        break;
                    case ShapesLibrary.Enums.CapsuleOrientation.Vertical:
                        var rectHeight = midRectangle.Height;
                        switch (DimensionsPresentationOptions.RectangleDimensionPosition)
                        {
                            case CapsuleRectangleDimensionPosition.LeftTop:
                            case CapsuleRectangleDimensionPosition.LeftBottom:
                                dimStart = new(midRectangle.LeftX - DimensionsPresentationOptions.RectangleDimensionMarginFromShape, midRectangle.TopY);
                                dimEnd = new(midRectangle.LeftX - DimensionsPresentationOptions.RectangleDimensionMarginFromShape, midRectangle.BottomY);
                                break;
                            case CapsuleRectangleDimensionPosition.RightTop:
                            case CapsuleRectangleDimensionPosition.RightBottom:
                                dimStart = new(midRectangle.RightX + DimensionsPresentationOptions.RectangleDimensionMarginFromShape, midRectangle.TopY);
                                dimEnd = new(midRectangle.RightX + DimensionsPresentationOptions.RectangleDimensionMarginFromShape, midRectangle.BottomY);
                                break;
                            default:
                                throw new EnumValueNotSupportedException(DimensionsPresentationOptions.RectangleDimensionPosition);
                        }
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Shape.Orientation);
                }
                var rectDim = builder.SetStart(dimStart)
                    .SetEnd(dimEnd)
                    .SetPresentationOptions(DimensionsPresentationOptions.RectangleDimensionPresentationOptions)
                    .SetDimensionLineOptions(DimensionsPresentationOptions.RectangleDimensionLineOptions)
                    .SetDimensionValue(originalShape.Orientation == CapsuleOrientation.Horizontal ? originalShape.GetMiddleRectangle().Length : originalShape.GetMiddleRectangle().Height)
                    .SetName($"Inner Rectangle Dimension ({this.Name})")
                    .BuildDimensionLine();
                dims.Add(rectDim);
            }

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
            line.SetLocation(this.LocationX, this.Shape.LocationY - this.Shape.GetTotalHeight() / 2d);
            return line;
        }
        private LineDrawing GetHorizontalCenterHelpline()
        {
            var line = GetUnpositionedHorizontalCenterHelpLine();
            line.SetLocation(this.Shape.LocationX -this.Shape.GetTotalLength() / 2d, this.LocationY);
            return line;
        }


        protected override void BuildPathData()
        {
            pathDataBuilder.AddCapsule(Shape);
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.AddCapsuleHole(Shape);
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
        public override CapsuleDrawing GetDeepClone()
        {
            var clone = (CapsuleDrawing)base.GetDeepClone();
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        public override CapsuleDrawing GetDeepClone(bool generateUniqueId)
        {
            var clone = (CapsuleDrawing)base.GetDeepClone(generateUniqueId);
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
    }

    public class CapsuleRingDrawing : ShapeDrawing<CapsuleRingInfo,ShapeDimensionsPresentationOptions>, IDeepClonable<CapsuleRingDrawing>
    {
        public override ShapeDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<ShapeDimensionsPresentationOptions>();

        public CapsuleRingDrawing(CapsuleRingInfo capsuleRingInfo) : base(capsuleRingInfo) { }
        public CapsuleRingDrawing(CapsuleRingInfo capsuleRingInfo, DrawingPresentationOptions? presentationOptions = null) : base(capsuleRingInfo, presentationOptions) { }
        protected override void BuildPathData()
        {
            pathDataBuilder.AddCapsule(Shape).AddCapsuleHole(Shape.GetInnerRingWholeShape());
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.AddCapsuleHole(Shape).AddCapsule(Shape.GetInnerRingWholeShape());
        }

        public override CapsuleRingDrawing GetDeepClone()
        {
            var clone = (CapsuleRingDrawing)base.GetDeepClone();
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        public override CapsuleRingDrawing GetDeepClone(bool generateUniqueId)
        {
            var clone = (CapsuleRingDrawing)base.GetDeepClone(generateUniqueId);
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            throw new NotImplementedException();
        }

        public override IEnumerable<IDrawing> GetHelpLinesDrawings()
        {
            throw new NotImplementedException();
        }
    }

}
