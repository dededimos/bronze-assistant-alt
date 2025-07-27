using CommonHelpers.Comparers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using ShapesLibrary.Attributes;
using ShapesLibrary.Exceptions;
using ShapesLibrary.Interfaces;
using ShapesLibrary.Services;
using System.Diagnostics.CodeAnalysis;

namespace ShapesLibrary.ShapeInfoModels
{
    /// <summary>
    /// Circle Information , Origin Point Circle's Center
    /// </summary>
    [ShapeOrigin("CircleCenter")]
    public class CircleInfo : ShapeInfo,IPolygonSimulatable, IRingableShape, IDeepClonable<CircleInfo>, IEqualityComparerCreator<CircleInfo>
    {
        public const int MINSIMULATIONSIDES = 8;
        public const int OPTIMALSIMULATIONSIDES = 36;
        public int OptimalSimulationSides => OPTIMALSIMULATIONSIDES;
        public double Diameter { get => Radius * 2d; }
        public double Radius { get; set; }
        public double CenterX { get => LocationX; }
        public double CenterY { get => LocationY; }
        /// <summary>
        /// The LeftMost X coordinate of the Circle
        /// </summary>
        public double LeftX { get => LocationX - Radius; }
        /// <summary>
        /// The RightMost X coordinate of the Circle
        /// </summary>
        public double RightX { get => LocationX + Radius; }
        /// <summary>
        /// The TopMost Y coordinate of the Circle
        /// </summary>
        public double TopY { get => LocationY - Radius; }
        /// <summary>
        /// The BottomMost Y coordinate of the Circle
        /// </summary>
        public double BottomY { get => LocationY + Radius; }
        public int MinSimulationSides => MINSIMULATIONSIDES;

        public CircleInfo(double radius , double locationX = 0 , double locationY = 0) :base(locationX , locationY)
        {
            ShapeType = Enums.ShapeInfoType.CircleShapeInfo;
            Radius = radius;
        }

