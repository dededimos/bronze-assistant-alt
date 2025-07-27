using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;

namespace DrawingLibrary.Models.ConcreteGraphics
{
    public class EllipseDrawing : ShapeDrawing<EllipseInfo,EllipseDimensionsPresentationOptions>,IPolygonSimulatableDrawing, IDeepClonable<EllipseDrawing>
    {
        public override EllipseDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<EllipseDimensionsPresentationOptions>();
        public int MinSimulationSides => EllipseInfo.MINSIMULATIONSIDES;

        public EllipseDrawing(EllipseInfo ellipseInfo) : base(ellipseInfo) { }
        public EllipseDrawing(EllipseInfo ellipseInfo, DrawingPresentationOptions? presentationOptions = null) : base(ellipseInfo, presentationOptions) { }
        public EllipseDrawing(EllipseInfo ellipseInfo, DrawingPresentationOptions? presentationOptions = null, EllipseDimensionsPresentationOptions? dimensionsOptions = null)
            : base(ellipseInfo, presentationOptions)
        {
            if (dimensionsOptions != null) DimensionsPresentationOptions = dimensionsOptions;
        }

        protected override void BuildPathData()
        {
            pathDataBuilder.AddEllipse(Shape);
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.AddEllipseHole(Shape);
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
        public override EllipseDrawing GetDeepClone()
        {
            var clone = (EllipseDrawing)base.GetDeepClone();
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        public override EllipseDrawing GetDeepClone(bool generateUniqueId)
        {
            var clone = (EllipseDrawing)base.GetDeepClone(generateUniqueId);
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            List<DimensionLineDrawing> dims = [.. base.GetDimensionsDrawings()];
            var builder = DimensionLineDrawing.GetBuilder();

            #region Radius Major
            if (DimensionsPresentationOptions.ShowRadiusMajor)
            {
                PointXY start;
                PointXY end;
                switch (Shape.Orientation)
                {
                    case ShapesLibrary.Enums.EllipseOrientation.Horizontal:
                        switch (DimensionsPresentationOptions.RadiusDimensionPosition)
                        {
                            case Enums.EllipseRadiusDimensionPosition.LeftTop:
                            case Enums.EllipseRadiusDimensionPosition.LeftBottom:
                                start = new(Shape.LocationX - Shape.RadiusMajor, Shape.LocationY);
                                end = new(Shape.LocationX, Shape.LocationY);
                                break;
                            case Enums.EllipseRadiusDimensionPosition.RightTop:
                            case Enums.EllipseRadiusDimensionPosition.RightBottom:
                                start = new(Shape.LocationX, Shape.LocationY);
                                end = new(Shape.LocationX + Shape.RadiusMajor, Shape.LocationY);
                                break;
                            default:
                                throw new EnumValueNotSupportedException(DimensionsPresentationOptions.RadiusDimensionPosition);
                        }
                        break;
                    case ShapesLibrary.Enums.EllipseOrientation.Vertical:
                        switch (DimensionsPresentationOptions.RadiusDimensionPosition)
                        {
                            case Enums.EllipseRadiusDimensionPosition.LeftTop:
                            case Enums.EllipseRadiusDimensionPosition.RightTop:
                                start = new(Shape.LocationX, Shape.LocationY - Shape.RadiusMajor);
                                end = new(Shape.LocationX, Shape.LocationY);
                                break;
                            case Enums.EllipseRadiusDimensionPosition.LeftBottom:
                            case Enums.EllipseRadiusDimensionPosition.RightBottom:
                                start = new(Shape.LocationX, Shape.LocationY);
                                end = new(Shape.LocationX, Shape.LocationY + Shape.RadiusMajor);
                                break;
                            default:
                                throw new EnumValueNotSupportedException(DimensionsPresentationOptions.RadiusDimensionPosition);
                        }
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Shape.Orientation);
                }

                var radiusMajor = builder.SetStart(start).SetEnd(end)
                    .SetPresentationOptions(DimensionsPresentationOptions.RadiusMajorPresentationOptions)
                    .SetDimensionLineOptions(DimensionsPresentationOptions.RadiusMajorLineOptions)
                    .SetDimensionValue(originalShape.RadiusMajor)
                    .SetName($"Radius Major ({this.Name})")
                    .BuildDimensionLine();
                dims.Add(radiusMajor);
            }
            #endregion

            #region Radius Minor

            if (DimensionsPresentationOptions.ShowRadiusMinor)
            {
                PointXY start;
                PointXY end;
                switch (Shape.Orientation)
                {
                    case ShapesLibrary.Enums.EllipseOrientation.Horizontal:
                        switch (DimensionsPresentationOptions.RadiusDimensionPosition)
                        {
                            case Enums.EllipseRadiusDimensionPosition.LeftTop:
                            case Enums.EllipseRadiusDimensionPosition.RightTop:
                                start = new(Shape.LocationX, Shape.LocationY - Shape.RadiusMinor);
                                end = new(Shape.LocationX, Shape.LocationY);
                                break;
                            case Enums.EllipseRadiusDimensionPosition.LeftBottom:
                            case Enums.EllipseRadiusDimensionPosition.RightBottom:
                                start = new(Shape.LocationX, Shape.LocationY);
                                end = new(Shape.LocationX, Shape.LocationY + Shape.RadiusMinor);
                                break;
                            default:
                                throw new EnumValueNotSupportedException(DimensionsPresentationOptions.RadiusDimensionPosition);
                        }
                        break;
                    case ShapesLibrary.Enums.EllipseOrientation.Vertical:
                        switch (DimensionsPresentationOptions.RadiusDimensionPosition)
                        {
                            case Enums.EllipseRadiusDimensionPosition.LeftTop:
                            case Enums.EllipseRadiusDimensionPosition.LeftBottom:
                                start = new(Shape.LocationX - Shape.RadiusMinor, Shape.LocationY);
                                end = new(Shape.LocationX, Shape.LocationY);
                                break;
                            case Enums.EllipseRadiusDimensionPosition.RightTop:
                            case Enums.EllipseRadiusDimensionPosition.RightBottom:
                                start = new(Shape.LocationX, Shape.LocationY);
                                end = new(Shape.LocationX + Shape.RadiusMinor, Shape.LocationY);
                                break;
                            default:
                                throw new EnumValueNotSupportedException(DimensionsPresentationOptions.RadiusDimensionPosition);
                        }
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Shape.Orientation);
                }
                var radiusMinor = builder.SetStart(start).SetEnd(end)
                    .SetPresentationOptions(DimensionsPresentationOptions.RadiusMinorPresentationOptions)
                    .SetDimensionLineOptions(DimensionsPresentationOptions.RadiusMinorLineOptions)
                    .SetDimensionValue(originalShape.RadiusMinor)
                    .SetName($"Radius Minor ({this.Name})")
                    .BuildDimensionLine();
                dims.Add(radiusMinor);
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
            line.SetLocation(this.LocationX, this.Shape.LocationY - this.Shape.GetTotalHeight()/2d);
            return line;
        }
        private LineDrawing GetHorizontalCenterHelpline()
        {
            var line = GetUnpositionedHorizontalCenterHelpLine();
            line.SetLocation(this.Shape.LocationX - this.Shape.GetTotalLength() / 2d, this.LocationY);
            return line;
        }
    }
}
