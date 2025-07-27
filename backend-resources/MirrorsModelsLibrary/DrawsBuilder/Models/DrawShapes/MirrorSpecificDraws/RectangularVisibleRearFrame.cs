using SVGDrawingLibrary.Models.ConcreteShapes;
using SVGDrawingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsModelsLibrary.DrawsBuilder.Models.DrawShapes.MirrorSpecificDraws
{
    /// <summary>
    /// Represents a frame that has a small Flap for the Glass (This is The Back Side of the All Around Mirror Frame)
    /// </summary>
    public class RectangularVisibleRearFrame : HoledRectangleDraw
    {
        /// <summary>
        /// The Thickness of the VisiblePart of the Frame
        /// </summary>
        public double MiddleRectangleThickness { get; set; }

        /// <summary>
        /// Clones the Rectangular Rear Frame
        /// </summary>
        /// <returns>The Clone</returns>
        public override RectangularVisibleRearFrame CloneSelf()
        {
            RectangularVisibleRearFrame clone = new();
            clone.StartX = StartX;
            clone.StartY = StartY;
            clone.Length = Length;
            clone.Height = Height;
            clone.RoundedCorners = RoundedCorners;
            clone.CornerRadius = CornerRadius;
            clone.RectangleThickness = RectangleThickness;
            clone.Name = Name;
            clone.Stroke = Stroke;
            clone.Fill = Fill;
            clone.Filter = Filter;
            clone.Opacity = Opacity;
            clone.ShapeCenterX = ShapeCenterX;
            clone.ShapeCenterY = ShapeCenterY;
            clone.MiddleRectangleThickness = this.MiddleRectangleThickness;
            return clone;
        }

        /// <summary>
        /// Returns the Path Data of a Rectangular Rear Frame
        /// </summary>
        /// <returns>Path Data String</returns>
        public override string GetShapePathData()
        {
            string outerRectangle = PathDataFactory.Rectangle(StartX, StartY, Length, Height, CornerRadius, RoundedCorners);
            string innerRectangle = PathDataFactory.RectangleAnticlockwise(InnerStartX, InnerStartY, InnerLength, InnerHeight);

            //The Middle Rectangle of the Frame on the Back (The Rectangle that Seperates the Part which is Visible from the Part that it is not)
            double middleRectangleStartX = StartX + MiddleRectangleThickness;
            double middleRectangleStartY = StartY + MiddleRectangleThickness;
            double middleRectangleLength = Length - 2 * MiddleRectangleThickness;
            double middleRectangleHeight = Height - 2 * MiddleRectangleThickness;
            string middleRectangle = PathDataFactory.Rectangle(middleRectangleStartX, middleRectangleStartY, middleRectangleLength, middleRectangleHeight);

            //The connection Lines of the Frame in the Back
            string connectionLine1 = PathDataFactory.Line(StartX, StartY, middleRectangleStartX, middleRectangleStartY);
            string connectionLine2 = PathDataFactory.Line(StartX + Length, StartY, middleRectangleStartX + middleRectangleLength, middleRectangleStartY);
            string connectionLine3 = PathDataFactory.Line(StartX, StartY + Height, middleRectangleStartX, middleRectangleStartY + middleRectangleHeight);
            string connectionLine4 = PathDataFactory.Line(StartX + Length, StartY + Height, middleRectangleStartX + middleRectangleLength, middleRectangleStartY + middleRectangleHeight);

            StringBuilder builder = new();
            string pathData = builder.Append(outerRectangle)
                                     .Append(innerRectangle)    //Have to add Two Times to Negate both Rectangles
                                     .Append(middleRectangle)
                                     .Append(innerRectangle)    //Have to add Two Times to Negate both Rectangles
                                     .Append(connectionLine1)
                                     .Append(connectionLine2)
                                     .Append(connectionLine3)
                                     .Append(connectionLine4)
                                     .ToString();
            return pathData;
        }
    }
}
