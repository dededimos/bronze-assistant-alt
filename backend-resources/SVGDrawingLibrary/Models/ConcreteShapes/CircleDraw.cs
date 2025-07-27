using SVGDrawingLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    public class CircleDraw : DrawShape
    {
        /// <summary>
        /// Circle's Center X
        /// </summary>
        public double CenterX { get; protected set; }
        /// <summary>
        /// Circle's Center Y
        /// </summary>
        public double CenterY { get; protected set; }
        /// <summary>
        /// Circle's Diameter
        /// </summary>
        public double Diameter { get; set; }
        /// <summary>
        /// Circle's Radius -- ReadOnly (Gets Set From Diameter)
        /// </summary>
        public double Radius { get => GetRadius(); }

        public CircleDraw()
        {

        }

        /// <summary>
        /// Defines a Circle with the Given Diameter at the given point (x,y) or (0,0)
        /// </summary>
        /// <param name="diameter">The Diameter of the Circle</param>
        public CircleDraw(double diameter , double centerX = 0 , double centerY=0)
        {
            this.Diameter = diameter;
            SetCenterOrStartX(centerX, CSCoordinate.Center);
            SetCenterOrStartY(centerY, CSCoordinate.Center);
        }

        /// <summary>
        /// Sets the ShapesCenterX or the StartX (In the Case of the Circle its the Same thing)
        /// </summary>
        /// <param name="newX">The newX we want to Set</param>
        /// <param name="centerOrStart">Irrelevant in this Case</param>
        public override void SetCenterOrStartX(double newX, CSCoordinate centerOrStart = CSCoordinate.Start)
        {
            CenterX = newX;
            ShapeCenterX = newX;
        }
        /// <summary>
        /// Sets the ShapesCenterY or the CenterY (In the Case of the Circle its the Same thing)
        /// </summary>
        /// <param name="newY">The newY we want to Set</param>
        /// <param name="centerOrStart">Irrelevant in this Case</param>
        public override void SetCenterOrStartY(double newY, CSCoordinate centerOrStart = CSCoordinate.Start)
        {
            CenterY = newY;
            ShapeCenterY = newY;
        }

        /// <summary>
        /// Gets the Radus from the Circles Diameter
        /// </summary>
        /// <returns>A double -- The Circles Radius</returns>
        private double GetRadius()
        {
            double radius = Diameter / 2d;
            return radius;
        }

        /// <summary>
        /// Returns the X Coordinate of the Bounding Box Center
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxCenterX()
        {
            return CenterX;
        }

        /// <summary>
        /// Returns the Y Coordinate of the Bounding Box Center
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxCenterY()
        {
            return CenterY;
        }

        /// <summary>
        /// Returns the Height of the Bounding Box
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxHeight()
        {
            return Diameter;
        }

        /// <summary>
        /// Returns the Length of the Bounding Box
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxLength()
        {
            return Diameter;
        }

        /// <summary>
        /// Returns the PathData string of the Circle
        /// </summary>
        /// <returns>The Path Data String</returns>
        public override string GetShapePathData()
        {
            string pathData = PathDataFactory.Circle(CenterX, CenterY, Radius);
            return pathData;
        }

        /// <summary>
        /// Clones the Circle
        /// </summary>
        /// <param name="shape">The Rectangle that will be Cloned</param>
        /// <returns>The Clone</returns>
        public override CircleDraw CloneSelf()
        {
            CircleDraw clone = new();
            clone.CenterX = this.CenterX;
            clone.CenterY = this.CenterY;
            clone.Diameter = this.Diameter;
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
