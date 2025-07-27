using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses;
using SVGDrawingLibrary;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SVGGlassDrawsLibrary.Helpers.GlassDrawExtensions;

namespace SVGGlassDrawsLibrary.ProcessDraws
{
    public class Cut9BDraw : RectangleDraw
    {
        private Cut9B cut;

        private Cut9BDraw(Cut9B cut):base(cut.Length,cut.Height)
        {
            this.cut = cut;
        }

        public override string GetShapePathData()
        {
            PathDataBuilder builder = new();
            return builder.MoveTo(StartX,StartY)
                .AddRectangleHole(Length,Height)
                .CloseDraw().GetPathData();
        }

        public static Cut9BDraw Create(Cut9B cut,Glass ownerGlass)
        {
            var cutDraw = new Cut9BDraw(cut);
            var startX = cut.GetProcessDistanceFromEdgeX(ownerGlass);
            var startY = cut.GetProcessDistanceFromEdgeY(ownerGlass);

            //The Start and Y Must Change According to the Provided Distancing
            if (cut.VerticalDistancing is VertDistancing.FromBottom)
            {
                //Must put the start Y of the Rectangle a little up because the start
                //Y returned from the Method is the Bottom Left Corner of the Rectangle and not the TopLeft
                startY -= cut.Height;
            }

            cutDraw.SetCenterOrStartX(startX, CSCoordinate.Start);
            cutDraw.SetCenterOrStartY(startY, CSCoordinate.Start);
            return cutDraw;
        }

        public override RectangleDraw CloneSelf()
        {
            throw new NotImplementedException();
        }
    }
}
