using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using ShapesLibrary.Attributes;
using ShapesLibrary.Enums;
using ShapesLibrary.Exceptions;
using ShapesLibrary.Interfaces;
using ShapesLibrary.Services;
using System.Diagnostics.CodeAnalysis;
using static ShapesLibrary.Services.MathCalculations;

namespace ShapesLibrary.ShapeInfoModels
{
    /// <summary>
    /// Ellipse information , Origin Point : Ellipse's center
    /// </summary>
    [ShapeOrigin("EllipseCenter")]
    public class EllipseInfo : ShapeInfo,IPolygonSimulatable , IDeepClonable<EllipseInfo> , IRingableShape , IEqualityComparerCreator<EllipseInfo>
    {
        public const int MINSIMULATIONSIDES = 8;
        public const int OPTIMALSIMULATIONSIDES = 48;
        public int OptimalSimulationSides => OPTIMALSIMULATIONSIDES;
        private double length;
        public double Length { get => length; set => SetTotalLength(value); }
        private double height;
        public double Height { get => height; set => SetTotalHeight(value); }
        public double RadiusMajor { get => DiameterMajor / 2d; }
        public double DiameterMajor { get => Orientation == EllipseOrientation.Horizontal ? Length : Height; }
        public double RadiusMinor { get => DiameterMinor / 2d; }
        public double DiameterMinor { get => Orientation == EllipseOrientation.Horizontal ? Height : Length; }

        public EllipseOrientation Orientation { get; private set; }
        public int MinSimulationSides => MINSIMULATIONSIDES;

        /// <summary>
        /// Creates an Ellipse
        /// </summary>
        /// <param name="length">The total Length of the Ellipse</param>
        /// <param name="height">The total Height of the Ellipse</param>
        /// <param name="locationX">The X Coordinate of the Center of the Ellipse</param>
        /// <param name="locationY">The Y Coordinate of the Center of the Ellipse</param>
        public EllipseInfo(double length, double height,double locationX = 0 , double locationY = 0) : base(locationX, locationY)
        {
            ShapeType = ShapeInfoType.EllipseShapeInfo;
            Length = length;
            Height = height;
        }
        /// <summary>
        /// Creates an Ellipse
        /// </summary>
        /// <param name="radius1">One of the Radiuses of the Ellipse</param>
        /// <param name="radius2">The other Radius of the Ellipse</param>
        /// <param name="orientation">The Orientation of the Ellipse</param>
        /// <param name="locationX">The X Coordinate of the Center of the Ellipse</param>
        /// <param name="locationY">The Y Coordinate of the Center of the Ellipse</param>
        /// <exception cref="Exception">When Orientation is Not Defined or not Supported Value</exception>
        public EllipseInfo(double radius1, double radius2, EllipseOrientation orientation, double locationX = 0, double locationY = 0) : base(locationX, locationY)
        {
            ShapeType = ShapeInfoType.EllipseShapeInfo;
            Orientation = orientation;
            switch (Orientation)
            {
                case EllipseOrientation.Horizontal:
                    Length = Math.Max(radius1, radius2) * 2d;
                    Height = Math.Min(radius1, radius2) * 2d;
                    break;
                case EllipseOrientation.Vertical:
                    Length = Math.Min(radius1, radius2) * 2d;
                    Height = Math.Max(radius1, radius2) * 2d;
                    break;
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }
        }

        public static EllipseInfo ZeroEllipse() => new(0, 0);

        public override EllipseInfo GetDeepClone()
        {
            return (EllipseInfo)this.MemberwiseClone();
        }
        
