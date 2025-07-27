using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    public class HoledCapsuleRectangleDraw : CapsuleRectangleDraw
    {
        public double Thickness { get; set; }

        public double InnerLength { get => GetInnerLength(); }
        public double InnerHeight { get => GetInnerHeight(); }
        public double InnerStartX { get => GetInnerStartX(); }
        public double InnerStartY { get => GetInnerStartY(); }
        public double InnerCornerRadius { get => GetInnerCornerRadius(); }

        /// <summary>
        /// Creates a CapsuleHole in a Capsule Draw with the corresponding thickness
        /// </summary>
        /// <param name="length">The Length of the Capsule</param>
        /// <param name="height">The Height of the Capsule</param>
        /// <param name="thickness">The Thickness of the Capsule</param>
        public HoledCapsuleRectangleDraw(double length, double height, double thickness) : base(length, height)
        {
            this.Thickness = thickness;
        }

        private CapsuleRectangleDraw GetInnerCapsule()
        {
            CapsuleRectangleDraw innerCapsule = new(InnerLength,InnerHeight);
            innerCapsule.SetCenterOrStartX(this.ShapeCenterX,CSCoordinate.Center);
            innerCapsule.SetCenterOrStartY(this.ShapeCenterY, CSCoordinate.Center);
            return innerCapsule;
        }

        private double GetInnerCornerRadius()
        {
            double innerCornerRadius = Math.Min(InnerLength, InnerHeight) / 2d;
            return innerCornerRadius;
        }
        private double GetInnerStartX()
        {
            double innerStartX = StartX + Thickness;
            return innerStartX;
        }
        private double GetInnerStartY()
        {
            double innerStartY = StartY + Thickness;
            return innerStartY;
        }
        private double GetInnerHeight()
        {
            double innerHeight = Height - 2 * Thickness;
            return innerHeight;
        }
        private double GetInnerLength()
        {
            double innerLength  = Length - 2 * Thickness;
            return innerLength;
        }
        public override HoledCapsuleRectangleDraw CloneSelf()
        {
            HoledCapsuleRectangleDraw clone = new(Length, Height,Thickness);
            clone.StartX = this.StartX;
            clone.StartY = this.StartY;
            //clone.Length = this.Length; Set by Constructor
            //clone.Height = this.Height; Set by Constructor
            //clone.RoundedCorners = this.RoundedCorners; Set by Constructor
            //clone.CornerRadius = this.CornerRadius; Set By Length and Height
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
        /// Returns the Path Data of a HoledCapsule
        /// </summary>
        /// <returns>PathData String</returns>
        public override string GetShapePathData()
        {
            string outerCapsule = PathDataFactory.Rectangle(StartX, StartY, Length, Height, CornerRadius, RoundedCorners);
            string innerCapsule = PathDataFactory.RectangleAnticlockwise(InnerStartX, InnerStartY, InnerLength, InnerHeight, InnerCornerRadius, RoundedCorners);

            StringBuilder builder = new();
            string pathData = builder.Append(outerCapsule).Append(innerCapsule).ToString();
            return pathData;
        }

    }
}
