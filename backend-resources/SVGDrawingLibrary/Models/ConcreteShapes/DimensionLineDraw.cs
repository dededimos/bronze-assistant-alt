using SVGDrawingLibrary.Helpers;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Models.ConcreteShapes
{
    /// <summary>
    /// Represents a Dimension Line Draw. The Path Data Consists of two Triangles and a mid striaght Line
    /// The Total Length of the Dimension Line includes also the lengths of the Arrowheads
    /// Available Method to get also the Mid Line Points.
    /// </summary>
    public class DimensionLineDraw : LineDraw
    {
        /// <summary>
        /// The Distance that the Text Anchor has from the Dimension Line
        /// </summary>
        private const double textDistanceFromLine = 20;
        private const double arrowThickness = 5;
        private const double arrowLength = 17;

        public double ArrowThickness { get; set; }
        public double ArrowLength { get; set; }
        public double? AngleWithAxisX { get; set; }
        /// <summary>
        /// The text represented by this Dimension
        /// </summary>
        public string RepresentedDimensionText { get; set; } = string.Empty;

        public DimensionArrowheads ArrowHeads { get; set; } = DimensionArrowheads.Both;

        public enum DimensionArrowheads
        {
            None,
            Start,
            End,
            Both
        }

        public DimensionLineDraw()
        {

        }
        public DimensionLineDraw(double startX , double startY , 
                                 double endX , double endY, 
                                 double? angleWithAxisX = null, double arrowThickness = arrowThickness , double arrowLength = arrowLength,
                                 DimensionArrowheads arrowheads = DimensionArrowheads.Both) 
            : base(startX, startY , endX , endY) 
        { 
            ArrowThickness = arrowThickness;
            ArrowLength = arrowLength;
            AngleWithAxisX= angleWithAxisX;
            ArrowHeads = arrowheads;
        }
        

        /// <summary>
        /// Returns the Start Point of the Mid Straight Line of the Dimension Draw
        /// </summary>
        /// <returns>The Start of the Mid Straight Line</returns>
        public DrawPoint GetMidLineStart()
        {
            DrawPoint midLineStart = MathCalc.GetPointOfLine(StartX, StartY, EndX, EndY, ArrowLength);
            return midLineStart;
        }

        /// <summary>
        /// Returns the End Point of the Mid Straight Line of the Dimension Draw
        /// </summary>
        /// <returns>The End of the Mid Straight Line</returns>
        public DrawPoint GetMidLineEnd() 
        {
            DrawPoint midLineEnd = MathCalc.GetPointOfLine(EndX, EndY, StartX, StartY, ArrowLength);
            return midLineEnd;
        }

        /// <summary>
        /// Returns the Anchor Line to Main Dimension . To be used for Anchoring text.
        /// </summary>
        /// <returns></returns>
        public LineDraw GetTextAnchorMiddleLine()
        {
            //This method must be Combined with the below one to Produce one result... Currently code is a mess here
            //We have to get a point on the Perpendicular Line that passes from the Center of our Dimension Line
            //Then the Line we are searching is the Parallel line that passes from this point ;

            //Get the Points of the Perpendicular Line (Our Mid Point and another Point in our Dimension Line)
            (DrawPoint p1, DrawPoint p2) = MathCalc.GetPointsOnPerpendicular((StartX + EndX) / 2d, (StartY + EndY) / 2d, StartX, StartY, 20);
            //Now having these p1,p2 we have identified the perpendicular Line , if we find the other perpendicular to this then
            //we have found the Parallel to our Dimension line at the Distance we set , and with the Length we want to

            (DrawPoint anchorLine2point1, DrawPoint anchorLine2point2) = MathCalc.GetPointsOnPerpendicular(p2.X, p2.Y, p1.X, p1.Y, 100); // This line then has a length of 200 (which is 100 * 2 from the distances each found point has from the Origin Center)
            //There are 2 parallel Lines according to which is the origin each time p1 or p2
            //We pick only p1 here , but we should pick the desired one each time according to the Slope we have (so we will know if we need the left,right,top,bottom...)

            //Have to Check orientation each time to make this automatic -- but now there is no time!! 
            if ((Name == DrawShape.HEIGHTDIM) || (AngleWithAxisX is 90 or 45))
            {
                return new LineDraw(anchorLine2point1.X, anchorLine2point1.Y, anchorLine2point2.X, anchorLine2point2.Y);
            }
            else if ((Name == DrawShape.LENGTHDIM) || (AngleWithAxisX is 0))
            {
                return new LineDraw(anchorLine2point2.X, anchorLine2point2.Y, anchorLine2point1.X, anchorLine2point1.Y);
            }
            else
            {
                return new LineDraw();
            }
        }

        /// <summary>
        /// Returns the PathData of a Line to which we can Anchor Text that will Follow the Dimension Line
        /// </summary>
        /// <returns>The Path Data of the Anchor Line</returns>
        public override string GetTextAnchorMiddleLinePath()
        {
            //We have to get a point on the Perpendicular Line that passes from the Center of our Dimension Line
            //Then the Line we are searching is the Parallel line that passes from this point ;

            //Get the Points of the Perpendicular Line (Our Mid Point and another Point in our Dimension Line)
            (DrawPoint p1, DrawPoint p2) = MathCalc.GetPointsOnPerpendicular((StartX + EndX) / 2d, (StartY + EndY) / 2d, StartX, StartY,20);
            //Now having these p1,p2 we have identified the perpendicular Line , if we find the other perpendicular to this then
            //we have found the Parallel to our Dimension line at the Distance we set , and with the Length we want to

            (DrawPoint anchorLine2point1, DrawPoint anchorLine2point2) = MathCalc.GetPointsOnPerpendicular(p2.X, p2.Y, p1.X, p1.Y, 100); // This line then has a length of 200 (which is 100 * 2 from the distances each found point has from the Origin Center)
            //There are 2 parallel Lines according to which is the origin each time p1 or p2
            //We pick only p1 here , but we should pick the desired one each time according to the Slope we have (so we will know if we need the left,right,top,bottom...)

            string pathData;
            //Have to Check orientation each time to make this automatic -- but now there is no time!! 11/2021
            if ((Name == DrawShape.HEIGHTDIM) || (AngleWithAxisX is not null and 90))
            {
                pathData = PathDataFactory.Line(anchorLine2point1.X, anchorLine2point1.Y, anchorLine2point2.X, anchorLine2point2.Y);
            }
            else if ((Name == DrawShape.LENGTHDIM) || (AngleWithAxisX is not null and 0))
            {
                pathData = PathDataFactory.Line(anchorLine2point2.X, anchorLine2point2.Y, anchorLine2point1.X, anchorLine2point1.Y);
            }
            else
            {
                pathData = "";
            }

            return pathData;
        }
        
        /// <summary>
        /// Gets the Two ArrowHeads of the DimensionLine
        /// </summary>
        /// <returns></returns>
        public (TriangleDraw,TriangleDraw) GetDimensionArrowheads()
        {   //Watch out we place Points clockwise ALWAYS!!!!

            TriangleDraw arrow1 = new();
            TriangleDraw arrow2 = new();

            //The Tip of the Arrowhead is the Tip of the Dimension Line
            arrow1.SecondX = StartX;
            arrow1.SecondY = StartY;

            //Get the mid Point of the Vertical Line of the ArrowHead
            DrawPoint midPoint1 = MathCalc.GetPointOfLine(StartX, StartY, EndX, EndY, ArrowLength);
            

            //Get the other two Points . Because we want to Draw Always Clockwise it is important,
            //which of the two points is the Start and which one is the End (Though we will not do it here yet!)
            DrawPoint p1 = new();
            DrawPoint p2 = new();
            (p1, p2) = MathCalc.GetPointsOnPerpendicular(midPoint1.X, midPoint1.Y, EndX, EndY, ArrowThickness / 2d); //Get the Two points that are on a perpendicular line to the Dimension Line and Pass from the Origin Point
            arrow1.StartX = p1.X;
            arrow1.StartY = p1.Y;
            arrow1.ThirdX = p2.X;
            arrow1.ThirdY = p2.Y;

            //With the Same Method we Find the Second ArrowHead
            arrow2.SecondX = EndX;
            arrow2.SecondY = EndY;

            DrawPoint midPoint2 = MathCalc.GetPointOfLine(EndX, EndY, StartX, StartY, ArrowLength);
            
            p1 = new();
            p2 = new();
            (p1, p2) = MathCalc.GetPointsOnPerpendicular(midPoint2.X, midPoint2.Y, StartX, StartY, ArrowThickness / 2d);
            arrow2.StartX = p1.X;
            arrow2.StartY = p1.Y;
            arrow2.ThirdX = p2.X;
            arrow2.ThirdY = p2.Y;

            return (arrow1, arrow2);
        }

        /// <summary>
        /// Deep Clones the DimensionLine
        /// </summary>
        /// <returns>The Clone</returns>
        public override DrawShape CloneSelf()
        {
            DimensionLineDraw clone = new();
            clone.StartX = this.StartX;
            clone.StartY = this.StartY;
            clone.EndX = this.EndX;
            clone.EndY = this.EndY;
            clone.ArrowThickness = this.ArrowThickness;
            clone.ArrowLength = this.ArrowLength;

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
        /// Returns the PathData String of the Line
        /// </summary>
        /// <returns>the Path Data</returns>
        public override string GetShapePathData()
        {
            //Get the Mid Line which is Shorter
            DrawPoint lineStart = GetMidLineStart();
            DrawPoint lineEnd = GetMidLineEnd();
            string dimensionLine = PathDataFactory.Line(lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y);

            //Get the ArrowHeads
            (TriangleDraw arrow1, TriangleDraw arrow2) = GetDimensionArrowheads();
            string pathArrow1 = arrow1.GetShapePathData();
            string pathArrow2 = arrow2.GetShapePathData();

            //Concatenate the Path Datas
            StringBuilder builder = new();
            builder.Append(dimensionLine);

            switch (ArrowHeads)
            {
                case DimensionArrowheads.Start:
                    builder.Append(pathArrow1);
                    break;
                case DimensionArrowheads.End:
                    builder.Append(pathArrow2);
                    break;
                case DimensionArrowheads.Both:
                    builder.Append(pathArrow1).Append(pathArrow2);
                    break;
                case DimensionArrowheads.None:
                default:
                    break;
            }

            return builder.ToString();
        }


    }
}
