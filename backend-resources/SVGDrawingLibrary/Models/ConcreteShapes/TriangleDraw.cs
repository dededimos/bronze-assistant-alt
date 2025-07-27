using SVGDrawingLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    /// <summary>
    /// The Draw Object of a Triangle each subsequent vertice is clockwise to the previous
    /// </summary>
    public class TriangleDraw : DrawShape
    {
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double SecondX { get; set; }
        public double SecondY { get; set; }
        public double ThirdX { get; set; }
        public double ThirdY { get; set; }

        public override double ShapeCenterX { get => (StartX + SecondX + ThirdX) / 3d;}
        public override double ShapeCenterY { get => (StartY + SecondY + ThirdY) / 3d; }

        /// <summary>
        /// Deep Clones the Triangle
        /// </summary>
        /// <returns>The Clone</returns>
        public override DrawShape CloneSelf()
        {
            TriangleDraw clone = new()
            {
                StartX = this.StartX,
                StartY = this.StartY,
                SecondX = this.SecondX,
                SecondY = this.SecondY,
                ThirdX = this.ThirdX,
                ThirdY = this.ThirdY,
                Name = this.Name,
                Stroke = this.Stroke,
                Fill = this.Fill,
                Filter = this.Filter,
                Opacity = this.Opacity,
                ShapeCenterX = this.ShapeCenterX,
                ShapeCenterY = this.ShapeCenterY
            };
            return clone;
        }

        /// <summary>
        /// Get the X Coordinate of the Triangle's Bounding Box Center
        /// </summary>
        /// <returns>The Center X Coordinate</returns>
        public override double GetBoundingBoxCenterX()
        {
            //The Middle distance from MaxX and MinX (Find the Min and Add half the Length of the Bounding Box)
            double minX = Math.Min(StartX, Math.Min(SecondX, ThirdX));
            double centerX = minX + GetBoundingBoxLength() / 2d;
            return centerX;
        }

        /// <summary>
        /// Get the Y Coordinate of the Triangle's Bounding Box Center
        /// </summary>
        /// <returns>The Center Y Coordinate</returns>
        public override double GetBoundingBoxCenterY()
        {
            //The Middle distance from MaxY and MinY (Find the Min and Add half the Height of the Bounding Box)
            double minY = Math.Min(StartY, Math.Min(SecondY, ThirdY));
            double centerY = minY + GetBoundingBoxHeight() / 2d;
            return centerY;
        }

        /// <summary>
        /// Get the Triangle's Bounding Box Height
        /// </summary>
        /// <returns>The Height of the Bounding Box</returns>
        public override double GetBoundingBoxHeight()
        {
            //The MaxY - MinY is the Height of the Rectangle Box containing the Triangle
            double maxY = Math.Max(StartY, Math.Max(SecondY, ThirdY));
            double minY = Math.Min(StartY, Math.Min(SecondY, ThirdY));
            double height = maxY - minY;
            return height;
        }

        /// <summary>
        /// Get the Triangle's Bounding Box Length
        /// </summary>
        /// <returns>The Length of the Bounding Box</returns>
        public override double GetBoundingBoxLength()
        {
            //The MaxX - MinX is the Length of the Rectangle Box containing the Triangle
            double maxX = Math.Max(StartX, Math.Max(SecondX, ThirdX));
            double minX = Math.Min(StartX, Math.Min(SecondX, ThirdX));
            double length = maxX - minX;
            return length;
        }

        /// <summary>
        /// Returns the PathData of the Triangle Shape
        /// </summary>
        /// <returns>The Path Data String</returns>
        public override string GetShapePathData()
        {
            string pathData = PathDataFactory.Triangle(StartX, StartY, SecondX, SecondY, ThirdX, ThirdY);
            return pathData;
        }

        /// <summary>
        /// Moves the Draw X to the specified location
        /// </summary>
        /// <param name="newX"></param>
        /// <param name="centerOrStart"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void SetCenterOrStartX(double newX, CSCoordinate centerOrStart)
        {
            //Get the Previous Center and find the differences with the new
            //Accordingly move the points of the vertices of the triangle
            double differenceX;

            if (centerOrStart is CSCoordinate.Center)
            {
                differenceX = newX - ShapeCenterX;
            }
            else 
            {
                differenceX = newX - StartX;
            }
            StartX += differenceX;
            SecondX += differenceX;
            ThirdX += differenceX;
        }

        /// <summary>
        /// Moves the Draw Y to the specified location
        /// </summary>
        /// <param name="newY"></param>
        /// <param name="centerOrStart"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void SetCenterOrStartY(double newY, CSCoordinate centerOrStart)
        {
            //Get the Previous Center and find the differences with the new
            //Accordingly move the points of the vertices of the triangle
            double differenceY;
            if (centerOrStart is CSCoordinate.Center)
            {
                differenceY = newY - ShapeCenterY;
            }
            else
            {
                differenceY = newY - StartY;
            }
            StartY += differenceY;
            SecondY += differenceY;
            ThirdY += differenceY;
        }
    }
}
