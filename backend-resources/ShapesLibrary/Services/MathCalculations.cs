using CommonHelpers;
using CommonHelpers.Comparers;
using CommonHelpers.Exceptions;
using ShapesLibrary.Enums;
using ShapesLibrary.Exceptions;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using static ShapesLibrary.Services.MathCalculations;

namespace ShapesLibrary.Services
{
    public static class MathCalculations
    {
        public static class VariousMath
        {
            /// <summary>
            /// Get the Solutions x1 , x2 of the Quadratic Equation Ax^2+Bx+G = 0
            /// </summary>
            /// <param name="a">A Part of Equation Ax^2+Bx+G</param>
            /// <param name="b">B Part of Equation Ax^2+Bx+G</param>
            /// <param name="c">G Part of Equation Ax^2+Bx+G</param>
            /// <returns> x1 , x2 or NaN if Equation has no Real Solutions </returns>
            public static double[] SolveQuadraticEquation(double a, double b, double c)
            {
                double x1;
                double x2;

                double D = GetDiscriminant(a, b, c);

                if (D > 0) //Two Solutions
                {
                    double sqrtD = Math.Sqrt(D);
                    x1 = (-b + sqrtD) / (2 * a);
                    x2 = (-b - sqrtD) / (2 * a);
                    return [x1, x2];
                }
                else if (D is 0) //One (Double) Solution
                {
                    x1 = -b / (2 * a);
                    //x2 = x1;
                    return [x1];
                }
                else //No real Solutions
                {
                    return [];
                }
            }
            /// <summary>
            /// Gets the Discriminant of a quadratic Equation aX^2 + bx + c = 0
            /// </summary>
            /// <param name="a">Term 'a' of the equation</param>
            /// <param name="b">Term 'b' of the equation</param>
            /// <param name="c">Term 'c' of the equation</param>
            /// <returns>The Discriminant of the quadratic Equation</returns>
            public static double GetDiscriminant(double a, double b, double c)
            {
                double D = Math.Pow(b, 2) - 4 * a * c;
                return D;
            }
            /// <summary>
            /// Converts degrees to radians
            /// </summary>
            /// <param name="degrees"></param>
            /// <returns></returns>
            public static double DegreesToRadians(double degrees)
            {
                return Math.PI / 180 * degrees;
            }
            /// <summary>
            /// Converts radians to degrees
            /// </summary>
            /// <param name="rad">The Radians to convert into degrees</param>
            /// <returns>The Degrees</returns>
            public static double RadiansToDegrees(double rad)
            {
                return 180 / Math.PI * rad;
            }
            /// <summary>
            /// Normalizes an angle which is in Radians and corrects it to be within 0 to 2π 
            /// <para>The Normalization takes into account also weather the angle is negative</para>
            /// </summary>
            /// <param name="angleRad">The Angle in Radians</param>
            /// <returns>The normalized angle from 0 to 2π </returns>
            public static double NormalizeAngle(double angleRad)
            {
                double normalizedAngle = angleRad % (2 * Math.PI);
                // Ensure the angle is positive
                if (normalizedAngle < 0)
                {
                    normalizedAngle += 2 * Math.PI;
                }
                return normalizedAngle;
            }
            public static double NormalizeAngleDegrees(double angleDegrees)
            {
                double normalizedAngle = angleDegrees % 360;
                // Ensure the angle is positive
                if (normalizedAngle < 0) normalizedAngle += 360;
                return normalizedAngle;
            }
        }
        public static class Circle
        {
            /// <summary>
            /// Returns the Perimeter of a circle 
            /// </summary>
            /// <param name="radius"></param>
            /// <returns></returns>
            public static double GetPerimeter(double radius)
            {
                return 2 * Math.PI * radius;
            }
            public static PointXY[] GetIntersectionsWithLine(PointXY circleCenter, double radius, PointXY linePoint1, PointXY linePoint2)
            {
                var line = Line.GetLineEquation(linePoint1, linePoint2);
                var circle = new CircleEquation(circleCenter.X, circleCenter.Y, radius);
                return GetIntersectionsWithLine(circle, line);
            }
            public static PointXY[] GetIntersectionsWithLine(CircleEquation circle, LineEquation line)
            {
                if (line.B == 0) //means line equation is x = someNumber and all points in line have the same x value x = -C/A
                {
                    var x = -line.C / line.A;
                    var ySolutions = Circle.SolveCircleEquationToY(circle.Cx, circle.Cy, circle.Radius, x);
                    //put its solution the same x , can be 0 solutions for no intersection or 1 solution for tangent or 2 solutions for intersecting
                    return ySolutions.Select(y => new PointXY(x, y)).ToArray();
                }
                else
                {
                    //Solve the line Equation to Y assuming B != 0
                    // y = -A/B * x - C/B
                    // put y into the circle equation and solve for x gives a quadratic equation of type:
                    // ax^2 + bx + c = 0 where :
                    // a = A^2/B^2 + 1 , b= 2 * ((A*C/B^2) + (A/B * Cy) - Cx) , c = ((C^2/B^2) + (2*C / B * Cy) + (Cy ^2) - r^2
                    // find a,b,c and solve the quadratic equation for x = (-b +- sqrt(b^2 - 4*a*c) ) / 2*a
                    var a = (Math.Pow(line.A, 2) / Math.Pow(line.B, 2)) + 1;
                    var b = 2 * ((line.A * line.C / Math.Pow(line.B, 2)) + ((line.A / line.B * circle.Cy) - circle.Cx));
                    var c = (Math.Pow(line.C, 2) / Math.Pow(line.B, 2)) + (2 * line.C / line.B * circle.Cy) + Math.Pow(circle.Cy, 2) - Math.Pow(circle.Radius, 2);
                    var x1x2 = VariousMath.SolveQuadraticEquation(a, b, c);
                    List<PointXY> solutions = [];
                    foreach (var solution in x1x2)
                    {
                        var y = (-line.A / line.B) * solution - line.C / line.B;
                        solutions.Add(new(solution, y));
                    }
                    return [.. solutions];
                }
            }
            public static double[] SolveCircleEquationToY(double cx, double cy, double radius, double x)
            {
                // Calculate the difference in x between the point and the circle's center
                double deltaX = x - cx;

                // Check if the point is outside the circle's radius
                if (Math.Abs(deltaX) > radius)
                {
                    // No real intersection points
                    //throw new InvalidOperationException($"The point with X:{x} does not intersect the circle with cx:{cx},cy:{cy} and radius:{radius}.");
                    return []; //Point is not part of the circle
                }

                // Calculate the Y component based on the circle's equation
                double deltaY = Math.Sqrt(radius * radius - deltaX * deltaX);

                // one single solution
                if (deltaY == 0) return [cy];

                // Calculate the actual Y coordinates of the intersection points
                double y1 = cy + deltaY; // Point above the center
                double y2 = cy - deltaY; // Point below the center

                return [y1, y2];
                /*
                 The line in this context is given by the equation x=cx=c, where cc is a specific value representing the x-coordinate of all points on the vertical line. The circle's equation, centered at (cx,cy)(cx,cy) with a radius of radiusradius, is given by:

                    (x−cx)2+(y−cy)2=radius2(x−cx)2+(y−cy)2=radius2
                    
                    When we substitute x=cx=c into the circle's equation, we focus on solving for yy. This substitution gives us the distance in the x-direction between the line and the circle's center, which is δX=c−cxδX=c−cx. By plugging δXδX into the circle's equation, we aim to find the corresponding yy values (or the vertical distance δYδY) that satisfy the circle's equation.
                    
                    Here's the breakdown:
                    
                        Starting with the Circle's Equation:
                        (c−cx)2+(y−cy)2=radius2(c−cx)2+(y−cy)2=radius2
                    
                        Solving for yy:
                        We rearrange the equation to isolate yy, focusing on the term (y−cy)2(y−cy)2 which represents the squared distance in the y-direction from the circle's center.
                    
                    (y−cy)2=radius2−(c−cx)2(y−cy)2=radius2−(c−cx)2
                    
                        Square Root:
                        To solve for y−cyy−cy, we take the square root of both sides:
                    
                    y−cy=±radius2−(c−cx)2y−cy=±radius2−(c−cx)2
                    
                    This equation shows the distance (δYδY) above and below the circle's center (cycy) where the line intersects the circle. The ±± indicates there are two potential values for yy, one above the center and one below, corresponding to the upper and lower intersection points with the circle, assuming an intersection exists.
                    
                    Calculate δYδY:
                    δY=radius2−δX2δY=radius2−δX2
                    
                        ​
                    
                    Here, δY=radius2−(c−cx)2δY=radius2−(c−cx)2
                    
​                    is the y-distance from the center of the circle to the points of intersection. This is what the code does when it calculates deltaY.
                    
                    Find the Actual yy Coordinates:
                    Finally, we add and subtract δYδY to/from cycy (the y-coordinate of the circle's center) to find the actual yy coordinates of the intersection points:
                    
                    y1=cy+δYy1​=cy+δY
                    y2=cy−δYy2​=cy−δY
                 
                 */
            }
            /// <summary>
            /// Returns a point on the Circle's Perimeter for a given angle
            /// </summary>
            /// <param name="circle">The Circle</param>
            /// <param name="angleRadians">The angle with X axis</param>
            /// <returns></returns>
            public static PointXY GetPointOnCirclePerimeter(CircleInfo circle, double angleRadians)
            {
                return GetPointOnCirclePerimeter(circle.Radius, circle.LocationX, circle.LocationY, angleRadians);
            }
            /// <summary>
            /// Returns a point on the Circle's Perimeter for a given angle
            /// </summary>
            /// <param name="circleRadius">The Radius of the Circle</param>
            /// <param name="centerX">The Circle's Center X Coordinate</param>
            /// <param name="centerY">The Circle's Center Y Coordinate</param>
            /// <param name="angleRadians">The Angle of the point in the circles perimeter in Radians</param>
            /// <returns>The Point on the perimeter</returns>
            public static PointXY GetPointOnCirclePerimeter(double circleRadius, double centerX, double centerY, double angleRadians)
            {
                double x = centerX + circleRadius * Math.Cos(angleRadians);
                double y = centerY + circleRadius * Math.Sin(angleRadians);
                return new PointXY(x, y);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="circle"></param>
            /// <param name="angleRadians">The Angle from which we should masure the distance from the perimeter (the position on the circle perimeter)</param>
            /// <param name="distanceFromPerimeter">The length away from the perimeter , works for negative also</param>
            /// <returns></returns>
            public static PointXY GetPointInDistanceFromCirclePerimeter(CircleInfo circle , double angleRadians , double distanceFromPerimeter)
            {
                //The Unit Vector towards the certain direction is (cos(angle),sing(angle))
                //we could do vect.UnitVector , but we already calculate the unit vector here with the angle
                //Multiply the Unit vector by the total needed distance in each of the x,y coordinates to find the new point
                //Add the centerX,centerY of the circle to the result to get the correct point location compared to the circles center
                double distance = circle.Radius + distanceFromPerimeter;
                Vector2D vect = new(Math.Cos(angleRadians) * distance, Math.Sin(angleRadians) * distance);
                return new PointXY(vect.X + circle.CenterX,vect.Y + circle.CenterY );
                
            }
            /// <summary>
            /// Returns the Vertices of an inscribed polygon in the provided Circle
            /// <para>The Vertices are given in clockwise Order</para>
            /// <para>If provided rotation is '0' then the base edge will be parallel to X Axis</para>
            /// </summary>
            /// <param name="circle">The circle from which to create the polygon</param>
            /// <param name="numberOfSides">The Number of sides of the Polygon</param>
            /// <param name="rotationAngleRadians">The Result of this function is always a base of the polygon is parallel to X , plus any rotation given (by default rotation is 'zero')</param>
            /// <returns></returns>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            public static List<PointXY> GetInscribedPolygonVertices(CircleInfo circle, int numberOfSides, double rotationAngleRadians = 0)
            {
                if (numberOfSides <= 2)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(numberOfSides)}", "An Inscribed Circle Polygon must have more than 2 sides");
                }

