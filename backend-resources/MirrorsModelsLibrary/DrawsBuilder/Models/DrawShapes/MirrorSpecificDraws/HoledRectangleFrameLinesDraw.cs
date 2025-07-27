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
    /// A Rectangular Frame Draw -- (HoledRectangle but with a Different Path Data to Add Extra Lines)
    /// </summary>
    public class HoledRectangleFrameLinesDraw : HoledRectangleDraw
    {
        /// <summary>
        /// Clones the Rectangular Front Frame
        /// </summary>
        /// <returns>The Clone</returns>
        public override HoledRectangleFrameLinesDraw CloneSelf()
        {
            HoledRectangleFrameLinesDraw clone = new();
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
            return clone;
        }

        /// <summary>
        /// Returns the Path Data of a Rectangular Front Frame
        /// </summary>
        /// <returns>Path Data String</returns>
        public override string GetShapePathData()
        {
            string outerRectangle = PathDataFactory.Rectangle(StartX, StartY, Length, Height, CornerRadius, RoundedCorners);
            string innerRectangle = PathDataFactory.RectangleAnticlockwise(InnerStartX, InnerStartY, InnerLength, InnerHeight);

            string connectionLine1 = PathDataFactory.Line(StartX, StartY, InnerStartX, InnerStartY);
            string connectionLine2 = PathDataFactory.Line(StartX + Length, StartY, InnerStartX + InnerLength, InnerStartY);
            string connectionLine3 = PathDataFactory.Line(StartX, StartY + Height, InnerStartX, InnerStartY + InnerHeight);
            string connectionLine4 = PathDataFactory.Line(StartX + Length, StartY + Height, InnerStartX + InnerLength, InnerStartY + InnerHeight);

            StringBuilder builder = new();
            string pathData = builder.Append(outerRectangle)
                                     .Append(innerRectangle)
                                     .Append(connectionLine1)
                                     .Append(connectionLine2)
                                     .Append(connectionLine3)
                                     .Append(connectionLine4)
                                     .ToString();
            return pathData;
        }
    }
}
