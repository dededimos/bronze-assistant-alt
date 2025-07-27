using CommonHelpers.Comparers;
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
    /// Quadrant information , Origin Point Circles Center
    /// </summary>
    [ShapeOrigin("CircleCenter")]
    public class CircleQuadrantInfo : ShapeInfo, IPolygonSimulatable, IRingableShape, IDeepClonable<CircleQuadrantInfo>, IEqualityComparerCreator<CircleQuadrantInfo>
    {
        public const int MINSIMULATIONSIDES = 4;
        public const int OPTIMALSIMULATIONSIDES = 20;
        public int OptimalSimulationSides => OPTIMALSIMULATIONSIDES;
        public double Radius { get; set; }
        public double Diameter { get => Radius * 2; }
        public int MinSimulationSides => MINSIMULATIONSIDES;
        /// <summary>
        /// The First Tip of the Quadrant (By Clockwise Order)
        /// </summary>
        public PointXY Tip1 { get => GetTip1(); }
        private PointXY GetTip1()
        {
            return QuadrantPart switch
            {
                CircleQuadrantPart.TopLeft => new(LocationX - Radius, LocationY),
                CircleQuadrantPart.TopRight => new(LocationX, LocationY - Radius),
                CircleQuadrantPart.BottomRight => new(LocationX + Radius, LocationY),
                CircleQuadrantPart.BottomLeft => new(LocationX, LocationY + Radius),
                _ => throw new EnumValueNotSupportedException(QuadrantPart),
            };
        }
        /// <summary>
        /// The Second Tip of the Quadrant (By Clockwise Order)
        /// </summary>
        public PointXY Tip2 { get => GetTip2(); }
        private PointXY GetTip2()
        {
            return QuadrantPart switch
            {
                CircleQuadrantPart.TopLeft => new(LocationX, LocationY - Radius),
                CircleQuadrantPart.TopRight => new(LocationX + Radius, LocationY),
                CircleQuadrantPart.BottomRight => new(LocationX, LocationY + Radius),
                CircleQuadrantPart.BottomLeft => new(LocationX - Radius, LocationY),
                _ => throw new EnumValueNotSupportedException(QuadrantPart),
            };
        }

        public CircleQuadrantPart QuadrantPart { get; private set; }
        public CircleQuadrantInfo(double radius, CircleQuadrantPart quadrantPart = CircleQuadrantPart.TopLeft, double locationX = 0, double locationY = 0)
            : base(locationX, locationY)
        {
            ShapeType = ShapeInfoType.CircleQuadrantShapeInfo;
            Radius = radius;
            QuadrantPart = quadrantPart;
        }

        public static CircleQuadrantInfo ZeroQuadrant() => new(0);

        public CircleInfo GetCircle() => new(this.Radius, this.LocationX, this.LocationY);
        public void SetQuadrantPart(CircleQuadrantPart quadrantPart) => QuadrantPart = quadrantPart;
        public override CircleQuadrantInfo GetDeepClone()
        {
            return (CircleQuadrantInfo)this.MemberwiseClone();
        }
        public override RectangleInfo GetBoundingBox()
        {
            double locX;
            double locY;
            switch (QuadrantPart)
            {
                case CircleQuadrantPart.TopLeft:
                    locX = this.LocationX - Radius * 0.5d;
                    locY = this.LocationY - Radius * 0.5d;
                    break;
                case CircleQuadrantPart.TopRight:
                    locX = this.LocationX + Radius * 0.5d;
                    locY = this.LocationY - Radius * 0.5d;
                    break;
                case CircleQuadrantPart.BottomLeft:
                    locX = this.LocationX - Radius * 0.5d;
                    locY = this.LocationY + Radius * 0.5d;
                    break;
                case CircleQuadrantPart.BottomRight:
                    locX = this.LocationX + Radius * 0.5d;
                    locY = this.LocationY + Radius * 0.5d;
                    break;
                case CircleQuadrantPart.Undefined:
                default:
                    throw new Exception($"Unrecognized or Undefined {nameof(CircleQuadrantPart)}");
            }
            //Make a bounding box in the Origin Center of the Quadrant
            var boundingBox = new RectangleInfo(this.Radius, this.Radius, 0, locX, locY);
            //Shift the bounding box Origin location according to the Orientation of the Quadrant
            
            return boundingBox;
        }

        public override void RotateClockwise()
        {
            RotateClockwise(this);
            RotationRadians += Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public static void RotateClockwise(CircleQuadrantInfo quadrant)
        {
            quadrant.QuadrantPart = quadrant.QuadrantPart switch
            {
                CircleQuadrantPart.TopLeft => CircleQuadrantPart.TopRight,
                CircleQuadrantPart.TopRight => CircleQuadrantPart.BottomRight,
                CircleQuadrantPart.BottomRight => CircleQuadrantPart.BottomLeft,
                CircleQuadrantPart.BottomLeft => CircleQuadrantPart.TopLeft,
                _ => throw new Exception($"Unrecognized or Undefined {nameof(CircleQuadrantPart)}"),
            };
        }
        public override void FlipHorizontally(double flipOriginX = double.NaN)
        {
            base.FlipHorizontally(flipOriginX);
            // Swap left and right quadrants
            QuadrantPart = QuadrantPart switch
            {
                CircleQuadrantPart.TopLeft => CircleQuadrantPart.TopRight,
                CircleQuadrantPart.TopRight => CircleQuadrantPart.TopLeft,
                CircleQuadrantPart.BottomLeft => CircleQuadrantPart.BottomRight,
                CircleQuadrantPart.BottomRight => CircleQuadrantPart.BottomLeft,
                _ => throw new ArgumentException("Unsupported quadrant part."),
            };

        }
        public override void FlipVertically(double flipOriginY = double.NaN)
        {
            base.FlipVertically(flipOriginY);
            // Swap top and bottom quadrants
            QuadrantPart = QuadrantPart switch
            {
                CircleQuadrantPart.TopLeft => CircleQuadrantPart.BottomLeft,
                CircleQuadrantPart.TopRight => CircleQuadrantPart.BottomRight,
                CircleQuadrantPart.BottomLeft => CircleQuadrantPart.TopLeft,
                CircleQuadrantPart.BottomRight => CircleQuadrantPart.TopRight,
                _ => throw new ArgumentException("Unsupported quadrant part."),
            };
        }

        public override void RotateAntiClockwise()
        {
            RotateAntiClockwise(this);
            RotationRadians -= Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public static void RotateAntiClockwise(CircleQuadrantInfo quadrant)
        {
            quadrant.QuadrantPart = quadrant.QuadrantPart switch
            {
                CircleQuadrantPart.TopLeft => CircleQuadrantPart.BottomLeft,
                CircleQuadrantPart.TopRight => CircleQuadrantPart.TopLeft,
                CircleQuadrantPart.BottomLeft => CircleQuadrantPart.BottomRight,
                CircleQuadrantPart.BottomRight => CircleQuadrantPart.TopRight,
                _ => throw new Exception($"Unrecognized or Undefined {nameof(CircleQuadrantPart)}"),
            };
        }

        public override double GetPerimeter()
        {
            return MathCalculations.CircleQuadrant.GetPerimeter(Radius);
        }
        public override double GetArea()
        {
            // Area of a full circle is π * r^2
            // Since this is a quadrant, we divide by 4
            return (Math.PI * Radius * Radius) / 4;
        }

        public CircleQuadrantRingInfo GetEquivalentRingShape(double ringThickness)
        {
            return new CircleQuadrantRingInfo(this.Radius, ringThickness, this.QuadrantPart, this.LocationX, this.LocationY);
        }
        IRingShape IRingableShape.GetRingShape(double ringThickness)
        {
            return GetEquivalentRingShape(ringThickness);
        }

        public override CircleQuadrantInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            double totalShrink = perimeterShrink * 2;
            double x = this.LocationX;
            double y = this.LocationY;
            if (shiftCenterToMatchParent)
            {
                switch (QuadrantPart)
                {
                    case CircleQuadrantPart.TopLeft:
                        x -= perimeterShrink;
                        y -= perimeterShrink;
                        break;
                    case CircleQuadrantPart.TopRight:
                        x += perimeterShrink;
                        y -= perimeterShrink;
                        break;
                    case CircleQuadrantPart.BottomLeft:
                        x -= perimeterShrink;
                        y += perimeterShrink;
                        break;
                    case CircleQuadrantPart.BottomRight:
                        x += perimeterShrink;
                        y += perimeterShrink;
                        break;
                    default:
                        throw new EnumValueNotSupportedException(QuadrantPart);
                }
            }
            return new CircleQuadrantInfo(Radius - totalShrink, QuadrantPart, x, y);
        }
        public override void Scale(double scaleFactor)
        {
            Radius *= scaleFactor;
        }
        /// <summary>
        /// Calculates the centroid of the circle quadrant.
        /// </summary>
        /// <returns>A PointXY representing the coordinates of the centroid of the quadrant.</returns>
        public override PointXY GetCentroid()
        {
            // Step 1: Calculate the centroid distance from the circle center
            double centroidDistance = (4 * Radius) / (3 * Math.PI);

            // Step 2: Determine the position of the centroid based on the quadrant's orientation
            double centroidX = LocationX;
            double centroidY = LocationY;

            switch (QuadrantPart)
            {
                case CircleQuadrantPart.TopLeft:
                    centroidX -= centroidDistance;
                    centroidY -= centroidDistance;
                    break;
                case CircleQuadrantPart.TopRight:
                    centroidX += centroidDistance;
                    centroidY -= centroidDistance;
                    break;
                case CircleQuadrantPart.BottomRight:
                    centroidX += centroidDistance;
                    centroidY += centroidDistance;
                    break;
                case CircleQuadrantPart.BottomLeft:
                    centroidX -= centroidDistance;
                    centroidY += centroidDistance;
                    break;
                default:
                    throw new EnumValueNotSupportedException(QuadrantPart);
            }

            return new PointXY(centroidX, centroidY);
        }
        static IEqualityComparer<CircleQuadrantInfo> IEqualityComparerCreator<CircleQuadrantInfo>.GetComparer()
        {
            return new CircleQuadrantInfoEqualityComparer();
        }

        public override double GetTotalLength()
        {
            return Radius;
        }

        public override double GetTotalHeight()
        {
            return Radius;
        }

        public override void SetTotalLength(double length)
        {
            Radius = length;
        }

        public override void SetTotalHeight(double height)
        {
            Radius = height;
        }

        public override bool Contains(PointXY point)
        {
            double dx = point.X - LocationX;
            double dy = point.Y - LocationY;

            //Is out of circle's boundary
            if (dx * dx + dy * dy > ((Radius * Radius) + DoubleSafeEqualityComparer.DefaultEpsilon)) return false;
            
            //Else it is inside the circle but maybe not inside the quadrant
            //To check it is within the quadrant we only need to validate that the dx and dy as follows

            switch (QuadrantPart)
            {
                case CircleQuadrantPart.TopLeft:
                    //dx and dy negative  which means the point is oriented left and above center
                    //Any other orientation means its outside of the TopLeft Quadrant
                    return dx < DoubleSafeEqualityComparer.DefaultEpsilon && dy < DoubleSafeEqualityComparer.DefaultEpsilon;
                case CircleQuadrantPart.TopRight:
                    //dx positive and dy negative  which means the point is oriented right and above center
                    //Any other orientation means its outside of the TopRight Quadrant
                    return dx > -DoubleSafeEqualityComparer.DefaultEpsilon && dy < DoubleSafeEqualityComparer.DefaultEpsilon;
                case CircleQuadrantPart.BottomLeft:
                    //dx negative and dy positive  which means the point is oriented left and below center
                    //Any other orientation means its outside of the BottomLeft Quadrant
                    return dx < DoubleSafeEqualityComparer.DefaultEpsilon && dy > -DoubleSafeEqualityComparer.DefaultEpsilon;
                case CircleQuadrantPart.BottomRight:
                    //dx positive and dy positive  which means the point is oriented right and below center
                    //Any other orientation means its outside of the BottomRight Quadrant
                    return dx > -DoubleSafeEqualityComparer.DefaultEpsilon && dy > -DoubleSafeEqualityComparer.DefaultEpsilon;
                default:
                    throw new EnumValueNotSupportedException(QuadrantPart);
            }
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
            return Contains(point);
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

        /// <summary>
        /// Returns the Inscribed Polygon of the Quadrant
        /// </summary>
        /// <param name="sides">The Sides of the Polygon <para>At least 3 are needed</para></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PolygonInfo GetPolygonSimulation(int sides)
        {
            if (sides < MinSimulationSides) throw new SimulationSidesOutOfRangeException(this);

            // The minimum number of sides should be 3: one for the center of the circle and two for the ends of the quadrant.
            // Any additional vertices will be placed along the arc of the quadrant.
            // To find the additional vertices, we divide PI/2 by the extra number of sides (+1) and calculate the positions
            // using the circle's parametric equations based on the angle within the quadrant.
            var extraSides = sides - 3;

            // Initialize the list of vertices with the center point and Tip1 (assuming this.GetLocation() and Tip1 are predefined).
            List<PointXY> vertices = new List<PointXY> { this.GetLocation(), Tip1 };

            // Calculate the angular increment for each additional vertex.
            double angleDiffPerSide = (Math.PI / 2) / (extraSides + 1);

            // Get the starting and ending angles for the arc iteration (assuming GetArcDirectionalRangeRadians provides the correct range).
            (double startingAngle, double endAngle) = GetArcDirectionalRangeRadians();

            // Loop through the number of extra sides and calculate vertices at each step.
            for (int i = 1; i <= extraSides; i++)
            {
                double currentAngle = startingAngle + i * angleDiffPerSide;
                var newVertex = MathCalculations.Circle.GetPointOnCirclePerimeter(GetCircle(), currentAngle);
                vertices.Add(newVertex);
            }

            // Add the final Tip2 vertex (assuming Tip2 is predefined).
            vertices.Add(Tip2);

            // Return the resulting polygon with the calculated vertices.
            return new PolygonInfo(vertices);

        }

        /// <summary>
        /// Returns the start and end angles of the Quadrant in Radians in comparison with its circle (Clockwise order)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public (double start, double end) GetArcDirectionalRangeRadians()
        {
            return QuadrantPart switch
            {
                CircleQuadrantPart.TopLeft => (Math.PI, 3 * Math.PI / 2),
                CircleQuadrantPart.TopRight => (3 * Math.PI / 2, 2 * Math.PI),
                CircleQuadrantPart.BottomLeft => (Math.PI / 2, Math.PI),
                CircleQuadrantPart.BottomRight => (0d, Math.PI / 2),
                _ => throw new EnumValueNotSupportedException(QuadrantPart),
            };
        }
    }
    public class CircleQuadrantInfoEqualityComparer : IEqualityComparer<CircleQuadrantInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public CircleQuadrantInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly ShapeInfoBaseEqualityComparer baseComparer;

        public bool Equals(CircleQuadrantInfo? x, CircleQuadrantInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.QuadrantPart == y!.QuadrantPart &&
                x.Radius == y.Radius;
        }

        public int GetHashCode([DisallowNull] CircleQuadrantInfo obj)
        {
            int hash = baseComparer.GetHashCode(obj);
            return HashCode.Combine(hash, obj.QuadrantPart, obj.Radius);
        }
    }
    public class CircleQuadrantRingInfo : CircleQuadrantInfo, IDeepClonable<CircleQuadrantRingInfo>, IRingShape , IEqualityComparerCreator<CircleQuadrantRingInfo>
    {
        public double Thickness { get; set; }
        public CircleQuadrantRingInfo(double radius, double thickness, CircleQuadrantPart quadrantPart = CircleQuadrantPart.TopLeft, double locationX = 0, double locationY = 0) : base(radius, quadrantPart, locationX, locationY)
        {
            ShapeType = ShapeInfoType.CircleQuadrantRingShapeInfo;
            Thickness = thickness;
        }
        public override CircleQuadrantRingInfo GetDeepClone()
        {
            return (CircleQuadrantRingInfo)this.MemberwiseClone();
        }
        public static CircleQuadrantRingInfo ZeroQuadrantRing() => new(0,0);
        public CircleQuadrantInfo GetInnerRingShape()
        {
            var innerRingShape = GetOuterRingWholeShape().GetReducedPerimeterClone(Thickness, true);
            return innerRingShape;
        }
        IRingableShape IRingShape.GetInnerRingWholeShape()
        {
            throw new NotImplementedException();
        }

        public CircleQuadrantInfo GetOuterRingWholeShape()
        {
            return new CircleQuadrantInfo(Radius, QuadrantPart, this.LocationX, this.LocationY);
        }

        IRingableShape IRingShape.GetOuterRingWholeShape()
        {
            return GetOuterRingWholeShape();
        }

        static IEqualityComparer<CircleQuadrantRingInfo> IEqualityComparerCreator<CircleQuadrantRingInfo>.GetComparer()
        {
            return new CircleQuadrantRingInfoEqualityComparer();
        }
    }
    public class CircleQuadrantRingInfoEqualityComparer : IEqualityComparer<CircleQuadrantRingInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public CircleQuadrantRingInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly CircleQuadrantInfoEqualityComparer baseComparer;

        public bool Equals(CircleQuadrantRingInfo? x, CircleQuadrantRingInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Thickness == y!.Thickness;
        }

        public int GetHashCode([DisallowNull] CircleQuadrantRingInfo obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.Thickness);
        }
    }
}
