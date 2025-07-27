using SVGDrawingLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    public class CapsuleRectangleDraw : RectangleDraw
    {
        private double length;
        private double height;
        private double cornerRadius;

        /// <summary>
        /// The Corner Radius of the Capsule - is Always either half Length or Half Height
        /// This property HIDES rectangles Corner Radius => So it can be protected from changes
        /// I am not sure this is the Correct way to do this
        /// </summary>
        new public double CornerRadius 
        { 
            get => cornerRadius; 
            private set
            {
                if (value != cornerRadius || value != base.CornerRadius)
                {
                    cornerRadius = value;
                    base.CornerRadius = value;
                }
            } 
        }
        
        /// <summary>
        /// The Length of the Capsule -- Changing the Length will also change the Mirror Orientation
        /// When Height GreaterThan Length = Vertical , When Height LessThan Length = Horizontal
        /// </summary>
        public override double Length 
        { 
            get =>length;
            set
            {
                if (value != length)
                {
                    length = value;
                    SetCornerRadius();
                }
            } 
        }

        /// <summary>
        /// The Height of the Capsule -- Changing the Height will also change the Mirror Orientation
        /// When Height GreaterThan Length = Vertical , When Height LessThan Length = Horizontal
        /// </summary>
        public override double Height 
        { 
            get => height;
            set
            {
                if (height!=value)
                {
                    height = value;
                    SetCornerRadius();
                }
            }
        }

        /// <summary>
        /// Weather the Capsule has Horizontal Orientation
        /// If Length >= Height true else false;
        /// </summary>
        public bool IsHorizontal { get => Length >= Height; }
        
        public CapsuleRectangleDraw(double length,double height,double centerX = 0 , double centerY = 0)
        {
            this.Length = length;
            this.Height = height;
            RoundedCorners = CornersToRound.All;
            SetCornerRadius();
            SetCenterOrStartX(centerX, CSCoordinate.Center);
            SetCenterOrStartY(centerY, CSCoordinate.Center);
        }

        /// <summary>
        /// Sets the Corner Radius
        /// </summary>
        private void SetCornerRadius()
        {
            if (Length >= Height)
            {
                CornerRadius = Height / 2d;
            }
            else
            {
                CornerRadius = Length / 2d;
            }
        }

        /// <summary>
        /// Gets the Shape of the Top Or Left Semicircle whichever is Present
        /// </summary>
        /// <returns></returns>
        public SemicircleRectangleDraw GetTopLeftSemicircle()
        {
            SemicircleRectangleDraw shape;
            if (IsHorizontal)
            {
                shape = new(CornerRadius , SemicircleOrientation.PointingLeft);
            }
            else
            {
                shape = new(CornerRadius, SemicircleOrientation.PointingTop);
            }
            //X Start as Start of Capsules Rectangle
            shape.SetCenterOrStartX(this.StartX, CSCoordinate.Start);
            //Y Start as Start of Capsules Rectangle
            shape.SetCenterOrStartY(this.StartY, CSCoordinate.Start);
            return shape;
        }
        /// <summary>
        /// Gets the Shape of the Bottom Or Right Semicircle whichever is Present
        /// </summary>
        /// <returns></returns>
        public SemicircleRectangleDraw GetBottomRightSemicircle()
        {
            SemicircleRectangleDraw shape;
            if (IsHorizontal)
            {
                shape = new(CornerRadius, SemicircleOrientation.PointingRight);
                //X Start , at the End of Capsules Rectangle Minus the Length of Semicircles Rectangle
                shape.SetCenterOrStartX(this.EndX - shape.Length, CSCoordinate.Start);
                //YStart same as StartY of Capsule Rectangle
                shape.SetCenterOrStartY(this.StartY, CSCoordinate.Start);
            }
            else
            {
                shape = new(CornerRadius, SemicircleOrientation.PointingBottom);
                //X Start same as X Start of Capsules Rectangle
                shape.SetCenterOrStartX(this.StartX, CSCoordinate.Start);
                //Y Start , EndY of Capsules Rectangle - the Hiehgt of the Semicircle Rectangle
                shape.SetCenterOrStartY(this.EndY - shape.Height, CSCoordinate.Start);
            }
            return shape;
        }

        /// <summary>
        /// Clones the CapsuleRectangle Draw
        /// </summary>
        /// <returns></returns>
        public override CapsuleRectangleDraw CloneSelf()
        {
            CapsuleRectangleDraw clone = new(Length, Height);
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

    }
}