        public override EllipseInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            return new EllipseInfo(this.RadiusMajor - perimeterShrink , this.RadiusMinor - perimeterShrink, this.Orientation, this.LocationX, this.LocationY);
        }
        private void SetOrientation()
        {
            if (height >= length)
            {
                Orientation = EllipseOrientation.Vertical;
            }
            else if (height < length)
            {
                Orientation = EllipseOrientation.Horizontal;
            }
            else
            {
                throw new InvalidOperationException("Undefined Comparison Operation in Ellipse");
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

        public override void RotateClockwise()
        {
            var length = Length;
            var height = Height;
            Height = length;
            Length = height;
            RotationRadians += Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void RotateAntiClockwise()
        {
            RotateClockwise();
            RotationRadians -= Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }

        public override double GetPerimeter()
        {
            return MathCalculations.Ellipse.GetPerimeter(RadiusMajor,RadiusMinor);
        }
        public override double GetArea()
        {
            return Math.PI * RadiusMajor * RadiusMinor;
        }
        public EllipseRingInfo GetEquivalentRingShape(double ringThickness)
        {
            return new EllipseRingInfo(GetTotalLength(), GetTotalHeight(), ringThickness, this.LocationX, this.LocationY);
        }
        IRingShape IRingableShape.GetRingShape(double ringThickness)
        {
            return GetEquivalentRingShape(ringThickness);
        }

        static IEqualityComparer<EllipseInfo> IEqualityComparerCreator<EllipseInfo>.GetComparer()
        {
            return new EllipseInfoEqualityComparer();
        }

        public override RectangleInfo GetBoundingBox()
        {
            var bBox = new RectangleInfo(GetTotalLength(), GetTotalHeight(), 0, this.LocationX, this.LocationY);
            return bBox;
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

        public override bool Contains(PointXY point)
        {
            EllipseEquation ellipse = new(this);
            return ellipse.ContainsPoint(point);
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

        public override bool IntersectsWithPoint(PointXY point)
        {
            EllipseEquation ellipse = new(this);
            return ellipse.IntersectsWithPoint(point);
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
            var vertices = MathCalculations.Ellipse.GetEllipseArcPoints(this, sides, 0, 2 * Math.PI);
            //vertices.Reverse();
            return new PolygonInfo(vertices);
        }
    }
    public class EllipseInfoEqualityComparer : IEqualityComparer<EllipseInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public EllipseInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly ShapeInfoBaseEqualityComparer baseComparer;

        public bool Equals(EllipseInfo? x, EllipseInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Length == y!.Length &&
                x.Height == y.Height &&
                x.Orientation == y.Orientation;
        }

        public int GetHashCode([DisallowNull] EllipseInfo obj)
        {
            int hash = baseComparer.GetHashCode(obj);
            return HashCode.Combine(hash, obj.Length, obj.Height, obj.Orientation);
        }
    }
    public class EllipseRingInfo : EllipseInfo, IDeepClonable<EllipseRingInfo> , IRingShape , IEqualityComparerCreator<EllipseRingInfo>
    {
        public double Thickness { get; set; }
        public EllipseRingInfo(double length, double height,double thickness, double locationX = 0, double locationY = 0) 
            : base(length, height, locationX, locationY)
        {
            ShapeType = ShapeInfoType.EllipseRingShapeInfo;
            Thickness = thickness;
        }

        public EllipseRingInfo(double radius1, double radius2,double thickness, EllipseOrientation orientation, double locationX = 0, double locationY = 0) 
            : base(radius1, radius2, orientation, locationX, locationY)
        {
            ShapeType = ShapeInfoType.EllipseRingShapeInfo;
            Thickness = thickness;
        }

        public static EllipseRingInfo ZeroEllipseRing() => new(0, 0,0);
        public override EllipseRingInfo GetDeepClone()
        {
            return (EllipseRingInfo)this.MemberwiseClone();
        }

        public EllipseInfo GetInnerRingWholeShape()
        {
            return new EllipseInfo(this.RadiusMajor - Thickness, this.RadiusMinor - Thickness, this.Orientation, this.LocationX, this.LocationY);
        }

        public EllipseInfo GetOuterRingWholeShape()
        {
            return new EllipseInfo(this.RadiusMajor, this.RadiusMinor, this.Orientation, this.LocationX, this.LocationY);
        }

        IRingableShape IRingShape.GetInnerRingWholeShape()
        {
            return GetInnerRingWholeShape();
        }

        IRingableShape IRingShape.GetOuterRingWholeShape()
        {
            return GetOuterRingWholeShape();
        }

        static IEqualityComparer<EllipseRingInfo> IEqualityComparerCreator<EllipseRingInfo>.GetComparer()
        {
            return new EllipseRingInfoEqualityComparer();
        }
    }
    public class EllipseRingInfoEqualityComparer : IEqualityComparer<EllipseRingInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public EllipseRingInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }
        private readonly EllipseInfoEqualityComparer baseComparer;

        public bool Equals(EllipseRingInfo? x, EllipseRingInfo? y)
        {
            return baseComparer.Equals(x, y) && 
                x!.Thickness == y!.Thickness;
        }

        public int GetHashCode([DisallowNull] EllipseRingInfo obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.Thickness);
        }
    }

}