        public override CircleInfo GetDeepClone()
        {
            return (CircleInfo)this.MemberwiseClone();
        }
        public override CircleInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            return new(this.Radius - perimeterShrink, this.LocationX, this.LocationY);
        }
        /// <summary>
        /// Returns the inscribed Polygon in the Circle
        /// </summary>
        /// <param name="sides">The Number of sides of the Polygon</param>
        /// <param name="rotationRadians">The rotation Angle of the Polygon</param>
        /// <returns></returns>
        public RegularPolygonInfo GetInscribedPolygon(int sides,double rotationRadians = 0)
        {
            return new RegularPolygonInfo(this,sides, rotationRadians);
        }
        public override PointXY GetCentroid()
        {
            return GetLocation();
        }
        public override void Scale(double scaleFactor)
        {
            Radius *= scaleFactor;
        }
        public override void RotateClockwise()
        {
            RotationRadians += Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void RotateAntiClockwise()
        {
            RotationRadians -= Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override double GetPerimeter()
        {
            return MathCalculations.Circle.GetPerimeter(Radius);
        }
        public override double GetArea()
        {
            return Math.PI * Math.Pow(Radius, 2);
        }

        public override bool Contains(PointXY point)
        {
            return MathCalculations.Circle.IsPointInCircle(this, point);
        }
        public override bool ContainsSimpleRectangle(RectangleInfo rect)
        {
            return Contains(rect.TopLeftVertex) &&
                   Contains(rect.TopRightVertex) &&
                   Contains(rect.BottomLeftVertex) &&
                   Contains(rect.BottomRightVertex);
        }
        public override bool Contains(ShapeInfo shape)
        {
            return shape switch
            {
                CircleInfo circle => ContainsCircle(circle),
                RectangleInfo rect => rect.IsSimpleRectangle ? ContainsSimpleRectangle(rect) : ContainsPolygon(rect.GetPolygonSimulation(rect.OptimalSimulationSides)),
                PolygonInfo polygon => ContainsPolygon(polygon),
                IPolygonSimulatable simulatable => ContainsPolygon(simulatable.GetOptimalPolygonSimulation()),
                _ => throw new NotSupportedContainmentException(this, shape),
            };
        }

        private bool ContainsPolygon(PolygonInfo polygon)
        {
            return polygon.Vertices.All(v=> Contains(v));
        }
        private bool ContainsCircle(CircleInfo circle)
        {
            return this.LeftX < circle.LeftX - DoubleSafeEqualityComparer.DefaultEpsilon &&
                   this.RightX > circle.RightX + DoubleSafeEqualityComparer.DefaultEpsilon &&
                   this.TopY < circle.TopY - DoubleSafeEqualityComparer.DefaultEpsilon &&
                   this.BottomY > circle.BottomY + DoubleSafeEqualityComparer.DefaultEpsilon;
        }

        public override bool IntersectsWithPoint(PointXY point)
        {
            // Get the distance squared between the point and the circle's center
            double distanceSquared = MathCalculations.Points.GetPointsSquaredDistance(point,GetCentroid());

            // Compare the squared distance to the squared radius (with epsilon for precision)
            double radiusSquared = Math.Pow(this.Radius, 2);

            // Check if the point is inside or on the boundary of the circle
            return distanceSquared <= radiusSquared + DoubleSafeEqualityComparer.DefaultEpsilon;
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

        public override ProjectionInfo GetProjectionOntoAxis(Vector2D direction)
        {
            double centerProjection = direction.Normalize().Dot(this.GetLocation());
            double min = centerProjection - Radius;
            double max = centerProjection + Radius;

            return new(min, max);
        }

        /// <summary>
        /// Returns the Inscribed Polygon in the Circle (Will always be a Regular Polygon)
        /// </summary>
        /// <param name="sides">The Number of Sides of the inscribed Polygon</param>
        /// <returns></returns>
        public PolygonInfo GetPolygonSimulation(int sides)
        {
            if (sides < MinSimulationSides) throw new SimulationSidesOutOfRangeException(this);
            return GetInscribedPolygon(sides);
        }

        public CircleRingInfo GetEquivalentRingShape(double ringThickness)
        {
            return new CircleRingInfo(this.Radius,ringThickness,this.LocationX,this.LocationY);
        }
        IRingShape IRingableShape.GetRingShape(double ringThickness)
        {
            return GetEquivalentRingShape(ringThickness);
        }


        public override RectangleInfo GetBoundingBox()
        {
            return new RectangleInfo(this.Diameter,this.Diameter,0, this.LocationX, this.LocationY);
        }
        /// <summary>
        /// Returns the Radius of the Circle as the Size Estimate
        /// </summary>
        /// <returns></returns>
        public override double GetSizeEstimate()
        {
            return Radius;
        }
        public override double GetTotalLength()
        {
            return Diameter;
        }
        public override double GetTotalHeight()
        {
            return Diameter;
        }
        public override void SetTotalLength(double length)
        {
            Radius = length / 2d;
        }
        public override void SetTotalHeight(double height)
        {
            Radius = height / 2d;
        }

        protected override string GetDimensionsString()
        {
            try
            {
                return $"Φ {Diameter}mm";
            }
            catch (Exception)
            {
                return $"Φ ???mm";
            }
        }
        public static CircleInfo ZeroCircle() => new(0);
        static IEqualityComparer<CircleInfo> IEqualityComparerCreator<CircleInfo>.GetComparer()
        {
            return new CircleInfoEqualityComparer();
        }

        
    }
    public class CircleInfoEqualityComparer : IEqualityComparer<CircleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public CircleInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }
        private readonly ShapeInfoBaseEqualityComparer baseComparer;

        public bool Equals(CircleInfo? x, CircleInfo? y)
        {
            return baseComparer.Equals(x, y)
                && x!.Radius == y!.Radius;
        }

        public int GetHashCode([DisallowNull] CircleInfo obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.Radius);
        }
    }
    [ShapeOrigin("CircleCenter")]
    public class CircleRingInfo : CircleInfo , IDeepClonable<CircleRingInfo> , IRingShape , IEqualityComparerCreator<CircleRingInfo>
    {
        public double Thickness { get; set; }
        public CircleRingInfo(double radius,double thickness, double locationX = 0, double locationY = 0) : base(radius, locationX, locationY)
        {
            ShapeType = Enums.ShapeInfoType.CircleRingShapeInfo;
            Thickness = thickness;
        }
        public override void Scale(double scaleFactor)
        {
            base.Scale(scaleFactor);
            Thickness *= scaleFactor;
        }
        public override CircleRingInfo GetDeepClone()
        {
            return (CircleRingInfo)this.MemberwiseClone();
        }

        public CircleInfo GetInnerRingWholeShape()
        {
            return new CircleInfo(Radius - Thickness, this.LocationX, this.LocationY);
        }
        IRingableShape IRingShape.GetInnerRingWholeShape()
        {
            return GetInnerRingWholeShape();
        }

        public CircleInfo GetOuterRingWholeShape()
        {
            return new CircleInfo(Radius, this.LocationX, this.LocationY);
        }

        IRingableShape IRingShape.GetOuterRingWholeShape()
        {
            return GetOuterRingWholeShape();
        }

        public override bool IntersectsWithPoint(PointXY point)
        {
            var innerCircle = GetInnerRingWholeShape();
            var outterCircle = GetOuterRingWholeShape();
            //outside must intersect , inside must not contain ! (can intersect no problem)
            return outterCircle.IntersectsWithPoint(point) && !innerCircle.Contains(point);
        }
        public override bool IntersectsWithShape(ShapeInfo shape)
        {
            var innerCircle = GetInnerRingWholeShape();
            var outterCircle = GetOuterRingWholeShape();
            //outside must intersect , inside must not contain ! (can intersect edge but not be wholly inside)
            return outterCircle.IntersectsWithShape(shape) && !innerCircle.Contains(shape);
        }


        public static CircleRingInfo ZeroCircleRing() => new(0, 0);

        static IEqualityComparer<CircleRingInfo> IEqualityComparerCreator<CircleRingInfo>.GetComparer()
        {
            return new CircleRingInfoEqualityComparer();
        }
    }
    public class CircleRingInfoEqualityComparer : IEqualityComparer<CircleRingInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public CircleRingInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }
        private readonly CircleInfoEqualityComparer baseComparer;

        public bool Equals(CircleRingInfo? x, CircleRingInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Thickness == y!.Thickness;
        }

        public int GetHashCode([DisallowNull] CircleRingInfo obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.Thickness);
        }
    }

}
