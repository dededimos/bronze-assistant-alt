using CommonInterfacesBronze;
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
    public class LineDrawing : ShapeDrawing<LineInfo,ShapeDimensionsPresentationOptions>, IDeepClonable<LineDrawing>
    {
        public override ShapeDimensionsPresentationOptions DimensionsPresentationOptions { get; set; } = ShapeDimensionsPresentationOptions.GetEmptyDimensionOptions<ShapeDimensionsPresentationOptions>();

        public LineDrawing(LineInfo lineInfo) : base(lineInfo){}
        public LineDrawing(LineInfo lineInfo , DrawingPresentationOptions? presentationOptions = null) :base(lineInfo , presentationOptions) { }

        public override IEnumerable<DimensionLineDrawing> GetDimensionsDrawings() => [];
        public override IEnumerable<IDrawing> GetHelpLinesDrawings() => [];
        protected override void BuildPathData()
        {
            pathDataBuilder.MoveTo(Shape.StartX, Shape.StartY).AddLine(Shape.EndX, Shape.EndY);
        }
        protected override void BuildReversePathData()
        {
            pathDataBuilder.MoveTo(Shape.EndX, Shape.EndY).AddLine(Shape.StartX, Shape.StartY);
        }
        
        public override LineDrawing GetDeepClone()
        {
            return (LineDrawing)base.GetDeepClone();
        }
        public override LineDrawing GetDeepClone(bool generateUniqueId)
        {
            return (LineDrawing)base.GetDeepClone(generateUniqueId);
        }
    }
}
