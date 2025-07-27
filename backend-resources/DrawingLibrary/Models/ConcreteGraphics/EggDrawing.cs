using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using ShapesLibrary.Enums;
using ShapesLibrary.Interfaces;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;

namespace DrawingLibrary.Models.ConcreteGraphics
{
    public class EggDrawing : ShapeDrawing<EggShapeInfo,EggDimensionsPresentationOptions>,IPolygonSimulatableDrawing, IDeepClonable<EggDrawing>
    {

        public override EggDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<EggDimensionsPresentationOptions>();
        public int MinSimulationSides => EggShapeInfo.MINSIMULATIONSIDES;

        public EggDrawing(EggShapeInfo eggInfo) : base(eggInfo) { }
        public EggDrawing(EggShapeInfo eggInfo, DrawingPresentationOptions? presentationOptions = null) : base(eggInfo, presentationOptions) { }
        public EggDrawing(EggShapeInfo eggInfo, DrawingPresentationOptions? presentationOptions = null, EggDimensionsPresentationOptions? dimensionsOptions = null) : base(eggInfo, presentationOptions)
        {
            if (dimensionsOptions != null) DimensionsPresentationOptions = dimensionsOptions;
        }
        protected override void BuildPathData()
        {
            pathDataBuilder.AddEggShape(Shape);
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.AddEggShapeHole(Shape);
        }
        
        public override EggDrawing GetDeepClone()
        {
            var clone = (EggDrawing)base.GetDeepClone();
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        public override EggDrawing GetDeepClone(bool generateUniqueId)
        {
            var clone = (EggDrawing)base.GetDeepClone(generateUniqueId);
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            List<DimensionLineDrawing> dims = [.. base.GetDimensionsDrawings()];
            var builder = DimensionLineDrawing.GetBuilder();

            #region EllipseRadius Dimension
            if (DimensionsPresentationOptions.ShowEllipseRadius)
            {
                PointXY start;
                PointXY end;

                switch (Shape.Orientation)
                {
                    case EggOrientation.VerticalPointingTop:
                    case EggOrientation.HorizontalPointingLeft:
                        start = Shape.GetEllipseTipPoint();
                        end = Shape.GetLocation();
                        break;

                    case EggOrientation.VerticalPointingBottom:
                    case EggOrientation.HorizontalPointingRight:
                        start = Shape.GetLocation();
                        end = Shape.GetEllipseTipPoint();
                        break;

                    default:
                        throw new EnumValueNotSupportedException(Shape.Orientation);
                }

                var ellipseRadius = builder.SetStart(start).SetEnd(end)
                    .SetPresentationOptions(DimensionsPresentationOptions.EllipseRadiusPresentationOptions)
                    .SetDimensionLineOptions(DimensionsPresentationOptions.EllipseRadiusLineOptions)
                    .SetDimensionValue(originalShape.EllipseRadius)
                    .SetName($"Ellipse Radius ({this.Name})")
                    .BuildDimensionLine();
                dims.Add(ellipseRadius);
            }
            #endregion

            #region CircleRadius Dimension
            if (DimensionsPresentationOptions.ShowCircleRadius)
            {
                PointXY start;
                PointXY end;

                switch (Shape.Orientation)
                {
                    case EggOrientation.VerticalPointingTop:
                    case EggOrientation.VerticalPointingBottom:
                        switch (DimensionsPresentationOptions.CircleRadiusDimensionsPosition)
                        {
                            case Enums.EggCircleRadiusDimensionPosition.LeftTop:
                            case Enums.EggCircleRadiusDimensionPosition.LeftBottom:
                                start = new(Shape.LocationX - Shape.CircleRadius, Shape.LocationY);
                                end = Shape.GetLocation();
                                break;
                            case Enums.EggCircleRadiusDimensionPosition.RightTop:
                            case Enums.EggCircleRadiusDimensionPosition.RightBottom:
                                start = Shape.GetLocation();
                                end = new(Shape.LocationX + Shape.CircleRadius, Shape.LocationY);
                                break;
                            default:
                                throw new EnumValueNotSupportedException(DimensionsPresentationOptions.CircleRadiusDimensionsPosition);
                        }
                        break;
                    case EggOrientation.HorizontalPointingRight:
                    case EggOrientation.HorizontalPointingLeft:
                        switch (DimensionsPresentationOptions.CircleRadiusDimensionsPosition)
                        {
                            case Enums.EggCircleRadiusDimensionPosition.LeftTop:
                            case Enums.EggCircleRadiusDimensionPosition.RightTop:
                                start = new(Shape.LocationX, Shape.LocationY - Shape.CircleRadius);
                                end = Shape.GetLocation();
                                break;
                            case Enums.EggCircleRadiusDimensionPosition.RightBottom:
                            case Enums.EggCircleRadiusDimensionPosition.LeftBottom:
                                start = Shape.GetLocation();
                                end = new(Shape.LocationX, Shape.LocationY + Shape.CircleRadius);
                                break;
                            default:
                                throw new EnumValueNotSupportedException(DimensionsPresentationOptions.CircleRadiusDimensionsPosition);
                        }
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Shape.Orientation);
                }
                var circleRadius = builder.SetStart(start).SetEnd(end)
                    .SetPresentationOptions(DimensionsPresentationOptions.CircleRadiusPresentationOptions)
                    .SetDimensionLineOptions(DimensionsPresentationOptions.CircleRadiusLineOptions)
                    .SetDimensionValue(originalShape.CircleRadius)
                    .SetName($"Circle Radius ({this.Name})")
                    .BuildDimensionLine();
                dims.Add(circleRadius);
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

            double shiftOfLineStartFromCenter = 0;
            if (Shape.Orientation == EggOrientation.VerticalPointingTop)
            {
                shiftOfLineStartFromCenter = Shape.EllipseRadius;
            }
            else
            {
                shiftOfLineStartFromCenter = Shape.CircleRadius;
            }

            line.SetLocation(this.LocationX, this.Shape.LocationY - shiftOfLineStartFromCenter);
            return line;
        }
        private LineDrawing GetHorizontalCenterHelpline()
        {
            var line = GetUnpositionedHorizontalCenterHelpLine();
            double shiftOfLineStartFromCenter = 0;
            if (Shape.Orientation == EggOrientation.HorizontalPointingLeft)
            {
                shiftOfLineStartFromCenter = Shape.EllipseRadius;
            }
            else
            {
                shiftOfLineStartFromCenter = Shape.CircleRadius;
            }
            line.SetLocation(this.Shape.LocationX - shiftOfLineStartFromCenter, this.LocationY);
            return line;
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
    }
}

