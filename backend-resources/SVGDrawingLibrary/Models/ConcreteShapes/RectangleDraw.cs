using SVGDrawingLibrary.Enums;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    public class RectangleDraw : DrawShape
    {
        public double StartX { get; protected set; }
        public double StartY { get; protected set; }
        public virtual double Length { get; set; }
        public virtual double Height { get; set; }
        /// <summary>
        /// To Be Deprecated
        /// </summary>
        public CornersToRound RoundedCorners { get; set; }
        /// <summary>
        /// To Be Deprecated
        /// </summary>
        public double CornerRadius { get; set; }

        /// <summary>
        /// The X Coordinate of the Bottom Right Corner
        /// </summary>
        public double EndX { get => GetEndX(); }

        /// <summary>
        /// The Y Coordinate of the Bottom Right Corner
        /// </summary>
        public double EndY { get => GetEndY(); }

        public RectangleDraw()
        {

        }

        #region 0.Additional Constructors

        /// <summary>
        /// Creates a Rectangle with the Defined Length and Height and CornerRadiuses
        /// </summary>
        /// <param name="length">The Length of the Rectangle</param>
        /// <param name="height">The Height of the Rectangle</param>
        /// <param name="cornersToRound">In Which Corners to Apply Rounding</param>
        /// <param name="cornerRadius">The Corner Radius</param>
        public RectangleDraw(double length , double height , CornersToRound cornersToRound = CornersToRound.None , double cornerRadius = 0)
        {
            Length = length;
            Height = height;
            RoundedCorners = cornersToRound;
            CornerRadius = cornerRadius;
            SetCenterOrStartX(0, CSCoordinate.Center);
            SetCenterOrStartY(0, CSCoordinate.Center);
        }

        /// <summary>
        /// Creates a Rectangle at the Defined Coordinates
        /// </summary>
        /// <param name="startX">The Start X</param>
        /// <param name="startY">The Start Y</param>
        /// <param name="endX">The end X</param>
        /// <param name="endY">The end Y</param>
        /// <param name="cornersToRound">Which corners to Round</param>
        /// <param name="cornerRadius">The corner Radius of rounding</param>
        public RectangleDraw(double startX , double startY , double endX , double endY , CornersToRound cornersToRound = CornersToRound.None , double cornerRadius = 0)
        {
            Length = endX - startX;
            Height = endY - startY;
            SetCenterOrStartX(startX, CSCoordinate.Start);
            SetCenterOrStartY(startY, CSCoordinate.Start);
            RoundedCorners= cornersToRound;
            CornerRadius = cornerRadius;
        }

        #endregion

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
            if (centerOrStart is CSCoordinate.Center)
            {
                ShapeCenterY = newY;
                StartY = newY - Height / 2d;
            }
            else if (centerOrStart is CSCoordinate.Start)
            {
                StartY = newY;
                ShapeCenterY = newY + Height / 2d;
            }
        }

        /// <summary>
        /// Gets the X Coordinate of the Bottom Right Corner
        /// </summary>
        /// <returns></returns>
        private double GetEndX()
        {
            double endX = StartX + Length;
            return endX;
        }

        /// <summary>
        /// Gets the Y Coordinate of the BottomRightCorner
        /// </summary>
        /// <returns></returns>
        private double GetEndY()
        {
            double endY = StartY + Height;
            return endY;
        }

        /// <summary>
        /// Clones the Rectangle
        /// </summary>
        /// <returns>The Clone</returns>
        public override RectangleDraw CloneSelf()
        {
            RectangleDraw clone = new();
            clone.StartX = this.StartX;
            clone.StartY = this.StartY;
            clone.Length = this.Length;
            clone.Height = this.Height;
            clone.RoundedCorners = this.RoundedCorners;
            clone.CornerRadius = this.CornerRadius;
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
        /// Returns the X Coordinate of the Bounding Box Center
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxCenterX()
        {
            double centerX = StartX + Length / 2d;
            return centerX;
        }

        /// <summary>
        /// Returns the Y Coordinate of the Bounding Box Center
        /// </summary>
        /// <returns></returns>
        public override double GetBoundingBoxCenterY()
        {
            double centerY = StartY + Height / 2d;
            return centerY;
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
        /// Returns the Path Data of a Rectangle
        /// </summary>
        /// <returns>Path Data String</returns>
        public override string GetShapePathData()
        {
            string pathData = PathDataFactory.Rectangle(StartX, StartY, Length, Height, CornerRadius, RoundedCorners);
            return pathData;
        }
    }
}
