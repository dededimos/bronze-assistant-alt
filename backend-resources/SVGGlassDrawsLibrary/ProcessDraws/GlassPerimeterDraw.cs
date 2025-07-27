using ShowerEnclosuresModelsLibrary.Models;
using SVGDrawingLibrary;
using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGGlassDrawsLibrary.ProcessDraws
{
    public class GlassPerimeterDraw : RectangleDraw
    {
        private readonly Glass glass;

        public GlassPerimeterDraw(Glass glass) : base(glass.Length, glass.Height)
        {
            this.glass = glass;
            SetCenterOrStartX(0, CSCoordinate.Start);
            SetCenterOrStartY(0, CSCoordinate.Start);
        }

        public override string GetShapePathData()
        {
            PathDataBuilder builder = new();
            return builder.MoveTo(StartX, StartY)
                          .AddRectangle(Length, Height, glass.CornerRadiusTopLeft, glass.CornerRadiusTopRight, 0, 0)
                          .GetPathData();
        }

        /// <summary>
        /// Created an Empty Perimeter Draw
        /// </summary>
        private GlassPerimeterDraw()
        {
            this.glass = new();
        }

        public static GlassPerimeterDraw Empty() => new GlassPerimeterDraw();
    }
}