                double anglePerSide = 2 * Math.PI / numberOfSides;

                // Adjust the starting angle to make the bottom edge parallel to the X-axis
                // This offset ensures the polygon's base is aligned with the X-axis
                double startingAngle = ((-Math.PI / numberOfSides) + Math.PI / 2) + rotationAngleRadians;

                List<PointXY> vertices = [];
                for (int i = 0; i < numberOfSides; i++)
                {
                    double angle = startingAngle + i * anglePerSide;
                    var point = GetPointOnCirclePerimeter(circle, angle);
                    vertices.Add(point);
                }
                return vertices;
            }
            /// <summary>
            /// Calculates and returns a list of points along the arc of a circle based on the specified number of points and angle range.
            /// </summary>
            /// <param name="circle">The circle's information, including its center and radius.</param>
            /// <param name="numberOfPoints">The number of points to generate along the arc.</param>
            /// <param name="startAngleRad">The starting angle of the arc, in radians.</param>
            /// <param name="endAngleRad">The ending angle of the arc, in radians.</param>
            /// <returns>A list of points representing the arc of the circle.</returns>
            public static List<PointXY> GetCircleArcPoints(CircleInfo circle, int numberOfPoints, double startAngleRad, double endAngleRad)
            {
                // Return an empty list if no points are requested.
                if (numberOfPoints <= 0 || startAngleRad == endAngleRad) return [];

                // If only one point is requested, return the midpoint of the arc.
                if (numberOfPoints == 1)
                    return [MathCalculations.Circle.GetPointOnCirclePerimeter(circle, startAngleRad + (endAngleRad - startAngleRad) / 2d)];

                var points = new List<PointXY>();

                // Calculate the angle between each point along the arc.
                double angleStep = (endAngleRad - startAngleRad) / (numberOfPoints - 1);

                // Loop through the arc and calculate points at each step.
                for (int i = 0; i < numberOfPoints; i++)
                {
                    double currentAngle = startAngleRad + i * angleStep;
                    var newPoint = MathCalculations.Circle.GetPointOnCirclePerimeter(circle, currentAngle);
                    points.Add(newPoint);
                }

                return points;
            }
            /// <summary>
            /// Returns weather a Point is inside a Circle
            /// </summary>
            /// <param name="circle"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static bool IsPointInCircle(CircleInfo circle, PointXY point)
            {
                //Calculate points Squared Distance and Compare it with the Squared Radius
                //If bigger or equal then the point is not inside the Circle
                var squaredDistance = MathCalculations.Points.GetPointsSquaredDistance(circle.GetLocation(), point);
                //Account for Floating Point Errors
                return squaredDistance < Math.Pow(circle.Radius, 2) + DoubleSafeEqualityComparer.DefaultEpsilon;
            }

        }
        public static class CircleSegment
        {
            public static bool IsThetaGreaterThan180(double chordLength, double sagitta)
            {
                return chordLength / 2d < sagitta + DoubleSafeEqualityComparer.DefaultEpsilon;
            }
            public static bool IsThetaGreaterOrEqualThan180(double chordLength, double sagitta)
            {
                return chordLength / 2d <= sagitta + DoubleSafeEqualityComparer.DefaultEpsilon;
            }
            public static bool IsThetaLesserOrEqualThan180(double chordLength, double sagitta)
            {
                return chordLength / 2d >= sagitta - DoubleSafeEqualityComparer.DefaultEpsilon;
            }
            public static bool IsThetaLesserThan180(double chordLength, double sagitta)
            {
                return chordLength / 2d > sagitta - DoubleSafeEqualityComparer.DefaultEpsilon;
            }
            /// <summary>
            /// Returns the perimeter of a circle chord seagment
            /// <para>L = 2 * r * ARCSIN(c / ( 2 * r ) ) + 2 * s * SQRT( (1 - (c / ( (2 * r) ^2) )))</para>
            /// </summary>
            /// <param name="radius"></param>
            /// <param name="chordLength"></param>
            /// <param name="sagitta"></param>
            /// <returns></returns>
            public static double GetPerimeter(double radius, double chordLength, double sagitta)
            {
                if (radius <= 0 || sagitta <= 0 || chordLength <= 0) return double.NaN;

                double arcsinPart = 2 * radius * Math.Asin(chordLength / (2 * radius));
                double poweredPart = Math.Pow(chordLength / (2 * radius), 2);
                double sqrtPart = 2 * sagitta * Math.Sqrt(1 - poweredPart);
                return arcsinPart + sqrtPart;
            }
            /// <summary>
            /// Retrives the Radius of the circle forming the chord seagment
            /// R = (sagitta ^ 2 + (L/2)^2 ) / (2 * sagitta)
            /// </summary>
            /// <param name="chordLength">The Length of the Chord</param>
            /// <param name="sagitta">The Sagitta of the chord</param>
            /// <returns></returns>
            public static double GetRadius(double chordLength, double sagitta)
            {
                if (sagitta == 0) return 0;
                if (chordLength == 0) return sagitta / 2d;

                var sagittaSquared = Math.Pow(sagitta, 2);
                var halfLengthSquared = Math.Pow(chordLength * 0.5d, 2);
                return ((sagittaSquared + halfLengthSquared) / (2 * sagitta));
            }
            /// <summary>
            /// The Theta angle of a Circle Seagment (how many degrees of a circle's 360 are used from the Seagment)
            /// </summary>
            /// <param name="chordLength">The Length of the seagments Chord</param>
            /// <param name="radius">The Radius of the Circle used to form this seagment</param>
            /// <returns>The Theta angle</returns>
            public static double GetTheta(double chordLength, double radius, double sagitta)
            {
                if (chordLength == 0 || radius == 0) return 0;
                // if sagitta < radius => small segment
                var isSmall = sagitta < radius;

                var thetta = 2 * Math.Asin(chordLength / (2 * radius));
                return isSmall ? thetta : 2 * Math.PI - thetta;
            }
            /// <summary>
            /// Gets the Height of a Chord called Sagitta (Distance from Midpoint of Chord to the Arcs highest point)
            /// </summary>
            /// <param name="radius">The radius of the Circle Forming the seagment</param>
            /// <param name="thetaAngle">The Theta angle of the Seagment</param>
            /// <returns></returns>
            public static double GetSagittaWithAngle(double radius, double thetaAngle)
            {
                if (radius == 0 || thetaAngle == 0) return 0;

                double sagitta = radius - (radius * Math.Cos(thetaAngle * 0.5d));

                return double.IsNaN(sagitta) ? 0 : sagitta;
            }
            /// <summary>
            /// Gets the Length of the Seagments Chord
            /// </summary>
            /// <param name="radius">The radius of the circle that forms the seagment</param>
            /// <param name="thetaAngle">The theta Angle of the Seagment</param>
            /// <returns>The Chord's length </returns>
            public static double GetChordLengthWithAngle(double radius, double thetaAngle)
            {
                if (radius == 0 || thetaAngle == 0) return 0;
                //Length of Chord is L = 2 * R * sin ( THETAradians / 2 )
                double chordLength = 2 * (radius) * Math.Sin(thetaAngle * 0.5d);
                return double.IsNaN(chordLength) ? 0 : chordLength;
            }
            /// <summary>
            /// Returns the ChordLength of a Segment by defining a Radius and Sagitta 
            /// If the Sagitta is more than the Diameter then it returns double.NaN (undefinable Segment)
            /// </summary>
            /// <param name="radius">Radius of the Defining Circle of the Segment</param>
            /// <param name="sagitta">The Sagitta of the Segment</param>
            /// <returns>The Chord Length or double.NaN if the Sagitta is more than the circle's Diameter</returns>
            public static double GetChordLengthWithSagitta(double radius, double sagitta)
            {
                if (radius == 0 || sagitta == 0) return 0;
                // Chord = 2 * SQRT(2*R*S - S^2)
                //Meaning R>=S/2 in order for the Segment to have meaning
                var sqrt = 2 * radius * sagitta - Math.Pow(sagitta, 2);
                if (sqrt < 0)
                {
                    return double.NaN;
                }
                else
                {
                    return 2 * Math.Sqrt(sqrt);
                }
            }
            /// <summary>
            /// Returns the circle Equation Instance forming the chord seagment
            /// </summary>
            /// <param name="chordLength">The Length of the Chord</param>
            /// <param name="sagitta">The Sagitta of the Chord</param>
            /// <param name="chordMiddle">The Middle of the Chord</param>
            /// <param name="angleOfBisector">The Angle Formed by the Middle bisector of the cord with the X Axis (bisector passes from the center of the Circle always)</param>
            /// <param name="isAngleRad">wheather the angle of the bisector is provided in radians (true) or degrees (false)</param>
            /// <returns>The center point of the circle, if chord length or sagitta are zero returns the chord middle point provided</returns>
            public static CircleEquation GetParentCircleEquation(double chordLength, double sagitta, PointXY chordMiddle, double angleOfBisector, bool isAngleRad = true)
            {
                if (chordLength == 0 || sagitta == 0)
                {
                    //There is no circle or chord everything is a point, so just return the point
                    return new CircleEquation(chordMiddle.X, chordMiddle.Y, 0);
                }
                //from the two triangles forming from the center of circle to the edge of the chord and the center of the chord.
                //Find the Radius and then the distance from chord center to circle is R-Sagitta
                var radius = GetRadius(chordLength, sagitta);
                var distanceFromChordCenter = radius - sagitta;
                var rad = angleOfBisector;
                if (!isAngleRad) rad = VariousMath.DegreesToRadians(angleOfBisector);
                //Then Cx = chordMiddle.X + distanceFromChordCenter * Math.Cos(rad);
                //Then Cy = chordMiddle.Y + distanceFromChordCenter * Math.Sin(rad);
                var cx = chordMiddle.X + distanceFromChordCenter * Math.Cos(rad);
                var cy = chordMiddle.Y + distanceFromChordCenter * Math.Sin(rad);

                return new(cx, cy, radius);
            }
        }
        public static class CircleQuadrant
        {
            /// <summary>
            /// Returns the Perimeter of a circle quadrant
            /// </summary>
            /// <param name="radius">The Radius of the Circle that defined the Quadrant</param>
            /// <returns></returns>
            public static double GetPerimeter(double radius)
            {
                return ((Math.PI * radius) / 2) + (2 * radius);
            }
        }
        public static class Egg
        {
            /// <summary>
            /// Returns the perimeter of an egg Shape
            /// </summary>
            /// <param name="circleRadius">The Radius of the Circle in the Egg Shape</param>
            /// <param name="ellipseRadius">The Radius of the Ellipse in the Egg Shape</param>
            /// <returns></returns>
            public static double GetPerimeter(double circleRadius, double ellipseRadius)
            {
                var circlePartPerimeter = Circle.GetPerimeter(circleRadius) / 2;
                var ellipsePartPerimeter = Ellipse.GetPerimeter(circleRadius, ellipseRadius) / 2;
                return circlePartPerimeter + ellipsePartPerimeter;
            }
        }
        public static class Ellipse
        {
            /// <summary>
            /// Returns an approximation of the perimeter of an ellipse(Ramanujan Approximation)
            /// <para>P ~ pi * [ 3 *( a + b ) - SQRT( (3 * a + b) * (a + 3 * b) ) </para>
            /// </summary>
            /// <param name="ellipseRadius1">One of the Ellipse Radii</param>
            /// <param name="ellipseRadius2">The other ellipse Radius</param>
            /// <returns>The Ellipse perimeter with a margin of error of a few percent based on the ellipse eccentricity</returns>
            public static double GetPerimeter(double ellipseRadius1, double ellipseRadius2)
            {
                if (ellipseRadius1 <= 0 || ellipseRadius2 <= 0) return 0;

                //Major Axis
                double a = Math.Max(ellipseRadius1, ellipseRadius2);
                //Minor Axis
                double b = Math.Min(ellipseRadius1, ellipseRadius2);

                double sqrtPart = Math.Sqrt((3 * a + b) * (a + 3 * b));
                double restPart = 3 * (a + b);
                return (Math.PI * (restPart - sqrtPart));
            }
            /// <summary>
            /// Returns the Radial Distance from the Ellipse Center to the Ellipse Perimeter for a given angle
            /// The ellipse radiuses are accepted in any order as arguments
            /// </summary>
            /// <param name="radius1">One of the Radiuses of the Ellipse</param>
            /// <param name="radius2">The Other Radius of the Ellipse</param>
            /// <param name="angleRadians">The Angle forming with the center of the Ellipse in Radians</param>
            /// <returns>The Solution of : R(Θ) = (a * b) / ( (b*cos(θ)^2) + (a*sin(θ)^2) )^0.5  , where a Major Axis Radius , b Minor Axis Radius , θ angleRadians , R radial Distance</returns>
            public static double GetEllipseRadialDistance(double radius1, double radius2, double angleRadians)
            {
                if (radius1 <= 0 || radius2 <= 0)
                {
                    return 0;
                }

                double a = Math.Max(radius1, radius2);
                double b = Math.Min(radius2, radius1);

                double bcosTheta = b * Math.Cos(angleRadians);
                double asinTheta = a * Math.Sin(angleRadians);

                double squaredBcosTheta = Math.Pow(bcosTheta, 2);
                double squaredAsinTheta = Math.Pow(asinTheta, 2);

                return ((a * b) / Math.Sqrt(squaredBcosTheta + squaredAsinTheta));
            }

