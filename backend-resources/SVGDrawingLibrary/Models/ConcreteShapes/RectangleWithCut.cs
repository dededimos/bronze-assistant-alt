using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    public class RectangleWithCutDraw : RectangleDraw
    {
        /// <summary>
        /// The Length of the Cut
        /// </summary>
        public double CutLength { get; set; }
        /// <summary>
        /// The Height of the Cut
        /// </summary>
        public double CutHeight { get; set; }

        /// <summary>
        /// The Center X of the Cut(Cut is a Rectangle) - Relative to the Main Rectangle (Start X , Start Y Of Main Rectangle is 0,0)
        /// </summary>
        public double CutRelativeCenterX { get=> GetCutRelativeCenterX(); }

        /// <summary>
        /// The Center Y of the Cut(Cut is a Rectangle) - Relative to the Main Rectangle (Start X , Start Y Of Main Rectangle is 0,0)
        /// </summary>
        public double CutRelativeCenterY { get => GetCutRelativeCenterY(); }

        /// <summary>
        /// The StartX of the Cut(Cut is a Rectangle) - Relative to the Main Rectangle (Start X , Start Y Of Main Rectangle is 0,0)
        /// </summary>
        public double CutRelativeStartX { get; protected set; }
        /// <summary>
        /// The StartY of the Cut(Cut is a Rectangle) - Relative to the Main Rectangle (Start X , Start Y Of Main Rectangle is 0,0)
        /// </summary>
        public double CutRelativeStartY { get; protected set; }

        /// <summary>
        /// The StartX of the Cut Rectangle
        /// </summary>
        public double CutStartX { get => GetCutStartX(); }

        /// <summary>
        /// The StartY of the Cut Rectangle
        /// </summary>
        public double CutStartY { get => GetCutStartY(); }

        /// <summary>
        /// Initializes a Rectangle with a Cut
        /// </summary>
        /// <param name="length">The Length of the Rectangle</param>
        /// <param name="height">The Height of the Rectangle</param>
        /// <param name="cutLength">The Cut Length</param>
        /// <param name="cutHeight">The Cut Height</param>
        /// <param name="cutRelativeStartX">The Relative StartX Point of the Cut (0,0 is StartX StartY of Main Rectangle)</param>
        /// <param name="cutRelativeStartY">The Relative StartY Point of the Cut (0,0 is StartX StartY of Main Rectangle)</param>
        public RectangleWithCutDraw(double length , 
                                double height , 
                                double cutLength , 
                                double cutHeight , 
                                double cutRelativeStartX,
                                double cutRelativeStartY)
        {
            this.Length = length;
            this.Height = height;
            this.CutLength = cutLength;
            this.CutHeight = cutHeight;
            this.CutRelativeStartX = cutRelativeStartX;
            this.CutRelativeStartY = cutRelativeStartY;
        }

        /// <summary>
        /// Returns the Inner Rectangle of the HoledRectangle Draw
        /// </summary>
        /// <returns>The Inner Rectangle Draw</returns>
        public RectangleDraw GetCutRectangle()
        {
            RectangleDraw rectangle = new();
            rectangle.SetCenterOrStartX(this.StartX + CutRelativeStartX, CSCoordinate.Start);
            rectangle.SetCenterOrStartY(this.StartY + CutRelativeStartY, CSCoordinate.Start);
            rectangle.Length = CutLength;
            rectangle.Height = CutHeight;
            rectangle.RoundedCorners = Enums.CornersToRound.None;
            return rectangle;
        }

        /// <summary>
        /// Returns the CenterX Position of the Cut (Relative to the StartX of the Main Rectangle)
        /// </summary>
        /// <returns></returns>
        private double GetCutRelativeCenterX()
        {
            double relativeCenterX = CutRelativeStartX + CutLength / 2d;
            return relativeCenterX;
        }

        /// <summary>
        /// Returns the CenterY Position of the Cut (Relative to the StartY of the Main Rectangle)
        /// </summary>
        /// <returns></returns>
        private double GetCutRelativeCenterY()
        {
            double relativeCenterY = CutRelativeStartY + CutHeight / 2d;
            return relativeCenterY;
        }

        /// <summary>
        /// Returns the StartX of the Cut Rectangle
        /// </summary>
        /// <returns></returns>
        private double GetCutStartX()
        {
            double cutStartX = StartX + CutRelativeStartX;
            return cutStartX;
        }

        /// <summary>
        /// Returns the StartY of the Cut Rectangle
        /// </summary>
        /// <returns></returns>
        private double GetCutStartY()
        {
            double cutStartY = StartY + CutRelativeStartY;
            return cutStartY;
        }

        /// <summary>
        /// Clones the RectangleWithCut
        /// </summary>
        /// <returns>The Clone</returns>
        public override RectangleWithCutDraw CloneSelf()
        {
            RectangleWithCutDraw clone = new(Length,Height,CutLength,CutHeight,CutRelativeStartX,CutRelativeStartY);
            clone.StartX = this.StartX;
            clone.StartY = this.StartY;
            clone.ShapeCenterX = this.ShapeCenterX;
            clone.ShapeCenterY = this.ShapeCenterY;
            clone.RoundedCorners = this.RoundedCorners;
            clone.CornerRadius = this.CornerRadius;
            clone.Name = this.Name;
            clone.Stroke = this.Stroke;
            clone.Fill = this.Fill;
            clone.Filter = this.Filter;
            clone.Opacity = this.Opacity;
            
            return clone;
        }

        /// <summary>
        /// Returns the Path Data of a HoledRectangle
        /// </summary>
        /// <returns>Path Data String</returns>
        public override string GetShapePathData()
        {
            StringBuilder builder = new();
            string pathData;

            //This is to Avoid Stroke! on the Cut Area
            if (CutRelativeStartX != 0) //When the Cut Does not Start in the Right Edge of the Main Rectangle
            {
                string mainRectangle = PathDataFactory.Rectangle(StartX, StartY, Length, Height, CornerRadius, RoundedCorners);
                string cutRectangle = PathDataFactory.RectangleAnticlockwise(CutStartX, CutStartY, CutLength, CutHeight);
                pathData = builder.Append(mainRectangle).Append(cutRectangle).ToString();
            }
            else
            {
                builder.Append(PathDataFactory.Line(StartX, StartY, EndX, StartY))
                       .Append(PathDataFactory.ContinuePolygon(EndX,EndY))
                       .Append(PathDataFactory.ContinuePolygon(StartX + CutRelativeStartX + CutLength,EndY))
                       .Append(PathDataFactory.ContinuePolygon(StartX + CutRelativeStartX + CutLength,StartY+CutRelativeStartY))
                       .Append(PathDataFactory.ContinuePolygon(StartX, StartY + CutRelativeStartY,true));
                pathData = builder.ToString();
            }
            return pathData;
        }

    }
}
