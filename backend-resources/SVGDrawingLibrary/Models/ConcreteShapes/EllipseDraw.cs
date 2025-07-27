using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    public class EllipseDraw : DrawShape
    {
        /// <summary>
        /// The X Coordinate of the LeftMost Point
        /// </summary>
        public double StartX { get; protected set; }
        /// <summary>
        /// The Y Coordinate of the LeftMost Point
        /// </summary>
        public double StartY { get; protected set; }
        /// <summary>
        /// The Length of the Ellipse
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// The Height of the Ellipse
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Creates an Ellipse with a center at (x,y) and the given Length and Height
        /// </summary>
        /// <param name="length">The Length of the Ellipse</param>
        /// <param name="height">The Height of the Ellipse</param>
        /// <param name="centerX">The X Coordinate of the Ellipse Center</param>
        /// <param name="centerY">The Y Coordinate of the Ellipse Center</param>
        public EllipseDraw(double length, double height,double centerX = 0 , double centerY = 0)
        {
            this.Length = length;
            this.Height = height;
            SetCenterOrStartX(centerX, CSCoordinate.Center);
            SetCenterOrStartY(centerY, CSCoordinate.Center);
        }

        /// <summary>
        /// Sets the ShapeCenterX or the StartX 
        /// </summary>
        /// <param name="newX">The newX we want to Set</param>
        /// <param name="centerOrStart">Sets The Selected Property</param>
        public override void SetCenterOrStartX(double newX, CSCoordinate centerOrStart)
        {
            if (centerOrStart is CSCoordinate.Center)
            {
                ShapeCenterX = newX;
                StartX = newX - Length / 2d;
            }
            else if (centerOrStart is CSCoordinate.Start)
            {
                StartX = newX;
                ShapeCenterX = newX + Length / 2d;
            }
        }

        /// <summary>
        /// Sets the ShapeCenterY or the StartY 
        /// </summary>
        /// <param name="newY">The newY we want to Set</param>
        /// <param name="centerOrStart">Sets The Selected Property</param>
        public override void SetCenterOrStartY(double newY, CSCoordinate centerOrStart)
        {
            ShapeCenterY = newY;
            StartY = newY;
        }

        /// <summary>
        /// Gets the Eccentricity of the Ellipse
        /// </summary>
        /// <returns> a Number from 0 to 1 Defining the Ellipse Eccentricity</returns>
        public double GetEccentricity()
        {
            double e;
            if (Length > Height && Length != 0 && Height != 0)
            {
                double a2 = Math.Pow(Length / 2d, 2);
                double b2 = Math.Pow(Height / 2d, 2);
                e = Math.Sqrt(1 - (b2 / a2));
            }
            else if (Height > Length && Length != 0 && Height != 0)
            {
                double a2 = Math.Pow(Length / 2d, 2);
                double b2 = Math.Pow(Height / 2d, 2);
                e = Math.Sqrt(1 - (a2 / b2));
            }
            else
            {
                //Ellipse is a circle or cannot be Defined with the Given Length/Height
                e = 0;
            }
            return e;
        }

        /// <summary>
        /// Clones the Rectangle
        /// </summary>
        /// <returns>The Clone</returns>
        public override EllipseDraw CloneSelf()
        {
            EllipseDraw clone = new(this.Length,this.Height,this.ShapeCenterX,this.ShapeCenterY);
            //clone.StartX = this.StartX;
            //clone.StartY = this.StartY;
            //clone.Length = this.Length;
            //clone.Height = this.Height;
            //clone.ShapeCenterX = this.ShapeCenterX;
            //clone.ShapeCenterY = this.ShapeCenterY;
            clone.Name = this.Name;
            clone.Stroke = this.Stroke;
            clone.Fill = this.Fill;
            clone.Filter = this.Filter;
            clone.Opacity = this.Opacity;
            return clone;
        }

        /// <summary>
        /// Returns the X Coordinate of the Bounding Box Center
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxCenterX()
        {
            return this.ShapeCenterX;
        }

        /// <summary>
        /// Returns the Y Coordinate of the Bounding Box Center
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxCenterY()
        {
            return this.ShapeCenterY;
        }

        /// <summary>
        /// Returns the Height of the Bounding Box
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxHeight()
        {
            return Height;
        }

        /// <summary>
        /// Returns the Length of the Bounding Box
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxLength()
        {
            return Length;
        }

        /// <summary>
        /// Returns the Path Data of an Ellipse
        /// </summary>
        /// <returns>Path Data String</returns>
        public override string GetShapePathData()
        {
            string pathData = PathDataFactory.Ellipse(ShapeCenterX, ShapeCenterY, Length, Height);
            return pathData;
        }
    }
}
