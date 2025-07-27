using System.Text;


namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    /// <summary>
    /// The Draw Object of a Mirrors Touch Switch
    /// </summary>
    public class ConcentricRectangles : RectangleDraw
    {
        /// <summary>
        /// The Inner Rectangle
        /// </summary>
        public RectangleDraw InnerRectangle { get; set; } = new();

        public ConcentricRectangles()
        {

        }

        /// <summary>
        /// Sets the ShapeCenterX or the StartX
        /// </summary>
        /// <param name="newX">The newX we want to Set</param>
        /// <param name="centerOrStart">Sets The Selected Property</param>
        public override void SetCenterOrStartX(double newX, CSCoordinate centerOrStart)
        {
            // The Inner Rectangle will have the Same Center as the OuterRectangle
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
            InnerRectangle.SetCenterOrStartX(ShapeCenterX, CSCoordinate.Center);
        }

        /// <summary>
        /// Sets the ShapeCenterY or the StartY 
        /// </summary>
        /// <param name="newY">The newY we want to Set</param>
        /// <param name="centerOrStart">Sets The Selected Property</param>
        public override void SetCenterOrStartY(double newY, CSCoordinate centerOrStart)
        {
            // The Inner Rectangle will have the Same Center as the OuterRectangle
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
            InnerRectangle.SetCenterOrStartY(ShapeCenterY, CSCoordinate.Center);
        }

        /// <summary>
        /// Returns a Clone of the ConcentricRectangles
        /// </summary>
        /// <returns>The Clone</returns>
        public override ConcentricRectangles CloneSelf()
        {
            ConcentricRectangles clone = new();
            clone.InnerRectangle = InnerRectangle.CloneSelf();
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
        /// Returns the Path Data of the ConcentricRectangles
        /// </summary>
        /// <returns>Path Data String</returns>
        public override string GetShapePathData()
        {
            string basePathData = base.GetShapePathData();
            string innerPathData = InnerRectangle.GetShapePathData();
            StringBuilder builder = new();
            builder.Append(basePathData).Append(innerPathData);
            return builder.ToString();
        }
    }
}
