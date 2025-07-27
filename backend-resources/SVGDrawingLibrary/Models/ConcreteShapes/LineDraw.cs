using SVGDrawingLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    public class LineDraw : DrawShape
    {
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public double Length { get => GetLength(); }
        public double Slope { get => GetSlope(); }

        public LineDraw()
        {

        }

        public LineDraw(double startX , double startY , double endX , double endY)
        {
            this.StartX = startX;
            this.StartY = startY;
            this.EndX = endX;
            this.EndY = endY;
            SetCenterOrStartX(startX, CSCoordinate.Start);
            SetCenterOrStartY(startY, CSCoordinate.Start);
        }

        /// <summary>
        /// Gets the Slope of the Line (y=mx+b) where m=Slope , tanθ = m , where θ=Line Inclination (Angle with X axis)
        /// </summary>
        /// <returns>The Line Slope</returns>
        private double GetSlope()
        {
            double slope = MathCalc.GetLineSlope(StartX, StartY, EndX, EndY);
            return slope;
        }

        //The Shape Center Placement here is Broken

        /// <summary>
        /// Gets the Length of the Line
        /// </summary>
        /// <returns>the Length of the Line</returns>
        private double GetLength()
        {
            //Pythagorean
            double length = Math.Sqrt(Math.Pow((EndX - StartX), 2) + Math.Pow((EndY - StartY), 2));
            return length;
        }

        /// <summary>
        /// Sets the ShapesCenterX or the LineStart (This method ONLY FOR THE LINE SHAPE , WILL SET ONLY ONE PROPERTY NOT BOTH)
        /// The Line Needs Always also its EndX
        /// </summary>
        /// <param name="newX">The new X</param>
        /// <param name="centerOrStart">Which coordinate to Set</param>
        public override void SetCenterOrStartX(double newX, CSCoordinate centerOrStart)
        {
            if (centerOrStart is CSCoordinate.Center)
            {
                //Find the Difference and Deduct,Add to both end and Start , Then Set the New Center
                StartX += (newX - ShapeCenterX);
                EndX += (newX - ShapeCenterX);
                ShapeCenterX = newX;
            }
            else if (centerOrStart is CSCoordinate.Start)
            {
                EndX += (newX - StartX);
                StartX = newX;
                ShapeCenterX = (StartX + EndX) / 2d;
            }
        }

        /// <summary>
        /// Sets the ShapesCenterY or the LineStart (This method ONLY FOR THE LINE SHAPE , WILL SET ONLY ONE PROPERTY NOT BOTH)
        /// The Line Needs Always also its EndY
        /// </summary>
        /// <param name="newY">The new Y</param>
        /// <param name="centerOrStart">Which coordinate to Set</param>
        public override void SetCenterOrStartY(double newY, CSCoordinate centerOrStart)
        {
            if (centerOrStart is CSCoordinate.Center)
            {
                StartY += (newY - ShapeCenterY);
                EndY += (newY - ShapeCenterY);
                ShapeCenterY = newY;
            }
            else if (centerOrStart is CSCoordinate.Start)
            {
                EndY += newY - StartY;
                StartY = newY;
                ShapeCenterY = (StartY + EndY) / 2d;
            }
        }

        /// <summary>
        /// Returns a DeepClone of the Line
        /// </summary>
        /// <returns>The Clone</returns>
        public override DrawShape CloneSelf()
        {
            LineDraw clone = new();
            clone.StartX = this.StartX;
            clone.StartY = this.StartY;
            clone.EndX = this.EndX;
            clone.EndY = this.EndY;

            clone.Name = this.Name;
            clone.Stroke = this.Stroke;
            clone.Fill = this.Fill;
            clone.Filter = this.Filter;
            clone.Opacity = this.Opacity;
            clone.ShapeCenterX = this.ShapeCenterX;
            clone.ShapeCenterY = this.ShapeCenterY;
            return clone;
        }

        /// <summary>
        /// Get the Center X of the Line's Bounding Box
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxCenterX()
        {
            //Practically the Line Center X
            double centerX = (StartX + EndX) / 2d;
            return centerX;
        }

        /// <summary>
        /// Get the CenterY of the Line's Bounding Box
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxCenterY()
        {
            //Practically the Line Center Y
            double centerY = (StartY + EndY) / 2d;
            return centerY;
        }

        /// <summary>
        /// Gets the Height of the Line's Bounding Box (If line is Vertical it matches the Length of the Line)
        /// </summary>
        /// <returns>The Height of the Bounding Box</returns>
        public override double GetBoundingBoxHeight()
        {
            double height = Math.Abs(EndY - StartY);
            return height;
        }

        /// <summary>
        /// Gets the Length of the Line's Bounding Box (If line is Horizontal it matches the Length of the Line)
        /// </summary>
        /// <returns>The Length of the Bounding Box</returns>
        public override double GetBoundingBoxLength()
        {
            double length = Math.Abs(EndX - StartX);
            return length;
        }
        
        /// <summary>
        /// Returns the PathData String of the Line
        /// </summary>
        /// <returns>the Path Data</returns>
        public override string GetShapePathData()
        {
            string pathData = PathDataFactory.Line(StartX, StartY, EndX, EndY);
            return pathData;
        }
    }
}
