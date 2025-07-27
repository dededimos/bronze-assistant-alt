using SVGDrawingLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    /// <summary>
    /// A Semicircle Draw - Made from a Rectangle (By Rounding two of its Corners)
    /// </summary>
    public class SemicircleRectangleDraw : RectangleDraw
    {

        private double length;
        private double height;
        private double cornerRadius;
        private SemicircleOrientation orientation;
        private CornersToRound roundedCorners;

        /// <summary>
        /// Where the Semicircle is Pointing
        /// </summary>
        public SemicircleOrientation Orientation 
        { 
            get => orientation;
            set 
            {
                if (orientation != value)
                {
                    orientation = value;
                    SetBaseLengthHeight();
                    SetRoundedCorners();
                }
            } 
        }

        /// <summary>
        /// The Corner Radius of the Semicircle - is Always either half Length or Half Height
        /// This property HIDES rectangles Corner Radius => So it can be protected from changes
        /// When Corner Radius Changes the Length-Height of the Rectangle Also Change
        /// </summary>
        new public double CornerRadius 
        { 
            get => cornerRadius;
            set
            {
                if (value != cornerRadius || value != base.CornerRadius)
                {
                    cornerRadius = value;
                    base.CornerRadius = value;
                    SetBaseLengthHeight();
                }
            } 
        }

        /// <summary>
        /// Semicirce Length Hides Rectangle Length Base Property
        /// The Base Property gets updated whenever this one Changes
        /// </summary>
        new public double Length 
        { 
            get => length; 

            private set
            {
                if (value != length || value != base.Length)
                {
                    length = value;
                    base.Length = value;
                }
            } 
        }
        /// <summary>
        /// Semicirce Height Hides Rectangle Height Base Property
        /// The Base Property gets updated whenever this one Changes
        /// </summary>
        new public double Height 
        { 
            get=>height; 

            private set
            {
                if (value != height || value != base.Height)
                {
                    height = value;
                    base.Height = value;
                }
            } 
        }

        /// <summary>
        /// Which Corners are to be Rounded (Practiva)
        /// </summary>
        new public CornersToRound RoundedCorners 
        { 
            get => roundedCorners; 
            private set
            {
                if (value != roundedCorners || value != base.RoundedCorners)
                {
                    roundedCorners = value;
                    base.RoundedCorners = value;
                }
            } 
        }

        public SemicircleRectangleDraw(double radius , SemicircleOrientation orientation)
        {
            this.CornerRadius = radius;
            this.Orientation = orientation;
            SetRoundedCorners();
            SetBaseLengthHeight();
        }

        /// <summary>
        /// Sets Which Corners Should be Rounded
        /// </summary>
        private void SetRoundedCorners()
        {
            switch (Orientation)
            {
                case SemicircleOrientation.PointingTop:
                    RoundedCorners = CornersToRound.UpperCorners;
                    break;
                case SemicircleOrientation.PointingBottom:
                    RoundedCorners = CornersToRound.BottomCorners;
                    break;
                case SemicircleOrientation.PointingLeft:
                    RoundedCorners = CornersToRound.LeftCorners;
                    break;
                case SemicircleOrientation.PointingRight:
                    RoundedCorners = CornersToRound.RightCorners;
                    break;
            }
        }

        /// <summary>
        /// Sets Length and Height of the Rectangle According to the Radius and OIrientation of the Semicircle
        /// </summary>
        private void SetBaseLengthHeight()
        {
            switch (Orientation)
            {
                case SemicircleOrientation.PointingTop:
                case SemicircleOrientation.PointingBottom:
                    Length = CornerRadius * 2d;
                    Height = CornerRadius;
                    break;
                case SemicircleOrientation.PointingLeft:
                case SemicircleOrientation.PointingRight:
                    Length = CornerRadius;
                    Height = CornerRadius * 2d;
                    break;
            }
        }

        public DrawPoint GetCircleCenter()
        {
            double x;
            double y;
            switch (Orientation)
            {
                case SemicircleOrientation.PointingTop:
                    x = this.ShapeCenterX;
                    y = this.ShapeCenterY + (CornerRadius / 2d);
                    break;
                case SemicircleOrientation.PointingBottom:
                    x = this.ShapeCenterX;
                    y = this.ShapeCenterY - (CornerRadius / 2d);
                    break;
                case SemicircleOrientation.PointingLeft:
                    x = this.ShapeCenterX + (CornerRadius /2d);
                    y = this.ShapeCenterY;
                    break;
                case SemicircleOrientation.PointingRight:
                    x = this.ShapeCenterX - (CornerRadius / 2d);
                    y = this.ShapeCenterY;
                    break;
                default:
                    throw new NotSupportedException("Not Recognized Orientation for Semicircle");
            }
            DrawPoint point = new(x, y);
            return point;
        }

        /// <summary>
        /// Gets the Circle in which the Semicircle is a part of (The Completed Circle)
        /// </summary>
        /// <returns>The Circle formed by the Semicircle</returns>
        public CircleDraw GetParentCircle() 
        {
            CircleDraw circle = new(CornerRadius * 2d);
            circle.SetCenterOrStartX(GetCircleCenter().X, CSCoordinate.Center);
            circle.SetCenterOrStartY(GetCircleCenter().Y, CSCoordinate.Center);
            return circle;
        }

        /// <summary>
        /// Deep Clones the Semicircle
        /// </summary>
        /// <returns>The Clone</returns>
        public override SemicircleRectangleDraw CloneSelf()
        {
            SemicircleRectangleDraw clone = new(this.CornerRadius,this.Orientation);
            clone.StartX = this.StartX;
            clone.StartY = this.StartY;
            //clone.Length = this.Length; set by radius
            //clone.Height = this.Height; set by radius
            //clone.RoundedCorners = this.RoundedCorners; set by orientation
            //clone.CornerRadius = this.CornerRadius; set by constructor
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
