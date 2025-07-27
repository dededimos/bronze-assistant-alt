using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    /// <summary>
    /// A Rectangle with a Rectangle Hole in its Center
    /// </summary>
    public class HoledRectangleDraw : RectangleDraw
    {
        public double RectangleThickness { get; set; }
        public double InnerLength { get => GetInnerLength();}
        public double InnerHeight { get => GetInnerHeight(); }

        public double InnerStartX { get => GetInnerStartX(); }
        public double InnerStartY { get => GetInnerStartY(); }

        /// <summary>
        /// The Corner Radius of the Inner Rectangle
        /// Diminishes Equally with the Smallest Dimension  (Height or Length)
        /// </summary>
        public double InnerCornerRadius { get => GetInnerCornerRadius(); }

        /// <summary>
        /// Calculates the Inner Radius as a Proportion of Inner Length or Height whichever is Smaller
        /// The proportion is the same as the Proportion of the Outer Corner Radius
        /// </summary>
        /// <returns>TheInnerCornerRadius</returns>
        private double GetInnerCornerRadius()
        {
            //Get Outer Corner Radius Proportion
            var smallestFromLengthHeight = Math.Min(Length,Height);
            var proportionOfCornerRadius = CornerRadius / smallestFromLengthHeight;

            var smallestFromInnerLengthHeight = Math.Min(InnerLength, InnerHeight);
            return proportionOfCornerRadius * smallestFromInnerLengthHeight;
        }

        public HoledRectangleDraw()
        {

        }

        /// <summary>
        /// Creates a new Holed Rectangle with the Specified Parameters
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">The Height of the Rectangle</param>
        /// <param name="rectangleThickness">The Thickness of the Rectangle (Rest space will be Holed)</param>
        /// <param name="cornersToRound">Which of the Corners to Round</param>
        /// <param name="cornerRadius">The Corner Radius of the rounding</param>
        public HoledRectangleDraw(double length 
                                , double height 
                                , double rectangleThickness
                                , CornersToRound cornersToRound = CornersToRound.None
                                , double cornerRadius = 0):base(length,height,cornersToRound,cornerRadius)
        {
            RectangleThickness = rectangleThickness;
        }

        /// <summary>
        /// Returns the Inner Rectangle of the HoledRectangle Draw
        /// </summary>
        /// <returns>The Inner Rectangle Draw</returns>
        public RectangleDraw GetInnerRectangle()
        {
            RectangleDraw rectangle = new();
            rectangle.SetCenterOrStartX(InnerStartX,CSCoordinate.Start);
            rectangle.SetCenterOrStartY(InnerStartY, CSCoordinate.Start);
            rectangle.Length = InnerLength;
            rectangle.Height = InnerHeight;
            rectangle.RoundedCorners = Enums.CornersToRound.None;
            return rectangle;
        }

        /// <summary>
        /// Returns the InnerRectangles StartY
        /// </summary>
        /// <returns></returns>
        private double GetInnerStartY()
        {
            double innerStartY = StartY + RectangleThickness;
            return innerStartY;
        }

        /// <summary>
        /// Returns the InnerRectangles StartX
        /// </summary>
        /// <returns></returns>
        private double GetInnerStartX()
        {
            double innerStartX = StartX + RectangleThickness;
            return innerStartX;
        }

        /// <summary>
        /// Gets the Inner Length of the HoledRectangle
        /// </summary>
        /// <returns></returns>
        private double GetInnerLength()
        {
            double innerLength = Length - 2 * RectangleThickness;
            return innerLength;
        }

        /// <summary>
        /// Gets the Inner Height of the Holed Rectangle
        /// </summary>
        /// <returns></returns>
        private double GetInnerHeight()
        {
            double innerHeight = Height - 2 * RectangleThickness;
            return innerHeight;
        }

        /// <summary>
        /// Clones the HoledRectangle
        /// </summary>
        /// <returns>The Clone</returns>
        public override HoledRectangleDraw CloneSelf()
        {
            HoledRectangleDraw clone = new();
            clone.StartX = this.StartX;
            clone.StartY = this.StartY;
            clone.Length = this.Length;
            clone.Height = this.Height;
            clone.RoundedCorners = this.RoundedCorners;
            clone.CornerRadius = this.CornerRadius;
            clone.RectangleThickness = this.RectangleThickness;
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
        /// Returns the Path Data of a HoledRectangle
        /// </summary>
        /// <returns>Path Data String</returns>
        public override string GetShapePathData()
        {
            string outerRectangle = PathDataFactory.Rectangle(StartX, StartY, Length, Height, CornerRadius, RoundedCorners);
            string innerRectangle = PathDataFactory.RectangleAnticlockwise(InnerStartX, InnerStartY, InnerLength, InnerHeight,InnerCornerRadius,RoundedCorners);
            
            StringBuilder builder = new();
            string pathData = builder.Append(outerRectangle).Append(innerRectangle).ToString();
            return pathData;
        }

    }
}
