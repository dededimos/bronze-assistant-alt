using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses;
using SVGDrawingLibrary;
using SVGDrawingLibrary.Models.ConcreteShapes;
using SVGGlassDrawsLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGGlassDrawsLibrary.ProcessDraws
{
    public class GlassHoleDraw : CircleDraw
    {
        private readonly GlassHole glassHole;

        private GlassHoleDraw(GlassHole glassHole, double centerX = 0, double centerY = 0) : base(glassHole.Diameter, centerX, centerY)
        {
            this.glassHole = glassHole;
        }

        public override string GetShapePathData()
        {
            PathDataBuilder builder = new();
            return builder.AddCircleHole(CenterX, CenterY, Radius)
                          .CloseDraw()
                          .GetPathData();
        }
        public override GlassHoleDraw CloneSelf()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Creates a Glass Hole Draw for a certain Glass
        /// </summary>
        /// <param name="glass">The Glass</param>
        /// <param name="hole">The Hole</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static GlassHoleDraw Create(GlassHole hole, Glass glass)
        {
            var centerX = hole.GetProcessDistanceFromEdgeX(glass);
            var centerY = hole.GetProcessDistanceFromEdgeY(glass);

            return new GlassHoleDraw(hole,centerX, centerY);
        }
    }
}
