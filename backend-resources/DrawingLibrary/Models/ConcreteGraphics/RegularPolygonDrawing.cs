using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using DrawingLibrary.Models.PresentationOptions;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapesLibrary.Services;
using ShapesLibrary;

namespace DrawingLibrary.Models.ConcreteGraphics
{
    public class RegularPolygonDrawing : ShapeDrawing<RegularPolygonInfo, RegularPolygonDimensionsPresentationOptions>
    {
        public override RegularPolygonDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<RegularPolygonDimensionsPresentationOptions>();

        public RegularPolygonDrawing(RegularPolygonInfo polygonInfo) : base(polygonInfo) { }
        public RegularPolygonDrawing(RegularPolygonInfo polygonInfo, DrawingPresentationOptions? presentationOptions = null) : base(polygonInfo, presentationOptions) { }
        public RegularPolygonDrawing(RegularPolygonInfo polygonInfo, DrawingPresentationOptions? presentationOptions, RegularPolygonDimensionsPresentationOptions? dimensionsOptions)
            : base(polygonInfo, presentationOptions)
        {
            if (dimensionsOptions is not null) DimensionsPresentationOptions = dimensionsOptions;
        }
        protected override void BuildPathData()
        {
            pathDataBuilder.AddPolygon(Shape);
        }
        protected override void BuildReversePathData()
        {
            throw new NotImplementedException($"Inverse Polygon Path Data is not Implemented");
        }

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings()
        {
            if (!DimensionsPresentationOptions.ShowDimensions) return [];
            List<DimensionLineDrawing> dims = [.. base.GetDimensionsDrawings()];

            if (DimensionsPresentationOptions.ShowRadius)
            {
                var helpCircleDraw = GetCircleHelpLine();
                var radiusLine = GetRadiusDimension(helpCircleDraw.Shape);
                dims.Add(radiusLine);
            }

            return dims;
        }
        public override IEnumerable<IDrawing> GetHelpLinesDrawings()
        {
            List<IDrawing> helplines = [];
            if (DimensionsPresentationOptions.ShowHelpCircle)
            {
                var helpCircleDraw = GetCircleHelpLine();
                helplines.Add(helpCircleDraw);
            }
            return helplines;
        }
        private CircleDrawing GetCircleHelpLine()
        {
            var circle = Shape.GetCircumscribedCircle();
            var circleDraw = new CircleDrawing(circle, DimensionsPresentationOptions.HelpCirclePresentationOptions);
            return circleDraw;
        }
        private DimensionLineDrawing GetRadiusDimension(CircleInfo helpCircle)
        {
            //find Radius Point
            PointXY point1 = new(helpCircle.LocationX - helpCircle.Radius - DimensionsPresentationOptions.RadiusMarginFromShape, helpCircle.LocationY);
            PointXY point2 = new(point1.X - DimensionsPresentationOptions.RadiusLineOptions.OneEndLineLength, point1.Y);
            var radiusLine = DimensionLineDrawing.GetBuilder().SetPresentationOptions(DimensionsPresentationOptions.RadiusPresentationOptions)
                .SetDimensionLineOptions(DimensionsPresentationOptions.RadiusLineOptions)
                .SetStart(point1)
                .SetEnd(point2)
                .SetDimensionValue(originalShape.CircumscribedRadius)
                .SetName($"Radius ({this.Name})")
                .BuildDimensionLine();
            return radiusLine;
        }
    }
}