            /// <summary>
            /// Returns a point on the Ellipse Perimeter for a given angle
            /// </summary>
            /// <param name="rx">The X axis Radius</param>
            /// <param name="ry">The Y axis Radius</param>
            /// <param name="angleRadians">The Angle forming from the center of the Ellipse to the Point</param>
            /// <param name="cx">The CenterX of the Ellipse</param>
            /// <param name="cy">The CenterY of the Ellipse</param>
            /// <returns>the x,y coordinates of a Point in the Ellipse's Perimeter</returns>
            public static PointXY GetPointOnEllipsePerimeter(double rx, double ry, double angleRadians, double cx = 0, double cy = 0)
            {
                //The Parametric equations x = rx * Math.Cos(theta) + cx , y = ry * Math.Sin(theta) + cy
                if (rx <= 0 || ry <= 0)
                {
                    return new(0, 0);
                }

                double x = rx * Math.Cos(angleRadians) + cx;
                double y = ry * Math.Sin(angleRadians) + cy;

                return new(x, y);
            }
            /// <summary>
            /// Returns a point on the Ellipse Perimeter for a given angle
            /// </summary>
            /// <param name="ellipse">The Ellipse</param>
            /// <param name="angleRadians">The Angle forming from the center of the Ellipse to the Point</param>
            /// <returns>the x,y coordinates of a Point in the Ellipse's Perimeter</returns>
            public static PointXY GetPointOnEllipsePerimeter(EllipseInfo ellipse, double angleRadians)
            {
                // Find the Horizontal and Vertical Radiuses
                double rx;
                double ry;
                switch (ellipse.Orientation)
                {
                    case Enums.EllipseOrientation.Horizontal:
                        rx = ellipse.RadiusMajor;
                        ry = ellipse.RadiusMinor;
                        break;
                    case Enums.EllipseOrientation.Vertical:
                        rx = ellipse.RadiusMinor;
                        ry = ellipse.RadiusMajor;
                        break;
                    default:
                        throw new EnumValueNotSupportedException(ellipse.Orientation);
                }
                return GetPointOnEllipsePerimeter(rx, ry, angleRadians, ellipse.LocationX, ellipse.LocationY);
            }

