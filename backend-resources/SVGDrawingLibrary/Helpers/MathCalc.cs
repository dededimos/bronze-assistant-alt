using SVGDrawingLibrary.Enums;
using SVGDrawingLibrary.Models;
using SVGDrawingLibrary.Models.ConcreteShapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGDrawingLibrary.Helpers
{
    public static class MathCalc
    {

        #region 0.Get Points Coordinates/Properties Methods
        /// <summary>
        /// Get a point (x,y) inside a circle by defining the Center - Distance - Angle , Cartesian system moves Clockwise
        /// </summary>
        /// <param name="centerX">Center X of Circle</param>
        /// <param name="centerY">Center Y of Circle</param>
        /// <param name="distanceFromCenter">distance from circle</param>
        /// <param name="angle">The angle of the Line Forming from center to the Point Always measured clockwise starting from right side of Circle</param>
        /// <returns>a point x,y inside the Circle</returns>
        public static (double, double) GetPointInsideCircle(double centerX, double centerY, double distanceFromCenter, double angle)
        {
            //Get the distances with Cosinus and Sinus Definitions
            //Get X -- the relative position is the one if center was 0,0
            double relativePositionX = Math.Cos(angle) * distanceFromCenter;
            double x = relativePositionX + centerX;

            //Get Y -- the relative position is the one if center was 0,0
            double relativePositionY = Math.Sin(angle) * distanceFromCenter;
            double y = relativePositionY + centerY;

            return (x, y);
        }

        /// <summary>
        /// Returns the (X,Y) on the Circles Perimeter
        /// </summary>
        /// <param name="radius">The Radius of the circle</param>
        /// <param name="Cx">The X Coordinate of the Circles Center</param>
        /// <param name="Cy">The Y Coordinate of the Circles Center</param>
        /// <param name="angle">The Angle in Radians of the arc that leads to the Point , 0radians is Right Center of Circle</param>
        /// <returns>The XY point on the Circle</returns>
        public static (double,double) GetPointCoordinatesInCirclePerimeter(double radius,double Cx , double Cy , double angle)
        {
            double x = Cx + radius * Math.Cos(angle);
            double y = Cy + radius * Math.Sin(angle);
            return (x, y);
        }

        /// <summary>
        /// Returns the Point(X,Y) on the Circles Perimeter
        /// </summary>
        /// <param name="radius">The Radius of the circle</param>
        /// <param name="Cx">The X Coordinate of the Circles Center</param>
        /// <param name="Cy">The Y Coordinate of the Circles Center</param>
        /// <param name="angle">The Angle in Radians of the arc that leads to the Point , 0radians is Right Center of Circle</param>
        /// <returns>The Point(X,Y) on the Circle's Perimeter</returns>
        public static DrawPoint GetPointInCirclePerimeter(double radius ,double Cx , double Cy , double angle)
        {
            (double x , double y) = GetPointCoordinatesInCirclePerimeter(radius, Cx , Cy , angle);
            DrawPoint point = new(x,y);
            return point;
        }

        /// <summary>
        /// Returns the List of Points for the Inscribed Polygon Sides
        /// </summary>
        /// <param name="circle">The Circle</param>
        /// <param name="polygonSides">The PolygonSides</param>
        /// <returns>The List of Points of the Polygon Inscribed to the Circle</returns>
        public static List<DrawPoint> GetInscribedPolygonPoints(this CircleDraw circle , int polygonSides)
        {
            if (polygonSides <=2 )
            {
                throw new ArgumentOutOfRangeException($"{nameof(polygonSides)}","An Inscribed Circle Polygon must have more than 2 sides");
            }
            List<DrawPoint> points = new();
            //Arc Angle Per Polygon Side
            double angle = Math.PI * 2 / polygonSides;

            for (double i = 0; i < 2 * Math.PI; i += angle)
            {
                DrawPoint point = new();
                (point.X,point.Y) = GetPointCoordinatesInCirclePerimeter(circle.Radius,circle.CenterX,circle.CenterY,i);
                points.Add(point);
            }
            return points;
        }

        /// <summary>
        /// /// <summary>
        /// Calculates the a,b Factors from a Line Equation (y=ax+b) or ax + by +c = 0 for the more general equation
        /// </summary>
        /// <param name="x1">X of a point in the line</param>
        /// <param name="y1">Y of a point in the line</param>
        /// <param name="x2">X of a second point in the line</param>
        /// <param name="y2">Y of a second point in the line</param>
        /// <returns>a,b factors -- or NaN when x1=x2 </returns>
        public static (double, double) GetLineFactorsAB(double x1, double y1, double x2, double y2)
        {
            double a;
            double b;
            //The Line Equation is Y=aX+b so it must be true for both points , so we have to solve a system of 2 equations with 2 variables a,b
            if (x1 != x2)
            {
                a = (y2 - y1) / (x2 - x1); //Solving with the two given points
                b = y1 - a * x1;
            }
            else //When the Line is X=x1 or X=x2
            {
                a = double.NaN;
                b = double.NaN;
            }

            return (a, b);
        }

        /// <summary>
        /// Gets the Slope of the Line (y=mx+b) where m=Slope , tanθ = m , where θ=Line Inclination (Angle with X axis)
        /// </summary>
        /// <param name="x1">The X Coordinate of a point in the Line </param>
        /// <param name="y1">The Y Coordinate of a point in the Line </param>
        /// <param name="x2">The X Coordinate of another point in the Line </param>
        /// <param name="y2">The Y Coordinate of a another point in the Line </param>
        /// <returns>The Slope or double.NaN</returns>
        public static double GetLineSlope(double x1, double y1, double x2, double y2)
        {
            double slope;
            if (x1 == x2)
            {
                slope = double.NaN; //The Slope is Undefined
            }
            else if (y1 == y2)
            {
                slope = 0;
            }
            else
            {
                slope = (y2 - y1) / (x2 - x1);
            }
            return slope;
        }

        /// <summary>
        /// Get the two Points on a Perpendicular to a given Line (Xorigin,Yorigin)-(X2,Y2) at a certain Distance away from origin
        /// </summary>
        /// <param name="lineXorigin">X origin Point of the Line</param>
        /// <param name="lineYorigin">Y origin Point of the Line</param>
        /// <param name="lineX2">X2 of another Point on the Line</param>
        /// <param name="lineY2">Y2 of another Point on the Line</param>
        /// <param name="distanceFromOrigin">The Distance of X1Y1 from the point on the prependicular Line</param>
        /// <returns>The Two Points of the Perpendicular Line ,which have a certain distance from the Origin Point of the Origin Line</returns>
        public static (DrawPoint, DrawPoint) GetPointsOnPerpendicular(double lineXorigin, double lineYorigin, double lineX2, double lineY2, double distanceFromOrigin)
        {
            DrawPoint p1 = new();
            DrawPoint p2 = new();

            //First Find the Equation that represents our main Line

            //1.Find the Slope of the Given Line
            double m = GetLineSlope(lineXorigin, lineYorigin, lineX2, lineY2);

            // If the Slope is undefined it means X1=X2 so our line is Vertical
            // The perpendicular to our Vertical Line is the horizontal line that passes from Xorigin Yorigin so :
            if (double.IsNaN(m))
            {
                p1.Y = lineYorigin; // The Horizontal Line Passing through the Point Y1 
                p2.Y = lineYorigin;

                p1.X = lineXorigin + distanceFromOrigin; //Front Point
                p2.X = lineXorigin - distanceFromOrigin; //Back Point
            }
            // The Slope is 0 meaning our Given Line is Horizontal
            // The perpendicular to our Horizontal Line is the Vertical that passes from Xorigin Yorigin
            else if (m == 0)
            {
                p1.X = lineXorigin;
                p2.X = lineXorigin;

                p1.Y = lineYorigin + distanceFromOrigin;
                p2.Y = lineYorigin - distanceFromOrigin;
            }
            // We need to Find the Equation of the Perpendicular which has a Slope of -1/m (if m is the Slope of the Given Line)
            // The point-Slope Formula of a line is : y-y1 = m(x-x1) , So because our line has a slope of "m" the perpendicular has -1/m
            // So--> the Formula of the Perpendicular will be : y-y1 = -1/m(x-x1) ---- where x1,y1 here is our origin point
            // Finally we need to find on this Line the Intersections that it has with a -circle- or radius = distance and CenterX = Xorigin , CenterY = Yorigin
            // The Circles formula will be : (x-x1)^2 + (y-y1)^2 = d^2 --- where (x1,y1) = (Xorigin,Yorigin) the circles center and d=distance the circles Radius
            // Solving the System of these two equations gives us the following result : 
            // x = x1 +- SQRT(d^2/(1+1/m^2))
            // two solution for each x we (plugging those two x in the line equation of the perpendicular we get also the y)
            else
            {
                //from x = x1 +- SQRT(d^2/(1+1/m^2))
                p1.X = lineXorigin + Math.Sqrt(Math.Pow(distanceFromOrigin, 2) / (1 + (1 / Math.Pow(m, 2))));
                //from y-y1 = -1/m(x-x1)
                p1.Y = (-1 / m) * (p1.X - lineXorigin) + lineYorigin;

                //For the Second Point
                p2.X = lineXorigin - Math.Sqrt(Math.Pow(distanceFromOrigin, 2) / (1 + (1 / Math.Pow(m, 2))));
                p2.Y = (-1 / m) * (p2.X - lineXorigin) + lineYorigin;
            }
            return (p1, p2);
        }

        /// <summary>
        /// Gets a Point on a defined Line at a certain Distance from the OriginCoordinates and Out of the Defined Bounds
        /// The Defined Bounds is the Line defined only between the points (lineXorigin,lineYorigin) , (lineX2,lineY2)
        /// </summary>
        /// <param name="Xorigin">X of the Origin Point on the Line</param>
        /// <param name="Yorigin">Y of the Origin Point on the Line</param>
        /// <param name="X2">X of the other boundary Point of the Line</param>
        /// <param name="Y2">Y of the other boundary Point of the Line</param>
        /// <param name="distanceFromOrigin">Distance of the Origin Point to the Searched Point</param>
        /// <param name="inBounds">If the Point we are Searching is inside the Bounds set (xOrigin,yOrigin)-(x2,y2)</param>
        /// <returns>The Point that is on the Same Line , but out of the Defined Bounds</returns>
        public static DrawPoint GetPointOfLine(double Xorigin, double Yorigin, double X2, double Y2, double distanceFromOrigin, bool inBounds = true)
        {
            DrawPoint point = new();
            //Get the Slope of the Given Line
            double m = GetLineSlope(Xorigin, Yorigin, X2, Y2);

            if (double.IsNaN(m)) // Then the Slope is Undefined and our Line is Vertical X=Xorigin=X2
            {
                //The point we are searching has the same X as the rest of the points in our Line
                point.X = Xorigin;

                //We get two Solutions for the Y of the Searched Point
                double solution1Y = Yorigin + distanceFromOrigin;
                double solution2Y = Yorigin - distanceFromOrigin;
                //To get the Point Out of Bounds we must Get the solution that gives a Y Greater than both "Ys" or Less than both "Ys"
                if (Math.Max(solution1Y, Math.Max(Yorigin, Y2)) == solution1Y ||  //If bigger than both
                    Math.Min(solution1Y, Math.Min(Yorigin, Y2)) == solution1Y)    //If less than both
                {
                    point.Y = inBounds ? solution2Y : solution1Y; //If we need the in Bounds Point then Solution1 is not correct
                }
                else
                {
                    point.Y = inBounds ? solution1Y : solution2Y; //If we need the in Bounds Point then Solution2 is not correct
                }
            }
            else if (m == 0) //Then the Slope is 0 and our Line is the Horizontal Y=Yorigin=Y2
            {
                //The point we are searching has the same Y as the rest of the points in our Line
                point.Y = Yorigin;

                //We get two Solutions for the X of the Searched Point
                double solution1X = Xorigin + distanceFromOrigin;
                double solution2X = Xorigin - distanceFromOrigin;
                //To get the Point Out of Bounds we must Get the solution that gives an X Greater than both "Xs" or Less than both "Xs"
                if (Math.Max(solution1X, Math.Max(Xorigin, X2)) == solution1X ||  //If bigger than both
                    Math.Min(solution1X, Math.Min(Xorigin, X2)) == solution1X)    //If less than both
                {
                    point.X = inBounds ? solution2X : solution1X;
                }
                else
                {
                    point.X = inBounds ? solution1X : solution2X;
                }
            }
            else
            {
                // We need to Find the New Point using the Circle Formula (x-x1)^2 + (y-y1)^2 = d^2 (Center is Xorigin,Yorigin and Radius is Distance)
                // and using the Line slope-point formula  y-y1 = m(x-x1)
                // Solving this system of equations we get : x = Xorigin +- d/SQRT(1+M^2)
                double solution1X = Xorigin + distanceFromOrigin / Math.Sqrt(1 + Math.Pow(m, 2));
                double solution2X = Xorigin - distanceFromOrigin / Math.Sqrt(1 + Math.Pow(m, 2));

                //As Above we get the point with out of boundsX
                //To get the Point Out of Bounds we must Get the solution that gives an X Greater than both "Xs" or Less than both "Xs"
                if (Math.Max(solution1X, Math.Max(Xorigin, X2)) == solution1X ||  //If bigger than both
                    Math.Min(solution1X, Math.Min(Xorigin, X2)) == solution1X)    //If less than both
                {
                    point.X = inBounds ? solution2X : solution1X;
                }
                else
                {
                    point.X = inBounds ? solution1X : solution2X;
                }
                //and finally find als othe Y from y-y1 = m(x-x1)
                point.Y = m * (point.X - Xorigin) + Yorigin;
            }
            return point;
        }

        /// <summary>
        /// Gets the Squared Distance between two Points dx*dx + dy * dy (This way we will not use SQRT anywhere
        /// </summary>
        /// <param name="point1X">The X Coordinate of the First Point</param>
        /// <param name="point1Y">The Y Coordinate of the First Point</param>
        /// <param name="point2X">The X Coordinate of the Second Point</param>
        /// <param name="point2Y">The Y Coordinate of the Second Point</param>
        /// <returns>The Squared Distance between the two points (dx^2 + dy^2) = d^2</returns>
        public static double GetPointsSquaredDistance(double point1X, double point1Y, double point2X, double point2Y)
        {
            double dx = point1X - point2X;
            double dy = point1Y - point2Y;

            double squaredDistance = Math.Pow(dx, 2) + Math.Pow(dy, 2);
            return squaredDistance;
        }

        /// <summary>
        /// Gets the Squared Distance between two Points dx*dx + dy * dy (This way we will not use SQRT anywhere
        /// </summary>
        /// <param name="point1">The First Point</param>
        /// <param name="point2">The Second Point</param>
        /// <returns>The Squared Distance between the two points (dx^2 + dy^2) = d^2</returns>
        public static double GetPointsSquaredDistance(DrawPoint point1, DrawPoint point2)
        {
            return GetPointsSquaredDistance(point1.X, point1.Y, point2.X, point2.Y);
        }

        /// <summary>
        /// Returns the Distance between two points , It is advisable to use Squared Distance Method instead of this when Doable.
        /// </summary>
        /// <param name="point1X">The X coordinate of Point1</param>
        /// <param name="point1Y">The Y coordinate of Point1</param>
        /// <param name="point2X">The X coordinate of Point2</param>
        /// <param name="point2Y">The Y coordinate of Point2</param>
        /// <returns></returns>
        /// <exception cref="NotFiniteNumberException"></exception>
        public static double GetDistanceBetweenPoints(double point1X, double point1Y, double point2X, double point2Y)
        {
            double squaredDistance = GetPointsSquaredDistance(point1X, point1Y, point2X, point2Y);

            //This cannot happen but just in case
            if (squaredDistance < 0)
            {
                throw new NotFiniteNumberException("Not Allowed to use SQRT on a negative number");
            }
            else
            {
                double distance = Math.Sqrt(squaredDistance);
                return distance;
            }
        }

        /// <summary>
        /// Returns the Length of a Circle's Cord , given the Circle's Radius and distance of the Chord from the Center
        /// </summary>
        /// <param name="circlesRadius">Circles Center</param>
        /// <param name="chordDistanceFromCenter">Distance of the Chord from the Circles Center (Perpendicular)</param>
        /// <returns>The Length of the Chord or NaN when distance is Greater than Radius</returns>
        public static double GetCircleChordLength(double circlesRadius, double chordDistanceFromCenter)
        {
            if (chordDistanceFromCenter > circlesRadius)
            {
                return double.NaN;
            }
            double length = 2 * Math.Sqrt(Math.Pow(circlesRadius, 2) - Math.Pow(chordDistanceFromCenter, 2));
            return length;
        }

        /// <summary>
        /// Checks wheather a point(x,y) is inside or on The Perimeter of an Ellipse
        /// </summary>
        /// <param name="ellipseCenterX">The Cx coordinate of the Ellipse Center</param>
        /// <param name="ellipseCenterY">The Cy coordinate of the Ellipse Center</param>
        /// <param name="ellipseRx">The rx Radius of the Ellipse</param>
        /// <param name="ellipseRy">The ry Radius of the Ellipse </param>
        /// <param name="pointX">The X Coordinate of the Point</param>
        /// <param name="pointY">The Y Coordinate of the Point</param>
        /// <param name="isPerimeterIncluded">If true point on the perimeter are considered inside the Elipse</param>
        /// <returns>True if the Point is inside the Ellipse -- Flase if it is outside</returns>
        public static bool IsPointInsideEllipse(double ellipseCenterX, double ellipseCenterY, double ellipseRx, double ellipseRy, double pointX, double pointY, bool isPerimeterIncluded = false)
        {
            /*The Bounding Elipse Disk is represented by the following eniquality 
             [ ( (X-Cx)^2 ) / rx^2 + ((Y-Cy)^2) / ry^2 ] <= 1
             So Any Point that confirms this is either inside or on the perimeter when equal to 1
             being inside the elipse
             */

            // [(X-Cx)^2 / rx^2 ]
            double termX = Math.Pow(pointX - ellipseCenterX, 2) / Math.Pow(ellipseRx, 2);
            // [(Y-Cy)^2 / ry^2 ]
            double termY = Math.Pow(pointY - ellipseCenterY, 2) / Math.Pow(ellipseRy, 2);
            double result = termX + termY;

            //Return wheather its true or not
            if (isPerimeterIncluded)
            {
                return (result <= 1);
            }
            else
            {
                return (result < 1);
            }

        }

        /// <summary>
        /// Returns wheather a Point is Inside the Ellipse
        /// </summary>
        /// <param name="ellipse">The Ellipse</param>
        /// <param name="pointX">The X Coordinate of the Point</param>
        /// <param name="pointY">The Y Coordinate of the Point</param>
        /// <param name="isPerimeterIncluded">If True If the point is on the perimeter of the Ellipse will be considered Inside it</param>
        /// <returns>True if the Point is inside the Ellipse , False if it is not</returns>
        public static bool IsPointInsideEllipse(this EllipseDraw ellipse, double pointX, double pointY, bool isPerimeterIncluded = false)
        {
            return IsPointInsideEllipse(ellipse.ShapeCenterX, ellipse.ShapeCenterY, ellipse.Length / 2d, ellipse.Height / 2d, pointX, pointY, isPerimeterIncluded);
        }

        /// <summary>
        /// Gets a point in the Ellipse perimeter at the Defined Angle (Angle 0 is Right Center)
        /// </summary>
        /// <param name="ellipse">The Ellipse</param>
        /// <param name="angle">The Angle in Radians , 0radians in Right Center</param>
        /// <returns>The Point on the Ellipse Perimeter at the Defined Angle</returns>
        public static DrawPoint GetPointOnEllipsePerimeter(this EllipseDraw ellipse , double angle)
        {
            if (ellipse.Length <= 0 || ellipse.Height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ellipse), "The Length or Height of the Ellipse Cannot be Zero");
            }
            double a = ellipse.Length /2d;
            double b = ellipse.Height / 2d;

            double term1y = a * b * Math.Sin(angle);
            double term2y = Math.Sqrt(Math.Pow(b * Math.Cos(angle),2) + Math.Pow(a * Math.Sin(angle), 2));

            double term1x = a * b * Math.Cos(angle);
            double term2x = Math.Sqrt(Math.Pow(b * Math.Cos(angle), 2) + Math.Pow(a * Math.Sin(angle), 2));

            double relativeY = term1y / term2y;
            double relativeX = term1x / term2x;

            double x = ellipse.ShapeCenterX + relativeX;
            double y = ellipse.ShapeCenterY + relativeY;

            return new DrawPoint(x, y);
            
        }

        /// <summary>
        /// Gets the Y coordinate Solutions of the Ellipse Equation
        /// </summary>
        /// <param name="X">The X Coordinate of the Point</param>
        /// <param name="Cx">The CenterX of the Ellipse</param>
        /// <param name="Cy">The CenterY of the Ellipse</param>
        /// <param name="a">The radius on Xaxis of the Ellipse</param>
        /// <param name="b">The radius on Yaxis of the Ellipse</param>
        /// <returns>2 Solutions when X is not on the Tops of the Ellipse (LTRB), 1 Solution when X on the Tops(LTRB) , NaN when X is outside the Ellipse </returns>
        public static (double, double) GetYSolutionOfEllipseEquation(double X, double Cx, double Cy, double a, double b)
        {
            double a2 = Math.Pow(a, 2);
            double b2 = Math.Pow(b, 2);
            //Ellipse Equation : b^2 * (X - Cx)^2 + a^2 * (Y - Cy)^2 =  a^2 * b^2
            //Rearrange to a Quadratic Equation
            //a^2 * Y^2 - 2 * Y * Cy * a^2 + a^2 * Cy^2 + b^2 * (X-Cx)^2 - a^2 * b^2 = 0

            //Terms of the Quadratic Equation Ax^2 + Bx + C = 0
            double A = a2;
            double B = -2 * Cy * a2;
            double C = a2 * Math.Pow(Cy, 2) + b2 * Math.Pow((X - Cx), 2) - a2 * b2;

            (double y1, double y2) = SolveQuadraticEquation(A, B, C);

            return (y1, y2);
        }

        /// <summary>
        /// Gets the X coordinate Solutions of the Ellipse Equation
        /// </summary>
        /// <param name="Y">The Y Coordinate of the Point</param>
        /// <param name="Cx">The CenterX of the Ellipse</param>
        /// <param name="Cy">The CenterY of the Ellipse</param>
        /// <param name="a">The radius on Xaxis of the Ellipse</param>
        /// <param name="b">The radius on Yaxis of the Ellipse</param>
        /// <returns>2 Solutions when Y is not on the Edges, 1 Solution when Y on the Edges , NaN when Y is outside the Ellipse </returns>
        public static (double, double) GetXSolutionOfEllipseEquation(double Y, double Cx, double Cy, double a, double b)
        {
            double a2 = Math.Pow(a, 2);
            double b2 = Math.Pow(b, 2);
            //Ellipse Equation : b^2 * (X - Cx)^2 + a^2 * (Y - Cy)^2 =  a^2 * b^2
            //Rearrange to a Quadratic Equation
            //b^2*X^2 - 2 * X * Cx * b^2 + b^2 * Cx^2 + a^2 * (Y-Cy)^2 - a^2 * b^2 = 0

            //Terms of the Quadratic Equation Ax^2 + Bx + C = 0
            double A = b2;
            double B = -2 * Cx * b2;
            double C = b2 * Math.Pow(Cx, 2) + a2 * Math.Pow((Y - Cy), 2) - a2 * b2;

            (double x1, double x2) = SolveQuadraticEquation(A, B, C);

            return (x1, x2);
        }

        #endregion

        #region 1.Intersections

        /// <summary>
        /// Checks if two Rectangles Collide
        /// </summary>
        /// <param name="shape1">The First Rectangle</param>
        /// <param name="shape2">The Second Rectangle</param>
        /// <returns>True if they Collide -- False if they Dont</returns>
        private static bool AreIntersectingRectangles(RectangleDraw shape1, RectangleDraw shape2)
        {
            if ((shape1.Length == 0 && shape1.Height == 0) || (shape2.Length == 0 && shape2.Height == 0))
            {
                //if any of the two Rectangles have Length & Height 0 meaning they represent a point instead of a rectangle
                return false;
            }

            if (
                (shape2.StartX > shape1.EndX || shape2.EndX < shape1.StartX) ||  //Shape2 is Either Left or Right Not In Middle
                (shape2.StartY > shape1.EndY || shape2.EndY < shape1.StartY)     //Shape 2 is Either Up or Down Not in Middle
                )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Checks if two Circle Shapes Collide
        /// </summary>
        /// <param name="shape1">The 1st Circle</param>
        /// <param name="shape2">The Second Circle</param>
        /// <returns>True if the Circles Collide -- False if the Do not</returns>
        private static bool AreIntersectingCircles(CircleDraw shape1, CircleDraw shape2)
        {
            //If the Distance from the Center of the Circles is Less than R+r then the circles intersect -- otherwise theyu don't

            //Check the Squared Distance of the Centers 
            double centersSquaredDistance = GetPointsSquaredDistance(shape1.CenterX, shape1.CenterY, shape2.CenterX, shape2.CenterY);

            //Get the Squared Distance formed by the Radius of the Circles (This is what we must exceed to avoid collision
            double radiusDistanceSquared = Math.Pow(shape1.Radius + shape2.Radius, 2);
            if (centersSquaredDistance > radiusDistanceSquared)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Checks if a Circle Collides with a Rectangle -- Asumes the Rectangle is NOT ROTATED
        /// </summary>
        /// <param name="circleDraw">The Circle Draw</param>
        /// <param name="rectangleDraw">The Rectangle Draw</param>
        /// <returns>True if the Shapes Collide -- False if they Do Not</returns>
        private static bool AreIntersectingCircleRectangle(CircleDraw circleDraw, RectangleDraw rectangleDraw)
        {
            //To Check if the Rectangle Intersects with the Circle we use the Following Logic
            //WE ASSUME THAT THE RECTANGLE IS NOT ROTATED!!!!!
            //We must Find the Nearest point of the Rectangle to the Circle 
            //Then we Clamp the Circles Center to the Rectangles Coordinates (Confine)

            //Finding the Nearest Point of the Rectangle to the Circle
            double nearestX = Math.Max(rectangleDraw.StartX, Math.Min(circleDraw.CenterX, rectangleDraw.EndX));
            double nearestY = Math.Max(rectangleDraw.StartY, Math.Min(circleDraw.CenterY, rectangleDraw.EndY));
            //So the nearest point is X,Y!! If the Circles Center is Inside the Rectangle the point is the Circle Center!

            //Get the Squared Distance from the nearest point to the Circle
            double squaredDistance = GetPointsSquaredDistance(circleDraw.CenterX, circleDraw.CenterY, nearestX, nearestY);

            //Get the Squared Radius of the Circle (This is the Minimal Distance so that the Nearest Point of the Rectangle is Out of it
            double squaredRadius = Math.Pow(circleDraw.Radius, 2);

            if (squaredDistance > squaredRadius)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines wheather a rectange and a Semicircle Intersect
        /// </summary>
        /// <param name="semicircle">The Semicircle</param>
        /// <param name="rectangle">The Rectangle</param>
        /// <param name="isSemicircleLineIncluded">Wheather the Line Connecting the Arc Start/End must be Included in the intersection Check</param>
        /// <returns>True if there is an intersection - False if there is not</returns>
        private static bool AreIntersectingSemicircleRectangle(SemicircleRectangleDraw semicircle, RectangleDraw rectangle)
        {
            //*******THIS IS IRRELEVANT TO CHECK INTERSECTION WITH LINE HERE*******
            //#region 1. CHECK INTERSECTION WITH STRAIGHT LINE
            ////Checks wheather we want to incude at the Intersection Check the Straight Line of the Semicircle
            ////This check will either return true for an intersection or Continue to the following checks
            //if (isSemicircleLineIncluded)
            //{
            //    //The intersection with this straight line , in the Current Orientations (only vertical and horizontal) \
            //    //and with given that the Rectangle never rotates ,
            //    //Needs to check only one coordinate each time
            //    bool isIntersectingWithLine;
            //    switch (semicircle.Orientation)
            //    {
            //        case SemicircleOrientation.PointingTop: //Y of Center must be between Ys of Rectangle
            //        case SemicircleOrientation.PointingBottom:
            //            isIntersectingWithLine = (semicircle.GetCircleCenter().Y >= rectangle.StartY && semicircle.GetCircleCenter().Y <= rectangle.EndY);    
            //            break;
            //        case SemicircleOrientation.PointingLeft: //X of Center must be between of Xs of Rectangle
            //        case SemicircleOrientation.PointingRight:
            //            isIntersectingWithLine = (semicircle.GetCircleCenter().X >= rectangle.StartX && semicircle.GetCircleCenter().X <= rectangle.EndX);
            //            break;
            //        default:
            //            throw new NotSupportedException($"The Current Orientation of the Semicircle is not Supported, Orientation Value :(--{semicircle.Orientation}--)");
            //    }
            //    //If it intersects then return true otherwise continue to the next checks
            //    if (isIntersectingWithLine) { return true; }
            //}

            //#endregion

            //Console.WriteLine($"Intersects with whole circle :{AreIntersectingCircleRectangle(semicircle.GetParentCircle(), rectangle)}");
            #region 2. CHECK INTERSECTION WITH ARC
            //First Check wheather the Rectange Intersect with a circe with the same radius as the Semicircle
            if (AreIntersectingCircleRectangle(semicircle.GetParentCircle(), rectangle))
            {
                //Check wheather the intersection is with the Semicircle Part
                //By Knowing the orientation of the Semicircle we just and Having Calculated that there Is an Intersection.
                //We can easily find if that intersection happens to the actual semicircle or the Hidden part of the Circle.
                bool intersectsWithTheHiddenCircle;
                switch (semicircle.Orientation)
                {
                    case SemicircleOrientation.PointingTop: //Ys of Rectangle must be bigger than center of Semicircle
                        intersectsWithTheHiddenCircle = (rectangle.StartY > semicircle.GetCircleCenter().Y && rectangle.EndY > semicircle.GetCircleCenter().Y);
                        break;
                    case SemicircleOrientation.PointingBottom: //Ys of Rectangle must be less than center of Semicircle
                        intersectsWithTheHiddenCircle = (rectangle.StartY < semicircle.GetCircleCenter().Y && rectangle.EndY < semicircle.GetCircleCenter().Y);
                        break;
                    case SemicircleOrientation.PointingLeft: //Xs of Rectangle must be bigger than center of Semicrcile
                        intersectsWithTheHiddenCircle = (rectangle.StartX > semicircle.GetCircleCenter().X && rectangle.EndX < semicircle.GetCircleCenter().X);
                        break;
                    case SemicircleOrientation.PointingRight: //Xs of Rectangle must be less than center of Semicrcile
                        intersectsWithTheHiddenCircle = (rectangle.StartX > semicircle.GetCircleCenter().X && rectangle.EndX < semicircle.GetCircleCenter().X);
                        break;
                    default:
                        throw new NotSupportedException($"The Current Orientation of the Semicircle is not Supported, Orientation Value :(--{semicircle.Orientation}--)");
                }
                //Console.WriteLine($"Intersects with Hidden Part of Circle:{intersectsWithTheHiddenCircle}");
                //If it intersects with the hidden circle then there is no intersection with the Semicircle
                if (intersectsWithTheHiddenCircle) { return false; } else { return true; }
            }
            else
            {
                //If they do not intersect - there is no intersection also with the Semicircle
                return false;
            }
            #endregion
        }

        /// <summary>
        /// Returns weather a RectangleShape and a CapsuleShape Intersect
        /// </summary>
        /// <param name="capsue">The CapsuleRectangle Draw</param>
        /// <param name="rectangle">The Rectangle</param>
        /// <returns>True if the Do Intersect -- False if the Do not</returns>
        private static bool AreIntersectingCapsuleRectangle(CapsuleRectangleDraw capsule, RectangleDraw rectangle)
        {
            throw new NotImplementedException("Intersections with a Capsule Shape is not Implemented");
        }

        /// <summary>
        /// Checks if two Shapes are Intersecting (Rectangles-Circles Only)
        /// </summary>
        /// <param name="shape1">The First Shape</param>
        /// <param name="shape2">The Second Shape</param>
        /// <returns>True if The Shapes intersect -- False if they Do Not</returns>
        public static bool AreIntersectingShapes(DrawShape shape1, DrawShape shape2)
        {
            // Rectangle - Capsule (MUST RUN FIRST BECAUSE CAPSULE INHERITS FROM RECTANGLE)
            if (shape1 is RectangleDraw rect1 && shape2 is CapsuleRectangleDraw capsule)
            {
                return AreIntersectingCapsuleRectangle(capsule, rect1);
            }
            // Rectangle - Semicircle (MUST RUN FIRST BECAUSE SEMICIRCLE INHERITS FROM RECTANGLE)
            else if (shape1 is RectangleDraw rect && shape2 is SemicircleRectangleDraw semicircle)
            {
                return AreIntersectingSemicircleRectangle(semicircle, rect);
            }
            // Both Are Rectangles
            else if (shape1 is RectangleDraw rectangle1 && shape2 is RectangleDraw rectangle2)
            {
                return AreIntersectingRectangles(rectangle1, rectangle2);
            }
            // One Rectangle one Circle
            else if (shape1 is RectangleDraw rectangle && shape2 is CircleDraw circle)
            {
                return AreIntersectingCircleRectangle(circle, rectangle);
            }
            // One Rectangle one Combo Rectangles
            else if (shape1 is RectangleDraw rectangle3 && shape2 is ComboDrawShape comboShape)
            {
                //Get the list of Shapes of the ComboShape
                List<DrawShape> listOfShapesOfCombo = comboShape.GetComboShapes();
                //Check if all shapes are Rectangles else throw not Implemented Exception
                if (listOfShapesOfCombo.Any(s => s is not RectangleDraw))
                {
                    throw new NotImplementedException("Intersection Collision ,is Not implemented for Combo Draws that , Consist of Shapes other than Rectangles");
                }
                else
                {
                    foreach (DrawShape shape in listOfShapesOfCombo)
                    {
                        RectangleDraw rectanglePartOfCombo = shape as RectangleDraw ?? throw new InvalidCastException("Shape is not a Rectangle");
                        bool areIntersecting = AreIntersectingRectangles(rectangle3, rectanglePartOfCombo);
                        if (areIntersecting)
                        {
                            return true;
                        }
                    }
                    //if the loop exited without a return true this means all counts are not intersecting
                    return false;
                }
            }
            // Both are Circles
            else if (shape1 is CircleDraw c1 && shape2 is CircleDraw c2)
            {
                return AreIntersectingCircles(c1, c2);
            }
            // One Circle one Rectangle
            else if (shape1 is CircleDraw c3 && shape2 is RectangleDraw r3)
            {
                return AreIntersectingCircleRectangle(c3, r3);
            }
            // One Circle one ComboRectangles
            else if (shape1 is CircleDraw c6 && shape2 is ComboDrawShape comboShape2)
            {
                //Get the list of Shapes of the ComboShape
                List<DrawShape> listOfShapesOfCombo = comboShape2.GetComboShapes();
                //Check if all shapes are Rectangles else throw not Implemented Exception
                if (listOfShapesOfCombo.Any(s => s is not RectangleDraw))
                {
                    throw new NotImplementedException("Intersection Collision ,is Not implemented for Combo Draws that , Consist of Shapes other than Rectangles");
                }
                else
                {
                    foreach (DrawShape shape in listOfShapesOfCombo)
                    {
                        RectangleDraw rectanglePartOfCombo = shape as RectangleDraw ?? throw new InvalidCastException("Shape is not a Rectangle");
                        bool areIntersecting = AreIntersectingCircleRectangle(c6, rectanglePartOfCombo);
                        if (areIntersecting)
                        {
                            return true;
                        }
                    }
                    //if the loop exited without a return true this means all counts are not intersecting
                    return false;
                }
            }
            // One Combo Rectangles one Circle
            else if (shape1 is ComboDrawShape comboShape3 && shape2 is CircleDraw c5)
            {
                //Get the list of Shapes of the ComboShape
                List<DrawShape> listOfShapesOfCombo = comboShape3.GetComboShapes();
                //Check if all shapes are Rectangles else throw not Implemented Exception
                if (listOfShapesOfCombo.Any(s => s is not RectangleDraw))
                {
                    throw new NotImplementedException("Intersection Collision ,is Not implemented for Combo Draws that , Consist of Shapes other than Rectangles");
                }
                else
                {
                    foreach (DrawShape shape in listOfShapesOfCombo)
                    {
                        RectangleDraw rectanglePartOfCombo = shape as RectangleDraw ?? throw new InvalidCastException("Shape is not a Rectangle");
                        bool areIntersecting = AreIntersectingCircleRectangle(c5, rectanglePartOfCombo);
                        if (areIntersecting)
                        {
                            return true;
                        }
                    }
                    //if the loop exited without a return true this means all counts are not intersecting
                    return false;
                }
            }
            // One Combo Rectangles one Rectangle
            else if (shape1 is ComboDrawShape comboShape4 && shape2 is RectangleDraw r5)
            {
                //Get the list of Shapes of the ComboShape
                List<DrawShape> listOfShapesOfCombo = comboShape4.GetComboShapes();
                //Check if all shapes are Rectangles else throw not Implemented Exception
                if (listOfShapesOfCombo.Any(s => s is not RectangleDraw))
                {
                    throw new NotImplementedException("Intersection Collision ,is Not implemented for Combo Draws that , Consist of Shapes other than Rectangles");
                }
                else
                {
                    foreach (DrawShape shape in listOfShapesOfCombo)
                    {
                        RectangleDraw rectanglePartOfCombo = shape as RectangleDraw ?? throw new InvalidCastException("Shape is not a Rectangle");
                        bool areIntersecting = AreIntersectingRectangles(r5, rectanglePartOfCombo);
                        if (areIntersecting)
                        {
                            return true;
                        }
                    }
                    //if the loop exited without a return true this means all counts are not intersecting
                    return false;
                }
            }
            // Two Combo Rectangles
            else if (shape1 is ComboDrawShape comboShape5 && shape2 is ComboDrawShape comboShape6)
            {
                //Get the list of Shapes of all the Combo Shapes
                List<DrawShape> listOfShapesOfCombo = comboShape5.GetComboShapes();
                listOfShapesOfCombo.AddRange(comboShape6.GetComboShapes());

                //Check if all shapes are Rectangles else throw not Implemented Exception
                if (listOfShapesOfCombo.Any(s => s is not RectangleDraw))
                {
                    throw new NotImplementedException("Intersection Collision ,is Not implemented for Combo Draws that , Consist of Shapes other than Rectangles");
                }
                else
                {
                    //Check that all parts of the List are not Intersecting with each Other
                    //Each item checks with all the items below it , until the last one that checks with none (as the rest have already checked with it)
                    for (int i = 0; i < listOfShapesOfCombo.Count; i++)
                    {
                        RectangleDraw rectanglePartOfCombo = listOfShapesOfCombo[i] as RectangleDraw ?? throw new InvalidCastException("Shape is not a Rectangle");

                        if (listOfShapesOfCombo.Count >= i + 2) //If the count is less than i+2 then it means we are at the last item 
                        {
                            for (int j = i + 1; j < listOfShapesOfCombo.Count; j++)
                            {
                                RectangleDraw rectanglePartOfNextCombo = listOfShapesOfCombo[j] as RectangleDraw ?? throw new InvalidCastException("Shape is not a Rectangle");
                                bool areIntersecting = AreIntersectingRectangles(rectanglePartOfNextCombo, rectanglePartOfCombo);
                                if (areIntersecting)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    //if the loops exited without a return true this means all counts are not intersecting
                    return false;
                }
            }
            // Unrecognized Shapes Set
            else
            {
                throw new NotSupportedException($"The Provided Shapes Set, is not Supported for Collision Checks Shape1:{shape1.GetType()} -- Shape2:{shape2.GetType()}");
            }
        }

        #endregion

        #region 2.Containment

        /// <summary>
        /// Checks if a Shapwe fits inside another Shape (Works for Circles and Rectangles Only)
        /// </summary>
        /// <param name="boundary">The Shape that Acts as the Boundary</param>
        /// <param name="containedShape">The contained Shape</param>
        /// <returns>Returns true if the Shape is Contained False Otherwise</returns>
        public static bool IsShapeInsideBoundary(DrawShape boundary, DrawShape containedShape)
        {
            //Boundary Cases for all main Shapes (Rectangle,Circle and Combos of Rectangles)
            //Capsule must run before rectangle - because it is a rectangle
            if (boundary is CircleDraw circleBoundary)
            {
                if (containedShape is RectangleDraw containedRectangle)
                {
                    return IsRectangleInsideCircle(circleBoundary, containedRectangle);
                }
                else if (containedShape is CircleDraw containedCircle)
                {
                    return IsCircleInsideCircle(circleBoundary, containedCircle);
                }
                else if (containedShape is ComboDrawShape containedCombo)
                {
                    //Get the list of Shapes of the contained ComboShape
                    List<DrawShape> listOfShapesOfContainedCombo = containedCombo.GetComboShapes();
                    //Check if all shapes are Rectangles else throw not Implemented Exception
                    if (listOfShapesOfContainedCombo.Any(s => s is not RectangleDraw))
                    {
                        throw new NotImplementedException("Containment Collision ,is Not implemented for Combo Draws that , Consist of Shapes other than Rectangles");
                    }
                    else
                    {
                        foreach (DrawShape shape in listOfShapesOfContainedCombo)
                        {
                            RectangleDraw rectanglePartOfCombo = shape as RectangleDraw ?? throw new InvalidCastException("Shape is not a Rectangle");
                            bool isContained = IsRectangleInsideCircle(circleBoundary, rectanglePartOfCombo);
                            if (isContained == false)
                            {
                                return false;
                            }
                        }
                        //if the loop exited without a return false this means all counts succeded
                        return true;
                    }
                }
                else
                {
                    throw new ArgumentException($"The Provided Shape for Containment Detection cannot be Recognized {containedShape.GetType()}");
                }
            }
            else if (boundary is EllipseDraw ellipseBoundary)
            {
                if (containedShape is RectangleDraw rect)
                {
                    return IsRectangleInsideEllipse(ellipseBoundary, rect);
                }
                else if (containedShape is CircleDraw circle)
                {
                    return IsCircleInsideEllipse(ellipseBoundary, circle);
                }
                else
                {
                    throw new NotSupportedException("Containment within an Ellipse is only Supported for Rectangles and Circles");
                }
            }
            else if (boundary is CapsuleRectangleDraw capsuleRectangleBoundary)
            {
                if (containedShape is RectangleDraw containedRectangle)
                {
                    return IsRectangleInsideCapsule(capsuleRectangleBoundary, containedRectangle);
                }
                else if (containedShape is CircleDraw containedCircle)
                {
                    return IsCircleInsideCapsule(capsuleRectangleBoundary, containedCircle);
                }
                else
                {
                    throw new NotSupportedException("Capsule Containment for non Rectangle or non Circle Shapes is Not Supported");
                }
            }
            else if (boundary is RectangleDraw rectangleBoundary)
            {
                if (containedShape is RectangleDraw containedRectangle)
                {
                    return IsRectangleInsideRectangle(rectangleBoundary, containedRectangle);
                }
                else if (containedShape is CircleDraw containedCircle)
                {
                    return IsCircleInsideRectangle(rectangleBoundary, containedCircle);
                }
                else if (containedShape is ComboDrawShape containedCombo)
                {
                    //Get the list of Shapes of the contained ComboShape
                    List<DrawShape> listOfShapesOfContainedCombo = containedCombo.GetComboShapes();
                    //Check if all shapes are Rectangles else throw not Implemented Exception
                    if (listOfShapesOfContainedCombo.Any(s => s is not RectangleDraw))
                    {
                        throw new NotImplementedException("Containment Collision ,is Not implemented for Combo Draws that , Consist of Shapes other than Rectangles");
                    }
                    else
                    {
                        foreach (DrawShape shape in listOfShapesOfContainedCombo)
                        {
                            RectangleDraw rectanglePartOfCombo = shape as RectangleDraw ?? throw new InvalidCastException("Shape is not a Rectangle");
                            bool isContained = IsRectangleInsideRectangle(rectangleBoundary, rectanglePartOfCombo);
                            if (isContained == false)
                            {
                                return false;
                            }
                        }
                        //if the loop exited without a return false this means all counts succeded
                        return true;
                    }
                }
                else
                {
                    throw new ArgumentException($"The Provided Shape for Containment Detection cannot be Recognized {containedShape.GetType()} ");
                }
            }
            else if (boundary is ComboDrawShape comboShapeBoundary)
            {
                if (containedShape is RectangleDraw containedRectangle)
                {
                    return IsRectangleInsideRectangle(comboShapeBoundary.GetBoundingBoxRectangle(), containedRectangle);
                }
                else if (containedShape is CircleDraw containedCircle)
                {
                    return IsCircleInsideRectangle(comboShapeBoundary.GetBoundingBoxRectangle(), containedCircle);
                }
                else if (containedShape is ComboDrawShape containedComboShape)
                {
                    //Get the list of Shapes of the contained ComboShape
                    List<DrawShape> listOfShapesOfContainedCombo = containedComboShape.GetComboShapes();
                    //Check if all shapes are Rectangles else throw not Implemented Exception
                    if (listOfShapesOfContainedCombo.Any(s => s is not RectangleDraw))
                    {
                        throw new NotImplementedException("Containment Collision ,is Not implemented for Combo Draws that , Consist of Shapes other than Rectangles");
                    }
                    else
                    {
                        foreach (DrawShape shape in listOfShapesOfContainedCombo)
                        {
                            RectangleDraw rectanglePartOfCombo = shape as RectangleDraw ?? throw new InvalidCastException("Shape is not a Rectangle");
                            bool isContained = IsRectangleInsideRectangle(comboShapeBoundary.GetBoundingBoxRectangle(), rectanglePartOfCombo);
                            if (isContained == false)
                            {
                                return false;
                            }
                        }
                        //if the loop exited without a return false this means all counts succeded
                        return true;
                    }
                }
                else
                {
                    throw new ArgumentException($"The Provided Shape for Containment Detection cannot be Recognized {containedShape.GetType()} ");
                }
            }
            else
            {
                throw new ArgumentException($"The Provided Boundary Shape for Containment Detection is Not Recognized {boundary.GetType()}");
            }

        }

        /// <summary>
        /// Checks if a rectangle is contained inside a Circle
        /// </summary>
        /// <param name="boundaryCircle">The Circle Boundary</param>
        /// <param name="rectangle">The contained Rectangle</param>
        /// <returns>True if the Rectangle is Contained within the Circle -- False if it is not</returns>
        private static bool IsRectangleInsideCircle(CircleDraw boundaryCircle, RectangleDraw rectangle)
        {
            //If All Edges of the Rectangle are inside the Circle then All Rectangle points are inside the Circle
            //So the Distance of Each Corner must be Less than r
            //We use Squared Distances so we do not have to use the Calculations Intensive SQRT Function
            double squaredDistance1Corner = GetPointsSquaredDistance(boundaryCircle.CenterX, boundaryCircle.CenterY, rectangle.StartX, rectangle.StartY);
            double squaredDistance2Corner = GetPointsSquaredDistance(boundaryCircle.CenterX, boundaryCircle.CenterY, rectangle.StartX, rectangle.EndY);
            double squaredDistance3Corner = GetPointsSquaredDistance(boundaryCircle.CenterX, boundaryCircle.CenterY, rectangle.EndX, rectangle.StartY);
            double squaredDistance4Corner = GetPointsSquaredDistance(boundaryCircle.CenterX, boundaryCircle.CenterY, rectangle.EndX, rectangle.EndY);
            double squaredRadius = Math.Pow(boundaryCircle.Radius, 2);

            if (squaredDistance1Corner < squaredRadius &&
                squaredDistance2Corner < squaredRadius &&
                squaredDistance3Corner < squaredRadius &&
                squaredDistance4Corner < squaredRadius)
            {
                return true;
            }
            else
            {
                return false;
            }

            ////Find the Furthest Point of the Rectangle from the Circles Center . If this point is contained in the Circle 
            ////Then the Whole Rectangle is Contained in the Circle
            //double furthestX = Math.Max(boundaryCircle.CenterX - rectangle.StartX, boundaryCircle.CenterX - rectangle.EndX);
            //double furthestY = Math.Max(boundaryCircle.CenterY - rectangle.StartY, boundaryCircle.CenterY - rectangle.EndY);

            ////Get the Squared Distance of this Point from the Circles Center
            //double squaredDistance = GetPointsSquaredDistance(boundaryCircle.CenterX, boundaryCircle.CenterY, furthestX, furthestY);

            ////Get the Squared Maximum Distance so that the Rectangle Can be Contained in the Circle (The Radius Squared)
            //double squaredRadius = Math.Pow(boundaryCircle.Radius,2);
            //if (squaredDistance < squaredRadius)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        /// <summary>
        /// Checks if a Rectangle is Contained inside another Rectangle
        /// </summary>
        /// <param name="boundaryRectangle">The Boundary Rectangle</param>
        /// <param name="rectangle">The Contained Rectangle</param>
        /// <returns>True if the Rectangle is Contained -- False if it is Not</returns>
        private static bool IsRectangleInsideRectangle(RectangleDraw boundaryRectangle, RectangleDraw rectangle)
        {
            //Check if all Vertices of the contained Rectangle are located inside the boundary Rectangle
            if (rectangle.StartX > boundaryRectangle.StartX &&
                rectangle.EndX < boundaryRectangle.EndX &&
                rectangle.StartY > boundaryRectangle.StartY &&
                rectangle.EndY < boundaryRectangle.EndY)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        /// <summary>
        /// Checks if a Circle is Contained inside another Circle
        /// </summary>
        /// <param name="boundaryCircle">The Boundary Circle</param>
        /// <param name="circle">The contained Circle</param>
        /// <returns>True if the Circle is Contained -- False if it Not</returns>
        private static bool IsCircleInsideCircle(CircleDraw boundaryCircle, CircleDraw circle)
        {
            //Check Weather the Squared Distance of the Circles Centers is Smaller than the Boundary Circles Radius
            //dx^2 + dy^2 < r^2
            double distanceOfCenters = GetDistanceBetweenPoints(boundaryCircle.CenterX, boundaryCircle.CenterY, circle.CenterX, circle.CenterY);

            //where furthestDistance == Furthest point from CircleBoundary Center
            double furthestDistance = distanceOfCenters + circle.Radius;

            if (boundaryCircle.Radius > furthestDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a Circle is Contained inside a Rectangle
        /// </summary>
        /// <param name="boundaryRectangle">The Boundary Rectangle</param>
        /// <param name="circle">The Contained Circle</param>
        /// <returns>True if the Circle is Contained -- False if It is Not</returns>
        private static bool IsCircleInsideRectangle(RectangleDraw boundaryRectangle, CircleDraw circle)
        {
            //Check if the Center X +- Radius is Inside the X Boundaries of the Rectangle
            //Check if the Center Y +- Radius is Inside the Y Boundaries of the Rectangle
            if (circle.CenterX - circle.Radius > boundaryRectangle.StartX &&
                circle.CenterX + circle.Radius < boundaryRectangle.EndX &&
                circle.CenterY - circle.Radius > boundaryRectangle.StartY &&
                circle.CenterY + circle.Radius < boundaryRectangle.EndY)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        /// <summary>
        /// Determines wheather a Rectangle is Inside a Semicircle
        /// </summary>
        /// <param name="boundarySemicircle">The Semicircle Boundary</param>
        /// <param name="rectangle">The Rectangle we need Inside the Semicircle</param>
        /// <returns>TRUE if rectangle is contained , False if it is not</returns>
        private static bool IsRectangleInsideSemicircle(SemicircleRectangleDraw boundarySemicircle, RectangleDraw rectangle)
        {
            //If All Edges of the Rectangle are inside the Circle then All Rectangle points are inside the Circle
            //So the Distance of Each Corner must be Less than r
            //We use Squared Distances so we do not have to use the Calculations Intensive SQRT Function
            DrawPoint semicircleCenter = new(boundarySemicircle.GetCircleCenter().X, boundarySemicircle.GetCircleCenter().Y);
            double semicircleRadius = boundarySemicircle.CornerRadius;

            double squaredDistance1Corner = GetPointsSquaredDistance(semicircleCenter.X, semicircleCenter.Y, rectangle.StartX, rectangle.StartY);
            double squaredDistance2Corner = GetPointsSquaredDistance(semicircleCenter.X, semicircleCenter.Y, rectangle.StartX, rectangle.EndY);
            double squaredDistance3Corner = GetPointsSquaredDistance(semicircleCenter.X, semicircleCenter.Y, rectangle.EndX, rectangle.StartY);
            double squaredDistance4Corner = GetPointsSquaredDistance(semicircleCenter.X, semicircleCenter.Y, rectangle.EndX, rectangle.EndY);
            double squaredRadius = Math.Pow(semicircleRadius, 2);

            //These are the Square Distances of the Rectangle edges from the Semicircles Center
            //If the Below is true it means there  are no points in the Rectangle that are outside a circle with radius = semicircles radius
            if (squaredDistance1Corner < squaredRadius &&
                squaredDistance2Corner < squaredRadius &&
                squaredDistance3Corner < squaredRadius &&
                squaredDistance4Corner < squaredRadius)
            {
                //Apart from the Above Distance Check all the Points of the rectangle must have X or Y such that it does not get out of the semicircles straight line
                //This depends on the Semicircles Orientation so we can check for less or greater than
                //So below it returns true or false when the conditions needed are met 
                return boundarySemicircle.Orientation switch
                {
                    // All Ys must be less than the Semicircles
                    SemicircleOrientation.PointingTop => (rectangle.StartY < semicircleCenter.Y && rectangle.EndY < semicircleCenter.Y),
                    // All Ys must be Greater than the Semicircles
                    SemicircleOrientation.PointingBottom => (rectangle.StartY > semicircleCenter.Y && rectangle.EndY > semicircleCenter.Y),
                    // All Xs must be Greater than the Semicircles
                    SemicircleOrientation.PointingLeft => (rectangle.StartX > semicircleCenter.X && rectangle.EndX > semicircleCenter.X),
                    // All Xs must be less than the Semicircles
                    SemicircleOrientation.PointingRight => (rectangle.StartX < semicircleCenter.X && rectangle.EndX < semicircleCenter.X),
                    _ => false,
                };
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines wheather a Rectangle is Inside a CapsuleRectangle
        /// </summary>
        /// <param name="boundaryCapsule">The Boundary Capsule Shape</param>
        /// <param name="rectangle">The Rectangle</param>
        /// <returns>True if it is Inside the Capsule , False if it is Not</returns>
        private static bool IsRectangleInsideCapsule(CapsuleRectangleDraw boundaryCapsule, RectangleDraw rectangle)
        {
            /*
             1.Case Rectangle Inside the Middle Area
             2.Case Some of the Area of Rectangle Inside the Semicircle 1 and some out
             3.Case Some of the Area of Rectangle Inisde the Semicircle 2 and some out 
             4.Both 2.3 at the same time when rectangle is too big but also fits inside capsule
             5.All cases rectangle must fit between the two vertical lines of capsule
             */

            //Needed Points and Distances
            DrawPoint semicircleCenter1 = new(boundaryCapsule.GetTopLeftSemicircle().GetCircleCenter().X, boundaryCapsule.GetTopLeftSemicircle().GetCircleCenter().Y);
            DrawPoint semicircleCenter2 = new(boundaryCapsule.GetBottomRightSemicircle().GetCircleCenter().X, boundaryCapsule.GetBottomRightSemicircle().GetCircleCenter().Y);
            double semicircleRadius = boundaryCapsule.CornerRadius;

            //Check rectangle is inside Boundaries of Capsules Horizontal/Vertical Lines else return false
            //This Ensures that the Rectangle is within the Bounding Box of the Capsule
            if (IsRectangleInsideRectangle(boundaryCapsule, rectangle) is false) { return false; }

            //HORIZONTAL ORIENTATION
            //(All cases for reactangle within semicircle1 or semicircle2 or inbetween
            if (boundaryCapsule.IsHorizontal)
            {
                if (rectangle.StartX > semicircleCenter1.X) //Case where rectangle is out of Semicircle1
                {
                    if (rectangle.EndX < semicircleCenter2.X)//Case where rectangle is also out of Semicircle2
                    {
                        return true;
                    }
                    else //Case where rectangle is inside Semicircle2 -- Square distances from center for the furthest sides must be <squared radius
                    {
                        double squaredDistanceRightTop = GetPointsSquaredDistance(semicircleCenter2.X, semicircleCenter2.Y, rectangle.EndX, rectangle.StartY); //RightTop Edge
                        double squaredDistanceRightBottom = GetPointsSquaredDistance(semicircleCenter2.X, semicircleCenter2.Y, rectangle.EndX, rectangle.EndY);   //RightBottom Edge
                        double squaredRadius = Math.Pow(semicircleRadius, 2);
                        //Both must be <r^2 to return TRUE (inside Capsule) else returns false there is an intersection
                        return (squaredDistanceRightTop < squaredRadius && squaredDistanceRightBottom < squaredRadius);
                    }
                }
                else //Case where part of the Rectangle is Inisde Semicircle1 -- Square distances from center for the furthest sides must be <squared radius
                {
                    double squaredDistanceLeftTop = GetPointsSquaredDistance(semicircleCenter1.X, semicircleCenter1.Y, rectangle.StartX, rectangle.StartY); //LeftTop Edge
                    double squaredDistanceLeftBottom = GetPointsSquaredDistance(semicircleCenter1.X, semicircleCenter1.Y, rectangle.StartX, rectangle.EndY);   //LeftBottom Edge
                    double squaredRadius = Math.Pow(semicircleRadius, 2);

                    //Case where both edges are inside the Semicircle1 and not intersecting with it
                    if (squaredDistanceLeftTop < squaredRadius && squaredDistanceLeftBottom < squaredRadius)
                    {
                        //Case where the right side is before Semicircle2 center
                        if (rectangle.EndX < semicircleCenter2.X)
                        {
                            return true;
                        }
                        //Case where right side is after Semicircle2 center
                        else
                        {
                            double squaredDistanceRightTop = GetPointsSquaredDistance(semicircleCenter2.X, semicircleCenter2.Y, rectangle.EndX, rectangle.StartY); //RightTop Edge
                            double squaredDistanceRightBottom = GetPointsSquaredDistance(semicircleCenter2.X, semicircleCenter2.Y, rectangle.EndX, rectangle.EndY);   //RightBottom Edge
                            //Both must be <r^2 to return TRUE (inside Capsule) else returns false there is an intersection
                            return (squaredDistanceRightTop < squaredRadius && squaredDistanceRightBottom < squaredRadius);
                        }

                    }
                    //Case where one of the Edges intersects with the Arc
                    else
                    {
                        return false;
                    }
                }
            }
            //VERTICAL ORIENTATION
            //(All cases for reactangle within semicircle1 or semicircle2 or inbetween
            else
            {
                if (rectangle.StartY > semicircleCenter1.Y) //Case where rectangle is out of Semicircle1
                {
                    if (rectangle.EndY < semicircleCenter2.Y)//Case where rectangle is also out of Semicircle2
                    {
                        return true;
                    }
                    else //Case where rectangle is inside Semicircle2 -- Square distances from center for the furthest sides must be <squared radius
                    {
                        double squaredDistanceLeftBottom = GetPointsSquaredDistance(semicircleCenter2.X, semicircleCenter2.Y, rectangle.StartX, rectangle.EndY); //LeftBottom Edge
                        double squaredDistanceRightBottom = GetPointsSquaredDistance(semicircleCenter2.X, semicircleCenter2.Y, rectangle.EndX, rectangle.EndY);   //RightBottom Edge
                        double squaredRadius = Math.Pow(semicircleRadius, 2);
                        //Both must be <r^2 to return TRUE (inside Capsule) else returns false there is an intersection
                        return (squaredDistanceLeftBottom < squaredRadius && squaredDistanceRightBottom < squaredRadius);
                    }
                }
                else //Case where part of the Rectangle is Inisde Semicircle1 -- Square distances from center for the furthest sides must be <squared radius
                {
                    double squaredDistanceLeftTop = GetPointsSquaredDistance(semicircleCenter1.X, semicircleCenter1.Y, rectangle.StartX, rectangle.StartY); //LeftTop Edge
                    double squaredDistanceRightTop = GetPointsSquaredDistance(semicircleCenter1.X, semicircleCenter1.Y, rectangle.EndX, rectangle.StartY);   //RightTop Edge
                    double squaredRadius = Math.Pow(semicircleRadius, 2);

                    //Case where both edges are inside the Semicircle1 and not intersecting with it
                    if (squaredDistanceLeftTop < squaredRadius && squaredDistanceRightTop < squaredRadius)
                    {
                        //Case where the Bottom side is before Semicircle2 center
                        if (rectangle.EndY < semicircleCenter2.Y)
                        {
                            return true;
                        }
                        //Case where Bottom side is after Semicircle2 center
                        else
                        {
                            double squaredDistanceLeftBottom = GetPointsSquaredDistance(semicircleCenter2.X, semicircleCenter2.Y, rectangle.StartX, rectangle.EndY); //LeftBottom Edge
                            double squaredDistanceRightBottom = GetPointsSquaredDistance(semicircleCenter2.X, semicircleCenter2.Y, rectangle.EndX, rectangle.EndY);   //RightBottom Edge
                            //Both must be <r^2 to return TRUE (inside Capsule) else returns false there is an intersection
                            return (squaredDistanceLeftBottom < squaredRadius && squaredDistanceRightBottom < squaredRadius);
                        }

                    }
                    //Case where one of the Edges intersects with the Arc
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Checks wheather a Circle is Contained Within a CapsuleRectangle
        /// </summary>
        /// <param name="boundaryCapsule">The Capsule Boundary</param>
        /// <param name="circle">The Circle</param>
        /// <returns>True if its Contained , False if it is not</returns>
        private static bool IsCircleInsideCapsule(CapsuleRectangleDraw boundaryCapsule, CircleDraw circle)
        {
            //Check that Circle is within the Bounding box of the Capsule
            if (IsCircleInsideRectangle(boundaryCapsule, circle) is false)
            {
                return false;
            }

            DrawPoint semicircleCenter1 = new(boundaryCapsule.GetTopLeftSemicircle().GetCircleCenter().X, boundaryCapsule.GetTopLeftSemicircle().GetCircleCenter().Y);
            DrawPoint semicircleCenter2 = new(boundaryCapsule.GetBottomRightSemicircle().GetCircleCenter().X, boundaryCapsule.GetBottomRightSemicircle().GetCircleCenter().Y);

            //Check that the Circle is no intersecting with any parts of the semicircles
            //HORIZONTAL ORIENTATION
            if (boundaryCapsule.IsHorizontal)
            {
                //Case where circle is located within the middle rectange of the Capsule
                if (circle.ShapeCenterX > semicircleCenter1.X && circle.CenterX < semicircleCenter2.X)
                {
                    return true;
                }
                //Case where circle center is either on 1st semicircle or 2nd semicircle
                else
                {
                    if (circle.ShapeCenterX < semicircleCenter1.X)
                    {
                        //Now that we know the Circles center is inside the Semicircle we only need to see if the 
                        //Circle is contained within another Circle with the Semicircles Radius and center .
                        if (IsCircleInsideCircle(boundaryCapsule.GetTopLeftSemicircle().GetParentCircle(), circle))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    //Case where circle is inside Semicircle2
                    else
                    {
                        if (IsCircleInsideCircle(boundaryCapsule.GetBottomRightSemicircle().GetParentCircle(), circle))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            //VERTICAL ORIENTATION
            else
            {
                //Case where circle is located within the middle rectange of the Capsule
                if (circle.ShapeCenterY > semicircleCenter1.Y && circle.CenterY < semicircleCenter2.Y)
                {
                    return true;
                }
                //Case where circle center is either on 1st semicircle or 2nd semicircle
                else
                {
                    if (circle.ShapeCenterY < semicircleCenter1.Y)
                    {
                        //Now that we know the Circles center is inside the Semicircle we only need to see if the 
                        //Circle is contained within another Circle with the Semicircles Radius and center .
                        if (IsCircleInsideCircle(boundaryCapsule.GetTopLeftSemicircle().GetParentCircle(), circle))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    //Case where circle is inside Semicircle2
                    else
                    {
                        if (IsCircleInsideCircle(boundaryCapsule.GetBottomRightSemicircle().GetParentCircle(), circle))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cheacks wheather a rectangle is Contained within an Ellipse
        /// </summary>
        /// <param name="boundaryEllipse">The Ellipse Boundary</param>
        /// <param name="rectangle">The Rectangle</param>
        /// <returns>True if the Rectangle is inside the Ellipse - Fasle if its not</returns>
        private static bool IsRectangleInsideEllipse(EllipseDraw boundaryEllipse, RectangleDraw rectangle)
        {
            //Fast Escape (If Its not inside the Bounding Rectangle its not inside the elipse also
            if (IsRectangleInsideRectangle(boundaryEllipse.GetBoundingBoxRectangle(), rectangle) is false)
            {
                return false;
            }

            //In our case the Rectangle and the Ellipse are never rotated
            //so we just have to check the 4 edges of the Rectangle
            //being inside the elipse

            bool isInsideTopLeft = boundaryEllipse.IsPointInsideEllipse(rectangle.StartX, rectangle.StartY);
            bool isInsideBottomLeft = boundaryEllipse.IsPointInsideEllipse(rectangle.StartX, rectangle.EndY);
            bool isInsideTopRight = boundaryEllipse.IsPointInsideEllipse(rectangle.EndX, rectangle.StartY);
            bool isInsideBottomRight = boundaryEllipse.IsPointInsideEllipse(rectangle.EndX, rectangle.EndY);

            return (isInsideTopLeft && isInsideBottomLeft && isInsideTopRight && isInsideBottomRight);
        }

        /// <summary>
        /// Determines weather a Circle is Contained to an Ellipse
        /// </summary>
        /// <param name="boundaryEllipse">The Ellipse Boundary</param>
        /// <param name="circle">The Circle</param>
        /// <returns>True if the Circle is contained inside -- Flase if its not</returns>
        /// <exception cref="NotFiniteNumberException">When the Solutions are not what is expected- Should not happen</exception>
        private static bool IsCircleInsideEllipse(EllipseDraw boundaryEllipse, CircleDraw circle)
        {
            //Early Escape
            //1.When the Circle is not Contained by the boundarys Bounding Box
            if (IsCircleInsideRectangle(boundaryEllipse.GetBoundingBoxRectangle(), circle) is false)
            {
                return false;
            }
            //2.When the Bounding Box of the Circle is Inside the Ellipse
            if (IsRectangleInsideEllipse(boundaryEllipse,circle.GetBoundingBoxRectangle()))
            {
                return true;
            }

            //3.If there is no Early Escape then get the Points of an Inscribed Polygon to the Circle
            //Check if the Points are Inside the Ellipse . We will take 20Sides for a Polygon and See !
            List<DrawPoint> inscribedPolygonPoints = circle.GetInscribedPolygonPoints(20);
            foreach (DrawPoint point in inscribedPolygonPoints)
            {
                if (IsPointInsideEllipse(boundaryEllipse,point.X,point.Y) is false)
                {
                    return false;
                }
            }
            //If there is no point outside then return true!
            return true;
        }

        #endregion

        /// <summary>
        /// Moves the Coordinate of a Shape in Relation to another Shape
        /// The Movement Always Happens Using the Bounding Boxes of the Provided Shapes
        /// </summary>
        /// <param name="shapeToMove">The Shape that needs to be Moved</param>
        /// <param name="shapeInRelation">The Shape relative to which we will make the movement</param>
        /// <param name="directionToMove">The Direction we need to effect the movement</param>
        /// <param name="safeDistance">The Minimum Distance that the shapes must be apart</param>
        public static void RepositionCollidingShapes(DrawShape shapeToMove, DrawShape shapeInRelation, DrawMoveDirection directionToMove, double safeDistance)
        {
            switch (directionToMove)
            {
                case DrawMoveDirection.Up:

                    #region 1. Move Center Y Up
                    //Find the Y Distance of the two Centers of the Colliding Shapes
                    double currentDistance = Math.Abs(shapeToMove.ShapeCenterY - shapeInRelation.ShapeCenterY);
                    //Find the MinimumDistance they Must Have
                    double minimumAllowableDistance = shapeToMove.BoundingBoxHeight / 2d + shapeInRelation.BoundingBoxHeight / 2d + safeDistance;

                    //Find the Adjustment that must be Done to the Y Coordinate of the shapeToMove
                    //dy is always Positive as the shapes are coliding the Minallowabe distance is always bigger than current
                    double dy = minimumAllowableDistance - currentDistance;

                    double newCenterYShapeToMove;
                    //Find the new Center of the Shape to Move
                    if (shapeInRelation.ShapeCenterY > shapeToMove.ShapeCenterY) //ShapeToMove is Above ShapeInRelation
                    {
                        newCenterYShapeToMove = shapeToMove.ShapeCenterY - dy;
                    }
                    else //ShapeToMove is Below or at the Same Level with the Shape in Relation
                    {
                        //When the CenterY of the shapeToMove is beow the centerY of the Shape in Relation then we must move it dy PLUS the Current Height of the bounding Box
                        newCenterYShapeToMove = shapeToMove.ShapeCenterY - dy - shapeInRelation.BoundingBoxHeight / 2d;
                    }
                    //Set the New Center
                    shapeToMove.SetCenterOrStartY(newCenterYShapeToMove, DrawShape.CSCoordinate.Center);
                    #endregion

                    break;
                case DrawMoveDirection.Down:

                    throw new NotImplementedException("Down Movement not Implemented");

                case DrawMoveDirection.Left:

                    #region 3. Move Center X Left
                    //Find the X Distance of the two Centers
                    currentDistance = Math.Abs(shapeToMove.ShapeCenterX - shapeInRelation.ShapeCenterX);
                    //Find the MinimumDistance they Must Have
                    minimumAllowableDistance = shapeToMove.BoundingBoxLength / 2d + shapeInRelation.BoundingBoxLength / 2d + safeDistance;

                    //Find the Adjustment that must be Done to the CenterX Coordinate
                    double dx = minimumAllowableDistance - currentDistance;

                    double newCenterXShapeToMove;
                    //Find the new Center X of the shape to Move
                    if (shapeInRelation.ShapeCenterX > shapeToMove.ShapeCenterX) //ShapeToMove is Left of Shape in Relation
                    {
                        newCenterXShapeToMove = shapeToMove.ShapeCenterX - dx;
                    }
                    else //Shape to Move is Right or At the Same Level with shape in Relation
                    {
                        //Move an extra Bounding Box Length/2d the Shape to Move
                        newCenterXShapeToMove = shapeToMove.ShapeCenterX - dx - shapeInRelation.BoundingBoxLength / 2d;
                    }

                    shapeToMove.SetCenterOrStartX(newCenterXShapeToMove, DrawShape.CSCoordinate.Center);
                    #endregion

                    break;
                case DrawMoveDirection.Right:

                    throw new NotImplementedException("Right Movement not Implemented");

                default:
                    throw new ArgumentException($"The Provided {nameof(DrawMoveDirection)} is not Recognized or is Null -- Method:{nameof(RepositionCollidingShapes)} could not finish");
            }
        }

        #region 3.Mathematics Various

        /// <summary>
        /// Get the Solutions x1 , x2 of the Quadratic Equation Ax^2+Bx+G = 0
        /// </summary>
        /// <param name="a">A Part of Equation Ax^2+Bx+G</param>
        /// <param name="b">B Part of Equation Ax^2+Bx+G</param>
        /// <param name="c">G Part of Equation Ax^2+Bx+G</param>
        /// <returns> x1 , x2 or NaN if Equation has no Real Solutions </returns>
        public static (double, double) SolveQuadraticEquation(double a, double b, double c)
        {
            double x1;
            double x2;

            double D = GetDiscriminant(a, b, c);

            if (D > 0) //Two Solutions
            {
                double sqrtD = Math.Sqrt(D);
                x1 = (-b + sqrtD) / (2 * a);
                x2 = (-b - sqrtD) / (2 * a);
            }
            else if (D is 0) //One (Double) Solution
            {
                x1 = -b / (2 * a);
                x2 = x1;
            }
            else //No real Solutions
            {
                x1 = double.NaN;
                x2 = double.NaN;
            }
            return (x1, x2);
        }

        /// <summary>
        /// Gets the Discriminant of a quadratic Equation aX^2 + bx + c = 0
        /// </summary>
        /// <param name="a">Term 'a' of the equation</param>
        /// <param name="b">Term 'b' of the equation</param>
        /// <param name="c">Term 'c' of the equation</param>
        /// <returns>The Discriminant of the quadratic Equation</returns>
        public static double GetDiscriminant(double a , double b ,double c)
        {
            double D = Math.Pow(b,2) - 4 * a * c;
            return D;
        }


        #endregion

        #region 4. Bounding Boxes
        public static RectangleDraw GetShapesBoundingBox(IEnumerable<DrawShape> shapes)
        {
            // Collect all Xs and Ys of the various bounding Boxes of the shapes
            List<double> xs = new();
            List<double> ys = new();
            foreach (var shape in shapes)
            {
                xs.Add(shape.BoundingBoxCenterX - shape.BoundingBoxLength / 2d);
                xs.Add(shape.BoundingBoxCenterX + shape.BoundingBoxLength / 2d);
                ys.Add(shape.BoundingBoxCenterY - shape.BoundingBoxHeight / 2d);
                ys.Add(shape.BoundingBoxCenterY + shape.BoundingBoxHeight / 2d);
            }
            return new RectangleDraw(xs.Min(),ys.Min(),xs.Max(),ys.Min());
        }

        #endregion

    }
}

