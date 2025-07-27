using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary
{
    /// <summary>
    /// Class Containing Methods that Return the Path Data String of Various Shapes
    /// </summary>
    public static class PathDataFactory
    {

        #region 1.Basic Shapes

        /// <summary>
        /// Returns the Path Data string of a Rectangle
        /// </summary>
        /// <param name="startX">Upper Left Corner X</param>
        /// <param name="startY">Upper Left Corner Y</param>
        /// <param name="length">Rectangle Length</param>
        /// <param name="height">Rectangle Height</param>
        /// <param name="arcRadius">The Rounded Corners Radius</param>
        /// <param name="cornersToRound">Which Sets of Corners Should be Rounded</param>
        /// <returns>The Path Data of the Rectangle specified by the parameters</returns>
        public static string Rectangle(double startX, double startY, double length, double height, double arcRadius, CornersToRound? cornersToRound = CornersToRound.All)
        {

            #region 1.Arcs Radius and Lines Length/Height

            // Case Without Rounding
            double arcUpLeft;
            double arcUpRight;
            double arcBottomLeft;
            double arcBottomRight;

            switch (cornersToRound)
            {
                case CornersToRound.All:
                    arcUpLeft = arcRadius;
                    arcUpRight = arcRadius;
                    arcBottomLeft = arcRadius;
                    arcBottomRight = arcRadius;
                    break;
                case CornersToRound.UpperCorners:
                    arcUpLeft = arcRadius;
                    arcUpRight = arcRadius;
                    arcBottomLeft = 0;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.BottomCorners:
                    arcUpLeft = 0;
                    arcUpRight = 0;
                    arcBottomLeft = arcRadius;
                    arcBottomRight = arcRadius;
                    break;
                case CornersToRound.LeftCorners:
                    arcUpLeft = arcRadius;
                    arcUpRight = 0;
                    arcBottomLeft = arcRadius;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.RightCorners:
                    arcUpLeft = 0;
                    arcUpRight = arcRadius;
                    arcBottomLeft = 0;
                    arcBottomRight = arcRadius;
                    break;
                case CornersToRound.TopRightCorner:
                    arcUpLeft = 0;
                    arcUpRight = arcRadius;
                    arcBottomLeft = 0;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.TopLeftCorner:
                    arcUpLeft = arcRadius;
                    arcUpRight = 0;
                    arcBottomLeft = 0;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.BottomLeftCorner:
                    arcUpLeft = 0;
                    arcUpRight = 0;
                    arcBottomLeft = arcRadius;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.BottomRightCorner:
                    arcUpLeft = 0;
                    arcUpRight = 0;
                    arcBottomLeft = 0;
                    arcBottomRight = arcRadius;
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

            #region 2. Define Points and Form PathData
            StringBuilder builder = new();

            builder
                .Append('M')
                .Append(startX + arcUpLeft)
                .Append(' ')
                .Append(startY)
                .Append(' ');
            double x2 = startX + arcUpLeft + lineUp;    // End of H.Line1 Start of Arc1
            double y2 = startY;
            double x3 = x2 + arcUpRight;         // End of Arc1 Start of V.Line1
            double y3 = y2 + arcUpRight;
            double x4 = x3;                     // End of V.Line 2 Start of Arc2
            double y4 = y3 + lineRight;
            double x5 = x4 - arcBottomRight;        // End of Arc2 start of HLine2
            double y5 = y4 + arcBottomRight;
            double x6 = x5 - lineBottom;       // End of HLine2 start of Arc3
            double y6 = y5;
            double x7 = x6 - arcBottomLeft;       // End of Arc3 start of HLine2
            double y7 = y6 - arcBottomLeft;
            double x8 = x7;                    // End of HLine2 start of Arc4 (Arc 4 has end the same as the start
            double y8 = y7 - lineLeft;

            builder
                // HLine1
                .Append('L').Append(x2).Append(' ').Append(y2).Append(' ')
                // Arc1
                .Append('A').Append(arcRadius).Append(' ').Append(arcRadius).Append(' ')
                .Append(0).Append(' ').Append(0).Append(' ').Append(1).Append(' ')
                .Append(x3).Append(' ').Append(y3).Append(' ')
                // VLine1
                .Append('L').Append(x4).Append(' ').Append(y4).Append(' ')
                // Arc2
                .Append('A').Append(arcRadius).Append(' ').Append(arcRadius).Append(' ')
                .Append(0).Append(' ').Append(0).Append(' ').Append(1).Append(' ')
                .Append(x5).Append(' ').Append(y5).Append(' ')
                // HLine2
                .Append('L').Append(x6).Append(' ').Append(y6).Append(' ')
                // Arc3
                .Append('A').Append(arcRadius).Append(' ').Append(arcRadius).Append(' ')
                .Append(0).Append(' ').Append(0).Append(' ').Append(1).Append(' ')
                .Append(x7).Append(' ').Append(y7).Append(' ')
                // VLine2
                .Append('L').Append(x8).Append(' ').Append(y8).Append(' ')
                // Arc4
                .Append('A').Append(arcRadius).Append(' ').Append(arcRadius).Append(' ')
                .Append(0).Append(' ').Append(0).Append(' ').Append(1).Append(' ')
                .Append(x8 + arcUpLeft).Append(' ').Append(y8 - arcUpLeft).Append(' ')
                //END
                .Append("Z ");

            return builder.ToString().Replace(',', '.');
            #endregion

        }

        /// <summary>
        /// Returns the Path Data string of a Rectangle with Sharp Corners
        /// </summary>
        /// <param name="startX">Upper Left Corner X</param>
        /// <param name="startY">Upper Left Corner Y</param>
        /// <param name="length">Rectangle Length</param>
        /// <param name="height">Rectangle Height</param>
        /// <returns>The Path Data of the Rectangle specified by the parameters</returns>
        public static string Rectangle(double startX, double startY, double length, double height)
        {
            string pathData = Rectangle(startX, startY, length, height, 0, CornersToRound.None);
            return pathData;
        }

        /// <summary>
        /// Returns the Path Data string of a Rectangle antiClockwise (Append to Clockwise Svg to make a Hole)
        /// The Path must have a default fill-rule and the hole path must be added to an existing path that is drawn Clockwise 
        /// </summary>
        /// <param name="startX">The TopLeft Edge X Coordinate</param>
        /// <param name="startY">The TopLeft Edge Y Coordinate</param>
        /// <param name="length">The Length of the Rectangle</param>
        /// <param name="height">The Height of the Rectangle</param>
        /// <returns></returns>
        public static string RectangleAnticlockwise(double startX, double startY, double length, double height ,double arcRadius = 0 , CornersToRound cornersToRound = CornersToRound.None)
        {
            #region 1.Arcs Radius and Lines Length/Height

            // Case Without Rounding
            double arcUpLeft;
            double arcUpRight;
            double arcBottomLeft;
            double arcBottomRight;

            switch (cornersToRound)
            {
                case CornersToRound.All:
                    arcUpLeft = arcRadius;
                    arcUpRight = arcRadius;
                    arcBottomLeft = arcRadius;
                    arcBottomRight = arcRadius;
                    break;
                case CornersToRound.UpperCorners:
                    arcUpLeft = arcRadius;
                    arcUpRight = arcRadius;
                    arcBottomLeft = 0;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.BottomCorners:
                    arcUpLeft = 0;
                    arcUpRight = 0;
                    arcBottomLeft = arcRadius;
                    arcBottomRight = arcRadius;
                    break;
                case CornersToRound.LeftCorners:
                    arcUpLeft = arcRadius;
                    arcUpRight = 0;
                    arcBottomLeft = arcRadius;
                    arcBottomRight = 0;
                    break;
                case CornersToRound.RightCorners:
                    arcUpLeft = 0;
                    arcUpRight = arcRadius;
                    arcBottomLeft = 0;
                    arcBottomRight = arcRadius;
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

            #region 2. Define Points and Form PathData
            StringBuilder builder = new();

            double x1 = startX;
            double y1 = startY + arcUpLeft;
            double x2 = x1;
            double y2 = y1 + lineLeft;
            double x3 = x2 + arcBottomLeft;
            double y3 = y2 + arcBottomLeft;
            double x4 = x3 + lineBottom;
            double y4 = y3;
            double x5 = x4 + arcBottomRight;
            double y5 = y4 - arcBottomRight;
            double x6 = x5;
            double y6 = y5 - lineRight;
            double x7 = x6 - arcUpRight;
            double y7 = y6 - arcUpRight;
            double x8 = x7 - lineUp;
            double y8 = y7;

            builder
               .Append('M').Append(x1).Append(' ').Append(y1).Append(' ') //Move to x1,y1
               
               .Append('L').Append(x2).Append(' ').Append(y2).Append(' ') //Line to x2,y2 (Left Vertical)
               
               .Append('A').Append(arcRadius).Append(' ').Append(arcRadius).Append(' ') //Arc to x3,y3
               .Append(0).Append(' ').Append(0).Append(' ').Append(0).Append(' ')
               .Append(x3).Append(' ').Append(y3).Append(' ')
               
               .Append('L').Append(x4).Append(' ').Append(y4).Append(' ') //Line to x4,y4
               
               .Append('A').Append(arcRadius).Append(' ').Append(arcRadius).Append(' ') //Arc to x5,y5
               .Append(0).Append(' ').Append(0).Append(' ').Append(0).Append(' ')
               .Append(x5).Append(' ').Append(y5).Append(' ')
               
               .Append('L').Append(x6).Append(' ').Append(y6).Append(' ') //Line to x6,y6
               
               .Append('A').Append(arcRadius).Append(' ').Append(arcRadius).Append(' ') //Arc to x7,y7
               .Append(0).Append(' ').Append(0).Append(' ').Append(0).Append(' ')
               .Append(x7).Append(' ').Append(y7).Append(' ')
               
               .Append('L').Append(x8).Append(' ').Append(y8).Append(' ') //Line to x8,y8
               
               .Append('A').Append(arcRadius).Append(' ').Append(arcRadius).Append(' ') //Arc to x1,y1
               .Append(0).Append(' ').Append(0).Append(' ').Append(0).Append(' ')
               .Append(x1).Append(' ').Append(y1).Append(' ')

               .Append("Z ");

            #endregion

            return builder.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Returns the Path Data string of a Circle
        /// </summary>
        /// <param name="centerX">The Center's X coordinate</param>
        /// <param name="centerY">The Center's Y coordinate</param>
        /// <param name="radius">The Circles Radius</param>
        /// <returns>A Path Data String</returns>
        public static string Circle(double centerX, double centerY, double radius)
        {
            // Created two semicircular Arcs
            StringBuilder builder = new();
            builder
                .Append('M')                //Move To
                .Append(centerX - radius)   //The horizontal Point of the Circles perimeter (centerX-radius,centerY)
                .Append(' ')
                .Append(centerY)
                .Append(' ')
                .Append('A')            //draw an arc (ABSOLUTE to 0.0 of container(A) or (a) Relative to the Current Starting Point)
                .Append(radius)         //With rx
                .Append(' ')
                .Append(radius)        //With ry
                .Append(' ')
                .Append(0)             //Rotate the Arc by 0 degrees (relative to x Axis)
                .Append(' ')
                .Append(0)            //Big Arc YES(1) >=180degrees , //Small Arc NO(0) <= 180degrees
                .Append(' ')
                .Append(1)            // Move at positive angles (1) or negative angles (0)
                .Append(' ')
                .Append(centerX + radius)   //End x
                .Append(' ')
                .Append(centerY)            //End Y
                .Append(' ')                //Second consecutive arc (does not need again letter 'a')

                .Append(radius)             //With rx
                .Append(' ')
                .Append(radius)             //With ry
                .Append(' ')
                .Append(0)                  //Rotate the Arc by 0 degrees (relative to x Axis)
                .Append(' ')
                .Append(0)                  //Big Arc YES(1) >=180degrees , //Small Arc NO(0) <= 180degrees
                .Append(' ')
                .Append(1)                  // Move at positive angles (1) or negative angles (0)
                .Append(' ')
                .Append(centerX - radius)   //End x (Back to start)
                .Append(' ')
                .Append(centerY)            //End Y
                .Append(' ')
                .Append("Z ");              //Close Path

            return builder.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Returns the Path Data string of a Circle antiClockwise (Append to Clockwise Svg to make a Hole)
        /// The Path must have a default fill-rule and the hole path must be added to an existing path that is drawn Clockwise
        /// </summary>
        /// <param name="centerX">The Center's X coordinate</param>
        /// <param name="centerY">The Center's Y coordinate</param>
        /// <param name="radius">The Circles Radius</param>
        /// <returns>A Path Data String</returns>
        public static string CircleAntiClockwise(double centerX, double centerY, double radius)
        {
            // Created two semicircular Arcs
            StringBuilder builder = new();
            builder
                .Append('M')                //Move To
                .Append(centerX - radius)   //The horizontal Point of the Circles perimeter (centerX-radius,centerY)
                .Append(' ')
                .Append(centerY)
                .Append(' ')
                .Append('A')            //draw an arc (ABSOLUTE to 0.0 of container(A) or (a) Relative to the Current Starting Point)
                .Append(radius)         //With rx
                .Append(' ')
                .Append(radius)        //With ry
                .Append(' ')
                .Append(0)             //Rotate the Arc by 0 degrees (relative to x Axis)
                .Append(' ')
                .Append(0)            //Big Arc YES(1) >=180degrees , //Small Arc NO(0) <= 180degrees
                .Append(' ')
                .Append(0)            // Move at positive angles (1) or negative angles (0)
                .Append(' ')
                .Append(centerX + radius)   //End x
                .Append(' ')
                .Append(centerY)            //End Y
                .Append(' ')                //Second consecutive arc (does not need again letter 'a')

                .Append(radius)             //With rx
                .Append(' ')
                .Append(radius)             //With ry
                .Append(' ')
                .Append(0)                  //Rotate the Arc by 0 degrees (relative to x Axis)
                .Append(' ')
                .Append(0)                  //Big Arc YES(1) >=180degrees , //Small Arc NO(0) <= 180degrees
                .Append(' ')
                .Append(0)                  // Move at positive angles (1) or negative angles (0)
                .Append(' ')
                .Append(centerX - radius)   //End x (Back to start)
                .Append(' ')
                .Append(centerY)            //End Y
                .Append(' ')
                .Append("Z ");              //Close Path

            return builder.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Returns the Path Data string of a Line
        /// </summary>
        /// <param name="startX">The Start X of the Line</param>
        /// <param name="startY">The Start Y of the Line</param>
        /// <param name="endX">The end X of the Line</param>
        /// <param name="endY">The end Y of the Line</param>
        /// <returns>The Path Data string of the Line</returns>
        public static string Line(double startX, double startY, double endX, double endY)
        {
            StringBuilder builder = new();
            builder
                .Append('M')        //Move To
                .Append(startX)     //Point sX,sY
                .Append(' ')
                .Append(startY)
                .Append(' ')
                .Append('L')        //Line To
                .Append(endX)       //Point eX , eY
                .Append(' ')
                .Append(endY)
                .Append(' ');

            return builder.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Adds a Line to the 
        /// </summary>
        /// <param name="pathData">The Path Data On Which to Continue with More Lines</param>
        /// <param name="endX">The EndX of the New Line</param>
        /// <param name="endY">The EndY of the New Line</param>
        /// <returns></returns>
        public static string ContinuePolygon(double endX,double endY,bool closePolygon = false)
        {
            StringBuilder builder = new();
            builder
                .Append('L')//Add another Line - Continuing from the Previous
                .Append(endX)
                .Append(' ')
                .Append(endY)
                .Append(' ');
            //If this is the Last Line Close The Polygon
            if (closePolygon) { builder.Append("Z "); }
            return builder.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Draws an Arc from the Last Point (a M command or any point(x,y) must precede this Command otherwise the Path will be invalid)
        /// </summary>
        /// <param name="radiusX">The x Radius of the Ellipse defining the arc</param>
        /// <param name="radiusY">The y Radius of the Ellipse defining the arc</param>
        /// <param name="endX">The Arc's 'endX' or the 'dx' from the start point when it is relative</param>
        /// <param name="endY">The Arc's 'endY' or the 'dy' from the Last point when it is relative</param>
        /// <param name="rotationAxisX">The rotation of the x Axis of the Ellipse Defining the Arc</param>
        /// <param name="isBigArc">Weather the big or the Small arc should be drawn , when the line that connects the start and end of the arc passes through the center of the defining ellipse this parameter has no relevance and both big or small arc means the same arc </param>
        /// <param name="isClockwiseArc">Weather the arc is drawn clockwise or anticlockwise </param>
        /// <param name="isRelative">Weather the (endX , endY) parameters are relative to the start point(the lengths from x and y respectively) and not absolute coordinates</param>
        /// <returns>the Path Data of the Arc</returns>
        public static string Arc(double radiusX , double radiusY,double endX , double endY,double rotationAxisX,bool isBigArc,bool isClockwiseArc,bool isRelative)
        {
            StringBuilder builder = new();
            builder.Append(isRelative ? 'a' : 'A')
                .Append(radiusX).Append(' ')
                .Append(radiusY).Append(' ')
                .Append(rotationAxisX).Append(' ')
                .Append(isBigArc ? 1 : 0).Append(' ')
                .Append(isClockwiseArc ? 1 : 0).Append(' ') //1 when its clockwise - 0 when its negative (anti-clockwise)
                .Append(endX).Append(' ')
                .Append(endY).Append(' ');

            return builder.ToString().Replace(',','.');
        }

        /// <summary>
        /// Returns the Path Data String of a Triangle
        /// </summary>
        /// <param name="startX">The X Start</param>
        /// <param name="startY">The Y Start</param>
        /// <param name="secondX">The X of the Second Point</param>
        /// <param name="secondY">The Y of the Second Point</param>
        /// <param name="thirdX">The X of the Third Point</param>
        /// <param name="thirdY">The Y of the Third Point</param>
        /// <returns>The Path Data String</returns>
        public static string Triangle(double startX, double startY, double secondX, double secondY, double thirdX, double thirdY)
        {
            StringBuilder builder = new();
            builder
                .Append('M')        //Move To
                .Append(startX)     //Point FirstPoint of Triangle
                .Append(' ')
                .Append(startY)
                .Append(' ')
                .Append('L')        //Line To
                .Append(secondX)    //Second Point of Triangle
                .Append(' ')
                .Append(secondY)
                .Append(' ')
                .Append('L')        //Line to
                .Append(thirdX)     //Third Point of Triangle
                .Append(' ')
                .Append(thirdY)
                .Append(" Z ");   //Close Path

            return builder.ToString().Replace(',', '.');
        }

        #endregion

        #region 2.Extra Shapes

        /// <summary>
        /// Returns the PathData for a Four Sided Polygon
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x4"></param>
        /// <param name="y4"></param>
        /// <returns>the Path Data String</returns>
        public static string Quadrilateral(double x1 , double y1,
                                           double x2 , double y2, 
                                           double x3 , double y3, 
                                           double x4 , double y4)
        {
            StringBuilder builder = new();

            builder.Append('M').Append(x1).Append(' ').Append(y1).Append(' ')
                   .Append('L').Append(x2).Append(' ').Append(y2).Append(' ')
                   .Append('L').Append(x3).Append(' ').Append(y3).Append(' ')
                   .Append('L').Append(x4).Append(' ').Append(y4).Append(' ')
                   .Append("Z ");


            return builder.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Returns the Path Data String of an Ellipse Shape
        /// </summary>
        /// <param name="centerX">The X Coordinate of the Ellipse Center</param>
        /// <param name="centerY">The Y Coordinate of the Ellipse Center</param>
        /// <param name="length">The Length of the Ellipse</param>
        /// <param name="height">The Height of the Ellipse</param>
        /// <returns>The Path Data String</returns>
        public static string Ellipse(double centerX, double centerY, double length, double height)
        {
            //The changes respective to each axis in each drawn Arc
            //We have to draw Four Arcs to achieve the ellipse (All clockwise)
            double dx = length / 2;
            double dy = height / 2;

            //The LeftMost Point
            double x1 = centerX - dx;
            double y1 = centerY;

            //Top
            double x2 = x1 + dx;
            double y2 = y1 - dy;

            //Right
            double x3 = x2 + dx;
            double y3 = y2 + dy;

            //Bottom
            double x4 = x3 - dx;
            double y4 = y3 + dy;
            
            StringBuilder builder = new();
            builder.Append('M').Append(' ').Append(x1).Append(' ').Append(y1).Append(' ')

                   //First Arc from (x1,y1) to (x2,y2)
                   .Append('A').Append(dx).Append(' ').Append(dy).Append(' ')
                   .Append('0').Append(' ').Append('0').Append(' ').Append('1').Append(' ')
                   .Append(x2).Append(' ').Append(y2).Append(' ')

                   //Second Arc from (x2,y2) to (x3,y3)
                   .Append('A').Append(dx).Append(' ').Append(dy).Append(' ')
                   .Append('0').Append(' ').Append('0').Append(' ').Append('1').Append(' ')
                   .Append(x3).Append(' ').Append(y3).Append(' ')

                   //Third Arc from (x3,y3) to (x4,y4)
                   .Append('A').Append(dx).Append(' ').Append(dy).Append(' ')
                   .Append('0').Append(' ').Append('0').Append(' ').Append('1').Append(' ')
                   .Append(x4).Append(' ').Append(y4).Append(' ')

                   //Fourth Arc from (x3,y3) to (x4,y4)
                   .Append('A').Append(dx).Append(' ').Append(dy).Append(' ')
                   .Append('0').Append(' ').Append('0').Append(' ').Append('1').Append(' ')
                   .Append(x1).Append(' ').Append(y1).Append(' ')
                   //Close Ellipse
                   .Append("Z ");

            return builder.ToString().Replace(',', '.');
        }

        /// <summary>
        /// Returns the Path Data String of an Ellipse Shape AntiClockwise
        /// </summary>
        /// <param name="centerX">The X Coordinate of the Ellipse Center</param>
        /// <param name="centerY">The Y Coordinate of the Ellipse Center</param>
        /// <param name="length">The Length of the Ellipse</param>
        /// <param name="height">The Height of the Ellipse</param>
        /// <returns>The Path Data String</returns>
        public static string EllipseAntiClockwise(double centerX, double centerY, double length, double height)
        {
            //The changes respective to each axis in each drawn Arc
            //We have to draw Four Arcs to achieve the ellipse (All clockwise)
            double dx = length / 2;
            double dy = height / 2;

            //The LeftMost Point
            double x1 = centerX - dx;
            double y1 = centerY;

            //Top
            double x2 = x1 + dx;
            double y2 = y1 - dy;

            //Right
            double x3 = x2 + dx;
            double y3 = y2 + dy;

            //Bottom
            double x4 = x3 - dx;
            double y4 = y3 + dy;

            StringBuilder builder = new();
            builder.Append('M').Append(' ').Append(x1).Append(' ').Append(y1).Append(' ')

                   //First Arc from (x1,y1) to (x4,y4)
                   .Append('A').Append(dx).Append(' ').Append(dy).Append(' ')
                   .Append('0').Append(' ').Append('0').Append(' ').Append('0').Append(' ')
                   .Append(x4).Append(' ').Append(y4).Append(' ')

                   //Second Arc from (x4,y4) to (x3,y3)
                   .Append('A').Append(dx).Append(' ').Append(dy).Append(' ')
                   .Append('0').Append(' ').Append('0').Append(' ').Append('0').Append(' ')
                   .Append(x3).Append(' ').Append(y3).Append(' ')

                   //Third Arc from (x3,y3) to (x2,y2)
                   .Append('A').Append(dx).Append(' ').Append(dy).Append(' ')
                   .Append('0').Append(' ').Append('0').Append(' ').Append('0').Append(' ')
                   .Append(x2).Append(' ').Append(y2).Append(' ')

                   //Fourth Arc from (x2,y2) to (x1,y1)
                   .Append('A').Append(dx).Append(' ').Append(dy).Append(' ')
                   .Append('0').Append(' ').Append('0').Append(' ').Append('0').Append(' ')
                   .Append(x1).Append(' ').Append(y1).Append(' ')
                   //Close Ellipse
                   .Append("Z ");

            return builder.ToString().Replace(',', '.');
        }

        #endregion

       
    }
}

