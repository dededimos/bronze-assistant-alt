using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using ShapesLibrary.Attributes;
using ShapesLibrary.Enums;
using ShapesLibrary.Exceptions;
using ShapesLibrary.Interfaces;
using ShapesLibrary.Services;
using System.Diagnostics.CodeAnalysis;

namespace ShapesLibrary.ShapeInfoModels
{
    /// <summary>
    /// Information of Capsule , Origin Point : Capsules Rectangle Center
    /// </summary>
    [ShapeOrigin("RectangleCenter")]
    public class CapsuleInfo : ShapeInfo, IPolygonSimulatable, IRingableShape, IDeepClonable<CapsuleInfo>, IEqualityComparerCreator<CapsuleInfo>
    {
        public const int MINSIMULATIONSIDES = 6;
        public const int OPTIMALSIMULATIONSIDES = 36;

        public CapsuleInfo(double length, double height, double locationX = 0, double locationY = 0) : base(locationX, locationY)
        {
            ShapeType = ShapeInfoType.CapsuleShapeInfo;
            Length = length;
            Height = height;
        }
        /// <summary>
        /// Creates a Capsule with the Defined dimensions and Orientation
        /// </summary>
        /// <param name="dimension1">One of the Dimensions Height or Length</param>
        /// <param name="dimension2">The other Dimension</param>
        /// <param name="orientation">The Orientation</param>
        /// <param name="locationX"></param>
        /// <param name="locationY"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public CapsuleInfo(double dimension1, double dimension2, CapsuleOrientation orientation, double locationX = 0, double locationY = 0) : base(locationX, locationY)
        {
            Orientation = orientation;
            switch (Orientation)
            {
                case CapsuleOrientation.Horizontal:
                    Length = Math.Max(dimension1, dimension2);
                    Height = Math.Min(dimension1, dimension2);
                    break;
                case CapsuleOrientation.Vertical:
                    Height = Math.Max(dimension1, dimension2);
                    Length = Math.Min(dimension1, dimension2);
                    break;
                default:
                case CapsuleOrientation.Undefined:
                    throw new EnumValueNotSupportedException(Orientation);
            }
        }

        private double length;
        public double Length { get => length; set => SetTotalLength(value); }

        private double height;
        public double Height { get => height; set => SetTotalHeight(value); }

        public double MajorLength { get => Orientation == CapsuleOrientation.Horizontal ? Length : Height; }
        public double MinorLength { get => Orientation == CapsuleOrientation.Horizontal ? Height : Length; }
        public double CircleDiameter { get => MinorLength; }
        public double CircleRadius { get => MinorLength / 2d; }
        public CapsuleOrientation Orientation { get; private set; }
        public int MinSimulationSides => MINSIMULATIONSIDES;
        public int OptimalSimulationSides => OPTIMALSIMULATIONSIDES;

