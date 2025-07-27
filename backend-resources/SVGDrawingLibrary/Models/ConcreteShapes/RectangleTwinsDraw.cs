using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    /// <summary>
    /// The Draw Object of Two Parallel Rectangles of the Same Size and Height
    /// </summary>
    public class RectangleTwinsDraw : ComboDrawShape
    {
        public RectangleDraw FirstRectangle { get; set; } = new();
        public RectangleDraw SecondRectangle { get; set; } = new();
        
        //The Length between the Rectangles Centers
        public double RectanglesCentersDistance { get; set; }
        
        /// <summary>
        /// Wheather the Rectangles are One Right or Left -- or One Up One Down
        /// </summary>
        public bool AreRightLeftRectangles { get; set; }

        /// <summary>
        /// Sets the Shape Center X or the TopLeftCorner of the First Rectangle (The Second Rectangle X Gets updated by the known Distance)
        /// </summary>
        /// <param name="newX">the new X</param>
        /// <param name="centerOrStart">Which Coordinate to Set</param>
        public override void SetCenterOrStartX(double newX, CSCoordinate centerOrStart)
        {
            if (centerOrStart is CSCoordinate.Center)
            {
                FirstRectangle.SetCenterOrStartX(AreRightLeftRectangles ? newX - RectanglesCentersDistance / 2d : newX, centerOrStart);
                SecondRectangle.SetCenterOrStartX(AreRightLeftRectangles ? newX + RectanglesCentersDistance / 2d : newX, centerOrStart);
            }
            else if (centerOrStart is CSCoordinate.Start)
            {
                //The Given Point is the TopLeft Corner of the First Rectangle (Of the Bounding Box)
                FirstRectangle.SetCenterOrStartX(newX , centerOrStart);
                SecondRectangle.SetCenterOrStartX(AreRightLeftRectangles ? newX + RectanglesCentersDistance : newX,centerOrStart);
            }
        }

        /// <summary>
        /// Sets the Shape Center Y or the TopLeftCorner of the First Rectangle (The Second Rectangle Y Gets updated by the known Distance)
        /// </summary>
        /// <param name="newX">the new Y</param>
        /// <param name="centerOrStart">Which Coordinate to Set</param>
        public override void SetCenterOrStartY(double newY, CSCoordinate centerOrStart)
        {
            if (centerOrStart is CSCoordinate.Center)
            {
                FirstRectangle.SetCenterOrStartY(AreRightLeftRectangles ? newY : newY - RectanglesCentersDistance /2d, centerOrStart);
                SecondRectangle.SetCenterOrStartY(AreRightLeftRectangles ? newY : newY + RectanglesCentersDistance / 2d, centerOrStart);
            }
            else if (centerOrStart is CSCoordinate.Start)
            {
                //The Given Point is the TopLeft Corner of the First Rectangle (Of the Bounding Box)
                FirstRectangle.SetCenterOrStartY(newY , centerOrStart);
                SecondRectangle.SetCenterOrStartY(AreRightLeftRectangles ? newY : newY + RectanglesCentersDistance, centerOrStart);
            }
        }
        public override RectangleTwinsDraw CloneSelf()
        {
            RectangleTwinsDraw clone = new();
            clone.FirstRectangle = FirstRectangle.CloneSelf();
            clone.SecondRectangle = SecondRectangle.CloneSelf();
            clone.Name = this.Name;
            clone.Stroke = this.Stroke;
            clone.Fill = this.Fill;
            clone.Filter = this.Filter;
            clone.Opacity = this.Opacity;
            clone.ShapeCenterX = this.ShapeCenterX;
            clone.ShapeCenterY = this.ShapeCenterY;
            clone.RectanglesCentersDistance = this.RectanglesCentersDistance;
            clone.AreRightLeftRectangles = this.AreRightLeftRectangles;
            return clone;
        }

        public override double GetBoundingBoxCenterX()
        {
            //Find the Maximum and Minimum X between the two Rectangles
            double minX = Math.Min(FirstRectangle.StartX, SecondRectangle.StartX);
            double maxX = Math.Max(FirstRectangle.StartX + FirstRectangle.Length, SecondRectangle.StartX + SecondRectangle.Length);

            //The Bounding Box's center will be the Point between the Min-Max X of those rectangles
            double centerX = (minX + maxX) / 2d;
            return centerX;
        }

        public override double GetBoundingBoxCenterY()
        {
            //Find the Maximum and Minimum Y between the two Rectangles
            double minY = Math.Min(FirstRectangle.StartY, SecondRectangle.StartY);
            double maxY = Math.Max(FirstRectangle.StartY + FirstRectangle.Height, SecondRectangle.StartY + SecondRectangle.Height);

            //The Bounding Box's center will be the Point between the Min-Max Y of those rectangles
            double centerY = (minY + maxY) / 2d;
            return centerY;
        }

        public override double GetBoundingBoxHeight()
        {
            //Find the Maximum and Minimum Y between the two Rectangles
            double minY = Math.Min(FirstRectangle.StartY, SecondRectangle.StartY);
            double maxY = Math.Max(FirstRectangle.StartY + FirstRectangle.Height, SecondRectangle.StartY + SecondRectangle.Height);

            //The Bounding Box's Height will be the Difference of those two points
            double bbHeight = maxY - minY;
            return bbHeight;
        }

        public override double GetBoundingBoxLength()
        {
            //Find the Maximum and Minimum X between the two Rectangles
            double minX = Math.Min(FirstRectangle.StartX, SecondRectangle.StartX);
            double maxX = Math.Max(FirstRectangle.StartX + FirstRectangle.Length, SecondRectangle.StartX + SecondRectangle.Length);

            //The Bounding Box's Length will be the Difference of those two points
            double bbLength = maxX - minX;
            return bbLength;
        }

        /// <summary>
        /// Returns a List with Both Rectangle Shapes
        /// </summary>
        /// <returns>A DrawShape List containing the two Rectangles</returns>
        public override List<DrawShape> GetComboShapes()
        {
            List<DrawShape> shapes = new() {FirstRectangle,SecondRectangle};
            return shapes;
        }
    }
}
