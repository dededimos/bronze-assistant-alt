using SVGDrawingLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary
{
    public class PathDataBuilder
    {
        private StringBuilder builder;

        public PathDataBuilder()
        {
            builder = new();
        }

        public void ResetBuilder()
        {
            builder = new();
        }

        /// <summary>
        /// Closes the Start and End of the Draw with a Straight Line
        /// </summary>
        /// <returns></returns>
        public PathDataBuilder CloseDraw()
        {
            builder.Append('Z');
            
            return this;
        }
        /// <summary>
        /// Moves the draw cursor at the specified point 
        /// </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <returns></returns>
        public PathDataBuilder MoveTo(double x, double y)
        {
            builder.Append('M')
                .Append(x).Append(' ')
                .Append(y).Append(' ');
            return this;
        }
        /// <summary>
        /// Moves the cursor of teh draw to a new point at the specified distance from the current cursor point
        /// </summary>
        /// <param name="xDistance">The distance traversed at the X axis</param>
        /// <param name="yDistance">The distance traversed at the Y axis</param>
        /// <returns></returns>
        public PathDataBuilder MoveToRelative(double xDistance,double yDistance)
        {
            builder.Append('m')
                .Append(xDistance).Append(' ')
                .Append(yDistance).Append(' ');
            return this;
        }
        /// <summary>
        /// Draws a Line from the Current Cursor Position to the specified point
        /// </summary>
        /// <param name="endX">The X Coordinate of the End of the Line</param>
        /// <param name="endY">The Y Coordinate of the End of the Line</param>
        /// <returns></returns>
        public PathDataBuilder AddLine(double endX,double endY)
        {
            builder.Append('L')     //Line To
                .Append(endX)            
                .Append(' ')
                .Append(endY)
                .Append(' ');
            return this;
        }
        /// <summary>
        /// Draws a Line from the Current Cursor position ending at the specified distances from the current (X,Y) point
        /// </summary>
        /// <param name="distanceEndX">The Distance traversed in Xaxis</param>
        /// <param name="distanceEndY">The Distance traversed in Yaxis</param>
        /// <returns></returns>
        public PathDataBuilder AddLineRelative(double distanceEndX , double distanceEndY)
        {
            builder.Append('l')     //Line To
                .Append(distanceEndX)
                .Append(' ')
                .Append(distanceEndY)
                .Append(' ');
            return this;
        }
        /// <summary>
        /// Draws an Arc from the current Cursor Position to the specified point , according to the provided arguments
        /// </summary>
        /// <param name="radiusX">The x Radius of the Ellipse defining the arc</param>
        /// <param name="radiusY">The y Radius of the Ellipse defining the arc</param>
        /// <param name="endX">The Arc's 'endX'</param>
        /// <param name="endY">The Arc's 'endY'</param>
        /// <param name="rotationAxisX">The rotation of the x Axis of the Ellipse Defining the Arc</param>
        /// <param name="isBigArc">Weather the big or the Small arc should be drawn , when the line that connects the start and end of the arc passes through the center of the defining ellipse this parameter has no relevance and both big or small arc means the same arc </param>
        /// <param name="isClockwiseArc">Weather the arc is drawn clockwise or anticlockwise </param>
        /// <returns></returns>
        public PathDataBuilder AddArc(double radiusX, double radiusY, double endX, double endY, double rotationAxisX, bool isBigArc, bool isClockwiseArc)
        {
            builder.Append('A')
                .Append(radiusX).Append(' ')
                .Append(radiusY).Append(' ')
                .Append(rotationAxisX).Append(' ')
                .Append(isBigArc ? 1 : 0).Append(' ')
                .Append(isClockwiseArc ? 1 : 0).Append(' ') //1 when its clockwise - 0 when its negative (anti-clockwise)
                .Append(endX).Append(' ')
                .Append(endY).Append(' ');
            return this;
        }
        /// <summary>
        /// Draws an Arc from the current Cursor Position ending at the specified distances from the current (X,Y), according to the provided arguments
        /// </summary>
        /// <param name="radiusX">The x Radius of the Ellipse defining the arc</param>
        /// <param name="radiusY">The y Radius of the Ellipse defining the arc</param>
        /// <param name="distanceX">The distance of the endpoint of the Arc from the Start , at the X axis</param>
        /// <param name="distanceY">The distance of the endpoint of the Arc from the Start , at the Y axis</param>
        /// <param name="rotationAxisX">The rotation of the x Axis of the Ellipse Defining the Arc</param>
        /// <param name="isBigArc">Weather the big or the Small arc should be drawn , when the line that connects the start and end of the arc passes through the center of the defining ellipse this parameter has no relevance and both big or small arc means the same arc </param>
        /// <param name="isClockwiseArc">Weather the arc is drawn clockwise or anticlockwise </param>
        /// <returns></returns>
        public PathDataBuilder AddArcRelative(double radiusX, double radiusY, double distanceX, double distanceY, double rotationAxisX, bool isBigArc, bool isClockwiseArc)
        {
            builder.Append('a')
                .Append(radiusX).Append(' ')
                .Append(radiusY).Append(' ')
                .Append(rotationAxisX).Append(' ')
                .Append(isBigArc ? 1 : 0).Append(' ')
                .Append(isClockwiseArc ? 1 : 0).Append(' ') //1 when its clockwise - 0 when its negative (anti-clockwise)
                .Append(distanceX).Append(' ')
                .Append(distanceY).Append(' ');
            return this;
        }
        /// <summary>
        /// Draws a circle with the specified center and radius
        /// </summary>
        /// <param name="centerX">The X Coordinate of the circle's center</param>
        /// <param name="centerY">The Y Coordinate of the circle's center</param>
        /// <param name="radius">The radius of the circle</param>
        /// <returns></returns>
        public PathDataBuilder AddCircle(double centerX,double centerY,double radius)
        {
            // Move the cursor a radius away from the center (only in x Axis) so it can be drawn by its left start (arcs drawing the circle)
            MoveTo(centerX - radius,centerY);
            AddCircle(radius);
            return this;
        }
        /// <summary>
        /// Adds a circle that starts being drawn at the current cursor position (its Left Side Starts from current X,Y)
        /// </summary>
        /// <param name="radius">The Circles Radius</param>
        /// <returns></returns>
        public PathDataBuilder AddCircle(double radius)
        {
            //Add two relative Arcs consecutively to add a Circle;
            AddArcRelative(radius, radius, 2 * radius, 0, 0, false, true);
            AddArcRelative(radius, radius, 2 * radius, 0, 0, false, true);
            return this;
        }
        /// <summary>
        /// Adds a Circle Hole to the Draw that starts being drawn at the current cursor position (its Left Side Starts from current X,Y)
        /// </summary>
        /// <param name="radius">The circle radius</param>
        /// <returns></returns>
        public PathDataBuilder AddCircleHole(double radius)
        {
            //Add two relative Arcs (anti-clockwise) consecutively to add a Hole Circle;
            AddArcRelative(radius, radius, 2 * radius, 0, 0, false, false);
            AddArcRelative(radius, radius, -2 * radius, 0, 0, false, false);
            return this;
        }
        /// <summary>
        /// Draws a circle Hole with the specified center and radius
        /// </summary>
        /// <param name="centerX">The X Coordinate of the circle's center</param>
        /// <param name="centerY">The Y Coordinate of the circle's center</param>
        /// <param name="radius">The radius of the circle</param>
        /// <returns></returns>
        public PathDataBuilder AddCircleHole(double centerX, double centerY, double radius)
        {
            // Move the cursor a radius away from the center (only in x Axis) so it can be drawn by its left start (arcs drawing the circle)
            MoveTo(centerX - radius, centerY);
            AddCircleHole(radius);
            return this;
        }
        /// <summary>
        /// Adds a Rectangle Starting from the current Cursor Point
        /// </summary>
        /// <param name="length">The Length of the Rectangle</param>
        /// <param name="height">The Height of the Rectangle</param>
        /// <returns></returns>
        public PathDataBuilder AddRectangle(double length , double height)
        {
            AddLineRelative(length, 0);
            AddLineRelative(0, height);
            AddLineRelative(-length, 0);
            AddLineRelative(0, -height);
            return this;
        }
        /// <summary>
        /// Adds a Rectangle with smooth Edges Starting from the current cursor Point
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="cornersArcRadius">The Radius of the Arcs of the Rectangle</param>
        /// <param name="cornersToRound">The Corners to Round</param>
        /// <returns></returns>
        public PathDataBuilder AddRectangle(double length, double height, params double[] cornersArcRadius) 
        {
            if (cornersArcRadius.All(d=> d==0))
            {
                AddRectangle(length, height);
                return this;
            }
            #region Corners Rounding
            if (cornersArcRadius.Length != 4) throw new ArgumentOutOfRangeException(nameof(cornersArcRadius),$"Array must define 4 values when set");
            // Case Without Rounding
            double arcUpLeft = cornersArcRadius[0];
            double arcUpRight = cornersArcRadius[1];
            double arcBottomLeft = cornersArcRadius[2];
            double arcBottomRight = cornersArcRadius[3];

            double lineUp = length - arcUpLeft - arcUpRight;
            double lineBottom = length - arcBottomLeft - arcBottomRight;
            double lineLeft = height - arcUpLeft - arcBottomLeft;
            double lineRight = height - arcUpRight - arcBottomRight;
            #endregion

            if (arcUpLeft != 0) MoveToRelative(arcUpLeft, 0);//move a little bit to pass the first corner radius
                        
            AddLineRelative(lineUp, 0);
            if (arcUpRight != 0) AddArcRelative(arcUpRight, arcUpRight, arcUpRight, arcUpRight, 0, false, true);

            AddLineRelative(0, lineRight);
            if (arcBottomRight != 0) AddArcRelative(arcBottomRight, arcBottomRight, -arcBottomRight, arcBottomRight, 0, false, true);

            AddLineRelative(-lineBottom, 0);
            if (arcBottomLeft != 0) AddArcRelative(arcBottomLeft, arcBottomLeft, -arcBottomLeft, -arcBottomLeft, 0, false, true);

            AddLineRelative(0, -lineLeft);
            if (arcUpLeft != 0) AddArcRelative(arcUpLeft, arcUpLeft, arcUpLeft, -arcUpLeft, 0, false, true);

            return this;
        }
        /// <summary>
        /// Adds a Rectangle Hole Starting from the Current Cursor Position
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <returns></returns>
        public PathDataBuilder AddRectangleHole(double length , double height)
        {
            AddLineRelative(0, height);
            AddLineRelative(length, 0);
            AddLineRelative(0, -height);
            AddLineRelative(-length, 0);
            return this;
        }
        /// <summary>
        /// Adds a smooth Rectangle Hole
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="cornersArcRadius">CornerRadius of the Rectangle's Arcs</param>
        /// <param name="cornersToRound">Which corners to Round</param>
        /// <returns></returns>
        public PathDataBuilder AddRectangleHole(double length, double height, double cornersArcRadius, CornersToRound cornersToRound = CornersToRound.All)
        {
            if (cornersToRound is CornersToRound.None || cornersArcRadius is 0)
            {
                AddRectangleHole(length, height);
                return this;
            }
            #region Corners Rounding
            // Case Without Rounding
            double arcUpLeft;
            double arcUpRight;
            double arcBottomLeft;
            double arcBottomRight;

            switch (cornersToRound)
            {
                case CornersToRound.All:
                    arcUpLeft = cornersArcRadius;
                    arcUpRight = cornersArcRadius;
                    arcBottomLeft = cornersArcRadius;
                    arcBottomRight = cornersArcRadius;
                    break;
                case CornersToRound.UpperCorners:
                    arcUpLeft = cornersArcRadius;
                    arcUpRight = cornersArcRadius;
                    arcBottomLeft = 0;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.BottomCorners:
                    arcUpLeft = 0;
                    arcUpRight = 0;
                    arcBottomLeft = cornersArcRadius;
                    arcBottomRight = cornersArcRadius;
                    break;
                case CornersToRound.LeftCorners:
                    arcUpLeft = cornersArcRadius;
                    arcUpRight = 0;
                    arcBottomLeft = cornersArcRadius;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.RightCorners:
                    arcUpLeft = 0;
                    arcUpRight = cornersArcRadius;
                    arcBottomLeft = 0;
                    arcBottomRight = cornersArcRadius;
                    break;
                case CornersToRound.TopRightCorner:
                    arcUpLeft = 0;
                    arcUpRight = cornersArcRadius;
                    arcBottomLeft = 0;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.TopLeftCorner:
                    arcUpLeft = cornersArcRadius;
                    arcUpRight = 0;
                    arcBottomLeft = 0;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.BottomLeftCorner:
                    arcUpLeft = 0;
                    arcUpRight = 0;
                    arcBottomLeft = cornersArcRadius;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.BottomRightCorner:
                    arcUpLeft = 0;
                    arcUpRight = 0;
                    arcBottomLeft = 0;
                    arcBottomRight = cornersArcRadius;
                    break;
                case CornersToRound.None:
                default:
                    arcUpLeft = 0;
                    arcUpRight = 0;
                    arcBottomLeft = 0;
                    arcBottomRight = 0;
                    break;
            }

            double lineUp = length - arcUpLeft - arcUpRight;
            double lineBottom = length - arcBottomLeft - arcBottomRight;
            double lineLeft = height - arcUpLeft - arcBottomLeft;
            double lineRight = height - arcUpRight - arcBottomRight;
            #endregion

            if (arcUpLeft != 0) MoveToRelative(0, arcUpLeft);//move a little bit to pass the first corner radius
            
            AddLineRelative(0, lineLeft);
            if (arcBottomLeft != 0) AddArcRelative(arcBottomLeft, arcBottomLeft, arcBottomLeft, arcBottomLeft, 0, false, false);

            AddLineRelative(lineBottom, 0);
            if (arcBottomRight != 0) AddArcRelative(arcBottomRight, arcBottomRight, arcBottomRight, -arcBottomRight, 0, false, false);

            AddLineRelative(0, -lineRight);
            if (arcUpRight != 0) AddArcRelative(arcUpRight, arcUpRight, -arcUpRight, -arcUpRight, 0, false, false);

            AddLineRelative(-lineUp, 0);
            if (arcUpLeft != 0) AddArcRelative(arcUpLeft, arcUpLeft, -arcUpLeft, arcUpLeft, 0, false, false);

            return this;
        }
        public PathDataBuilder AddEllipse()
        {
            throw new NotImplementedException();
        }
        public PathDataBuilder AddEllipseHole()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the Drawn Path Data
        /// </summary>
        /// <returns></returns>
        public string GetPathData()
        {
            return builder.ToString().Replace(',','.');
        }
    }
}
