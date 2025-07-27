using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using ShapesLibrary.Attributes;
using ShapesLibrary.Enums;
using ShapesLibrary.Exceptions;
using ShapesLibrary.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapesLibrary.ShapeInfoModels
{
    [ShapeOrigin("LineStart")]
    public class LineInfo : ShapeInfo , IDeepClonable<LineInfo> , IEqualityComparerCreator<LineInfo>
    {
        public LineInfo(double startX, double startY, double endX, double endY) : base(startX, startY)
        {
            EndX = endX;
            EndY = endY;
            ShapeType = ShapeInfoType.LineShapeInfo;
        }
        public LineInfo(PointXY lineStart, PointXY lineEnd) : this(lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y) { }
        /// <summary>
        /// Creates a Line Info object from the specified arguments
        /// </summary>
        /// <param name="lineStart">The Start point of the Line</param>
        /// <param name="length">The Length of the Line</param>
        /// <param name="angleRadiansWithX">The Angle with the X axis in Radians. Zero Angle is on the Right and moves clockwise</param>
        public LineInfo(PointXY lineStart,double length,double angleRadiansWithX):base(lineStart.X,lineStart.Y)
        {
            //find the EndPoint
            var endPoint = MathCalculations.Points.GetPointAtDistanceFromPoint(length, angleRadiansWithX, lineStart);
            EndX = endPoint.X;
            EndY = endPoint.Y;
            ShapeType = ShapeInfoType.LineShapeInfo;
        }
        protected override string GetDimensionsString()
        {
            try
            {
                return $"L {GetTotalLength()}mm";
            }
            catch (Exception)
            {
                return $"L ???mm";
            }
        }
        public double StartX { get; set; }
        public double StartY { get; set; }
        public PointXY Start { get => new(StartX, StartY); }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public PointXY End { get => new(EndX, EndY); }
        public override double LocationX { get => StartX; set => SetLocationX(value); }
        public override double LocationY { get => StartY; set => SetLocationY(value); }
        public bool IsHorizontal => StartY == EndY;
        public bool IsVertical => StartX == EndX;

        public override PointXY GetCentroid()
        {
            return GetLocation();
        }
        private void SetLocationX(double newX)
        {
            var diff = EndX - StartX;
            StartX = newX;
            EndX = StartX + diff;
        }
        private void SetLocationY(double newY)
        {
            var diff = EndY - StartY;
            StartY = newY;
            EndY = StartY + diff;
        }
        public override double GetTotalLength()
        {
            return MathCalculations.Points.GetDistanceBetweenPoints(StartX, StartY, EndX, EndY);
        }
        public override void SetTotalLength(double length)
        {
            if (length == 0)
            {
                EndX = StartX;
                EndY = StartX;
                return;
            }
            var currentLength = GetTotalLength();

            if (currentLength > 0)
            {
                //Calculate the ratio to scale the line 
                double scaleRatio = length / currentLength;

                //Update the End Point by keeping the start point fixed
                //Increase both by the same percentage
                EndX = StartX + (EndX - StartX) * scaleRatio;
                EndY = StartY + (EndY - StartY) * scaleRatio;
            }
            else
            {
                //If the current length was zero then make a horizontal Line
                EndX = StartX + length;
                EndY = StartY;
            }
        }
        public override void Scale(double scaleFactor)
        {
            SetTotalLength(GetTotalLength() * scaleFactor);
        }
        public void InterchangeEndWithStart()
        {
            var endX= EndX;
            var endY = EndY;
            EndX = StartX;
            EndY = StartY;
            StartX = endX;
            StartY = endY;
        }
        /// <summary>
        /// Returns the Equation that defines the Line which passes from the Start and End points 
        /// </summary>
        /// <returns></returns>
        public LineEquation GetEquation() => MathCalculations.Line.GetLineEquation(new(StartX, StartY), new(EndX, EndY));
        /// <summary>
        /// Determines weather a point is within the bounds of the Start and End Points of the Line Segment
        /// </summary>
        /// <param name="point"></param>
        /// <returns>True if the point is within bounds , False if it is not</returns>
        public bool IsPointWithinBounds(PointXY point)
        {
            return
                point.X >= Math.Min(StartX, EndX) &&
                point.X <= Math.Max(StartX, EndX) &&
                point.Y >= Math.Min(StartY, EndY) &&
                point.Y <= Math.Max(StartY, EndY);
        }
        /// <summary>
        /// Returns the MidPoint of the Line
        /// </summary>
        /// <returns></returns>
        public PointXY GetLineMidPoint() => new((StartX+EndX)/2d, (StartY + EndY)/2d);
        /// <summary>
        /// Returns the angle of the Line with the X axis spanning from -pi to pi (used Atan2 function taking into account the Line's direction)
        /// </summary>
        /// <returns></returns>
        public double GetAngleWithX()
        {
            //The Arguments are actually the (y2-y1/x2-x1) which is the slope of the line and internally used in Atan2 to produce the correct angle.
            //Takes into account special cases like slope = positiveInfinity or NegativeInfinity or when slope = 0 which can be PI or 0 angle
            if(StartX == EndX && StartY == EndY) return 0; //this is when the line is actually a point
            return Math.Atan2(EndY - StartY, EndX - StartX);
        }

        /// <summary>
        /// Returns the angle of the Line with the X axis spanning from -pi/2 to pi/2 (used Atan function not taking into account the Line's direction)
        /// </summary>
        /// <returns></returns>
        public double GetNonDirectionalAngleWithXAxis()
        {
            //Below method does not take into account directionality . The Atan2 Method actually calculates the Correct Angle
            //By taking into account the direction of the Line also rather than only producing a Radians Angle
            var slope = MathCalculations.Line.GetLineSlope(StartX, StartY, EndX, EndY);
            if (double.IsNaN(slope)) return 0; //this is when the line is actually a point
            else return Math.Atan(slope);
        }

        public override RectangleInfo GetBoundingBox()
        {
            return new RectangleInfo(Math.Min(StartX,EndX),Math.Min(StartY,EndY),Math.Max(StartX,EndX),Math.Max(StartY,EndY));
        }
        public override LineInfo GetDeepClone()
        {
            return (LineInfo)this.MemberwiseClone();
        }

        public override double GetPerimeter()
        {
            return GetTotalLength();
        }
        public override double GetArea()
        {
            return 0;
        }

        public override ShapeInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            var length = GetTotalLength();
            if (length == 0) return this.GetDeepClone();
            
            //find the diff in shrinkage and shrink the line from both ends the same
            var diffLengthPerSide = perimeterShrink / 2d;

            //As in Set Length method calculate how much to scale each point's coordinates
            double scaleX = (EndX - StartX) / length; //the percentage of length in X
            double scaleY = (EndY - StartY) / length; //the percentage of length in Y

            var newLine = this.GetDeepClone();

            //Update both end and Start with the new percentage
            newLine.StartX -= diffLengthPerSide * scaleX;
            newLine.StartY -= diffLengthPerSide * scaleY;
            newLine.EndX += diffLengthPerSide * scaleX;
            newLine.EndY += diffLengthPerSide * scaleY;
            return newLine;
        }

        public override void RotateAntiClockwise()
        {
            RotateAroundStart(-Math.PI / 2d);
            RotationRadians -= Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void RotateClockwise()
        {
            RotateAroundStart(Math.PI / 2d);
            RotationRadians += Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void FlipHorizontally(double flipOriginX = double.NaN)
        {
            if (double.IsNaN(flipOriginX)) flipOriginX = GetBoundingBox().GetLocation().X;
            StartX = 2 * flipOriginX - StartX;
            EndX = 2 * flipOriginX - EndX;
        }
        public override void FlipVertically(double flipOriginY = double.NaN)
        {
            if (double.IsNaN(flipOriginY)) flipOriginY = GetBoundingBox().GetLocation().Y;
            StartY = 2 * flipOriginY - StartY;
            EndY = 2 * flipOriginY - EndY;
        }

        public void RotateAroundStart(double angleRadians)
        {
            var rotatePoint = MathCalculations.Points.RotatePointAroundOrigin(EndX, EndY, StartX, StartY, angleRadians);
            EndX = rotatePoint.X;
            EndY = rotatePoint.Y;
        }
        public void RotateAroundEnd(double angleRadians)
        {
            var rotatedPoint = MathCalculations.Points.RotatePointAroundOrigin(StartX, StartY, EndX, EndY, angleRadians);
            StartX = rotatedPoint.X;
            StartY = rotatedPoint.Y;
        }
        /// <summary>
        /// Rotates the Line to the Designated angle with X axis
        /// </summary>
        /// <param name="angleRadians"></param>
        public void SetAngleWithX(double angleRadians)
        {
            var totalLength = GetTotalLength();
            var newEnd = MathCalculations.Points.GetPointAtDistanceFromPoint(totalLength, angleRadians, GetLocation());
            EndX = newEnd.X;
            EndY = newEnd.Y;
        }

        public static LineInfo Zero() => new(0, 0, 0, 0);
        static IEqualityComparer<LineInfo> IEqualityComparerCreator<LineInfo>.GetComparer()
        {
            return new LineInfoEqualityComparer();
        }

        public override double GetTotalHeight()
        {
            return 0;
        }

        public override void SetTotalHeight(double height)
        {
            return;
        }

        public override bool Contains(PointXY point)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(ShapeInfo shape)
        {
            throw new NotImplementedException();
        }

        public override bool IntersectsWithPoint(PointXY point)
        {
            throw new NotSupportedIntersectionException(this, point);
        }

        public override bool IntersectsWithShape(ShapeInfo shape)
        {
            throw new NotSupportedIntersectionException(this, shape);
        }
    }
    public class LineInfoEqualityComparer : IEqualityComparer<LineInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public LineInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly ShapeInfoBaseEqualityComparer baseComparer;

        public bool Equals(LineInfo? x, LineInfo? y)
        {
            return baseComparer.Equals(x, y)
                && x!.StartX == y!.StartX
                && x.StartY == y.StartY
                && x.EndX == y.EndX
                && x.EndY == y.EndY;
        }

        public int GetHashCode([DisallowNull] LineInfo obj)
        {
            int hash = baseComparer.GetHashCode(obj);
            return HashCode.Combine(hash, obj.StartX, obj.StartY, obj.EndX, obj.EndY);
        }
    }
}
