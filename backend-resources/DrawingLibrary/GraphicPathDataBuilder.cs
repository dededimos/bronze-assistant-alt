using CommonHelpers.Exceptions;
using ShapesLibrary.Enums;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLibrary
{
    public class GraphicPathDataBuilder
    {
        private StringBuilder builder;

        public GraphicPathDataBuilder()
        {
            builder = new();
        }

        public GraphicPathDataBuilder ResetBuilder()
        {
            builder.Clear();
            return this;
        }

        /// <summary>
        /// Closes the Start and End of the Draw with a Straight Line
        /// </summary>
        /// <returns></returns>
        public GraphicPathDataBuilder CloseDraw()
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
        public GraphicPathDataBuilder MoveTo(double x, double y)
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
        public GraphicPathDataBuilder MoveToRelative(double xDistance, double yDistance)
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
        public GraphicPathDataBuilder AddLine(double endX, double endY)
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
        public GraphicPathDataBuilder AddLineRelative(double distanceEndX, double distanceEndY)
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
        public GraphicPathDataBuilder AddArc(double radiusX, double radiusY, double endX, double endY, double rotationAxisX, bool isBigArc, bool isClockwiseArc)
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
        public GraphicPathDataBuilder AddArcRelative(double radiusX, double radiusY, double distanceX, double distanceY, double rotationAxisX, bool isBigArc, bool isClockwiseArc)
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
        /// Adds a Circle from the specified info object
        /// </summary>
        /// <param name="circleInfo"></param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircle(CircleInfo circleInfo)
        {
            return AddCircle(circleInfo.Radius, circleInfo.LocationX, circleInfo.LocationY);
        }
        /// <summary>
        /// Draws a circle with the specified center and radius
        /// </summary>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="centerX">The X Coordinate of the circle's center</param>
        /// <param name="centerY">The Y Coordinate of the circle's center</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircle(double radius, double centerX, double centerY)
        {
            // Move the cursor a radius away from the center (only in x Axis) so it can be drawn by its left start (arcs drawing the circle)
            return MoveTo(centerX - radius, centerY).
            AddCircle(radius);
        }
        /// <summary>
        /// Adds a circle that starts being drawn at the current cursor position (its Left Side Starts from current X,Y)
        /// </summary>
        /// <param name="radius">The Circles Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircle(double radius)
        {
            //Add two relative Arcs consecutively to add a Circle;
            return AddArcRelative(radius, radius, 2 * radius, 0, 0, false, true)
                .AddArcRelative(radius, radius, -2 * radius, 0, 0, false, true)
                .CloseDraw();
        }

        /// <summary>
        /// Adds a Circle HOLE from the specified info object
        /// </summary>
        /// <param name="circleInfo"></param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleHole(CircleInfo circleInfo)
        {
            return AddCircleHole(circleInfo.Radius, circleInfo.LocationX, circleInfo.LocationY);
        }
        /// <summary>
        /// Adds a Circle Hole to the Draw that starts being drawn at the current cursor position (its Left Side Starts from current X,Y)
        /// </summary>
        /// <param name="radius">The circle radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleHole(double radius)
        {
            //Add two relative Arcs (anti-clockwise) consecutively to add a Hole Circle;
            return AddArcRelative(radius, radius, 2 * radius, 0, 0, false, false)
                .AddArcRelative(radius, radius, -2 * radius, 0, 0, false, false)
                .CloseDraw();
        }
        /// <summary>
        /// Draws a circle Hole with the specified center and radius
        /// </summary>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="centerX">The X Coordinate of the circle's center</param>
        /// <param name="centerY">The Y Coordinate of the circle's center</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleHole(double radius, double centerX, double centerY)
        {
            // Move the cursor a radius away from the center (only in x Axis) so it can be drawn by its left start (arcs drawing the circle)
            return MoveTo(centerX - radius, centerY).
            AddCircleHole(radius);
        }

        /// <summary>
        /// Adds a rectangle using the specified info Object
        /// </summary>
        /// <param name="rectangleInfo"></param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangle(RectangleInfo rectangleInfo)
        {
            return AddRectangle(
                rectangleInfo.Length,
                rectangleInfo.Height,
                rectangleInfo.LocationX,
                rectangleInfo.LocationY,
                rectangleInfo.TopLeftRadius,
                rectangleInfo.TopRightRadius,
                rectangleInfo.BottomLeftRadius,
                rectangleInfo.BottomRightRadius);
        }
        /// <summary>
        /// Adds a Rectangle Starting from the current Cursor Point
        /// </summary>
        /// <param name="length">The Length of the Rectangle</param>
        /// <param name="height">The Height of the Rectangle</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangle(double length, double height)
        {
            return
                AddLineRelative(length, 0)
                .AddLineRelative(0, height)
                .AddLineRelative(-length, 0)
                .AddLineRelative(0, -height)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Rectangle with the specified length , height and Center(x,y)
        /// </summary>
        /// <param name="length">The Length of the Rectangle</param>
        /// <param name="height">The Height of the Rectangle</param>
        /// <param name="centerX">The Center X of the Rectangle</param>
        /// <param name="centerY">The Center Y of the Rectangle</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangle(double length, double height, double centerX, double centerY)
        {
            return MoveTo(centerX - length / 2d, centerY - height / 2d).AddRectangle(length, height);
        }
        /// <summary>
        /// Adds a Rectangle with smooth Edges Starting from the current cursor Point
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="topLeftRadius">Top Left corner Radius</param>
        /// <param name="topRightRadius">Top Right corner Radius</param>
        /// <param name="bottomLeftRadius">Bottom Left corner Radius</param>
        /// <param name="bottomRightRadius">Bottom Right corner Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangle(double length, double height, double topLeftRadius = 0, double topRightRadius = 0, double bottomLeftRadius = 0, double bottomRightRadius = 0)
        {
            if (topLeftRadius == 0 && topRightRadius == 0 && bottomLeftRadius == 0 && bottomRightRadius == 0)
            {
                return AddRectangle(length, height);
            }
            #region Corners Rounding
            double lineUp = length - topLeftRadius - topRightRadius;
            double lineBottom = length - bottomLeftRadius - bottomRightRadius;
            double lineLeft = height - topLeftRadius - bottomLeftRadius;
            double lineRight = height - topRightRadius - bottomRightRadius;
            #endregion

            if (topLeftRadius != 0) MoveToRelative(topLeftRadius, 0);//move a little bit to pass the first corner radius

            AddLineRelative(lineUp, 0);
            if (topRightRadius != 0) AddArcRelative(topRightRadius, topRightRadius, topRightRadius, topRightRadius, 0, false, true);

            AddLineRelative(0, lineRight);
            if (bottomRightRadius != 0) AddArcRelative(bottomRightRadius, bottomRightRadius, -bottomRightRadius, bottomRightRadius, 0, false, true);

            AddLineRelative(-lineBottom, 0);
            if (bottomLeftRadius != 0) AddArcRelative(bottomLeftRadius, bottomLeftRadius, -bottomLeftRadius, -bottomLeftRadius, 0, false, true);

            AddLineRelative(0, -lineLeft);
            if (topLeftRadius != 0) AddArcRelative(topLeftRadius, topLeftRadius, topLeftRadius, -topLeftRadius, 0, false, true);
            CloseDraw();

            return this;
        }
        /// <summary>
        /// Adds a Rectangle with smooth Edges with the specified dimensions and Center(x,y)
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="centerX">The Center X of the Rectangle</param>
        /// <param name="centerY">The Center Y of the Rectangle</param>
        /// <param name="topLeftRadius">Top Left corner Radius</param>
        /// <param name="topRightRadius">Top Right corner Radius</param>
        /// <param name="bottomLeftRadius">Bottom Left corner Radius</param>
        /// <param name="bottomRightRadius">Bottom Right corner Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangle(double length, double height, double centerX, double centerY, double topLeftRadius = 0, double topRightRadius = 0, double bottomLeftRadius = 0, double bottomRightRadius = 0)
        {
            return MoveTo(centerX - length / 2d, centerY - height / 2d)
                .AddRectangle(length, height, topLeftRadius, topRightRadius, bottomLeftRadius, bottomRightRadius);
        }
        /// <summary>
        /// Adds a Rectangle with smooth Edges Starting from the current cursor Point
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="cornerRadius">The Radius in the Corners</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangle(double length, double height, double cornerRadius)
        {
            return AddRectangle(length, height, cornerRadius, cornerRadius, cornerRadius, cornerRadius);
        }
        /// <summary>
        /// Adds a Rectangle with smooth Edges with the specified dimensions and Center(x,y)
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="centerX">The Center X of the Rectangle</param>
        /// <param name="centerY">The Center Y of the Rectangle</param>
        /// <param name="cornerRadius">The Radius in the Corners</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangle(double length, double height, double centerX, double centerY, double cornerRadius)
        {
            return MoveTo(centerX - length / 2d, centerY - height / 2d).AddRectangle(length, height, cornerRadius);
        }

        /// <summary>
        /// Adds a rectangle Hole using the specified info
        /// </summary>
        /// <param name="rectangleInfo"></param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangleHole(RectangleInfo rectangleInfo)
        {
            return AddRectangleHole(
                rectangleInfo.Length,
                rectangleInfo.Height,
                rectangleInfo.LocationX,
                rectangleInfo.LocationY,
                rectangleInfo.TopLeftRadius,
                rectangleInfo.TopRightRadius,
                rectangleInfo.BottomLeftRadius,
                rectangleInfo.BottomRightRadius);
        }
        /// <summary>
        /// Adds a Rectangle Hole Starting from the Current Cursor Position
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangleHole(double length, double height)
        {
            return
                AddLineRelative(0, height)
                .AddLineRelative(length, 0)
                .AddLineRelative(0, -height)
                .AddLineRelative(-length, 0)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Rectangle Hole Starting with the specified center (x,y)
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="centerX">The Center X of the Rectangle</param>
        /// <param name="centerY">The Center Y of the Rectangle</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangleHole(double length, double height, double centerX, double centerY)
        {
            return MoveTo(centerX - length / 2d, centerY - height / 2d).AddRectangleHole(length, height);
        }
        /// <summary>
        /// Adds a smooth Rectangle Hole
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="topLeftRadius">Top Left corner Radius</param>
        /// <param name="topRightRadius">Top Right corner Radius</param>
        /// <param name="bottomLeftRadius">Bottom Left corner Radius</param>
        /// <param name="bottomRightRadius">Bottom Right corner Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangleHole(double length, double height, double topLeftRadius = 0, double topRightRadius = 0, double bottomLeftRadius = 0, double bottomRightRadius = 0)
        {
            if (topLeftRadius == 0 && topRightRadius == 0 && bottomLeftRadius == 0 && bottomRightRadius == 0)
            {
                return AddRectangleHole(length, height);
            }
            #region Corners Rounding
            double lineLeft = height - topLeftRadius - bottomLeftRadius;
            double lineBottom = length - bottomLeftRadius - bottomRightRadius;
            double lineRight = height - topRightRadius - bottomRightRadius;
            double lineUp = length - topLeftRadius - topRightRadius;
            #endregion

            if (topLeftRadius != 0) MoveToRelative(0, topLeftRadius);//move a little bit to pass the first corner radius

            AddLineRelative(0, lineLeft);
            if (bottomLeftRadius != 0) AddArcRelative(bottomLeftRadius, bottomLeftRadius, bottomLeftRadius, bottomLeftRadius, 0, false, false);

            AddLineRelative(lineBottom, 0);
            if (bottomRightRadius != 0) AddArcRelative(bottomRightRadius, bottomRightRadius, bottomRightRadius, -bottomRightRadius, 0, false, false);

            AddLineRelative(0, -lineRight);
            if (topRightRadius != 0) AddArcRelative(topRightRadius, topRightRadius, -topRightRadius, -topRightRadius, 0, false, false);

            AddLineRelative(-lineUp, 0);
            if (topLeftRadius != 0) AddArcRelative(topLeftRadius, topLeftRadius, -topLeftRadius, topLeftRadius, 0, false, false);
            CloseDraw();
            return this;
        }
        /// <summary>
        /// Adds a smooth Rectangle Hole with the specified center (x,y)
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="centerX">The Center X of the Rectangle</param>
        /// <param name="centerY">The Center Y of the Rectangle</param>
        /// <param name="topLeftRadius">Top Left corner Radius</param>
        /// <param name="topRightRadius">Top Right corner Radius</param>
        /// <param name="bottomLeftRadius">Bottom Left corner Radius</param>
        /// <param name="bottomRightRadius">Bottom Right corner Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangleHole(double length, double height, double centerX, double centerY, double topLeftRadius = 0, double topRightRadius = 0, double bottomLeftRadius = 0, double bottomRightRadius = 0)
        {
            return MoveTo(centerX - length / 2d, centerY - height / 2d).AddRectangleHole(length, height, topLeftRadius, topRightRadius, bottomLeftRadius, bottomRightRadius);
        }
        /// <summary>
        /// Adds a smooth Rectangle Hole
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="cornerRadius">The corner Radius of the edges of the Rectangle</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangleHole(double length, double height, double cornerRadius)
        {
            return AddRectangleHole(length, height, cornerRadius, cornerRadius, cornerRadius, cornerRadius);
        }
        /// <summary>
        /// Adds a smooth Rectangle Hole with the specified center (x,y)
        /// </summary>
        /// <param name="length">Length of the Rectangle</param>
        /// <param name="height">Height of the Rectangle</param>
        /// <param name="centerX">The Center X of the Rectangle</param>
        /// <param name="centerY">The Center Y of the Rectangle</param>
        /// <param name="cornerRadius">The corner Radius of the edges of the Rectangle</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRectangleHole(double length, double height, double centerX, double centerY, double cornerRadius)
        {
            return MoveTo(centerX - length / 2d, centerY - height / 2d).AddRectangleHole(length, height, cornerRadius);
        }

        /// <summary>
        /// Adds an Ellipse from the specified info object
        /// </summary>
        /// <param name="ellipseInfo"></param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddEllipse(EllipseInfo ellipseInfo)
        {
            return AddEllipse(ellipseInfo.GetTotalLength(), ellipseInfo.GetTotalHeight(), ellipseInfo.LocationX, ellipseInfo.LocationY);
        }
        /// <summary>
        /// Adds an Ellipse with the specified center and length - height
        /// </summary>
        /// <param name="length">The Length of the Ellipse</param>
        /// <param name="height">The Height of the Ellipse</param>
        /// <param name="centerX">The CenterX of the Ellipse</param>
        /// <param name="centerY">The CenterY of the Ellipse</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddEllipse(double length, double height, double centerX, double centerY)
        {
            // Move the cursor a radius away from the center (only in x Axis) so it can be drawn by its left start (arcs drawing the ellipse)
            return MoveTo(centerX - length / 2d, centerY).
            AddEllipse(length, height);
        }
        /// <summary>
        /// Adds an Ellipse starting from the current cursor position
        /// </summary>
        /// <param name="length">The Length of the Ellipse</param>
        /// <param name="height">The Height of the Ellipse</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddEllipse(double length, double height)
        {
            return AddArcRelative(length / 2d, height / 2d, length, 0, 0, false, true)
                    .AddArcRelative(length / 2d, height / 2d, -length, 0, 0, false, true)
                    .CloseDraw();
        }
        /// <summary>
        /// Adds an Ellipse Hole from the specified info object
        /// </summary>
        /// <param name="ellipseInfo"></param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddEllipseHole(EllipseInfo ellipseInfo)
        {
            return AddEllipseHole(ellipseInfo.GetTotalLength(), ellipseInfo.GetTotalHeight(), ellipseInfo.LocationX, ellipseInfo.LocationY);
        }
        /// <summary>
        /// Adds an Ellipse hole with the specified center and Length/Height values
        /// </summary>
        /// <param name="length">The Length of the Ellipse</param>
        /// <param name="height">The Height of the Ellipse</param>
        /// <param name="centerX">The CenterX of the Ellipse</param>
        /// <param name="centerY">The CenterY of the Ellipse</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddEllipseHole(double length, double height, double centerX, double centerY)
        {
            // Move the cursor a radius away from the center (only in x Axis) so it can be drawn by its left start (arcs drawing the ellipse)
            return MoveTo(centerX - length / 2d, centerY).
                   AddEllipseHole(length, height);
        }
        /// <summary>
        /// Adds an Ellipse Hole at the current cursor position
        /// </summary>
        /// <param name="length">The Length of the Ellipse</param>
        /// <param name="height">The Height of the Ellipse</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddEllipseHole(double length, double height)
        {
            if (length == height)
            {
                return AddCircleHole(length);
            }
            //horizontal or Vertical Ellipse
            else
            {
                return AddArcRelative(length / 2d, height / 2d, -length, 0, 0, false, false)
                    .AddArcRelative(length / 2d, height / 2d, length, 0, 0, false, false)
                    .CloseDraw();
            }
        }

        /// <summary>
        /// Adds a circle quadrant from the Specified info Object
        /// </summary>
        /// <param name="quadrant"></param>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public GraphicPathDataBuilder AddCircleQuadrant(CircleQuadrantInfo quadrant)
        {
            return quadrant.QuadrantPart switch
            {
                CircleQuadrantPart.TopLeft => AddTopLeftCircleQuadrant(quadrant.Radius, quadrant.LocationX, quadrant.LocationY),
                CircleQuadrantPart.TopRight => AddTopRightCircleQuadrant(quadrant.Radius, quadrant.LocationX, quadrant.LocationY),
                CircleQuadrantPart.BottomLeft => AddBottomLeftCircleQuadrant(quadrant.Radius, quadrant.LocationX, quadrant.LocationY),
                CircleQuadrantPart.BottomRight => AddBottomRightCircleQuadrant(quadrant.Radius, quadrant.LocationX, quadrant.LocationY),
                _ => throw new EnumValueNotSupportedException(quadrant.QuadrantPart),
            };
        }
        /// <summary>
        /// Adds a circle quadrant HOLE from the Specified info Object
        /// </summary>
        /// <param name="quadrant"></param>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public GraphicPathDataBuilder AddCircleQuadrantHole(CircleQuadrantInfo quadrant)
        {
            return quadrant.QuadrantPart switch
            {
                CircleQuadrantPart.TopLeft => AddTopLeftCircleQuadrantHole(quadrant.Radius, quadrant.LocationX, quadrant.LocationY),
                CircleQuadrantPart.TopRight => AddTopRightCircleQuadrantHole(quadrant.Radius, quadrant.LocationX, quadrant.LocationY),
                CircleQuadrantPart.BottomLeft => AddBottomLeftCircleQuadrantHole(quadrant.Radius, quadrant.LocationX, quadrant.LocationY),
                CircleQuadrantPart.BottomRight => AddBottomRightCircleQuadrantHole(quadrant.Radius, quadrant.LocationX, quadrant.LocationY),
                _ => throw new EnumValueNotSupportedException(quadrant.QuadrantPart),
            };
        }

        /// <summary>
        /// Adds the Top Left Circle Quadrant from a circle with the specified radius
        /// </summary>
        /// <param name="radius">The Circle's Radius</param>
        /// <returns></returns>
        /// 
        public GraphicPathDataBuilder AddTopLeftCircleQuadrant(double radius)
        {
            return AddArcRelative(radius, radius, radius, -radius, 0, false, true)
                .AddLineRelative(0, radius)
                .AddLineRelative(-radius, 0)
                .CloseDraw();
        }
        /// <summary>
        /// Adds the Top Left Circle Quadrant from a circle with the specified radius and Center
        /// </summary>
        /// <param name="radius">The Radius of the Circle</param>
        /// <param name="circleCenterX">The Center X of the Circle</param>
        /// <param name="circleCenterY">The Center Y of the Cirlce</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddTopLeftCircleQuadrant(double radius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - radius, circleCenterY).AddTopLeftCircleQuadrant(radius);
        }
        /// <summary>
        /// Adds a HOLE of the Top Left Circle Quadrant from a circle with the specified radius
        /// </summary>
        /// <param name="radius">The Circle's Radius</param>
        /// <returns></returns>
        /// 
        public GraphicPathDataBuilder AddTopLeftCircleQuadrantHole(double radius)
        {
            return AddLineRelative(radius, 0)
                .AddLineRelative(0, -radius)
                .AddArcRelative(radius, radius, -radius, radius, 0, false, false)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a HOLE of the Top Left Circle Quadrant from a circle with the specified radius and Center
        /// </summary>
        /// <param name="radius">The Radius of the Circle</param>
        /// <param name="circleCenterX">The Center X of the Circle</param>
        /// <param name="circleCenterY">The Center Y of the Cirlce</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddTopLeftCircleQuadrantHole(double radius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - radius, circleCenterY).AddTopLeftCircleQuadrantHole(radius);
        }

        /// <summary>
        /// Adds the Top Right Circle Quadrant from a circle with the specified radius
        /// </summary>
        /// <param name="radius">The Circle's Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddTopRightCircleQuadrant(double radius)
        {
            return AddLineRelative(0, -radius)
                .AddArcRelative(radius, radius, radius, radius, 0, false, true)
                .AddLineRelative(-radius, 0)
                .CloseDraw();
        }
        /// <summary>
        /// Adds the Top Right Circle Quadrant from a circle with the specified radius and Center
        /// </summary>
        /// <param name="radius">The Radius of the Circle</param>
        /// <param name="circleCenterX">The Center X of the Circle</param>
        /// <param name="circleCenterY">The Center Y of the Cirlce</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddTopRightCircleQuadrant(double radius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX, circleCenterY).AddTopRightCircleQuadrant(radius);
        }
        /// <summary>
        /// Adds a HOLE of the Top Right Circle Quadrant from a circle with the specified radius
        /// </summary>
        /// <param name="radius">The Circle's Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddTopRightCircleQuadrantHole(double radius)
        {
            return AddLineRelative(radius, 0)
                  .AddArcRelative(radius, radius, -radius, -radius, 0, false, false)
                  .AddLineRelative(0, radius)
                  .CloseDraw();
        }
        /// <summary>
        /// Adds a HOLE of the Top Right Circle Quadrant from a circle with the specified radius and Center
        /// </summary>
        /// <param name="radius">The Radius of the Circle</param>
        /// <param name="circleCenterX">The Center X of the Circle</param>
        /// <param name="circleCenterY">The Center Y of the Cirlce</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddTopRightCircleQuadrantHole(double radius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX, circleCenterY).AddTopRightCircleQuadrant(radius);
        }

        /// <summary>
        /// Adds the Bottom Right Circle Quadrant from a circle with the specified radius
        /// </summary>
        /// <param name="radius">The Circle's Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomRightCircleQuadrant(double radius)
        {
            return AddLineRelative(radius, 0)
                .AddArcRelative(radius, radius, -radius, radius, 0, false, true)
                .AddLineRelative(0, -radius)
                .CloseDraw();
        }
        /// <summary>
        /// Adds the Bottom Right Circle Quadrant from a circle with the specified radius and Center
        /// </summary>
        /// <param name="radius">The Radius of the Circle</param>
        /// <param name="circleCenterX">The Center X of the Circle</param>
        /// <param name="circleCenterY">The Center Y of the Cirlce</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomRightCircleQuadrant(double radius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX, circleCenterY).AddBottomRightCircleQuadrant(radius);
        }
        /// <summary>
        /// Adds a HOLE of the Bottom Right Circle Quadrant from a circle with the specified radius
        /// </summary>
        /// <param name="radius">The Circle's Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomRightCircleQuadrantHole(double radius)
        {
            return AddLineRelative(0, radius)
                .AddArcRelative(radius, radius, radius, -radius, 0, false, false)
                .AddLineRelative(-radius, 0)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a HOLE of the Bottom Right Circle Quadrant from a circle with the specified radius and Center
        /// </summary>
        /// <param name="radius">The Radius of the Circle</param>
        /// <param name="circleCenterX">The Center X of the Circle</param>
        /// <param name="circleCenterY">The Center Y of the Cirlce</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomRightCircleQuadrantHole(double radius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX, circleCenterY).AddBottomRightCircleQuadrantHole(radius);
        }

        /// <summary>
        /// Adds the Bottom Left Circle Quadrant from a circle with the specified radius
        /// </summary>
        /// <param name="radius">The Circle's Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomLeftCircleQuadrant(double radius)
        {
            return
                AddLineRelative(radius, 0)
                .AddLineRelative(0, radius)
                .AddArcRelative(radius, radius, -radius, -radius, 0, false, true)
                .CloseDraw();
        }
        /// <summary>
        /// Adds the Bottom Left Circle Quadrant from a circle with the specified radius and Center
        /// </summary>
        /// <param name="radius">The Radius of the Circle</param>
        /// <param name="circleCenterX">The Center X of the Circle</param>
        /// <param name="circleCenterY">The Center Y of the Cirlce</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomLeftCircleQuadrant(double radius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - radius, circleCenterY).AddBottomLeftCircleQuadrant(radius);
        }
        /// <summary>
        /// Adds a HOLE of the Bottom Left Circle Quadrant from a circle with the specified radius
        /// </summary>
        /// <param name="radius">The Circle's Radius</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomLeftCircleQuadrantHole(double radius)
        {
            return
                AddArcRelative(radius, radius, radius, radius, 0, false, false)
                .AddLineRelative(0, -radius)
                .AddLineRelative(-radius, 0)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a HOLE of the Bottom Left Circle Quadrant from a circle with the specified radius and Center
        /// </summary>
        /// <param name="radius">The Radius of the Circle</param>
        /// <param name="circleCenterX">The Center X of the Circle</param>
        /// <param name="circleCenterY">The Center Y of the Cirlce</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomLeftCircleQuadrantHole(double radius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - radius, circleCenterY).AddBottomLeftCircleQuadrantHole(radius);
        }

        /// <summary>
        /// Adds a Capsule from the specified info Object
        /// </summary>
        /// <param name="capsuleInfo"></param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCapsule(CapsuleInfo capsuleInfo)
        {
            return AddCapsule(capsuleInfo.GetTotalLength(), capsuleInfo.GetTotalHeight(), capsuleInfo.LocationX, capsuleInfo.LocationY);
        }
        /// <summary>
        /// Adds a Capsule with the specified length and height at the cursor position
        /// </summary>
        /// <param name="length">The Length of the Capsule</param>
        /// <param name="height">The Height of the Capsule</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCapsule(double length, double height)
        {
            //Horizontal
            if (length >= height)
            {
                double circleRadius = height / 2d;
                double rectLength = length - 2 * circleRadius;

                return AddArcRelative(circleRadius, circleRadius, circleRadius, -circleRadius, 0, false, true)
                    .AddLineRelative(rectLength, 0)
                    .AddArcRelative(circleRadius, circleRadius, 0, 2 * circleRadius, 0, false, true)
                    .AddLineRelative(-rectLength, 0)
                    .AddArcRelative(circleRadius, circleRadius, -circleRadius, -circleRadius, 0, false, true)
                    .CloseDraw();
            }
            //Vertical
            else
            {
                double circleRadius = length / 2d;
                double rectLength = height - 2 * circleRadius;
                return AddLineRelative(0, -rectLength / 2d)
                    .AddArcRelative(circleRadius, circleRadius, 2 * circleRadius, 0, 0, false, true)
                    .AddLineRelative(0, rectLength)
                    .AddArcRelative(circleRadius, circleRadius, -2 * circleRadius, 0, 0, false, true)
                    .AddLineRelative(0, -rectLength / 2d)
                    .CloseDraw();
            }
        }
        /// <summary>
        /// Adds a Capsule with the specified length , height and Center(x,y)
        /// </summary>
        /// <param name="length">The Length of the Capsule</param>
        /// <param name="height">The Height of the Capsule</param>
        /// <param name="centerX">The Center X of the Capsule</param>
        /// <param name="centerY">The Center Y of the Capsule</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCapsule(double length, double height, double centerX, double centerY)
        {
            return MoveTo(centerX - length / 2d, centerY).AddCapsule(length, height);
        }
        /// <summary>
        /// Adds a Capsule HOLE from the specified info Object
        /// </summary>
        /// <param name="capsuleInfo"></param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCapsuleHole(CapsuleInfo capsuleInfo)
        {
            return AddCapsuleHole(capsuleInfo.GetTotalLength(), capsuleInfo.GetTotalHeight(), capsuleInfo.LocationX, capsuleInfo.LocationY);
        }
        /// <summary>
        /// Adds a Capsule Hole with the specified length and height at the cursor position
        /// </summary>
        /// <param name="length">The Length of the Capsule</param>
        /// <param name="height">The Height of the Capsule</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCapsuleHole(double length, double height)
        {
            //Horizontal
            if (length >= height)
            {
                double circleRadius = height / 2d;
                double rectLength = length - 2 * circleRadius;

                return AddArcRelative(circleRadius, circleRadius, circleRadius, circleRadius, 0, false, false)
                    .AddLineRelative(rectLength, 0)
                    .AddArcRelative(circleRadius, circleRadius, 0, -2 * circleRadius, 0, false, false)
                    .AddLineRelative(-rectLength, 0)
                    .AddArcRelative(circleRadius, circleRadius, -circleRadius, circleRadius, 0, false, false)
                    .CloseDraw();
            }
            //Vertical
            else
            {
                double circleRadius = length / 2d;
                double rectLength = length - 2 * circleRadius;
                return AddLineRelative(0, rectLength / 2d)
                    .AddArcRelative(circleRadius, circleRadius, 2 * circleRadius, 0, 0, false, false)
                    .AddLineRelative(0, -rectLength)
                    .AddArcRelative(circleRadius, circleRadius, -2 * circleRadius, 0, 0, false, false)
                    .AddLineRelative(0, rectLength / 2d)
                    .CloseDraw();
            }
        }
        /// <summary>
        /// Adds a Capsule Hole with the specified length , height and Center(x,y)
        /// </summary>
        /// <param name="length">The Length of the Capsule</param>
        /// <param name="height">The Height of the Capsule</param>
        /// <param name="centerX">The Center X of the Capsule</param>
        /// <param name="centerY">The Center Y of the Capsule</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCapsuleHole(double length, double height, double centerX, double centerY)
        {
            return MoveTo(centerX - length / 2d, centerY).AddCapsuleHole(length, height);
        }

        /// <summary>
        /// Adds a Circle Segment from the specified info Object
        /// </summary>
        /// <param name="segmentInfo"></param>
        /// <param name="drawOnlyArc">Weather to only draw the Arc of the segment without the Chord (Open Draw)</param>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public GraphicPathDataBuilder AddCircleSegment(CircleSegmentInfo segmentInfo, bool drawOnlyArc = false)
        {
            if (segmentInfo.ChordLength == 0)
            {
                return AddCircle(segmentInfo.GetDefiningCircle());
            }
            else return segmentInfo.Orientation switch
            {
                CircleSegmentOrientation.PointingTop => AddCircleTopSegment(segmentInfo.ChordLength, segmentInfo.Sagitta, segmentInfo.LocationX, segmentInfo.LocationY, drawOnlyArc),
                CircleSegmentOrientation.PointingBottom => AddCircleBottomSegment(segmentInfo.ChordLength, segmentInfo.Sagitta, segmentInfo.LocationX, segmentInfo.LocationY, drawOnlyArc),
                CircleSegmentOrientation.PointingLeft => AddCircleLeftSegment(segmentInfo.ChordLength, segmentInfo.Sagitta, segmentInfo.LocationX, segmentInfo.LocationY, drawOnlyArc),
                CircleSegmentOrientation.PointingRight => AddCircleRightSegment(segmentInfo.ChordLength, segmentInfo.Sagitta, segmentInfo.LocationX, segmentInfo.LocationY, drawOnlyArc),
                _ => throw new EnumValueNotSupportedException(segmentInfo.Orientation),
            };
        }
        /// <summary>
        /// Adds a Circle Segment HOLE from the specified info Object
        /// </summary>
        /// <param name="segmentInfo"></param>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public GraphicPathDataBuilder AddCircleSegmentHole(CircleSegmentInfo segmentInfo)
        {
            return segmentInfo.Orientation switch
            {
                CircleSegmentOrientation.PointingTop => AddCircleTopSegmentHole(segmentInfo.ChordLength, segmentInfo.Sagitta, segmentInfo.LocationX, segmentInfo.LocationY),
                CircleSegmentOrientation.PointingBottom => AddCircleBottomSegmentHole(segmentInfo.ChordLength, segmentInfo.Sagitta, segmentInfo.LocationX, segmentInfo.LocationY),
                CircleSegmentOrientation.PointingLeft => AddCircleLeftSegmentHole(segmentInfo.ChordLength, segmentInfo.Sagitta, segmentInfo.LocationX, segmentInfo.LocationY),
                CircleSegmentOrientation.PointingRight => AddCircleRightSegmentHole(segmentInfo.ChordLength, segmentInfo.Sagitta, segmentInfo.LocationX, segmentInfo.LocationY),
                _ => throw new EnumValueNotSupportedException(segmentInfo.Orientation),
            };
        }

        /// <summary>
        /// Adds a Top Circle Segment with the specified Dimensions at the cursor position
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="drawOnlyArc">Weather to only draw the Arc of the segment without the Chord (Open Draw)</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleTopSegment(double chordLength, double sagitta, bool drawOnlyArc)
        {
            //check if length is greater than height to see weather the arc is the big or the small one
            var radius = MathCalculations.CircleSegment.GetRadius(chordLength, sagitta);
            var isBigArc = (radius < sagitta);
            if (double.IsNaN(radius)) radius = 0;

            AddArcRelative(radius, radius, chordLength, 0, 0, isBigArc, true);

            if (drawOnlyArc)
            {
                return this;
            }
            else
            {
                return AddLineRelative(-chordLength, 0).CloseDraw();
            }

        }
        /// <summary>
        /// Adds a Top Circle Segment with the specified Dimensions and the specified Chord's midpoint coordinates (x,y)
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="chordMidpointX">The X Coordinate of the Chord's midpoint</param>
        /// <param name="chordMidpointY">The Y Coordinate of the Chord's midpoint</param>
        /// <param name="drawOnlyArc">Weather to only draw the Arc of the segment without the Chord (Open Draw)</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleTopSegment(double chordLength, double sagitta, double chordMidpointX, double chordMidpointY, bool drawOnlyArc)
        {
            return MoveTo(chordMidpointX - chordLength / 2d, chordMidpointY).AddCircleTopSegment(chordLength, sagitta, drawOnlyArc);
        }
        /// <summary>
        /// Adds a Top Circle Segment Hole with the specified Dimensions at the cursor position
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleTopSegmentHole(double chordLength, double sagitta)
        {
            //check if length is greater than height to see weather the arc is the big or the small one
            var radius = MathCalculations.CircleSegment.GetRadius(chordLength, sagitta);
            var isBigArc = (radius < sagitta);
            if (double.IsNaN(radius)) radius = 0;

            return AddLineRelative(chordLength, 0)
                .AddArcRelative(radius, radius, -chordLength, 0, 0, isBigArc, false)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Top Circle Segment Hole with the specified Dimensions and the specified Chord's midpoint coordinates (x,y)
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="chordMidpointX">The X Coordinate of the Chord's midpoint</param>
        /// <param name="chordMidpointY">The Y Coordinate of the Chord's midpoint</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleTopSegmentHole(double chordLength, double sagitta, double chordMidpointX, double chordMidpointY)
        {
            return MoveTo(chordMidpointX - chordLength / 2d, chordMidpointY).AddCircleTopSegmentHole(chordLength, sagitta);
        }

        /// <summary>
        /// Adds a Right Circle Segment with the specified Dimensions at the cursor position
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="drawOnlyArc">Weather to only draw the Arc of the segment without the Chord (Open Draw)</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleRightSegment(double chordLength, double sagitta, bool drawOnlyArc)
        {
            //check if length is greater than height to see weather the arc is the big or the small one
            var radius = MathCalculations.CircleSegment.GetRadius(chordLength, sagitta);
            var isBigArc = (radius < sagitta);
            if (double.IsNaN(radius)) radius = 0;

            AddArcRelative(radius, radius, 0, chordLength, 0, isBigArc, true);

            if (drawOnlyArc)
            {
                return this;
            }
            else
            {
                return
                AddLineRelative(0, -chordLength)
                .CloseDraw();
            }
        }
        /// <summary>
        /// Adds a Right Circle Segment with the specified Dimensions and the specified Chord's midpoint coordinates (x,y)
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="chordMidpointX">The X Coordinate of the Chord's midpoint</param>
        /// <param name="chordMidpointY">The Y Coordinate of the Chord's midpoint</param>
        /// <param name="drawOnlyArc">Weather to only draw the Arc of the segment without the Chord (Open Draw)</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleRightSegment(double chordLength, double sagitta, double chordMidpointX, double chordMidpointY, bool drawOnlyArc)
        {
            return MoveTo(chordMidpointX, chordMidpointY - chordLength / 2d).AddCircleRightSegment(chordLength, sagitta, drawOnlyArc);
        }
        /// <summary>
        /// Adds a Right Circle Segment HOLE with the specified Dimensions at the cursor position
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleRightSegmentHole(double chordLength, double sagitta)
        {
            //check if length is greater than height to see weather the arc is the big or the small one
            var radius = MathCalculations.CircleSegment.GetRadius(chordLength, sagitta);
            var isBigArc = (radius < sagitta);
            if (double.IsNaN(radius)) radius = 0;

            return AddLineRelative(0, chordLength)
                .AddArcRelative(radius, radius, 0, -chordLength, 0, isBigArc, false)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Right Circle Segment HOLE with the specified Dimensions and the specified Chord's midpoint coordinates (x,y)
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="chordMidpointX">The X Coordinate of the Chord's midpoint</param>
        /// <param name="chordMidpointY">The Y Coordinate of the Chord's midpoint</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleRightSegmentHole(double chordLength, double sagitta, double chordMidpointX, double chordMidpointY)
        {
            return MoveTo(chordMidpointX, chordMidpointY - chordLength / 2d).AddCircleRightSegmentHole(chordLength, sagitta);
        }

        /// <summary>
        /// Adds a Bottom Circle Segment with the specified Dimensions at the cursor position
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="drawOnlyArc">Weather to only draw the Arc of the segment without the Chord (Open Draw)</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleBottomSegment(double chordLength, double sagitta, bool drawOnlyArc)
        {
            //check if length is greater than height to see weather the arc is the big or the small one
            var radius = MathCalculations.CircleSegment.GetRadius(chordLength, sagitta);
            var isBigArc = (radius < sagitta);
            if (double.IsNaN(radius)) radius = 0;

            if (drawOnlyArc)
            {
                return MoveToRelative(chordLength, 0).AddArcRelative(radius, radius, -chordLength, 0, 0, isBigArc, true);
            }
            else
            {
                return AddLineRelative(chordLength, 0)
                .AddArcRelative(radius, radius, -chordLength, 0, 0, isBigArc, true)
                .CloseDraw();
            }


        }
        /// <summary>
        /// Adds a Bottom Circle Segment with the specified Dimensions and the specified Chord's midpoint coordinates (x,y)
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="chordMidpointX">The X Coordinate of the Chord's midpoint</param>
        /// <param name="chordMidpointY">The Y Coordinate of the Chord's midpoint</param>
        /// <param name="drawOnlyArc">Weather to only draw the Arc of the segment without the Chord (Open Draw)</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleBottomSegment(double chordLength, double sagitta, double chordMidpointX, double chordMidpointY, bool drawOnlyArc)
        {
            return MoveTo(chordMidpointX - chordLength / 2d, chordMidpointY).AddCircleBottomSegment(chordLength, sagitta, drawOnlyArc);
        }
        /// <summary>
        /// Adds a Bottom Circle Segment HOLE with the specified Dimensions at the cursor position
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleBottomSegmentHole(double chordLength, double sagitta)
        {
            //check if length is greater than height to see weather the arc is the big or the small one
            var radius = MathCalculations.CircleSegment.GetRadius(chordLength, sagitta);
            var isBigArc = (radius < sagitta);
            if (double.IsNaN(radius)) radius = 0;

            return AddArcRelative(radius, radius, chordLength, 0, 0, isBigArc, false)
                .AddLineRelative(-chordLength, 0)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Bottom Circle Segment HOLE with the specified Dimensions and the specified Chord's midpoint coordinates (x,y)
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="chordMidpointX">The X Coordinate of the Chord's midpoint</param>
        /// <param name="chordMidpointY">The Y Coordinate of the Chord's midpoint</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleBottomSegmentHole(double chordLength, double sagitta, double chordMidpointX, double chordMidpointY)
        {
            return MoveTo(chordMidpointX - chordLength / 2d, chordMidpointY).AddCircleBottomSegmentHole(chordLength, sagitta);
        }

        /// <summary>
        /// Adds a Left Circle Segment with the specified Dimensions at the cursor position
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="drawOnlyArc">Weather to only draw the Arc of the segment without the Chord (Open Draw)</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleLeftSegment(double chordLength, double sagitta, bool drawOnlyArc)
        {
            //check if length is greater than height to see weather the arc is the big or the small one
            //var isBigArc = (chordLength < sagitta);
            //Not need to check for big arc here we draw it half and half is always less than 180deg

            var radius = MathCalculations.CircleSegment.GetRadius(chordLength, sagitta);
            if (double.IsNaN(radius)) radius = 0;

            if (drawOnlyArc)
            {
                return AddArcRelative(radius, radius, sagitta, -chordLength / 2d, 0, false, true)
                    .MoveToRelative(0, chordLength)
                    .AddArcRelative(radius, radius, -sagitta, -chordLength / 2d, 0, false, true);
            }
            else
            {
                return AddArcRelative(radius, radius, sagitta, -chordLength / 2d, 0, false, true)
                .AddLineRelative(0, chordLength)
                .AddArcRelative(radius, radius, -sagitta, -chordLength / 2d, 0, false, true)
                .CloseDraw();
            }
        }
        /// <summary>
        /// Adds a Left Circle Segment with the specified Dimensions and the specified Chord's midpoint coordinates (x,y)
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="chordMidpointX">The X Coordinate of the Chord's midpoint</param>
        /// <param name="chordMidpointY">The Y Coordinate of the Chord's midpoint</param>
        /// <param name="drawOnlyArc">Weather to only draw the Arc of the segment without the Chord (Open Draw)</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleLeftSegment(double chordLength, double sagitta, double chordMidpointX, double chordMidpointY, bool drawOnlyArc)
        {
            return MoveTo(chordMidpointX - sagitta, chordMidpointY).AddCircleLeftSegment(chordLength, sagitta, drawOnlyArc);
        }
        /// <summary>
        /// Adds a Left Circle Segment HOLE with the specified Dimensions at the cursor position
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleLeftSegmentHole(double chordLength, double sagitta)
        {
            //check if length is greater than height to see weather the arc is the big or the small one
            //var isBigArc = (chordLength < sagitta);
            //Not need to check for big arc here we draw it half and half is always less than 180deg

            var radius = MathCalculations.CircleSegment.GetRadius(chordLength, sagitta);
            if (double.IsNaN(radius)) radius = 0;

            return AddArcRelative(radius, radius, sagitta, chordLength / 2d, 0, false, false)
                .AddLineRelative(0, -chordLength)
                .AddArcRelative(radius, radius, -sagitta, chordLength / 2d, 0, false, false)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Left Circle Segment HOLE with the specified Dimensions and the specified Chord's midpoint coordinates (x,y)
        /// </summary>
        /// <param name="chordLength">The Length of the Chord Forming the segment</param>
        /// <param name="sagitta">The sagitta of the Segment</param>
        /// <param name="chordMidpointX">The X Coordinate of the Chord's midpoint</param>
        /// <param name="chordMidpointY">The Y Coordinate of the Chord's midpoint</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddCircleLeftSegmentHole(double chordLength, double sagitta, double chordMidpointX, double chordMidpointY)
        {
            return MoveTo(chordMidpointX - sagitta, chordMidpointY).AddCircleLeftSegmentHole(chordLength, sagitta);
        }

        /// <summary>
        /// Adds an Egg Shape from the specified info object
        /// </summary>
        /// <param name="eggInfo"></param>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public GraphicPathDataBuilder AddEggShape(EggShapeInfo eggInfo)
        {
            return eggInfo.Orientation switch
            {
                EggOrientation.VerticalPointingTop => AddTopEggShape(eggInfo.CircleRadius, eggInfo.EllipseRadius, eggInfo.LocationX, eggInfo.LocationY),
                EggOrientation.VerticalPointingBottom => AddBottomEggShape(eggInfo.CircleRadius, eggInfo.EllipseRadius, eggInfo.LocationX, eggInfo.LocationY),
                EggOrientation.HorizontalPointingRight => AddRightEggShape(eggInfo.CircleRadius, eggInfo.EllipseRadius, eggInfo.LocationX, eggInfo.LocationY),
                EggOrientation.HorizontalPointingLeft => AddLeftEggShape(eggInfo.CircleRadius, eggInfo.EllipseRadius, eggInfo.LocationX, eggInfo.LocationY),
                _ => throw new EnumValueNotSupportedException(eggInfo.Orientation),
            };
        }
        /// <summary>
        /// Adds an Egg Shape HOLE from the specified info object
        /// </summary>
        /// <param name="eggInfo"></param>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public GraphicPathDataBuilder AddEggShapeHole(EggShapeInfo eggInfo)
        {
            return eggInfo.Orientation switch
            {
                EggOrientation.VerticalPointingTop => AddTopEggShapeHole(eggInfo.CircleRadius, eggInfo.EllipseRadius, eggInfo.LocationX, eggInfo.LocationY),
                EggOrientation.VerticalPointingBottom => AddBottomEggShapeHole(eggInfo.CircleRadius, eggInfo.EllipseRadius, eggInfo.LocationX, eggInfo.LocationY),
                EggOrientation.HorizontalPointingRight => AddRightEggShapeHole(eggInfo.CircleRadius, eggInfo.EllipseRadius, eggInfo.LocationX, eggInfo.LocationY),
                EggOrientation.HorizontalPointingLeft => AddLeftEggShapeHole(eggInfo.CircleRadius, eggInfo.EllipseRadius, eggInfo.LocationX, eggInfo.LocationY),
                _ => throw new EnumValueNotSupportedException(eggInfo.Orientation),
            };
        }

        /// <summary>
        /// Adds a Top Egg Shape with the specified dimensions at the cursor position
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddTopEggShape(double circleRadius, double ellipseRadius)
        {
            return AddArcRelative(circleRadius, ellipseRadius, circleRadius * 2d, 0, 0, false, true)
                .AddArcRelative(circleRadius, circleRadius, -circleRadius * 2d, 0, 0, false, true)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Top Egg Shape with the specified dimensions and the specified center (x,y) of its circle part 
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <param name="circleCenterX">The Center X of the Egg's Circle Part</param>
        /// <param name="circleCenterY">The Center Y of the Egg's Circle Part</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddTopEggShape(double circleRadius, double ellipseRadius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - circleRadius, circleCenterY).AddTopEggShape(circleRadius, ellipseRadius);
        }
        /// <summary>
        /// Adds a Top Egg Shape Hole with the specified dimensions at the cursor position
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddTopEggShapeHole(double circleRadius, double ellipseRadius)
        {
            return AddArcRelative(circleRadius, circleRadius, circleRadius * 2d, 0, 0, false, false)
                  .AddArcRelative(circleRadius, ellipseRadius, -circleRadius * 2d, 0, 0, false, false)
                  .CloseDraw();
        }
        /// <summary>
        /// Adds a Top Egg Shape Hole with the specified dimensions and the specified center (x,y) of its circle part 
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <param name="circleCenterX">The Center X of the Egg's Circle Part</param>
        /// <param name="circleCenterY">The Center Y of the Egg's Circle Part</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddTopEggShapeHole(double circleRadius, double ellipseRadius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - circleRadius, circleCenterY).AddTopEggShapeHole(circleRadius, ellipseRadius);
        }


        /// <summary>
        /// Adds a Right Egg Shape with the specified dimensions at the cursor position
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRightEggShape(double circleRadius, double ellipseRadius)
        {
            return AddArcRelative(circleRadius, circleRadius, circleRadius, -circleRadius, 0, false, true)
                .AddArcRelative(ellipseRadius, circleRadius, 0, circleRadius * 2d, 0, false, true)
                .AddArcRelative(circleRadius, circleRadius, -circleRadius, -circleRadius, 0, false, true)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Right Egg Shape with the specified dimensions and the specified center (x,y) of its circle part 
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <param name="circleCenterX">The Center X of the Egg's Circle Part</param>
        /// <param name="circleCenterY">The Center Y of the Egg's Circle Part</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRightEggShape(double circleRadius, double ellipseRadius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - circleRadius, circleCenterY).AddRightEggShape(circleRadius, ellipseRadius);
        }
        /// <summary>
        /// Adds a Right Egg Shape HOLE with the specified dimensions at the cursor position
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRightEggShapeHole(double circleRadius, double ellipseRadius)
        {
            return AddArcRelative(circleRadius, circleRadius, circleRadius, circleRadius, 0, false, false)
                  .AddArcRelative(ellipseRadius, circleRadius, 0, -circleRadius * 2d, 0, false, false)
                  .AddArcRelative(circleRadius, circleRadius, -circleRadius, circleRadius, 0, false, false)
                  .CloseDraw();
        }
        /// <summary>
        /// Adds a Right Egg Shape HOLE with the specified dimensions and the specified center (x,y) of its circle part 
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <param name="circleCenterX">The Center X of the Egg's Circle Part</param>
        /// <param name="circleCenterY">The Center Y of the Egg's Circle Part</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddRightEggShapeHole(double circleRadius, double ellipseRadius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - circleRadius, circleCenterY).AddRightEggShapeHole(circleRadius, ellipseRadius);
        }

        /// <summary>
        /// Adds a Bottom Egg Shape with the specified dimensions at the cursor position
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomEggShape(double circleRadius, double ellipseRadius)
        {
            return AddArcRelative(circleRadius, circleRadius, circleRadius * 2d, 0, 0, false, true)
                .AddArcRelative(circleRadius, ellipseRadius, -circleRadius * 2d, 0, 0, false, true)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Bottom Egg Shape with the specified dimensions and the specified center (x,y) of its circle part 
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <param name="circleCenterX">The Center X of the Egg's Circle Part</param>
        /// <param name="circleCenterY">The Center Y of the Egg's Circle Part</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomEggShape(double circleRadius, double ellipseRadius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - circleRadius, circleCenterY).AddBottomEggShape(circleRadius, ellipseRadius);
        }
        /// <summary>
        /// Adds a Bottom Egg Shape HOLE with the specified dimensions at the cursor position
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomEggShapeHole(double circleRadius, double ellipseRadius)
        {
            return AddArcRelative(circleRadius, ellipseRadius, circleRadius * 2d, 0, 0, false, false)
                .AddArcRelative(circleRadius, circleRadius, -circleRadius * 2d, 0, 0, false, false)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Bottom Egg Shape HOLE with the specified dimensions and the specified center (x,y) of its circle part 
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <param name="circleCenterX">The Center X of the Egg's Circle Part</param>
        /// <param name="circleCenterY">The Center Y of the Egg's Circle Part</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddBottomEggShapeHole(double circleRadius, double ellipseRadius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - circleRadius, circleCenterY).AddBottomEggShapeHole(circleRadius, ellipseRadius);
        }

        /// <summary>
        /// Adds a Left Egg Shape with the specified dimensions at the cursor position
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddLeftEggShape(double circleRadius, double ellipseRadius)
        {
            return AddArcRelative(ellipseRadius, circleRadius, ellipseRadius, -circleRadius, 0, false, true)
                .AddArcRelative(circleRadius, circleRadius, 0, circleRadius * 2d, 0, false, true)
                .AddArcRelative(ellipseRadius, circleRadius, -ellipseRadius, -circleRadius, 0, false, true)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Left Egg Shape with the specified dimensions and the specified center (x,y) of its circle part 
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <param name="circleCenterX">The Center X of the Egg's Circle Part</param>
        /// <param name="circleCenterY">The Center Y of the Egg's Circle Part</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddLeftEggShape(double circleRadius, double ellipseRadius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - ellipseRadius, circleCenterY).AddLeftEggShape(circleRadius, ellipseRadius);
        }
        /// <summary>
        /// Adds a Left Egg Shape HOLE with the specified dimensions at the cursor position
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddLeftEggShapeHole(double circleRadius, double ellipseRadius)
        {
            return AddArcRelative(ellipseRadius, circleRadius, ellipseRadius, circleRadius, 0, false, false)
                .AddArcRelative(circleRadius, circleRadius, 0, -circleRadius * 2d, 0, false, false)
                .AddArcRelative(ellipseRadius, circleRadius, -ellipseRadius, circleRadius, 0, false, false)
                .CloseDraw();
        }
        /// <summary>
        /// Adds a Left Egg Shape HOLE with the specified dimensions and the specified center (x,y) of its circle part 
        /// </summary>
        /// <param name="circleRadius">The Radius of the circle part of the Egg</param>
        /// <param name="ellipseRadius">The Radius of the Ellipse part of the Egg</param>
        /// <param name="circleCenterX">The Center X of the Egg's Circle Part</param>
        /// <param name="circleCenterY">The Center Y of the Egg's Circle Part</param>
        /// <returns></returns>
        public GraphicPathDataBuilder AddLeftEggShapeHole(double circleRadius, double ellipseRadius, double circleCenterX, double circleCenterY)
        {
            return MoveTo(circleCenterX - ellipseRadius, circleCenterY).AddLeftEggShapeHole(circleRadius, ellipseRadius);
        }

        /// <summary>
        /// Adds a Polygon from the specified Polygon Info Object
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public GraphicPathDataBuilder AddPolygon(PolygonInfo polygon)
        {
            var verticesNo = polygon.Vertices.Count;
            if (verticesNo <= 2) throw new ArgumentException($"A {nameof(PolygonInfo)} cannot have less than 3 vertices");

            var firstVertex = polygon.Vertices[0];
            MoveTo(firstVertex.X, firstVertex.Y);

            //Exclude the first vertice already moved there , When in the Last vertice will close the Draw Afterwards
            for (int i = 1; i <= verticesNo - 1; i++)
            {
                var currentVertex = polygon.Vertices[i];
                AddLine(currentVertex.X, currentVertex.Y);
            }
            return CloseDraw();
        }

        /// <summary>
        /// Adds a data string to the end of the current path Data
        /// </summary>
        /// <param name="data"></param>
        public GraphicPathDataBuilder AddExternalPathData(string data)
        {
            builder.Append(data);
            return this;
        }
        /// <summary>
        /// Returns the Drawn Path Data
        /// </summary>
        /// <returns></returns>
        public string GetPathData()
        {
            return builder.ToString().Replace(',', '.');
        }
    }
}
