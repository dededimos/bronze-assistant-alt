using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.PresentationOptions;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary.Models.ConcreteGraphics
{
    public class PolygonDrawing : ShapeDrawing<PolygonInfo , ShapeDimensionsPresentationOptions>
    {
        public override ShapeDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<ShapeDimensionsPresentationOptions>();

        public PolygonDrawing(PolygonInfo polygonInfo) : base(polygonInfo) { }
        public PolygonDrawing(PolygonInfo polygonInfo, DrawingPresentationOptions? presentationOptions = null) : base(polygonInfo, presentationOptions) { }

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings() => [];
        public override IEnumerable<IDrawing> GetHelpLinesDrawings() => [];
        protected override void BuildPathData()
        {
            pathDataBuilder.AddPolygon(Shape);
        }
        protected override void BuildReversePathData()
        {
            throw new NotImplementedException($"Inverse Polygon Path Data is not Implemented");
        }

    }
}