        public RectangleInfo GetMiddleRectangle()
        {
            return Orientation switch
            {
                CapsuleOrientation.Horizontal => new(Length - CircleDiameter, Height, 0, this.LocationX, this.LocationY),
                CapsuleOrientation.Vertical => new(Length, Height - CircleDiameter, 0, this.LocationX, this.LocationY),
                _ => throw new EnumValueNotSupportedException(Orientation),
            };
        }
        /// <summary>
        /// Returns the circle which forms the edge parts of the Capsule .
        /// </summary>
        /// <param name="isFirstCircle">Weather this is the Left/Top(true) or Right/Bottom (false) circle</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CircleInfo GetCapsuleCircle(bool isFirstCircle)
        {
            switch (Orientation)
            {
                case CapsuleOrientation.Horizontal:
                    double distanceToCircleH = GetTotalLength() / 2d - CircleRadius;
                    if (isFirstCircle) return new(CircleRadius, LocationX - distanceToCircleH, LocationY);
                    else return new(CircleRadius, LocationX + distanceToCircleH, LocationY);
                case CapsuleOrientation.Vertical:
                    double distanceToCircleV = GetTotalHeight() / 2d - CircleRadius;
                    if (isFirstCircle) return new(CircleRadius, LocationX, LocationY - distanceToCircleV);
                    else return new(CircleRadius, LocationX, LocationY + distanceToCircleV);
                case CapsuleOrientation.Undefined:
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }
        }
        public override PointXY GetCentroid()
        {
            return GetLocation();
        }
        public override void Scale(double scaleFactor)
        {
            Length *= scaleFactor;
            Height *= scaleFactor;
        }
        public override CapsuleInfo GetDeepClone()
        {
            return (CapsuleInfo)this.MemberwiseClone();
        }
        public override CapsuleInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            double shrink = perimeterShrink * 2;
            return new CapsuleInfo(this.GetTotalHeight() - shrink, this.GetTotalLength() - shrink, this.LocationX, this.LocationY);
        }
        public override void RotateClockwise()
        {
            RotateClockwise(this);
            RotationRadians += Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        /// <summary>
        /// Rotates a Capsule Clockwise
        /// </summary>
        /// <param name="capsule"></param>
        public static void RotateClockwise(CapsuleInfo capsule)
        {
            var length = capsule.Length;
            var height = capsule.Height;

            capsule.Length = height;
            capsule.Height = length;
        }
        public override void RotateAntiClockwise()
        {
            //there are only two orientations , the clockwise or anticlockwise rotation has no meaning
            //here both are the same
            RotateAntiClockwise(this);
            RotationRadians -= Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        /// <summary>
        /// Rotates a Capsule Anti-Clockwise (same as clockwise capsule is symmetrical)
        /// </summary>
        /// <param name="capsule"></param>
        public static void RotateAntiClockwise(CapsuleInfo capsule) => RotateClockwise(capsule);
        public override double GetPerimeter()
        {
            return MathCalculations.Capsule.GetPerimeter(GetTotalHeight(), GetTotalLength());
        }
        public override double GetArea()
        {
            double rectangleArea = (MajorLength - CircleDiameter) * MinorLength;
            double circleArea = Math.PI * Math.Pow(CircleRadius, 2);
            return rectangleArea + circleArea;
        }

        public override bool Contains(PointXY point)
        {
            return GetCapsuleCircle(true).Contains(point)
                || GetCapsuleCircle(false).Contains(point) || GetMiddleRectangle().Contains(point);
        }
        public override bool Contains(ShapeInfo shape)
        {
            return shape switch
            {
                CircleInfo circle => GetPolygonSimulation(OptimalSimulationSides).Contains(circle),
                PolygonInfo polygon => GetPolygonSimulation(OptimalSimulationSides).Contains(polygon),
                IPolygonSimulatable simulatable => GetPolygonSimulation(OptimalSimulationSides).Contains(simulatable.GetOptimalPolygonSimulation()),
                _ => throw new NotSupportedContainmentException(this, shape),
            };
        }

        public CapsuleRingInfo GetEquivalentRingShape(double ringThickness)
        {
            return new CapsuleRingInfo(GetTotalLength(), GetTotalHeight(), ringThickness, this.LocationX, this.LocationY);
        }
        IRingShape IRingableShape.GetRingShape(double ringThickness)
        {
            return GetEquivalentRingShape(ringThickness);
        }

        public override RectangleInfo GetBoundingBox()
        {
            return new RectangleInfo(GetTotalLength(), GetTotalHeight(), 0, this.LocationX, this.LocationY);
        }
        public override double GetTotalLength()
        {
            return length;
        }
        public override double GetTotalHeight()
        {
            return height;
        }
        public override void SetTotalLength(double length)
        {
            this.length = length;
            SetOrientation();
        }
        public override void SetTotalHeight(double height)
        {
            this.height = height;
            SetOrientation();
        }

        private void SetOrientation()
        {
            if (Height >= Length)
            {
                Orientation = CapsuleOrientation.Vertical;
            }
            else if (Height < Length)
            {
                Orientation = CapsuleOrientation.Horizontal;
            }
            else
            {
                throw new InvalidOperationException("Undefined Comparison Operation in Capsule");
            }
        }
        public static CapsuleInfo CapsuleZero() => new(0, 0);
        static IEqualityComparer<CapsuleInfo> IEqualityComparerCreator<CapsuleInfo>.GetComparer()
        {
            return new CapsuleInfoEqualityComparer();
        }

        public override bool IntersectsWithPoint(PointXY point)
        {
            return this.GetPolygonSimulation(OptimalSimulationSides).IntersectsWithPoint(point);
        }
        public override bool IntersectsWithShape(ShapeInfo shape)
        {
            return shape switch
            {
                RectangleRingInfo rectRing => rectRing.IntersectsWithShape(this),
                CircleRingInfo circleRing => circleRing.IntersectsWithShape(this),
                PolygonInfo polygon => GetPolygonSimulation(OptimalSimulationSides).IntersectsWithPolygon(polygon),
                IPolygonSimulatable simulatable => GetPolygonSimulation(OptimalSimulationSides).IntersectsWithPolygon(simulatable.GetOptimalPolygonSimulation()),
                _ => throw new NotSupportedIntersectionException(this, shape),
            };
        }

        public PolygonInfo GetPolygonSimulation(int sides)
        {
            if (sides < MinSimulationSides) throw new SimulationSidesOutOfRangeException(this);

            // The basic structure needs at least the 6 sides , 2 for the middle rectangle and 4 for the two circles (2 each).
            // The Rectangle Vertices will be included in the vertices of the two circle's disection once the lists are combined
            // The Needed sides for each circle are the (points - 1) as we will not close the arc but continue from the rectangle side
            // Thus we assume sides = points for the Capsule

            // Determine whether the remaining sides are odd or even.
            var isOdd = sides % 2 != 0;
            // Calculate how to distribute sides between the two circles.
            int circle1Sides, circle2Sides;

            if (isOdd)
            {
                // If odd, assign one extra side to the first circle.
                sides--; // Adjust for odd number by subtracting one.
                circle1Sides = (sides / 2) + 1;
                circle2Sides = sides / 2;
            }
            else
            {
                // If even, distribute equally between both circles.
                circle1Sides = sides / 2;
                circle2Sides = sides / 2;
            }

            //Find the two circles
            var circle1 = GetCapsuleCircle(true); //Left-Top
            var circle2 = GetCapsuleCircle(false); //Right-Bottom

            // Create the vertices Lists
            List<PointXY> circle1Points, circle2Points;

            //According to orientation proceed with adding vertices in order
            switch (Orientation)
            {
                case CapsuleOrientation.Horizontal:
                    //Add the Left Circle Vertices
                    circle1Points = MathCalculations.Circle.GetCircleArcPoints(circle1, circle1Sides, Math.PI / 2, 3 * Math.PI / 2);

                    //Add the Right Circle Vertices
                    circle2Points = MathCalculations.Circle.GetCircleArcPoints(circle2, circle2Sides, -Math.PI / 2, Math.PI / 2);

                    break;
                case CapsuleOrientation.Vertical:
                    //Add the Top Circle Vertices
                    circle1Points = MathCalculations.Circle.GetCircleArcPoints(circle1, circle1Sides, -Math.PI, 0);

                    //Add the Bottom Circle Vertices
                    circle2Points = MathCalculations.Circle.GetCircleArcPoints(circle2, circle2Sides, 0, Math.PI);

                    break;
                case CapsuleOrientation.Undefined:
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }

            return new PolygonInfo([.. circle1Points, .. circle2Points]);
        }
    }

    public class CapsuleInfoEqualityComparer : IEqualityComparer<CapsuleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public CapsuleInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly ShapeInfoBaseEqualityComparer baseComparer;

        public bool Equals(CapsuleInfo? x, CapsuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Length == y!.Length &&
                x.Height == y.Height &&
                x.Orientation == y.Orientation;
        }

        public int GetHashCode([DisallowNull] CapsuleInfo obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.Length, obj.Height, obj.Orientation);
        }
    }

    public class CapsuleRingInfo : CapsuleInfo, IDeepClonable<CapsuleRingInfo>, IRingShape , IEqualityComparerCreator<CapsuleRingInfo>
    {
        public double Thickness { get; set; }
        public CapsuleRingInfo(double length, double height, double thickness, double locationX = 0, double locationY = 0) : base(length, height, locationX, locationY)
        {
            ShapeType = ShapeInfoType.CapsuleRingShapeInfo;
            Thickness = thickness;
        }
        public override CapsuleRingInfo GetDeepClone()
        {
            return (CapsuleRingInfo)this.MemberwiseClone();
        }
        public static CapsuleRingInfo CapsuleRingZero() => new(0, 0,0);

        public CapsuleInfo GetInnerRingWholeShape()
        {
            var thicknessX2 = Thickness * 2;
            return new CapsuleInfo(GetTotalLength() - thicknessX2, GetTotalHeight() - thicknessX2, this.LocationX, this.LocationY);
        }

        IRingableShape IRingShape.GetInnerRingWholeShape()
        {
            return GetInnerRingWholeShape();
        }

        public CapsuleInfo GetOuterRingWholeShape()
        {
            return new CapsuleInfo(GetTotalLength(), GetTotalHeight(), this.LocationX, this.LocationY);
        }

        IRingableShape IRingShape.GetOuterRingWholeShape()
        {
            return GetOuterRingWholeShape();
        }

        static IEqualityComparer<CapsuleRingInfo> IEqualityComparerCreator<CapsuleRingInfo>.GetComparer()
        {
            return new CapsuleRingInfoEqualityComparer();
        }
    }

    public class CapsuleRingInfoEqualityComparer : IEqualityComparer<CapsuleRingInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public CapsuleRingInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly CapsuleInfoEqualityComparer baseComparer;

        public bool Equals(CapsuleRingInfo? x, CapsuleRingInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Thickness == y!.Thickness;
        }

        public int GetHashCode([DisallowNull] CapsuleRingInfo obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.Thickness);
        }
    }
}
