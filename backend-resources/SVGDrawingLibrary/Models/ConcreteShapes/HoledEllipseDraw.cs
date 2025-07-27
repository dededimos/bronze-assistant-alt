using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    public class HoledEllipseDraw : EllipseDraw
    {
        public double Thickness { get; set; }

        public double InnerLength { get => GetInnerLength(); }
        public double InnerHeight { get => GetInnerHeight(); }
        public double InnerStartX { get => GetInnerStartX(); }
        public double InnerStartY { get => GetInnerStartY(); }

        /// <summary>
        /// Creates an EllipseHole in an Ellipse Draw with the corresponding thickness
        /// </summary>
        /// <param name="length">The Length of the Ellipse</param>
        /// <param name="height">The Height of the Ellipse</param>
        /// <param name="thickness">The Thickness of the Ellipse</param>
        /// <param name="centerX">The centerX of the Ellipse -- Defaults to 0</param>
        /// <param name="centerY">The centerY of the Ellipse -- Defaults to 0</param>
        public HoledEllipseDraw(double length, double height, double thickness , double centerX = 0 , double centerY = 0) : base(length, height,centerX,centerY)
        {
            this.Thickness = thickness;
        }

        private double GetInnerStartX()
        {
            double innerStartX = StartX + Thickness;
            return innerStartX;
        }
        private double GetInnerStartY()
        {
            double innerStartY = StartY;
            return innerStartY;
        }
        private double GetInnerHeight()
        {
            double innerHeight = Height - 2 * Thickness;
            return innerHeight;
        }
        private double GetInnerLength()
        {
            double innerLength = Length - 2 * Thickness;
            return innerLength;
        }

        public override HoledEllipseDraw CloneSelf()
        {
            HoledEllipseDraw clone = new(Length, Height, Thickness,ShapeCenterX,ShapeCenterY);
            //clone.Length = this.Length; Set by Constructor
            //clone.Height = this.Height; Set by Constructor
            //clone.RoundedCorners = this.RoundedCorners; Set by Constructor
            //clone.CornerRadius = this.CornerRadius; Set By Length and Height
            clone.Name = this.Name;
            clone.Stroke = this.Stroke;
            clone.Fill = this.Fill;
            clone.Filter = this.Filter;
            clone.Opacity = this.Opacity;
            return clone;
        }

        /// <summary>
        /// Returns the Path Data of a HoledEllipse
        /// </summary>
        /// <returns>PathData String</returns>
        public override string GetShapePathData()
        {
            string outerEllipse = PathDataFactory.Ellipse(ShapeCenterX, ShapeCenterY, Length, Height);
            string innerEllipse = PathDataFactory.EllipseAntiClockwise(ShapeCenterX,ShapeCenterY, InnerLength,InnerHeight);

            StringBuilder builder = new();
            string pathData = builder.Append(outerEllipse).Append(innerEllipse).ToString();
            return pathData;
        }

    }
}
