using SVGDrawingLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    public class HoledCircleDraw : CircleDraw
    {
        /// <summary>
        /// The Inner Diameter of the Holed Circle
        /// </summary>
        public double InnerDiameter { get; set; }
        /// <summary>
        /// The Inner Radius of the Holed Circle
        /// </summary>
        public double InnerRadius { get => GetInnerRadius(); }

        public HoledCircleDraw()
        {

        }

        public HoledCircleDraw(double diameter , double innerDiameter)
        {
            if (innerDiameter >= diameter)
            {
                throw new ArgumentOutOfRangeException("Inner Dimeter of a Holed Circle ,Cannot be Greater or equal to the Main Circle's Diameter");
            }

            InnerDiameter = innerDiameter;
            Diameter = diameter;
        }


        /// <summary>
        /// Gets the Inner Radius of the Holed Circle
        /// </summary>
        /// <returns></returns>
        private double GetInnerRadius()
        {
            double innerRadius = InnerDiameter / 2d;
            return innerRadius;
        }

        /// <summary>
        /// Returns the Draw of the Inner Circle of the HoledCircle
        /// </summary>
        /// <returns>the Circle Draw of the Inner Circle</returns>
        public CircleDraw GetInnerCircle()
        {
            CircleDraw circle = new();
            circle.SetCenterOrStartX(this.CenterX, CSCoordinate.Start);
            circle.SetCenterOrStartY(this.CenterY,CSCoordinate.Start);
            circle.Diameter = this.InnerDiameter;
            circle.Name = $"{this.Name}InnerArea";
            return circle;
        }

        /// <summary>
        /// Returns the PathData string of the Holed Circle
        /// </summary>
        /// <returns>The Path Data String</returns>
        public override string GetShapePathData()
        {
            string outerCircle = PathDataFactory.Circle(CenterX, CenterY, Radius);
            string innerCircle = PathDataFactory.CircleAntiClockwise(CenterX, CenterY, InnerRadius);

            StringBuilder builder = new();
            string pathData = builder.Append(outerCircle).Append(innerCircle).ToString();
            
            return pathData;
        }

        /// <summary>
        /// Clones the Circle
        /// </summary>
        /// <returns>The Clone</returns>
        public override HoledCircleDraw CloneSelf()
        {
            HoledCircleDraw clone = new();
            clone.CenterX = this.CenterX;
            clone.CenterY = this.CenterY;
            clone.Diameter = this.Diameter;
            clone.InnerDiameter = this.InnerDiameter;
            clone.Name = this.Name;
            clone.Stroke = this.Stroke;
            clone.Fill = this.Fill;
            clone.Filter = this.Filter;
            clone.Opacity = this.Opacity;
            clone.ShapeCenterX = this.ShapeCenterX;
            clone.ShapeCenterY = this.ShapeCenterY;
            return clone;
        }
    }
}
