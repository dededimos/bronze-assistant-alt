using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using ShapesLibrary.Enums;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.ConcreteGraphics
{
    public class CircleQuadrantDrawing : ShapeDrawing<CircleQuadrantInfo,CircleQuadrantDimensionsPresentationOptions>, IPolygonSimulatableDrawing, IDeepClonable<CircleQuadrantDrawing>
    {
        public override CircleQuadrantDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<CircleQuadrantDimensionsPresentationOptions>();
        public int MinSimulationSides => CircleQuadrantInfo.MINSIMULATIONSIDES;

        public CircleQuadrantDrawing(CircleQuadrantInfo quadrantInfo) : base(quadrantInfo) { }
        public CircleQuadrantDrawing(CircleQuadrantInfo quadrantInfo, DrawingPresentationOptions? presentationOptions = null) : base(quadrantInfo, presentationOptions) { }
        public CircleQuadrantDrawing(CircleQuadrantInfo quadrantInfo, DrawingPresentationOptions? presentationOptions = null, CircleQuadrantDimensionsPresentationOptions? dimensionsOptions = null)
            : base(quadrantInfo, presentationOptions)
        {
            if (dimensionsOptions != null) DimensionsPresentationOptions = dimensionsOptions;
        }
        protected override void BuildPathData()
        {
            pathDataBuilder.AddCircleQuadrant(Shape);
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.AddCircleQuadrantHole(Shape);
        }

        public override CircleQuadrantDrawing GetDeepClone()
        {
            var clone = (CircleQuadrantDrawing)base.GetDeepClone();
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }
        public override CircleQuadrantDrawing GetDeepClone(bool generateUniqueId)
        {
            var clone = (CircleQuadrantDrawing)base.GetDeepClone(generateUniqueId);
            clone.DimensionsPresentationOptions = this.DimensionsPresentationOptions.GetDeepClone();
            return clone;
        }

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            List<DimensionLineDrawing> dims = [.. base.GetDimensionsDrawings()];
            var builder = DimensionLineDrawing.GetBuilder();

            #region Radius Dimension
            if (DimensionsPresentationOptions.ShowRadius)
            {
                PointXY start;
                PointXY end;
                var circle = Shape.GetCircle();
                switch (Shape.QuadrantPart)
                {
                    case CircleQuadrantPart.TopLeft:
                        //Draw the Line Right to Left
                        start = MathCalculations.Circle.GetPointOnCirclePerimeter(circle, 5 * Math.PI / 4);
                        start = new(start.X - DimensionsPresentationOptions.RadiusMarginFromShape, start.Y);
                        end = new(start.X - DimensionsPresentationOptions.RadiusLineOptions.OneEndLineLength, start.Y);
                        break;
                    case CircleQuadrantPart.TopRight:
                        start = MathCalculations.Circle.GetPointOnCirclePerimeter(circle, 7 * Math.PI / 4);
                        start = new(start.X + DimensionsPresentationOptions.RadiusMarginFromShape, start.Y);
                        end = new(start.X + DimensionsPresentationOptions.RadiusLineOptions.OneEndLineLength, start.Y);
                        break;
                    case CircleQuadrantPart.BottomLeft:
                        start = MathCalculations.Circle.GetPointOnCirclePerimeter(circle, 3 * Math.PI / 4);
                        start = new(start.X - DimensionsPresentationOptions.RadiusMarginFromShape, start.Y);
                        end = new(start.X - DimensionsPresentationOptions.RadiusLineOptions.OneEndLineLength, start.Y);
                        break;
                    case CircleQuadrantPart.BottomRight:
                        start = MathCalculations.Circle.GetPointOnCirclePerimeter(circle, Math.PI / 4);
                        start = new(start.X + DimensionsPresentationOptions.RadiusMarginFromShape, start.Y);
                        end = new(start.X + DimensionsPresentationOptions.RadiusLineOptions.OneEndLineLength, start.Y);
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Shape.QuadrantPart);
                }
                var radius = builder.SetStart(start)
                        .SetEnd(end)
                        .SetPresentationOptions(DimensionsPresentationOptions.RadiusPresentationOptions)
                        .SetDimensionLineOptions(DimensionsPresentationOptions.RadiusLineOptions)
                        .SetDimensionValue(originalShape.Radius)
                        .SetName($"Radius ({this.Name})")
                        .BuildDimensionLine();
                dims.Add(radius);
            }
            #endregion

            return dims;
        }
        public override IEnumerable<IDrawing> GetHelpLinesDrawings() => [];

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
