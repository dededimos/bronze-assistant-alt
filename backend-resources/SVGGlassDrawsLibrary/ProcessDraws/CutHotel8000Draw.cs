using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.GlassProcesses;
using SVGDrawingLibrary;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SVGGlassDrawsLibrary.Helpers.GlassDrawExtensions;

namespace SVGGlassDrawsLibrary.ProcessDraws
{
    public class CutHotel8000Draw : CompositeDraw
    {
        private CutHotel8000 cut;

        private RectangleDraw rectangle;
        private CapsuleRectangleDraw capsule;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cut"></param>
        /// <param name="dx">The X Distance from Edge </param>
        /// <param name="dy">The Y Distance from Edge</param>
        private CutHotel8000Draw(CutHotel8000 cut ,Glass glassOwner)
        {
            this.cut = cut;
            capsule = new CapsuleRectangleDraw(cut.SemiCircleDiameter, cut.SemiCirclesCentersDistance + cut.SemiCircleDiameter);
            rectangle = new RectangleDraw(cut.Length,cut.Height);

            double rectangleStartX = cut.HorizontalDistancing switch
            {
                HorizDistancing.FromLeft => cut.HorizontalDistance,
                HorizDistancing.FromRight => glassOwner.Length - cut.HorizontalDistance - rectangle.Length,
                HorizDistancing.FromMiddleLeft or HorizDistancing.FromMiddleRight => throw new NotImplementedException(),
                HorizDistancing.Undefined => throw new ArgumentNullException($"{nameof(HorizDistancing)} has not been set"),
                _ => throw new NotSupportedException($"This value of HorziontalDistancing is not Supported"),
            } ;
            double rectangleStartY = cut.VerticalDistancing switch
            {
                VertDistancing.FromTop => cut.VerticalDistance,
                VertDistancing.FromBottom => glassOwner.Height - cut.VerticalDistance - rectangle.Height,
                VertDistancing.FromMiddleUp or VertDistancing.FromMiddleDown => throw new NotImplementedException(),
                VertDistancing.Undefined => throw new ArgumentNullException($"{nameof(VertDistancing)} has not been set"),
                _ => throw new NotSupportedException($"This value of VerticalDistancing is not Supported"),
            };

            rectangle.SetCenterOrStartX(rectangleStartX, CSCoordinate.Start);
            rectangle.SetCenterOrStartY(rectangleStartY,CSCoordinate.Start);
            if (cut.IsPlacedLeft)
            {
                capsule.SetCenterOrStartX(rectangle.EndX + capsule.Length / 2d, CSCoordinate.Center);
            }
            else
            {
                capsule.SetCenterOrStartX(rectangleStartX - capsule.Length/2d,CSCoordinate.Center);
            }
            capsule.SetCenterOrStartY(rectangle.ShapeCenterY,CSCoordinate.Center);
            
            AddDraw(rectangle);
            AddDraw(capsule);
        }

        public override string GetShapePathData()
        {
            PathDataBuilder builder = new();
            if (cut.IsPlacedLeft)
            {
                //Move after the start of rectangle Omit
                builder.MoveTo(rectangle.StartX, rectangle.StartY)
                    // Add the rectangle two sides as a hole down then right
                    .AddLineRelative(0, rectangle.Height)
                    .AddLineRelative(rectangle.Length, 0)
                    // Add the Small line to start after the arc
                    .AddLineRelative(0, (cut.SemiCirclesCentersDistance - rectangle.Height) / 2d)
                    // Draw a semi-Circle anti-clockwise
                    .AddArcRelative(capsule.Length / 2d, capsule.Length / 2d, capsule.Length, 0, 0, false, false)
                    // draw the straight line of the capsule
                    .AddLineRelative(0, -cut.SemiCirclesCentersDistance)
                    // draw a Semi-Circle anti-clockwise
                    .AddArcRelative(capsule.Length / 2d, capsule.Length / 2d, -capsule.Length, 0, 0, false, false)
                    // add the small line that is now touching the Rectangle
                    .AddLineRelative(0, (cut.SemiCirclesCentersDistance - rectangle.Height) / 2d)
                    // add the last Rectangle Line by closing the shape
                    .CloseDraw();
                //Do not close the path otherwise the Capsule will be drawn in full and we do not want that
                return builder.GetPathData();
            }
            else
            {
                //Move to start of rectangle
                builder.MoveTo(rectangle.StartX, rectangle.StartY)
                    // Add the small line
                    .AddLineRelative(0, -(cut.SemiCirclesCentersDistance - rectangle.Height) / 2d)
                    // Draw a semicircle anti-clockwise
                    .AddArcRelative(capsule.Length / 2d, capsule.Length / 2d, -capsule.Length, 0, 0, false, false)
                    // Draw  the straight line of the capsule
                    .AddLineRelative(0, cut.SemiCirclesCentersDistance)
                    // Draw a semicircle anti-clockwise
                    .AddArcRelative(capsule.Length / 2d, capsule.Length / 2d, capsule.Length, 0, 0, false, false)
                    // Draw the small line
                    .AddLineRelative(0, -(cut.SemiCirclesCentersDistance - rectangle.Height) / 2d)
                    //Add the Rectangle Lines right-up-left
                    .AddLineRelative(rectangle.Length, 0)
                    .AddLineRelative(0, -rectangle.Height)
                    .CloseDraw();
                
                //Do not close the path otherwise the Capsule will be drawn in full and we do not want that
                return builder.GetPathData();
            }
        }
        public override RectangleDraw CloneSelf()
        {
            throw new NotImplementedException();
        }

        public static CutHotel8000Draw Create(CutHotel8000 cut , Glass glass)
        {
            var cutDraw = new CutHotel8000Draw(cut,glass);
            return cutDraw;
        }
    }
}