            /// <summary>
            /// Generates a list of points along an arc of an ellipse defined by the specified angles.
            /// </summary>
            /// <param name="ellipse">The ellipse information containing the semi-major and semi-minor axes.</param>
            /// <param name="numberOfPoints">The number of points to generate along the arc.</param>
            /// <param name="startAngleRad">The starting angle of the arc in radians.</param>
            /// <param name="endAngleRad">The ending angle of the arc in radians.</param>
            /// <returns>A list of points along the specified arc of the ellipse.</returns>
            public static List<PointXY> GetEllipseArcPoints(EllipseInfo ellipse, int numberOfPoints, double startAngleRad, double endAngleRad)
            {
                // Return an empty list if no points are requested.
                if (numberOfPoints <= 0 || startAngleRad == endAngleRad)
                {
                    return [];
                }

                // If only one point is requested, return the midpoint of the arc.
                if (numberOfPoints == 1)
                {
                    double midpointAngle = startAngleRad + (endAngleRad - startAngleRad) / 2d;
                    return [MathCalculations.Ellipse.GetPointOnEllipsePerimeter(ellipse, midpointAngle)];
                }

                var points = new List<PointXY>();

                // Calculate the angle between each point along the arc.
                double angleStep = (endAngleRad - startAngleRad) / (numberOfPoints - 1);

                // Loop through the arc and calculate points at each step.
                for (int i = 0; i < numberOfPoints; i++)
                {
                    double currentAngle = startAngleRad + i * angleStep;
                    var newPoint = MathCalculations.Ellipse.GetPointOnEllipsePerimeter(ellipse, currentAngle);
                    points.Add(newPoint);
                }

                return points;
            }
        }
        public static class Rectangle
        {
            /// <summary>
            /// Returns the perimeter of a rectangle
            /// </summary>
            /// <param name="length">The Length of the Rectangle</param>
            /// <param name="height">The Height of the Rectangle</param>
            /// <returns></returns>
            public static double GetPerimeter(double length, double height)
            {
                return 2 * (length + height);
            }
            /// <summary>
            /// Returns the Distance (diagonal at 45deg) from the Edge of the Rectangle to the Middle of the Arc formed by a Corner Radius
            /// </summary>
            /// <param name="cornerRadius"></param>
            /// <returns></returns>
            public static double GetDistanceFromCornerRadiusArcMiddleToRectangleEdge(double cornerRadius)
            {
                //The Corner Radius Formed in a rectangle Edge is always a CircleQuadrant (as rx,ry radius is the same)
                //Also the dx,dy distances from the edge of the Rectangle are = cornerRadius (form a Square)
                //If we assume the Corner Radius is TopLeft then the Circle's center is located at the bottom Right edge of this small Square
                //Drawing a quadrant with this as center and then appling pythagorean theorem we find the below equation
                //distances x + y are the same = corner Radius
                var squaredCornerRadius = Math.Pow(cornerRadius, 2);
                return Math.Sqrt(squaredCornerRadius + squaredCornerRadius);
            }
            /// <summary>
            /// Returns the shift on X,Y Axis that is applied to the Edge Point when corner radius is Applied 
            /// </summary>
            /// <param name="cornerRadius"></param>
            /// <returns></returns>
            public static double GetXYShiftFromRectangleEdgeToCornerRadiusArcMiddle(double cornerRadius)
            {
                var total45DegDistance = GetDistanceFromCornerRadiusArcMiddleToRectangleEdge(cornerRadius);
                var lostDistanceFromAppliedArc = total45DegDistance - cornerRadius;

                //Pythagorean ,both shiftX and shiftY are the same (shift^2 + shift^2 = lostDistanceFromAppliedArc^2 => Sqrt(lostdist^2/2) )
                return lostDistanceFromAppliedArc / Math.Sqrt(2);
            }
            /// <summary>
            /// Returns weather a Point is inside a Simple Rectangle(No Corner Radiuses)
            /// </summary>
            /// <param name="rectangle"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static bool IsPointInSimpleRectangle(RectangleInfo rectangle, PointXY point)
            {
                //Account for floating point errors
                return point.X > rectangle.LeftX - DoubleSafeEqualityComparer.DefaultEpsilon &&
                    point.X < rectangle.RightX + DoubleSafeEqualityComparer.DefaultEpsilon &&
                    point.Y > rectangle.TopY - DoubleSafeEqualityComparer.DefaultEpsilon &&
                    point.Y < rectangle.BottomY + DoubleSafeEqualityComparer.DefaultEpsilon;
            }
            /// <summary>
            /// Returns the closest point on the Rectangle Perimeter to the provided point
            /// </summary>
            /// <param name="rectangle"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static PointXY GetClosestPointOnPerimeterFromPoint(RectangleInfo rectangle, PointXY point)
            {
                //Check if the point is outside the Rectangle 
                bool isOutside = !rectangle.IntersectsWithPoint(point);

                if (isOutside)
                {
                    double clampedX = Math.Clamp(point.X, rectangle.LeftX, rectangle.RightX);
                    double clampedY = Math.Clamp(point.Y, rectangle.TopY, rectangle.BottomY);
                    return new(clampedX, clampedY);
                }
                else
                {
                    // The point is inside the rectangle; find the closest edge
                    //Calculate distance to each edge :

                    double distanceToLeft = point.X - rectangle.LeftX;
                    double distanceToRight = rectangle.RightX - point.X;
                    double distanceToTop = point.Y - rectangle.TopY;
                    double distanceToBottom = rectangle.BottomY - point.Y;

                    // Find the minimum distance
                    double minDistance = Math.Min(Math.Min(distanceToLeft, distanceToRight),
                                                 Math.Min(distanceToTop, distanceToBottom));
                    // Determine which edge is the closest and set the closest point accordingly
                    if (minDistance == distanceToLeft)
                    {
                        return new PointXY(rectangle.LeftX, point.Y);
                    }
                    else if (minDistance == distanceToRight)
                    {
                        return new PointXY(rectangle.RightX, point.Y);
                    }
                    else if (minDistance == distanceToTop)
                    {
                        return new PointXY(point.X, rectangle.TopY);
                    }
                    else // minDistance == distanceToBottom
                    {
                        return new PointXY(point.X, rectangle.BottomY);
                    }
                }
            }
            /// <summary>
            /// Returns the Minimum Distance between Axis Aligned Rectangles (The distance of their closest edges)
            /// </summary>
            /// <param name="rect1"></param>
            /// <param name="rect2"></param>
            /// <returns>Returns the closes distance or "0" if the Rectangles are Colliding</returns>
            public static double GetMinimumDistanceBetweenAxisAlignedRectangles(RectangleInfo rect1,RectangleInfo rect2)
            {
                // Calculate horizontal distance
                double dx = 0;
                if (rect1.RightX < rect2.LeftX)
                    dx = rect2.LeftX - rect1.RightX;
                else if (rect2.RightX < rect1.LeftX)
                    dx = rect1.LeftX - rect2.RightX;

                // Calculate vertical distance
                double dy = 0;
                if (rect1.BottomY < rect2.TopY)
                    dy = rect2.TopY - rect1.BottomY;
                else if (rect2.BottomY < rect1.TopY)
                    dy = rect1.TopY - rect2.BottomY;

                // Return Euclidean distance if rectangles do not overlap; otherwise, zero
                return Math.Sqrt(dx * dx + dy * dy);
            }
        }
        public static class Capsule
        {
            /// <summary>
            /// Returns the perimeter of a capsule
            /// </summary>
            /// <param name="height">The Height of the Capsule</param>
            /// <param name="length">The Length of the Capsule</param>
            /// <returns></returns>
            public static double GetPerimeter(double height, double length)
            {
                double majorPart = Math.Max(height, length);
                double minorPart = Math.Min(height, length);

                double circleRadius = minorPart / 2;
                double rectangleMajorPart = majorPart - minorPart;

                double circlePartsPerimeter = Circle.GetPerimeter(circleRadius);
                return 2 * rectangleMajorPart + circlePartsPerimeter;
            }
        }
        public static class Line
        {
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
                    //To find weather is -infinite or +infinite as it plays a role to calculate angles
                    if (y1 < y2)
                    {
                        slope = double.PositiveInfinity; //The Slope is Positive Inifinity
                    }
                    else if (y1 > y2)
                    {
                        slope = double.NegativeInfinity; //The Slope is Negative Inifinity
                    }
                    else
                    {
                        slope = double.NaN; //The Slope is undefined
                    }
                }
                else
                {
                    slope = (y2 - y1) / (x2 - x1);
                }
                return slope;
            }
            public static double GetLineSlope(LineEquation line)
            {
                if (line.B == 0) return double.PositiveInfinity;
                else return -line.A / line.B;
            }

            /// <summary>
            /// Returns a Line equation from two Points (x1,y1) and (x2,y2)
            /// </summary>
            /// <param name="x1"></param>
            /// <param name="y1"></param>
            /// <param name="x2"></param>
            /// <param name="y2"></param>
            /// <returns></returns>
            public static LineEquation GetLineEquation(double x1, double y1, double x2, double y2)
            {
                ////Ax + By + C = 0 Line Equation
                //if (x1 == x2 && y1 == y2)
                //{
                //    throw new InvalidOperationException($"{nameof(LineEquation)} cannot be calculated from the same point {nameof(x1)},{nameof(y1)} must not be the same with {nameof(x2)},{nameof(y2)}");
                //}

                //A = y1-y2
                var a = y1 - y2;
                //B = x2-x1
                var b = x2 - x1;
                //C = x1*y2-x2*y1
                var c = x1 * y2 - x2 * y1;
                return new LineEquation(a, b, c);
            }
            /// <summary>
            /// Returns a Line equation from two Points (x1,y1) and (x2,y2)
            /// </summary>
            /// <param name="point1"></param>
            /// <param name="point2"></param>
            /// <returns></returns>
            public static LineEquation GetLineEquation(PointXY point1, PointXY point2)
            {
                return GetLineEquation(point1.X, point1.Y, point2.X, point2.Y);
            }
            public static LineEquation GetLineEquation(double slope, double x1, double y1)
            {
                if (double.IsNaN(slope))
                {
                    //returns the form X = x1; deriving from Ax+ By + C = 0 where solving it for X is Ax = -C;
                    return new LineEquation(1, 0, -x1);
                }
                else
                {
                    //Get the Usual form when slope is not NaN y = ax + b
                    // A = a
                    // B = -1
                    // C = y1 - a * x1
                    double a = slope;
                    double b = -1;
                    double c = -b * y1 - a * x1;
                    return new LineEquation(a, b, c);
                }
            }
            public static LineEquation GetLineEquation(double x1, double y1, double angle, bool angleInRad)
            {
                var rad = angle;
                if (!angleInRad)
                {
                    rad = VariousMath.DegreesToRadians(angle);
                }

                // A = sing(theta)
                var a = Math.Sin(rad);
                var b = -Math.Cos(rad);
                var c = -b * y1 - a * x1;
                return new LineEquation(a, b, c);
            }
            /// <summary>
            /// Gets the Equation of a prependicular to the line of the Argument that intersects it in the given point
            /// </summary>
            /// <param name="line">The Equation of the Line which intersects with its prependicular</param>
            /// <param name="pointOnPrependicular">A point of the prependicular line</param>
            /// <returns></returns>
            public static LineEquation GetPerpendicular(LineEquation line, PointXY pointOnPrependicular)
            {
                //calculate the slope of the current Line
                double slope = GetLineSlope(line);
                if (double.IsPositiveInfinity(slope))
                {
                    // 0*x + 1*Y - constant = 0 , For a Vertical Line prependicular is Horizontal y= constant
                    return new LineEquation(0, 1, -pointOnPrependicular.Y);
                }
                else if (slope is 0)
                {
                    // 1*x + 0*Y - constant = 0 , For a Horizontal Line prependicular is Vertical x= constant
                    return new LineEquation(1, 0, -pointOnPrependicular.X);
                }
                else
                {
                    //The Slope of a prependicular to a line is the inverse negative of the slope of the line (-1/slope)
                    double prependicularSlope = -1 / slope;
                    //Use the point-slope form to find the equation of the prependicular
                    //y-y1 = slope * (x-x1)
                    //The negative "-" is here because of the C in AX+BY+C = 0
                    double c = -prependicularSlope * pointOnPrependicular.X + pointOnPrependicular.Y;
                    return new LineEquation(prependicularSlope, -1, c);
                }

            }
            /// <summary>
            /// Returns the parallel lines that are at a certain distance
            /// </summary>
            /// <param name="line">The line for which to find the parallels</param>
            /// <param name="distance">The Distance of the parallels from the main one</param>
            /// <returns></returns>
            public static (LineEquation parallel1, LineEquation parallel2) GetParallelsAtDistance(LineEquation line, double distance)
            {
                if (line.A == 0 && line.B == 0) throw new InvalidOperationException($"A and B in a Line Equation of : Ax+By+C cannot be both zero");
                // Calculate distance from origin
                double distanceFromOrigin = Math.Abs(line.C) / Math.Sqrt(Math.Pow(line.A, 2) + Math.Pow(line.B, 2));

                // Calculate new C value for parallel lines
                double newC = line.C + (distance * distanceFromOrigin);

                // Calculate parallel lines
                LineEquation parallelLine1 = new(line.A, line.B, newC);
                LineEquation parallelLine2 = new(line.A, line.B, -newC);

                return (parallelLine1, parallelLine2);

                //Calculations INFO
                /* 
                 Calculate the Distance from Origin:

                 The distance of a line from the origin (0,0) can be calculated using the formula for the distance from a point to a line. In this case, the formula is:
                 
                 distanceFromOrigin= Math.Abs(C) / SQRT(A^2 + B^2)
                 
                 This formula represents the perpendicular distance from the origin to the line.
                 
                 Calculate the New C Value for Parallel Lines:
                 
                 To find the new constant term C for the equations of the parallel lines, we use the given distance d and the distance from the origin. Multiplying the distance d by the ratio of the new distance to the origin to the original distance from the origin gives us the change in C.
                 
                 newC = C + ( d × distanceFromOrigin ) 
                 
                 This accounts for moving the line parallel to itself by the given distance d.
                 
                 Calculate Parallel Lines:
                 
                 Once we have the new constant term CC for the parallel lines, we create two new LineEquation objects with the same coefficients A and B as the original line equation, but with the new C value and its negation. This ensures that the new lines are parallel to the original line and equidistant from it.
                 
                 parallelLine1:Ax+By+newC=0
                 
                 parallelLine2:Ax+By−newC=0
                 */
            }
            /// <summary>
            /// Returns the parallel lines that are at a certain distance
            /// </summary>
            /// <param name="line">The line for which to find the parallels</param>
            /// <param name="distance">The Distance of the parallels from the main one</param>
            /// <returns></returns>
            public static LineInfo[] GetParallelsAtDistance(LineInfo line, double distance)
            {
                var lineLength = line.GetTotalLength();
                if (lineLength is 0) return [new(0, 0, 0, 0), new(0, 0, 0, 0)];

                // Calculate the slope of lineSegment1
                double dx = line.EndX - line.StartX;
                double dy = line.EndY - line.StartY;
                double unitX = dx / lineLength;
                double unitY = dy / lineLength;

                // Calculate the offset vector (How much it will be moved in Y or X for each point of distance)
                double offsetX = distance * unitY;
                double offsetY = -distance * unitX;

                // Calculate the coordinates for the new line segments
                double newStartX1 = line.StartX + offsetX;
                double newStartY1 = line.StartY + offsetY;
                double newEndX1 = line.EndX + offsetX;
                double newEndY1 = line.EndY + offsetY;

                double newStartX2 = line.StartX - offsetX;
                double newStartY2 = line.StartY - offsetY;
                double newEndX2 = line.EndX - offsetX;
                double newEndY2 = line.EndY - offsetY;

                return [new(newStartX1, newStartY1, newEndX1, newEndY1), new(newStartX2, newStartY2, newEndX2, newEndY2)];
            }

            /// <summary>
            /// Returns the Points in a line that are of equaly distant from another point in the line
            /// </summary>
            /// <param name="line">The Line</param>
            /// <param name="point">The Point</param>
            /// <returns></returns>
            public static (PointXY point1, PointXY point2) GetPointsAtLineEquallyDistantFromPoint(LineEquation line, PointXY point, double distance)
            {
                //Check that the given point is in the Line otherwise throw
                if (!line.ContainsPoint(point)) throw new InvalidOperationException($"The given Point: ({point}) is not part of the given Line: {line}");
                var slope = GetLineSlope(line);
                double deltaX;
                double deltaY;
                if (double.IsPositiveInfinity(slope))
                {
                    deltaX = 0;
                    deltaY = distance;
                }
                else
                {
                    //slope is actually tan(θ)
                    //so from pythagorean : d = SQRT(Dx^2 + Dy^2)
                    //then square it d^2 = Dx^2 + Dy^2
                    //then tan(θ) = Dy/Dx  , Dy = Dx * tan(θ)
                    //Substitute Dy and we get : d^2 = Dx^2 +  Dx^2 * slope^2  , slope = tan(θ)
                    //Solve to Dx = Sqrt ( d^2 / (1+slope^2)
                    deltaX = Math.Sqrt((distance * distance) / (1 + slope * slope));
                    deltaY = slope * deltaX;
                }
                //Then  apply the delta's to get the two solutions
                return (new(point.X + deltaX, point.Y + deltaY), new(point.X - deltaX, point.Y - deltaY));
            }
            /// <summary>
            /// Returns the Distance of a Line from a Point
            /// </summary>
            /// <param name="line"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static double GetDistanceOfLineSegmentFromPoint(LineInfo line, PointXY point)
            {
                // Special case: if the line is actually a point (both endpoints are the same)
                if (line.StartX == line.EndX && line.StartY == line.EndY)
                {
                    // Return the distance between the point and this degenerate line segment
                    return Points.GetDistanceBetweenPoints(point.X, point.Y, line.StartX, line.StartY);
                }

                // Create vectors
                double lineVectorX = line.EndX - line.StartX;
                double lineVectorY = line.EndY - line.StartY;
                double pointVectorX = point.X - line.StartX;
                double pointVectorY = point.Y - line.StartY;

                // Project point onto the line (find t such that Start + t * (End - Start) is the closest point)
                double lineLengthSquared = lineVectorX * lineVectorX + lineVectorY * lineVectorY; // |Line|^2
                // t is the scalar projection of the point onto the line, normalized by the length of the line
                double t = (pointVectorX * lineVectorX + pointVectorY * lineVectorY) / lineLengthSquared;

                // Special cases: handle when the projection is outside the segment bounds
                if (t < 0.0)
                {
                    // Closest to the start point
                    return Points.GetDistanceBetweenPoints(point.X, point.Y, line.StartX, line.StartY);
                }
                else if (t > 1.0)
                {
                    // Closest to the end point
                    return Points.GetDistanceBetweenPoints(point.X, point.Y, line.EndX, line.EndY);
                }

                // General case: projection falls on the segment, calculate the closest point
                double closestX = line.StartX + t * lineVectorX;
                double closestY = line.StartY + t * lineVectorY;

                // Return the distance from the point to the closest point on the line segment
                return MathCalculations.Points.GetDistanceBetweenPoints(point.X, point.Y, closestX, closestY);
            }
        }
        public static class Points
        {
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
                return Math.Sqrt(squaredDistance);

            }
            /// <summary>
            /// Returns the Distance between two points , It is advisable to use Squared Distance Method instead of this when Doable.
            /// </summary>
            /// <param name="point1">The 1st Point</param>
            /// <param name="point2">The 2nd Point</param>
            /// <returns></returns>
            /// <exception cref="NotFiniteNumberException"></exception>
            public static double GetDistanceBetweenPoints(PointXY point1, PointXY point2)
            {
                return GetDistanceBetweenPoints(point1.X, point1.Y, point2.X, point2.Y);
            }
            /// <summary>
            /// Returns the Closest Point on a Line Segment from a certain arbitrary point
            /// <para>If the projection point falls far from the line then the returned closest point is one of the edges of the Segment</para>
            /// </summary>
            /// <param name="point">The point from which we need to find the closest point</param>
            /// <param name="pointOnSegmentA">a Point in the Segment</param>
            /// <param name="pointOnSegmentB">another Point in the Segment</param>
            /// <returns></returns>
            public static PointXY GetClosestPointOnSegmentFromPoint(PointXY point, PointXY pointOnSegmentA, PointXY pointOnSegmentB)
            {
                // Compute the Projection of the Point on the Line

                // Compute the vector from point A to point P
                Vector2D AP = new(pointOnSegmentA, point);
                // Compute the vector from point A to point B (the line segment)
                Vector2D AB = new(pointOnSegmentA, pointOnSegmentB);

                // Compute the squared length of the line segment ab
                var squaredLengthAB = AB.MagnitudeSquared();
                // Check if the segment length is effectively zero (within epsilon)
                if (squaredLengthAB < DoubleSafeEqualityComparer.DefaultEpsilon)
                {
                    // The segment is effectively a point; return pointOnSegmentA
                    return pointOnSegmentA;
                }

                // Compute the dot product of the vectors AP and AB
                var dotProduct = AP.Dot(AB);

                // Compute the projection of the point P onto the line segment AB
                // Also called the parameter "t"
                double t = dotProduct / squaredLengthAB;

                // Clamp t to the range [0, 1] to ensure the projection falls on the segment
                //Parameter t:
                //    Represents the scalar projection of ap onto ab, normalized by the length of ab.
                //    If t:
                //        < 0: Projection is behind point a.
                //        0 ≤ t ≤ 1: Projection falls within the segment from a to b.
                //        > 1: Projection is beyond point b.
                //Calculation: t = (ap • ab) / | ab |²
                t = Math.Clamp(t, 0, 1);

                // Compute the coordinates of the closest point on the segment
                double closestX = pointOnSegmentA.X + t * AB.X;
                double closestY = pointOnSegmentA.Y + t * AB.Y;

                // Compute the coordinates of the projection point on the line segment (The Closest point to segment) 
                return new(closestX, closestY);
            }
            /// <summary>
            /// Returns the Closest Point on a Line Segment from a certain arbitrary point
            /// <para>The Found point is also a point of the Normal(perpendicular) line to the Segment , that also passes from <paramref name="point"/></para>
            /// </summary>
            /// <param name="point">The point from which we need to find the closest point</param>
            /// <param name="segment">The Segment</param>
            /// <returns></returns>
            public static PointXY GetClosestPointOnSegmentFromPoint(PointXY point, LineInfo segment)
            {
                return GetClosestPointOnSegmentFromPoint(point, new(segment.StartX, segment.StartY), new(segment.EndX, segment.EndY));
            }
            /// <summary>
            /// Returns the Squared Distance from a Point to a Segment
            /// </summary>
            /// <param name="P">The Point</param>
            /// <param name="pointOnSegmentA">a point in the segment</param>
            /// <param name="pointOnSegmentB">another point in the segment</param>
            /// <returns></returns>
            public static double GetSquaredDistanceBetweenPointAndSegment(PointXY P, PointXY pointOnSegmentA, PointXY pointOnSegmentB)
            {
                //Get the closest point to the Segment
                var projectionPoint = GetClosestPointOnSegmentFromPoint(P, pointOnSegmentA, pointOnSegmentB);

                // Step 8: Compute the vector from the projection point to point p and return its squared Magnitude
                return new Vector2D(projectionPoint, P).MagnitudeSquared();
            }
            /// <summary>
            /// Returns the Squared Distance from a Point to a Segment
            /// </summary>
            /// <param name="P">The Point</param>
            /// <param name="segment">The segment</param>
            /// <returns></returns>
            public static double GetSquaredDistanceBetweenPointAndSegment(PointXY P, LineInfo segment)
            {
                return GetSquaredDistanceBetweenPointAndSegment(P, new(segment.StartX, segment.StartY), new(segment.EndX, segment.EndY));
            }
            /// <summary>
            /// Returns the Distance from a Point to a Segment
            /// </summary>
            /// <param name="P">The Point</param>
            /// <param name="pointOnSegmentA">a point in the segment</param>
            /// <param name="pointOnSegmentB">another point in the segment</param>
            /// <returns></returns>
            public static double GetDistanceOfPointFromSegment(PointXY P, PointXY pointOnSegmentA, PointXY pointOnSegmentB)
            {
                var squaredDistance = GetSquaredDistanceBetweenPointAndSegment(P, pointOnSegmentA, pointOnSegmentB);
                return Math.Sqrt(squaredDistance);
            }
            /// <summary>
            /// Returns the Distance from a Point to a Segment
            /// </summary>
            /// <param name="P">The Point</param>
            /// <param name="segment">The segment</param>
            /// <returns></returns>
            public static double GetDistanceOfPointFromSegment(PointXY P, LineInfo segment)
            {
                return GetDistanceOfPointFromSegment(P, new(segment.StartX, segment.StartY), new(segment.EndX, segment.EndY));
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
            public static double GetPointsSquaredDistance(PointXY point1, PointXY point2)
            {
                return GetPointsSquaredDistance(point1.X, point1.Y, point2.X, point2.Y);
            }

            /// <summary>
            /// Get a point (x,y) at a certain Distance and angle from another point - Angle , Cartesian system moves Clockwise
            /// </summary>
            /// <param name="distanceFromPoint">distance from one point to the other</param>
            /// <param name="angleRad">The angle of the Line Forming from the Origina Point , Always measured clockwise starting from right side </param>
            /// <param name="originX">The X Coordinate of the Starting Point</param>
            /// <param name="originY">The Y Coordinate of the Starting Point</param>
            /// <returns>a point x,y </returns>
            public static PointXY GetPointAtDistanceFromPoint(double distanceFromPoint, double angleRad, double originX = 0, double originY = 0)
            {
                //Get the distances with Cosinus and Sinus Definitions
                //Get X -- the relative position is the one if center was 0,0
                double relativePositionX = Math.Cos(angleRad) * distanceFromPoint;
                double x = relativePositionX + originX;

                //Get Y -- the relative position is the one if center was 0,0
                double relativePositionY = Math.Sin(angleRad) * distanceFromPoint;
                double y = relativePositionY + originY;

                return new PointXY(x, y);
            }
            /// <summary>
            /// Get a point (x,y) at a certain Distance and angle from another point - Angle , Cartesian system moves Clockwise
            /// </summary>
            /// <param name="distanceFromPoint">distance from on point to the other</param>
            /// <param name="angleRad">The angle of the Line Forming from the Origin Point , Always measured clockwise starting from right side </param>
            /// <param name="origin">The origin Point from which the distance is measured</param>
            /// <returns>a point x,y </returns>
            public static PointXY GetPointAtDistanceFromPoint(double distanceFromPoint, double angleRad, PointXY origin)
            {
                return GetPointAtDistanceFromPoint(distanceFromPoint, angleRad, origin.X, origin.Y);
            }

            /// <summary>
            /// Rotates a Point around an Origin Point at a certain angle
            /// </summary>
            /// <param name="pointX">The X Coordinate of the Point to Rotate</param>
            /// <param name="pointY">The Y Coordinate of the point to Rotate</param>
            /// <param name="originX">The X Coordinate of the Origin</param>
            /// <param name="originY">The Y Coordinate of the Origin</param>
            /// <param name="angleRad">The Angle of Rotation in Radians</param>
            /// <returns>The rotated Point</returns>
            public static PointXY RotatePointAroundOrigin(double pointX, double pointY, double originX, double originY, double angleRad)
            {
                //translate X,Y so that the origin becomes the actual 0,0
                double x_ = pointX - originX;
                double y_ = pointY - originY;

                //Rotate the Translated Point from the rotation matrix equations
                double x__ = x_ * Math.Cos(angleRad) - y_ * Math.Sin(angleRad);
                double y__ = x_ * Math.Sin(angleRad) + y_ * Math.Cos(angleRad);

                //Translate the point back to the original Coordinates
                double xRotated = x__ + originX;
                double yRotated = y__ + originY;

                // Round to eliminate floating-point errors for right-angle rotations
                double tolerance = DoubleSafeEqualityComparer.DefaultEpsilon; // Adjust as needed
                xRotated = Math.Abs(xRotated) < tolerance ? 0 : xRotated;
                yRotated = Math.Abs(yRotated) < tolerance ? 0 : yRotated;

                return new(xRotated, yRotated);
                /*
                To rotate a collection of points around a point of origin by a certain angle, you'll need to perform a rotation transformation. This transformation can be accomplished using some basic trigonometry. Here's a step-by-step guide:
                Define the rotation angle in radians. If your angle is in degrees, you'll need to convert it to radians. The conversion is done by multiplying the degree value by π/180π/180.
                Identify the point of origin around which you want to rotate the other points. Let's call this point O(xo,yo)O(xo​,yo​).
                For each point P(x,y)P(x,y) in your collection that you wish to rotate:
                Translate PP such that OO becomes the origin of the coordinate system. This is done by subtracting OO's coordinates from PP's:
                x′=x−xo
                x′=x−xo​
                y′=y−yo
                y′=y−yo​
                Rotate the translated point P′(x′,y′)P′(x′,y′) around the origin by the rotation angle θθ. The rotated point P′′(x′′,y′′)P′′(x′′,y′′) can be calculated using the rotation matrix:
                x′′=x′cos⁡(θ)−y′sin⁡(θ)y′′=x′sin⁡(θ)+y′cos⁡(θ)
                x′′y′′​=x′cos(θ)−y′sin(θ)=x′sin(θ)+y′cos(θ)​
                Translate P′′P′′ back to the original coordinate system by adding OO's coordinates:
                xrotated=x′′+xo
                xrotated​=x′′+xo​
                yrotated=y′′+yo
                yrotated​=y′′+yo​

                This process effectively rotates each point in your collection around the point of origin OO by the angle θθ.
                */
            }
            /// <summary>
            /// Rotates a Point around an Origin Point at a certain angle
            /// </summary>
            /// <param name="point">The Point to Rotate</param>
            /// <param name="origin">The Origin around which the point is rotating</param>
            /// <param name="angleRad">The Angle of Rotation in Radians</param>
            /// <returns>The rotated Point</returns>
            public static PointXY RotatePointAroundOrigin(PointXY point, PointXY origin, double angleRad)
            {
                return RotatePointAroundOrigin(point.X, point.Y, origin.X, origin.Y, angleRad);
            }

            /// <summary>
            /// Returns the Mirrored Point of a Point to a certain Y Axis
            /// </summary>
            /// <param name="x">The Point's X coordinate</param>
            /// <param name="y">The Point's Y coordinate</param>
            /// <param name="yAxisXcoordinate">The x Coordinate of the Y Axis , when 0 its the X=0 Line</param>
            /// <returns></returns>
            public static PointXY FindMirrorPointToYAxis(double x, double y, double yAxisXcoordinate = 0)
            {
                return new(2 * yAxisXcoordinate - x, y);
            }
            /// <summary>
            /// Returns the Mirrored Point of a Point to a certain Y Axis
            /// </summary>
            /// <param name="point">The reference Point of which to find the mirror Point</param>
            /// <param name="yAxisXcoordinate">The x Coordinate of the Y Axis , when 0 its the X=0 Line</param>
            /// <returns></returns>
            public static PointXY FindMirrorPointToYAxis(PointXY point, double yAxisXcoordinate = 0)
            {
                return FindMirrorPointToYAxis(point.X, point.Y, yAxisXcoordinate);
            }
            /// <summary>
            /// Returns a List of the provided points in a clockwise manner (lowest Xs first , if tied highest Ys then)
            /// </summary>
            /// <param name="points"></param>
            /// <returns></returns>
            public static List<PointXY> GetPointsByClockwiseOrder(params PointXY[] points)
            {
                return GetPointsByClockwiseOrder(points.AsEnumerable());
            }
            /// <summary>
            /// Returns a List of the provided points in a clockwise manner (lowest Xs first , if tied highest Ys then)
            /// </summary>
            /// <param name="points"></param>
            /// <returns></returns>
            public static List<PointXY> GetPointsByClockwiseOrder(IEnumerable<PointXY> points)
            {
                return [.. points.OrderBy(p => p.X).ThenByDescending(p => p.Y)];
            }

            /// <summary>
            /// Determines if a point lies on the line segment defined by points a and b.
            /// Returns true if the point lies on the segment, false otherwise.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            public static bool IsPointOnSegment(PointXY a, PointXY b, PointXY p)
            {
                // Calculate the cross product to check for collinearity
                double cross = (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);
                if (Math.Abs(cross) > DoubleSafeEqualityComparer.DefaultEpsilon)
                {
                    // If the cross product is not close to zero, the point is not collinear with the segment
                    return false;
                }

                // Calculate the dot product to ensure p is between a and b
                double dot = (p.X - a.X) * (b.X - a.X) + (p.Y - a.Y) * (b.Y - a.Y);
                if (dot < -DoubleSafeEqualityComparer.DefaultEpsilon)
                {
                    // If the dot product is negative, p is behind a
                    return false;
                }

                // Calculate the squared length of the segment
                double squaredLength = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
                if (dot > squaredLength + DoubleSafeEqualityComparer.DefaultEpsilon)
                {
                    // If the dot product is greater than the squared length, p is beyond b
                    return false;
                }

                // If both checks pass, p lies on the segment
                return true;
            }
        }
        public static class Polygons
        {
            /// <summary>
            /// Returns the Centroid of the provided PolygonVertices
            /// </summary>
            /// <param name="vertices">The Vertices comprising a Polygon</param>
            /// <returns>Centeroid Point (a.k.a The Geometric center of the Polygon)</returns>
            public static PointXY GetCentroid(IList<PointXY> vertices)
            {
                if (vertices.Count <= 2) throw new ArgumentException("A Polygon must have at least 3 Vertices", nameof(vertices));

                //Get the Xs and Ys sums for all the Vertices
                double sumX = 0;
                double sumY = 0;
                foreach (var vertex in vertices)
                {
                    sumX += vertex.X;
                    sumY += vertex.Y;
                }

                //Get the Average of the Xs and Ys
                return new PointXY(sumX / vertices.Count, sumY / vertices.Count);
            }

            /// <summary>
            /// Returns the Radius of the Circumscribed Circle of the Regular Polygon with the provided Length and number of Sides
            /// </summary>
            /// <param name="polygonTotalLength">The Length of the Polygon (Bounding Box Length)</param>
            /// <param name="sides">The Number of Sides of the Polygon</param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            public static double GetRegularPolygonEdgeSize(double circumscribedCircleRadius, int sides)
            {
                if (sides <= 2) throw new ArgumentException($"Polygons cannot have less than 3 Sides");
                return 2 * circumscribedCircleRadius * Math.Sin(Math.PI / sides);
            }
            /// <summary>
            /// Returns the Circumscribed Circle Radius of the Regular Polygon with the provided Edge Size and number of Sides
            /// </summary>
            /// <param name="edgeSize"></param>
            /// <param name="sides"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            public static double GetCircumscribedRadiusFromEdgeSize(double edgeSize, int sides)
            {
                if (sides <= 2) throw new ArgumentException($"Polygons cannot have less than 3 Sides");
                return edgeSize / (2 * Math.Sin(Math.PI / sides));
            }

            /// <summary>
            /// Returns the Angle of each edge in a regular polygon
            /// </summary>
            /// <param name="numberOfSides"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentException"></exception>
            public static double GetRegularPolygonEdgeAngle(int numberOfSides)
            {
                if (numberOfSides <= 2) throw new ArgumentException($"Polygons cannot have less than 3 Sides");
                return (numberOfSides - 2) * Math.PI / numberOfSides;
            }
            /// <summary>
            /// Returns the Normal Axes on which lie the Edges of the Polygon (Clockwise Normals)
            /// </summary>
            /// <param name="vertices">The vertices of the Polygon</param>
            /// <returns></returns>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            public static List<Vector2D> GetPolygonEdgesNormalAxesNormalized(IReadOnlyList<PointXY> vertices)
            {
                if (vertices.Count < 3) throw new ArgumentOutOfRangeException(nameof(vertices), "A Polygon must have at least 3 Vertices");

                List<Vector2D> axes = [];
                int vertexCount = vertices.Count;

                for (int i = 0; i < vertexCount; i++)
                {
                    PointXY currentVertex = vertices[i];
                    //ensures that when at the last iteration the next vertex will be vertices[0] as the division will return a modulo "0"
                    PointXY nextVertex = vertices[(i + 1) % vertexCount];
                    Vector2D axe = new Vector2D(currentVertex, nextVertex).GetNormalClockwise().Normalize();
                    axes.Add(axe);
                }

                return axes;
            }
            /// <summary>
            /// Returns the Normal Axes on which lie the Edges of the Polygon (Clockwise Normals)
            /// </summary>
            /// <param name="polygon"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            public static List<Vector2D> GetPolygonEdgesNormalAxesNormalized(PolygonInfo polygon)
            {
                return GetPolygonEdgesNormalAxesNormalized(polygon.Vertices);
            }
            /// <summary>
            /// Returns the Edges of a collection of Polygon vertices as Vectors
            /// </summary>
            /// <param name="vertices">The vertices of the Polygon</param>
            /// <returns></returns>
            public static List<PositionVector> GetPolygonEdges(IReadOnlyList<PointXY> vertices)
            {
                if (vertices.Count < 3) throw new ArgumentOutOfRangeException(nameof(vertices), "A Polygon must have at least 3 Vertices");

                List<PositionVector> edges = [];
                int vertexCount = vertices.Count;

                for (int i = 0; i < vertexCount; i++)
                {
                    PointXY currentVertex = vertices[i];
                    //ensures that when at the last iteration the next vertex will be vertices[0] as the division will return a modulo "0"
                    PointXY nextVertex = vertices[(i + 1) % vertexCount];
                    PositionVector edge = new(currentVertex, nextVertex);
                    edges.Add(edge);
                }

                return edges;
            }
            /// <summary>
            /// Returns the Edges of a Polygon as Vectors
            /// </summary>
            /// <param name="vertices">The vertices of the Polygon</param>
            /// <returns></returns>
            public static List<PositionVector> GetPolygonEdges(PolygonInfo polygon)
            {
                return GetPolygonEdges(polygon.Vertices);
            }

            /// <summary>
            /// Checks weather a Point is contained withing a Polygon (strictly inside not in edges)
            /// <para>Uses the Half-Plave Method for a convex Polygon</para>
            /// </summary>
            /// <param name="polygon"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static bool IsPointInPolygon(PolygonInfo polygon, PointXY point)
            {
                //Algorithm:
                //  Half - Plane Method: For a convex polygon, a point is inside if it is on the "inside" side of all edges.
                //  Edge Normal: For each edge, compute the normal vector pointing outward.
                //Dot Product: Compute the dot product between the normal vector and the vector from one vertex of the edge to the point.
                //Positive Dot Product: The point is outside the edge(or on the edge).
                //Negative Dot Product: The point is inside relative to that edge.
                double epsilon = DoubleSafeEqualityComparer.DefaultEpsilon; // Small tolerance to account for floating-point errors
                int count = polygon.NumberOfSides;

                for (int i = 0; i < count; i++)
                {
                    PointXY edgeStart = polygon.Vertices[i];
                    PointXY edgeEnd = polygon.Vertices[(i + 1) % count];
                    Vector2D edge = new(edgeStart, edgeEnd);
                    Vector2D normal = edge.GetNormalClockwise(); // (Whatever the order of vertices of the polygon , which is always clockwise!!!!)

                    Vector2D edgeToPoint = new(edgeStart, point);
                    double dot = normal.Dot(edgeToPoint);

                    if (dot >= -epsilon)
                        return false; // Outside or on edge/vertex
                }

                return true; // Strictly inside
            }
            /// <summary>
            /// Check weather a Point Intersects with a Polygon (is on its edges or inside it)
            /// <para>Uses the Half-Plave Method for a convex Polygon</para>
            /// </summary>
            /// <param name="polygon"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static bool IsPointIntersectingPolygon(PolygonInfo polygon, PointXY point)
            {
                //Algorithm:
                //  Half - Plane Method: For a convex polygon, a point is inside if it is on the "inside" side of all edges.
                //  Edge Normal: For each edge, compute the normal vector pointing outward.
                //Dot Product: Compute the dot product between the normal vector and the vector from one vertex of the edge to the point.
                //Positive Dot Product: The point is outside the edge(or on the edge).
                //Negative Dot Product: The point is inside relative to that edge.
                double epsilon = DoubleSafeEqualityComparer.DefaultEpsilon; // Small tolerance to account for floating-point errors
                int count = polygon.NumberOfSides;
                int winding = polygon.GetPolygonWinding();

                for (int i = 0; i < count; i++)
                {
                    PointXY edgeStart = polygon.Vertices[i];
                    PointXY edgeEnd = polygon.Vertices[(i + 1) % count];
                    Vector2D edge = new(edgeStart, edgeEnd);
                    // Follow the order in which the vertices are on the generated polygon (if winding = zero just get whatever)
                    Vector2D normal = winding == 1 ? edge.GetNormalCounterClockwise() : edge.GetNormalClockwise();

                    Vector2D edgeToPoint = new(edgeStart, point);
                    double dot = normal.Dot(edgeToPoint);

                    if (dot > epsilon)
                        return false; // Outside or on edge/vertex
                }

                return true; // Strictly inside
            }

            /// <summary>
            /// Determines weather a point is inside a Polygon using the Ray Casting Method
            /// <para>The algorithm accurately identifies points strictly inside or outside the polygon based on the parity (odd/even) of intersection counts.</para>
            /// <para>Points on Edges or Vertices: No explicit handling is implemented for points lying exactly on an edge or vertex. As a result</para>
            /// <para>1.Such points may be incorrectly classified as either inside or outside, depending on the polygon's geometry and the specific edge's orientation relative to the ray.</para>
            /// <para>2.Especially in cases where the horizontal ray intersects a vertex, leading to ambiguities in counting intersections.</para>
            /// </summary>
            /// <param name="polygon"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static bool IsPointInPolygonRayCast(PolygonInfo polygon, PointXY point)
            {
                bool isInside = false;
                int count = polygon.Vertices.Count;

                for (int i = 0; i < count; i++)
                {
                    int j = (i + 1) % count;
                    var vertex1 = polygon.Vertices[i];
                    var vertex2 = polygon.Vertices[j];

                    //Check if the point is on the same Y level as the edge if it is then claculate if there is intersection
                    //(otherwise there is not as the point is in a different level)
                    if (Math.Abs(vertex2.Y - vertex1.Y) < DoubleSafeEqualityComparer.DefaultEpsilon)
                    {
                        continue; // Skip horizontal edges
                    }

                    // Check if point.Y is between vertex1.Y and vertex2.Y
                    bool yIntersection = (vertex1.Y > point.Y) != (vertex2.Y > point.Y);
                    if (yIntersection)
                    {
                        // Calculate the X coordinate of the intersection point
                        double intersectX = ((vertex2.X - vertex1.X) * (point.Y - vertex1.Y)) / (vertex2.Y - vertex1.Y) + vertex1.X;

                        // Check if the point is to the left of the intersection point
                        // Use epsilon to handle floating-point precision
                        if (point.X < intersectX + DoubleSafeEqualityComparer.DefaultEpsilon)
                        {
                            isInside = !isInside; // Toggle inside status
                        }
                    }
                }
                return isInside;
            }

            /// <summary>
            /// Determines if a point is inside a Polygon with the winding number algorithm
            /// <para>Points on edges are not considered inside the Polygon</para>
            /// <para>The Winding Number algorithm determines whether a point lies inside a polygon by calculating how many times the polygon winds around the point. The winding number increases when an edge crosses upward relative to the point and decreases when an edge crosses downward.</para>
            /// <para>Non-Zero Winding Number: The point is inside the polygon. ---- Zero Winding Number: The point is outside the polygon.</para>
            /// <para>Crosses upward means the horizontal line passing from the point is crossing with the edge , with the start vertex below and end vertex above , (Point left of Edge)</para>
            /// <para>Crosses downward means the horizontal line passing from the point is crossing with the edge, with the start vertex above and end vertex below (Point right of Edge)</para>
            /// <para>Depending on how many times this happens , we can understand if the point is inside or outside the Polygon</para>
            /// </summary>
            /// <param name="polygon"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static bool IsPointInPolygonWinding(PolygonInfo polygon, PointXY point)
            {
                //Initilize with zero (point outside the Polygon)
                int windingNumber = 0;
                int count = polygon.Vertices.Count;

                for (int i = 0; i < count; i++)
                {
                    int j = (i + 1) % count;
                    var vertex1 = polygon.Vertices[i];
                    var vertex2 = polygon.Vertices[j];

                    // Check if the current edge crosses the horizontal line at point.Y
                    // Considering floating-point precision with epsilon
                    if (IsEdgeCrossingHorizontalLineWindingHelper(vertex1.Y, vertex2.Y, point.Y))
                    {
                        // Determine if the point is to the left or right of the edge
                        double isLeftValue = IsLeftWindingHelper(vertex1, vertex2, point);

                        if (vertex1.Y <= point.Y)
                        {
                            // Edge crosses upward relative to point.Y
                            if (isLeftValue > DoubleSafeEqualityComparer.DefaultEpsilon)
                            {
                                windingNumber++; // Increment winding number
                            }
                        }
                        else
                        {
                            // Edge crosses downward relative to point.Y
                            if (isLeftValue < -DoubleSafeEqualityComparer.DefaultEpsilon)
                            {
                                windingNumber--; // Decrement winding number
                            }
                        }
                    }
                }
                return windingNumber != 0;
            }
            /// <summary>
            /// Determines if an edge crosses the horizontal line at a specific Y-coordinate.
            /// Accounts for floating-point precision using epsilon.
            /// </summary>
            /// <param name="y1">Y-coordinate of the first vertex.</param>
            /// <param name="y2">Y-coordinate of the second vertex.</param>
            /// <param name="y">Y-coordinate of the horizontal line.</param>
            /// <returns>True if the edge crosses the horizontal line, false otherwise.</returns>
            private static bool IsEdgeCrossingHorizontalLineWindingHelper(double y1, double y2, double y)
            {
                // Check if y is between y1 and y2, considering epsilon
                return (y1 <= y + DoubleSafeEqualityComparer.DefaultEpsilon && y2 > y + DoubleSafeEqualityComparer.DefaultEpsilon) ||
                       (y2 <= y + DoubleSafeEqualityComparer.DefaultEpsilon && y1 > y + DoubleSafeEqualityComparer.DefaultEpsilon);
            }
            /// <summary>
            /// Determines if the point is to the left, on, or to the right of the line segment from a to b.
            /// <para>Returns greater than 0 for left, 0 for on the line, and less than 0 for right.</para>
            /// <para>Imagine standing at point a and looking towards point b. The IsLeft method tells you whether point p is to your left, directly ahead/on, or to your right.</para>
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            private static double IsLeftWindingHelper(PointXY a, PointXY b, PointXY p)
            {
                return ((b.X - a.X) * (p.Y - a.Y)) - ((p.X - a.X) * (b.Y - a.Y));
            }

            /// <summary>
            /// Determines weather a Point is on the Edges of the Polygon Or Inside it
            /// </summary>
            /// <param name="polygon"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static bool IsPointIntersectingPolygonWinding(PolygonInfo polygon, PointXY point)
            {
                foreach (var edge in polygon.Edges)
                {
                    if (Points.IsPointOnSegment(edge.Start, edge.End, point))
                    {
                        return true; //Point is on one of the edges
                    }
                }

                // if the above does not return anything check for containment:
                return IsPointInPolygonWinding(polygon, point);
            }
        }
        public static class Containment
        {
            /// <summary>
            /// Returns the Combined Bounding Box of the provided Shapes
            /// </summary>
            /// <param name="shapes"></param>
            /// <returns></returns>
            public static RectangleInfo GetBoundingBox(IEnumerable<ShapeInfo> shapes)
            {
                if (!shapes.Any()) return RectangleInfo.ZeroRectangle();
                // Collect all Xs and Ys of the various bounding Boxes of the shapes
                double minX = double.PositiveInfinity;
                double maxX = double.NegativeInfinity;
                double minY = double.PositiveInfinity;
                double maxY = double.NegativeInfinity;

                foreach (var shape in shapes)
                {
                    var b = shape.GetBoundingBox();
                    minX = Math.Min(minX, b.LeftX);
                    maxX = Math.Max(maxX, b.RightX);
                    minY = Math.Min(minY, b.TopY);
                    maxY = Math.Max(maxY, b.BottomY);
                }

                return new RectangleInfo(minX, minY, maxX, maxY);
            }
            /// <summary>
            /// Returns the Bounding Box formed from the provided points
            /// </summary>
            /// <param name="points"></param>
            /// <returns></returns>
            public static RectangleInfo GetBoundingBox(params PointXY[] points)
            {
                if (!points.Any()) return RectangleInfo.ZeroRectangle();
                // Collect all Xs and Ys of the various bounding Boxes of the shapes
                double minX = double.PositiveInfinity;
                double maxX = double.NegativeInfinity;
                double minY = double.PositiveInfinity;
                double maxY = double.NegativeInfinity;

                foreach (var point in points)
                {
                    minX = Math.Min(minX, point.X);
                    maxX = Math.Max(maxX, point.X);
                    minY = Math.Min(minY, point.Y);
                    maxY = Math.Max(maxY, point.Y);
                }

                return new RectangleInfo(minX, minY, maxX, maxY);
            }
        }
        public static class Vectors
        {
            /// <summary>
            /// Defines a way to determine the position of P relative to V using the 2D cross product.
            /// </summary>
            /// <param name="v"></param>
            /// <param name="p"></param>
            /// <returns>
            /// CrossProduct > 0 : P is left of V (counterclockwise rotation , LOOKING FROM V)
            /// CrossProduct < 0 : P is right of V (clockwise rotation , LOOKING FROM V)
            /// CrossProduct = 0 : P is in the same Line as V (their points are Coolinear , though maybe they do not intersect)
            /// </returns>
            public static double CrossProduct(PositionVector v, PointXY p)
            {
                double val = v.Vector.Y * (p.X - v.Start.X) - v.Vector.X * (p.Y - v.Start.Y);

                if (val == 0) return 0; // Collinear
                return (val > 0) ? 1 : -1; // Clockwise or Counterclockwise
            }
            /// <summary>
            /// Returns the Orientation of a Point from a Vector
            /// 0 => point is on the Same Line with the Vector
            /// 1 => point is anticlockwise from the Vector
            /// -1 => point is clockwise from the Vector
            /// </summary>
            /// <param name="v">The vector.</param>
            /// <param name="p">The point to evaluate.</param>
            /// <returns></returns>
            public static int Orientation(PositionVector v, PointXY p)
            {
                var crossProduct = CrossProduct(v, p);
                if (crossProduct == 0) return 0;
                return crossProduct > 0 ? 1 : -1;
            }
            /// <summary>
            /// Returns the Orientation between two Vectors
            /// 0 => V2 is on the Same Line with V1
            /// 1 => V2 is anticlockwise from V1
            /// -1 => V2 is clockwise from V1
            /// </summary>
            /// <param name="v1">The first Vector</param>
            /// <param name="p2">The second Vector</param>
            /// <returns></returns>
            public static int Orientation(Vector2D v1, Vector2D v2)
            {
                var crossProduct = v1.Cross(v2);
                if (crossProduct == 0) return 0;
                return crossProduct > 0 ? 1 : -1;
            }
            /// <summary>
            /// Calculates the Projection Scaling Factor of the Point P*V/(Abs(Vlength)^2)
            /// <para>1.Tells us how far along the vector V the projection of the point P lies</para>
            /// <para>2.It's the length of the projection of P onto V as a proportion of the vector V</para>
            /// <para>3.Multipling this with the Vector we get the projection's Vector length</para>
            /// </summary>
            /// <param name="v"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            public static double PointProjectionScalingFactorToVector(Vector2D v, PointXY p)
            {
                var magnitudeSquared = v.MagnitudeSquared();
                if (magnitudeSquared == 0) return 0;
                var dotProduct = v.Dot(p);
                return dotProduct / magnitudeSquared;
            }
            /// <summary>
            /// Returns the Projection of a <see cref="PointXY"/> to a Vector
            /// <para>1.Projection of a point to a Vector is the Intersecting Point of the prependicular to the Vector Passing from the point</para>
            /// <para>2.The Projection is the point on the Vector that is closest to the given point</para>
            /// <para>3.Its calculated by finding the Point projection scaling factor and then multiplying with the Vector's Direction</para>
            /// </summary>
            /// <param name="v"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            public static PointXY GetPointProjectionToVector(Vector2D v, PointXY p)
            {
                var projScalingFactor = PointProjectionScalingFactorToVector(v, p);
                return new(v.X * projScalingFactor, v.Y * projScalingFactor);
            }
            /// <summary>
            /// Returns the min Distance of a point from a Vector
            /// <para>1.Calculates the Projection point</para>
            /// <para>2.Then Find the Distance between the two points</para>
            /// </summary>
            /// <param name="v"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            public static double GetDistanceOfPointFromVector(PositionVector v, PointXY p)
            {
                return Points.GetDistanceOfPointFromSegment(p, v.Start, v.End);
            }
            /// <summary>
            /// Returns the Squared (^2) distance of a Point from a Vector
            /// <para>1.Calculates the Projection point</para>
            /// <para>2.Then Find the Squared Distance between the two points</para>
            /// </summary>
            /// <param name="v"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            public static double GetSquaredDistanceOfPointFromVector(PositionVector v, PointXY p)
            {
                return Points.GetSquaredDistanceBetweenPointAndSegment(p, v.Start, v.End);
            }

            /// <summary>
            /// Weather the certain point is on the vector segment
            /// </summary>
            /// <param name="v">The Vector</param>
            /// <param name="p">The Point</param>
            /// <returns></returns>
            public static bool IsPointOnVector(PositionVector v, PointXY p)
            {
                return p.X <= Math.Max(v.Start.X, v.End.X) && p.X >= Math.Min(v.Start.X, v.End.X) + DoubleSafeEqualityComparer.DefaultEpsilon &&
                       p.Y <= Math.Max(v.Start.Y, v.End.Y) && p.Y >= Math.Min(v.Start.Y, v.End.Y) + DoubleSafeEqualityComparer.DefaultEpsilon;
            }

            /// <summary>
            /// Determines weather two Vectors Intersect
            /// <para>1. Checks the Orientations between the Vectors and determines weather the form an Intersection</para>
            /// <para>2. Checks weather any of the end points is Collinear and determines weather on the Segments of the Vectors</para>
            /// </summary>
            /// <param name="v1"></param>
            /// <param name="v2"></param>
            /// <returns></returns>
            public static bool DoVectorsIntersect(PositionVector v1, PositionVector v2)
            {
                // Find the 4 orientations needed for the general and special cases
                double o1 = Orientation(v1, v2.Start);
                double o2 = Orientation(v1, v2.End);
                double o3 = Orientation(v2, v1.Start);
                double o4 = Orientation(v2, v1.End);

                //Two line segments intersect if their endpoints lie on opposite sides of each other. In mathematical terms, this happens when:
                //o1≠o2 : Points B1​ and B2 are on opposite sides of line A1A2​.
                //o3≠o4 : Points A1 and A2​ are on opposite sides of line B1B2​.
                //If both conditions are true, the line segments intersect at some point.

                // General case
                //O1 != O2 and O3 != O4
                if ((o1 > DoubleSafeEqualityComparer.DefaultEpsilon && o2 < -DoubleSafeEqualityComparer.DefaultEpsilon
                    || o1 < -DoubleSafeEqualityComparer.DefaultEpsilon && o2 > DoubleSafeEqualityComparer.DefaultEpsilon) &&
                    (o3 > DoubleSafeEqualityComparer.DefaultEpsilon && o4 < -DoubleSafeEqualityComparer.DefaultEpsilon
                    || o3 < -DoubleSafeEqualityComparer.DefaultEpsilon && o4 > DoubleSafeEqualityComparer.DefaultEpsilon))
                {
                    return true;
                }

                // Special cases - Collinear points
                // B1 is on segment A1A2
                if (Math.Abs(o1) < DoubleSafeEqualityComparer.DefaultEpsilon && IsPointOnVector(v1, v2.Start)) return true;

                // B2 is on segment A1A2
                if (Math.Abs(o2) < DoubleSafeEqualityComparer.DefaultEpsilon && IsPointOnVector(v1, v2.End)) return true;

                // A1 is on segment B1B2
                if (Math.Abs(o3) < DoubleSafeEqualityComparer.DefaultEpsilon && IsPointOnVector(v2, v1.Start)) return true;

                // A2 is on segment B1B2
                if (Math.Abs(o4) < DoubleSafeEqualityComparer.DefaultEpsilon && IsPointOnVector(v2, v1.End)) return true;

                // No intersection
                return false;
            }
        }
    }
    /// <summary>
    /// Represents a Line in a XY Coordinate System
    /// where its general Formula is <para>Ax + By + C = 0</para>
    /// </summary>
    /// <param name="A">The A coefficient</param>
    /// <param name="B">The B coefficient</param>
    /// <param name="C">The C coefficient</param>
    public class LineEquation
    {
        /// <summary>
        /// The A coefficient from <para>Ax + By +C = 0</para>
        /// </summary>
        public double A { get; }
        /// <summary>
        /// The B coefficient from <para>Ax + By +C = 0</para>
        /// </summary>
        public double B { get; }
        /// <summary>
        /// The C coefficient from <para>Ax + By +C = 0</para>
        /// </summary>
        public double C { get; }
        /// <summary>
        /// Creates a new Instance of a <see cref="LineEquation"/> with the specified coefficients for
        /// <para>Ax + By + C = 0</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public LineEquation(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }
        /// <summary>
        /// Creates a new LineEquation from two points that are on the same line
        /// </summary>
        /// <param name="point1">Point 1</param>
        /// <param name="point2">Point 2</param>
        public LineEquation(PointXY point1, PointXY point2)
        {
            var eq = MathCalculations.Line.GetLineEquation(point1, point2);
            A = eq.A;
            B = eq.B;
            C = eq.C;
        }

        /// <summary>
        /// Determines weather a point is part of this line 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainsPoint(PointXY point)
        {
            DoubleSafeEqualityComparer comparer = new();
            return comparer.Equals(A * point.X + B * point.Y + C, 0);
        }
        /// <summary>
        /// Returns the Distance of a Point from this Line , if the point lies within the Line it returns zero
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public double DistanceFromPoint(PointXY point)
        {
            // Handle case where the line is invalid
            if (A == 0 && B == 0)
            {
                throw new InvalidOperationException("Invalid line coefficients. Both A and B cannot be zero.");
            }

            // Check if the point lies on the line
            if (ContainsPoint(point))
            {
                return 0; // The point lies on the line
            }

            // Calculate distance using the distance formula
            double distance = Math.Abs(A * point.X + B * point.Y + C) / Math.Sqrt(A * A + B * B);
            return distance;
        }
        public override string ToString()
        {
            return $"{A}x +{B}y + {C} = 0";
        }
    }
    /// <summary>
    /// Represents a Circle in a XY Coordinate System
    /// Where its general Formula is <para>(x-cx)^2 + (y-cy)^2 = r^2</para>
    /// </summary>
    /// <param name="Cx">The X Coordinate of the Circles Center</param>
    /// <param name="Cy">The Y Coordinate of the Circles Center</param>
    /// <param name="Radius">The Radius of the Circle</param>
    public class CircleEquation
    {
        /// <summary>
        /// Creates a new Instance of a <see cref="CircleEquation"/> with the specified coefficients for
        /// <para>(x-cx)^2 + (y-cy)^2 = r^2</para>
        /// </summary>
        /// <param name="cx">The Center X of the Circle</param>
        /// <param name="cy">The Center Y of the Circle</param>
        /// <param name="radius">The Radius of the Circle</param>
        public CircleEquation(double cx, double cy, double radius)
        {
            Cx = cx;
            Cy = cy;
            Radius = radius;
        }
        /// <summary>
        /// Center X of the Circle
        /// </summary>
        public double Cx { get; }
        /// <summary>
        /// Center Y of the Circle
        /// </summary>
        public double Cy { get; }
        /// <summary>
        /// Radius of the Circle
        /// </summary>
        public double Radius { get; }
    }
    /// <summary>
    /// Represents an Ellipse in a XY Coordinate System
    /// where its general Formula is <para>((x-Ex)^2 / A^2) + ((y-Ey)^2 / B ^2) = 1</para>
    /// </summary>
    /// <param name="Ex">The X Coordinate of the Ellipse's Center</param>
    /// <param name="Ey">The Y Coordinate of the Ellipse's Center</param>
    /// <param name="A">The x Axis Radius </param>
    /// <param name="B">The y Axis Radius </param>
    public class EllipseEquation
    {
        /// <summary>
        /// Ellipse center X Coordinate
        /// </summary>
        public double Ex { get; }
        /// <summary>
        /// Ellipse center Y Coordinate
        /// </summary>
        public double Ey { get; }
        /// <summary>
        /// The Radius in X Axis
        /// </summary>
        public double Rx { get; }
        /// <summary>
        /// The Radius in Y Axis
        /// </summary>
        public double Ry { get; }

        /// <summary>
        /// The Semi - Major Axis Radii
        /// </summary>
        public double A { get => Math.Max(Rx, Ry); }
        /// <summary>
        /// The Semi - Minor Axis Radii
        /// </summary>
        public double B { get => Math.Min(Rx, Ry); }

        /// <summary>
        /// Creates a new Instance of a <see cref="EllipseEquation"/> with the specified coefficients for
        /// <para>((x-Ex)^2 / A^2) + ((y-Ey)^2 / B ^2) = 1 (ASSUMES A = MAJOR HORIZONTAL , B= MINOR VERTICAL , interchange if different)</para>
        /// </summary>
        /// <param name="ex">The Ellipse Center X</param>
        /// <param name="ey">The Ellipse Center Y</param>
        /// <param name="rx">The X Axis Radius</param>
        /// <param name="ry">The Y Axis Radius</param>
        public EllipseEquation(double ex, double ey, double rx, double ry)
        {
            Ex = ex;
            Ey = ey;
            Rx = rx;
            Ry = ry;
        }
        public EllipseEquation(EllipseInfo ellipse)
        {
            Ex = ellipse.LocationX;
            Ey = ellipse.LocationY;
            Rx = ellipse.Orientation == EllipseOrientation.Horizontal ? ellipse.RadiusMajor : ellipse.RadiusMinor;
            Ry = ellipse.Orientation == EllipseOrientation.Horizontal ? ellipse.RadiusMinor : ellipse.RadiusMajor;
        }

        public bool ContainsPoint(PointXY point)
        {
            //Ellipse is degenerated into a point
            if (Math.Abs(Rx) < DoubleSafeEqualityComparer.DefaultEpsilon || Math.Abs(Ry) < DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                return false;
            }

            double dx = point.X - Ex;
            double dy = point.Y - Ey;
            return (dx * dx) / (Rx * Rx) + (dy * dy) / (Ry * Ry) < 1 + DoubleSafeEqualityComparer.DefaultEpsilon;
        }
        public bool IntersectsWithPoint(PointXY point)
        {
            //Ellipse is degenerated into a point
            if (Math.Abs(Rx) < DoubleSafeEqualityComparer.DefaultEpsilon || Math.Abs(Ry) < DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                //if the center of the ellipse is the point
                if (Math.Abs(point.X - Ex) < DoubleSafeEqualityComparer.DefaultEpsilon && Math.Abs(point.Y - Ey) < DoubleSafeEqualityComparer.DefaultEpsilon) return true;
                //if the center of the ellipse is not the point
                return false;
            }

            double dx = point.X - Ex;
            double dy = point.Y - Ey;
            return (dx * dx) / (Rx * Rx) + (dy * dy) / (Ry * Ry) <= 1 + DoubleSafeEqualityComparer.DefaultEpsilon;
        }

        public override string ToString()
        {
            return $"((x-{Ex})^2) / {Rx}^2 + ((y-{Ey})^2)/{Ry}^2";
        }

    }
    public class NoValidSolutionsException : Exception
    {
        public NoValidSolutionsException(string calculationName) : base($"There were no Valid Solutions for :{calculationName}") { }
    }
    public class Matrix
    {
        private readonly int rows;
        private readonly int columns;
        private readonly double[,] data;

        public Matrix(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
            data = new double[rows, columns];
        }

        public int Rows => rows;
        public int Columns => columns;

        public double this[int row, int column]
        {
            get { return data[row, column]; }
            set { data[row, column] = value; }
        }

        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.Rows != matrix2.Rows || matrix1.Columns != matrix2.Columns)
            {
                throw new ArgumentException("Matrices must have the same dimensions.");
            }

            Matrix result = new(matrix1.Rows, matrix1.Columns);

            for (int i = 0; i < matrix1.Rows; i++)
            {
                for (int j = 0; j < matrix1.Columns; j++)
                {
                    result[i, j] = matrix1[i, j] + matrix2[i, j];
                }
            }

            return result;
        }

        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.Rows != matrix2.Rows || matrix1.Columns != matrix2.Columns)
            {
                throw new ArgumentException("Matrices must have the same dimensions.");
            }

            Matrix result = new(matrix1.Rows, matrix1.Columns);

            for (int i = 0; i < matrix1.Rows; i++)
            {
                for (int j = 0; j < matrix1.Columns; j++)
                {
                    result[i, j] = matrix1[i, j] - matrix2[i, j];
                }
            }

            return result;
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.Columns != matrix2.Rows)
            {
                throw new ArgumentException("Number of columns in the first matrix must be equal to the number of rows in the second matrix.");
            }

            Matrix result = new(matrix1.Rows, matrix2.Columns);

            for (int i = 0; i < matrix1.Rows; i++)
            {
                for (int j = 0; j < matrix2.Columns; j++)
                {
                    double sum = 0;

                    for (int k = 0; k < matrix1.Columns; k++)
                    {
                        sum += matrix1[i, k] * matrix2[k, j];
                    }

                    result[i, j] = sum;
                }
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    sb.Append(data[i, j]);
                    sb.Append(' ');
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}


