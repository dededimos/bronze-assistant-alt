using CommonHelpers.Comparers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using ShapesLibrary.Attributes;
using ShapesLibrary.Enums;
using ShapesLibrary.Exceptions;
using ShapesLibrary.Interfaces;
using ShapesLibrary.Services;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace ShapesLibrary.ShapeInfoModels
{
    /// <summary>
    /// Rectangle Information Origin Point : Center
    /// </summary>
    [ShapeOrigin("RectangleCenter")]
    public class RectangleInfo : ShapeInfo, IPolygonSimulatable, IRingableShape, IDeepClonable<RectangleInfo>, IEqualityComparerCreator<RectangleInfo>
    {
        public const int MINSIMULATIONSIDES = 8;
        public const int OPTIMALSIMULATIONSIDES = 36;
        public double Length { get; set; }
        public double Height { get; set; }
        public double TopLeftRadius { get; set; }
        public double TopRightRadius { get; set; }
        public double BottomLeftRadius { get; set; }
        public double BottomRightRadius { get; set; }
        public int MinSimulationSides => MINSIMULATIONSIDES;
        public int OptimalSimulationSides => OPTIMALSIMULATIONSIDES;
        public double LeftX { get => LocationX - Length / 2; }
        public double RightX { get => LocationX + Length / 2; }
        public double TopY { get => LocationY - Height / 2; }
        public double BottomY { get => LocationY + Height / 2; }
        public bool HasTotalNonZeroRadius { get => TopLeftRadius != 0 && TopLeftRadius == TopRightRadius && TopRightRadius == BottomLeftRadius && BottomLeftRadius == BottomRightRadius; }
        public bool HasDifferentRadiuses { get => !HasTotalNonZeroRadius && !HasZeroRadius; }
        /// <summary>
        /// Weather the Corner Radiuses are all Zero
        /// </summary>
        public bool HasZeroRadius { get => TopLeftRadius == 0 && TopRightRadius == 0 && BottomLeftRadius == 0 && BottomRightRadius == 0; }
        /// <summary>
        /// A Zero Corner Radius Rectangle is a Simple Rectangle
        /// </summary>
        public bool IsSimpleRectangle { get => HasZeroRadius; }
        public PointXY TopLeftVertex => GetTopLeftVertex();
        public PointXY TopRightVertex => GetTopRightVertex();
        public PointXY BottomLeftVertex => GetBottomLeftVertex();
        public PointXY BottomRightVertex => GetBottomRightVertex();
        /// <summary>
        /// Returns the four vertices of the Rectangle
        /// </summary>
        /// <returns></returns>
        public List<PointXY> GetVectices() => [TopLeftVertex, TopRightVertex, BottomRightVertex, BottomLeftVertex];



        public RectangleInfo(double length, double height, double radius = 0, double locationX = 0, double locationY = 0) : base(locationX, locationY)
        {
            ShapeType = ShapeInfoType.RectangleShapeInfo;
            Length = length;
            Height = height;
            TopLeftRadius = radius;
            TopRightRadius = radius;
            BottomLeftRadius = radius;
            BottomRightRadius = radius;
        }
        public RectangleInfo(double length, double height, double topLeftRadius, double topRightRadius, double bottomLeftRadius, double bottomRightRadius, double locationX = 0, double locationY = 0) : base(locationX, locationY)
        {
            ShapeType = ShapeInfoType.RectangleShapeInfo;
            Length = length;
            Height = height;
            TopLeftRadius = topLeftRadius;
            TopRightRadius = topRightRadius;
            BottomLeftRadius = bottomLeftRadius;
            BottomRightRadius = bottomRightRadius;
        }
        public RectangleInfo(double leftX, double topY, double rightX, double bottomY) : base((leftX + rightX) / 2d, (topY + bottomY) / 2d)
        {
            //Calculate Length
            Length = Math.Abs(rightX - leftX);
            Height = Math.Abs(bottomY - topY);
        }

        public override PointXY GetCentroid()
        {
            return GetLocation();
        }
        public void SetTopY(double topY)
        {
            LocationY = topY + Height / 2;
        }
        public void SetBottomY(double bottomY)
        {
            LocationY = bottomY - Height / 2;
        }
        public void SetLeftX(double leftX)
        {
            LocationX = leftX + Length / 2;
        }
        public void SetRightX(double rightX)
        {
            LocationX = rightX - Length / 2;
        }
        /// <summary>
        /// Sets all the radiuses of the Rectangle to the same value
        /// </summary>
        /// <param name="radius">The radius to set</param>
        public void SetCornerRadius(double radius)
        {
            TopLeftRadius = radius;
            TopRightRadius = radius;
            BottomLeftRadius = radius;
            BottomRightRadius = radius;
        }
        /// <summary>
        /// Returns the circle which is circumscribed in this Rectangle (touches all its vertices)
        /// </summary>
        /// <returns></returns>
        public CircleInfo GetCircumscribedCircle()
        {
            //First find the circles Radius , which is the diagonal line of the rectangle from its center to one of its edges
            var circleRadius = GetHalfDiagonalSize();
            return new(circleRadius, this.LocationX, this.LocationY);
        }
        private PointXY GetTopLeftVertex()
        {
            return new PointXY(LeftX, TopY);
        }
        private PointXY GetTopRightVertex()
        {
            return new PointXY(RightX, TopY);
        }
        private PointXY GetBottomLeftVertex()
        {
            return new PointXY(LeftX, BottomY);
        }
        private PointXY GetBottomRightVertex()
        {
            return new PointXY(RightX, BottomY);
        }

        public override RectangleInfo GetDeepClone()
        {
            return (RectangleInfo)this.MemberwiseClone();
        }
        public override double GetPerimeter()
        {
            return MathCalculations.Rectangle.GetPerimeter(Height, Length);
        }
        public override double GetArea()
        {
            return Length * Height;
        }
        public override RectangleInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            double totalShrink = perimeterShrink * 2;
            return new RectangleInfo(this.Length - totalShrink, this.Height - totalShrink, 0, this.LocationX, this.LocationY);
        }
        public override void Scale(double scaleFactor)
        {
            Height *= scaleFactor;
            Length *= scaleFactor;
            TopLeftRadius *= scaleFactor;
            TopRightRadius *= scaleFactor;
            BottomLeftRadius *= scaleFactor;
            BottomRightRadius *= scaleFactor;
        }
        public override void RotateClockwise()
        {
            double length = Length;
            double height = Height;
            Length = height;
            Height = length;
            //storeRadiuses
            var topLeft = TopLeftRadius;
            var topRight = TopRightRadius;
            var bottomRight = BottomRightRadius;
            var bottomLeft = BottomLeftRadius;
            TopRightRadius = topLeft;
            BottomRightRadius = topRight;
            BottomLeftRadius = bottomRight;
            TopLeftRadius = bottomLeft;
            //The Origin is the center , remains unchanged in Rotation
            RotationRadians += Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void RotateAntiClockwise()
        {
            double length = Length;
            double height = Height;
            Length = height;
            Height = length;
            //storeRadiuses
            var topLeft = TopLeftRadius;
            var topRight = TopRightRadius;
            var bottomRight = BottomRightRadius;
            var bottomLeft = BottomLeftRadius;
            TopLeftRadius = topRight;
            BottomLeftRadius = topLeft;
            BottomRightRadius = bottomLeft;
            TopRightRadius = bottomRight;

            RotationRadians -= Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void FlipHorizontally(double flipOriginX = double.NaN)
        {
            base.FlipHorizontally(flipOriginX);

            //Flip the Radiuses also
            double topLeft = TopLeftRadius;
            double topRight = TopRightRadius;
            double bottomLeft = BottomLeftRadius;
            double bottomRight = BottomRightRadius;

            TopLeftRadius = topRight;
            TopRightRadius = topLeft;
            BottomLeftRadius = bottomRight;
            BottomRightRadius = bottomLeft;
        }
        public override void FlipVertically(double flipOriginY = double.NaN)
        {
            base.FlipVertically(flipOriginY);

            //Flip the Radiuses also
            double topLeft = TopLeftRadius;
            double topRight = TopRightRadius;
            double bottomLeft = BottomLeftRadius;
            double bottomRight = BottomRightRadius;

            TopLeftRadius = bottomLeft;
            TopRightRadius = bottomRight;
            BottomLeftRadius = topLeft;
            BottomRightRadius = topRight;
        }

        /// <summary>
        /// Gets the Equivalent <see cref="RectangleRingInfo"/> with the given thickness
        /// </summary>
        /// <param name="ringThickness"></param>
        /// <returns></returns>
        public RectangleRingInfo GetEquivalentRingShape(double ringThickness)
        {
            return new RectangleRingInfo(this.Length, this.Height, ringThickness, this.TopLeftRadius, this.TopRightRadius, this.BottomLeftRadius, this.BottomRightRadius, this.LocationX, this.LocationY);
        }
        IRingShape IRingableShape.GetRingShape(double ringThickness)
        {
            return GetEquivalentRingShape(ringThickness);
        }

        /// <summary>
        /// Checks weather a Point is Contained in the Rectangle
        /// <para>Compares the Maximum and Minimum Xs,Ys with those of the Point</para>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool Contains(PointXY point)
        {
            if (IsSimpleRectangle)
            {
                return MathCalculations.Rectangle.IsPointInSimpleRectangle(this, point);
            }
            else return this.GetPolygonSimulation(OptimalSimulationSides).Contains(point);
        }
        public override bool ContainsSimpleRectangle(RectangleInfo rect)
        {
            if (IsSimpleRectangle)
            {
                return this.TopLeftRadius < rect.LeftX - DoubleSafeEqualityComparer.DefaultEpsilon &&
                    this.TopRightRadius > rect.RightX + DoubleSafeEqualityComparer.DefaultEpsilon &&
                    this.TopY < rect.TopY - DoubleSafeEqualityComparer.DefaultEpsilon &&
                    this.BottomY > rect.BottomY + DoubleSafeEqualityComparer.DefaultEpsilon;
            }
            else
            {
                var polygon = this.GetPolygonSimulation(OptimalSimulationSides);
                return polygon.ContainsSimpleRectangle(rect);
            }
        }
        public override bool Contains(ShapeInfo shape)
        {
            return shape switch
            {
                CircleInfo circle => GetPolygonSimulation(OptimalSimulationSides).Contains(circle),
                RectangleInfo rect => rect.IsSimpleRectangle ? ContainsSimpleRectangle(rect) : GetPolygonSimulation(OptimalSimulationSides).Contains(rect.GetPolygonSimulation(rect.OptimalSimulationSides)),
                PolygonInfo polygon => GetPolygonSimulation(OptimalSimulationSides).Contains(polygon),
                IPolygonSimulatable simulatable => GetPolygonSimulation(OptimalSimulationSides).Contains(simulatable.GetOptimalPolygonSimulation()),
                _ => throw new NotSupportedContainmentException(this, shape),
            };
        }


        /// <summary>
        /// Checks weather a Point is Inside or on the Perimeter of the Rectangle
        /// <para>Compares the Points Coordinates with the Vertices Coordinates of the Rectangle</para>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool IntersectsWithPoint(PointXY point)
        {
            if (this.HasZeroRadius)
            {
                return this.LeftX <= point.X + DoubleSafeEqualityComparer.DefaultEpsilon &&
                   this.RightX >= point.X - DoubleSafeEqualityComparer.DefaultEpsilon &&
                   this.TopY <= point.Y + DoubleSafeEqualityComparer.DefaultEpsilon &&
                   this.BottomY >= point.Y - DoubleSafeEqualityComparer.DefaultEpsilon;
            }
            else
            {
                return this.GetPolygonSimulation(OptimalSimulationSides).IntersectsWithPoint(point);
            }
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
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            IReadOnlyList<PointXY> vertices;

            if (IsSimpleRectangle)
            {
                vertices = GetVectices();
            }
            else
            {
                vertices = GetPolygonSimulation(OptimalSimulationSides).Vertices;
            }

            //Project each vertex to the Given Direction (get only the scalar without finding the actual points)
            foreach (var vertex in vertices)
            {
                double projection = direction.Normalize().Dot(vertex);
                min = Math.Min(min, projection);
                max = Math.Max(max, projection);
            }

            return new ProjectionInfo(min, max);
        }

        /// <summary>
        /// Returns the same rectangle as the inscribed polygon
        /// </summary>
        /// <param name="sides">Sides is irrelevant here the perfect polygon is the rectangle itself</param>
        /// <returns></returns>
        public PolygonInfo GetPolygonSimulation(int sides)
        {
            if (sides < MinSimulationSides) throw new SimulationSidesOutOfRangeException(this);
            if (this.HasZeroRadius || sides == 4)
            {
                return new PolygonInfo([new(LeftX, TopY), new(RightX, TopY), new(RightX, BottomY), new(LeftX, BottomY)]);
            }

            int edgesWithRadius = 0;
            //Calculate how many edges have actually a radius
            if (HasTotalNonZeroRadius) edgesWithRadius = 4;
            else
            {
                if (TopLeftRadius != 0) edgesWithRadius++;
                if (TopRightRadius != 0) edgesWithRadius++;
                if (BottomLeftRadius != 0) edgesWithRadius++;
                if (BottomRightRadius != 0) edgesWithRadius++;
            }

            //If the sides with radius are not all 4 then we should keep 1 point for each side that does not have radius to be presented by a single point
            var remainingSides = sides - (4 - edgesWithRadius);

            //Calculate how many points per radius we can assign From the RemainingSides
            //Calculate the Reminder of the Division by the Edges that have radius so to equally distribute points to them
            //If there is a Reminder artificially increase the points per Side until they can be divided by the number of sides to return an integer
            int reminder = remainingSides % edgesWithRadius;
            int pointsPerSide = (remainingSides + reminder) / edgesWithRadius;

            List<PointXY> vertices = [];

            //Find the Points of the Polygon
            if (TopLeftRadius != 0)
            {
                var circleTopLeft = new CircleInfo(TopLeftRadius, LeftX + TopLeftRadius, TopY + TopLeftRadius);
                vertices.AddRange(MathCalculations.Circle.GetCircleArcPoints(circleTopLeft, pointsPerSide, -Math.PI, -Math.PI / 2d));
            }
            else vertices.Add(TopLeftVertex);

            if (TopRightRadius != 0)
            {
                var circleTopRight = new CircleInfo(TopRightRadius, RightX - TopRightRadius, TopY + TopRightRadius);
                vertices.AddRange(MathCalculations.Circle.GetCircleArcPoints(circleTopRight, pointsPerSide, -Math.PI / 2, 0));
            }
            else vertices.Add(TopRightVertex);

            if (BottomRightRadius != 0)
            {
                var circleBottomRight = new CircleInfo(BottomRightRadius, RightX - BottomRightRadius, BottomY - BottomRightRadius);
                vertices.AddRange(MathCalculations.Circle.GetCircleArcPoints(circleBottomRight, pointsPerSide, 0, Math.PI / 2d));
            }
            else vertices.Add(BottomRightVertex);

            if (BottomLeftRadius != 0)
            {
                var circleBottomLeft = new CircleInfo(BottomLeftRadius, LeftX + BottomLeftRadius, BottomY - BottomLeftRadius);
                vertices.AddRange(MathCalculations.Circle.GetCircleArcPoints(circleBottomLeft, pointsPerSide, Math.PI / 2d, Math.PI));
            }
            else vertices.Add(BottomLeftVertex);

            return new PolygonInfo(vertices);
        }



        public override RectangleInfo GetBoundingBox()
        {
            return new RectangleInfo(this.Length, this.Height, 0, this.LocationX, this.LocationY);
        }
        /// <summary>
        /// Returns the half size of the Diagonal Line of the Rectangle
        /// </summary>
        /// <returns></returns>
        public double GetHalfDiagonalSize()
        {
            return Math.Sqrt(Length * Length + Height * Height) / 2d;
        }
        public override double GetTotalLength()
        {
            return Length;
        }
        public override double GetTotalHeight()
        {
            return Height;
        }
        public override void SetTotalLength(double length)
        {
            Length = length;
        }
        public override void SetTotalHeight(double height)
        {
            Height = height;
        }

        /// <summary>
        /// Creates a rectangle with zero Length and Height at 0,0
        /// </summary>
        /// <returns></returns>
        public static RectangleInfo ZeroRectangle() => new(0, 0);

        /// <summary>
        /// Retrieves the Comparer for <see cref="RectangleInfo"/>
        /// </summary>
        /// <returns></returns>
        static IEqualityComparer<RectangleInfo> IEqualityComparerCreator<RectangleInfo>.GetComparer()
        {
            return new RectangleInfoEqualityComparer();
        }
    }
    public class RectangleInfoEqualityComparer : IEqualityComparer<RectangleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public RectangleInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly ShapeInfoBaseEqualityComparer baseComparer;

        public bool Equals(RectangleInfo? x, RectangleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Length == y!.Length &&
                x.Height == y.Height &&
                x.TopLeftRadius == y.TopLeftRadius &&
                x.TopRightRadius == y.TopRightRadius &&
                x.BottomLeftRadius == y.BottomLeftRadius &&
                x.BottomRightRadius == y.BottomRightRadius;
        }

        public int GetHashCode([DisallowNull] RectangleInfo obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            int hash = baseComparer.GetHashCode(obj);
            hash = HashCode.Combine(hash, obj.Length, obj.Height, obj.TopLeftRadius, obj.TopRightRadius, obj.BottomLeftRadius, obj.BottomRightRadius);
            return hash;
        }
    }
    public class RectangleRingInfo : RectangleInfo, IDeepClonable<RectangleRingInfo>, IRingShape, IEqualityComparerCreator<RectangleRingInfo>
    {
        public double Thickness { get; set; }
        public double InnerLeftX { get => LeftX + Thickness; }
        public double InnerTopY { get => TopY + Thickness; }
        public double InnerRightX { get => RightX - Thickness; }
        public double InnerBottomY { get => BottomY - Thickness; }
        public double TopLeftInnerRadius { get => CalculateInnerRadius(TopLeftRadius); }
        public double BottomLeftInnerRadius { get => CalculateInnerRadius(BottomLeftRadius); }
        public double TopRightInnerRadius { get => CalculateInnerRadius(TopRightRadius); }
        public double BottomRightInnerRadius { get => CalculateInnerRadius(BottomRightRadius); }

        /// <summary>
        /// Calculates the radius of a Corner of the Inner Rectangle
        /// <para>Returns zero if the Thickness is bigger or Equal to The Outer Radius</para>
        /// </summary>
        /// <param name="outerRadius"></param>
        /// <returns></returns>
        private double CalculateInnerRadius(double outerRadius)
        {
            return CalculateInnerRadius(outerRadius, Thickness);
        }
        /// <summary>
        /// Calculates the radius of a Corner of the Inner Rectangle
        /// <para>Returns zero if the Thickness is bigger or Equal to The Outer Radius</para>
        /// </summary>
        /// <param name="outerRadius">The radius of the corner of the outer Rectangle</param>
        /// <param name="thickness">The thickness of the Rectangle Ring , Distance between the inner and outer Rectangles</param>
        /// <returns></returns>
        public static double CalculateInnerRadius(double outerRadius, double thickness)
        {
            double diff = outerRadius - thickness;
            return diff > 0 ? diff : 0;
        }

        public RectangleRingInfo(double length, double height, double thickness, double radius = 0, double locationX = 0, double locationY = 0) : base(length, height, radius, locationX, locationY)
        {
            ShapeType = ShapeInfoType.RectangleRingShapeInfo;
            Thickness = thickness;
        }
        public RectangleRingInfo(double length, double height, double thickness, double topLeftRadius, double topRightRadius, double bottomLeftRadius, double bottomRightRadius, double locationX = 0, double locationY = 0) :
            base(length, height, topLeftRadius, topRightRadius, bottomLeftRadius, bottomRightRadius, locationX, locationY)
        {
            ShapeType = ShapeInfoType.RectangleRingShapeInfo;
            Thickness = thickness;
        }
        public override RectangleRingInfo GetDeepClone()
        {
            return (RectangleRingInfo)this.MemberwiseClone();
        }
        public static RectangleRingInfo RectangleRingZero() => new(0, 0, 0);

        public RectangleInfo GetInnerRingWholeShape()
        {
            double totShrink = Thickness * 2;
            return new RectangleInfo(this.Length - totShrink, this.Height - totShrink, TopLeftInnerRadius, TopRightInnerRadius, BottomLeftInnerRadius, BottomRightInnerRadius, this.LocationX, this.LocationY);
        }
        public override void Scale(double scaleFactor)
        {
            base.Scale(scaleFactor);
            Thickness *= scaleFactor;
        }
        public RectangleInfo GetOuterRingWholeShape()
        {
            return new RectangleInfo(this.Length, this.Height, 0, this.LocationX, this.LocationY);
        }
        IRingableShape IRingShape.GetInnerRingWholeShape()
        {
            return GetInnerRingWholeShape();
        }
        IRingableShape IRingShape.GetOuterRingWholeShape()
        {
            return GetOuterRingWholeShape();
        }

        public override bool Contains(PointXY point)
        {
            var innerRect = GetInnerRingWholeShape();
            var outerContains = base.Contains(point);

            //Returns true if the outer contains it but the inner does not
            //The inner is always a simple rectangle.
            return outerContains && !innerRect.Contains(point);

        }
        public override bool ContainsSimpleRectangle(RectangleInfo rect)
        {
            var outerContains = base.ContainsSimpleRectangle(rect);
            var innerShape = GetInnerRingWholeShape();

            //Returns true if the outer contains it but the inner does not
            //The inner is always a simple rectangle.
            return outerContains && !innerShape.ContainsSimpleRectangle(rect);
        }
        public override bool Contains(ShapeInfo shape)
        {
            var outerContains = base.Contains(shape);
            var innerRect = GetInnerRingWholeShape();
            return outerContains && !innerRect.Contains(shape);
        }

        public override bool IntersectsWithPoint(PointXY point)
        {
            var innerRect = GetInnerRingWholeShape();
            var outterRect = GetOuterRingWholeShape();
            //outside must intersect , inside must not contain ! (can intersect no problem)
            return outterRect.IntersectsWithPoint(point) && !innerRect.Contains(point);
        }
        public override bool IntersectsWithShape(ShapeInfo shape)
        {
            var innerRect = GetInnerRingWholeShape();
            var outterRect = GetOuterRingWholeShape();
            //outside must intersect , inside must not contain ! (can intersect edge but not be wholly inside)
            return outterRect.IntersectsWithShape(shape) && !innerRect.Contains(shape);
        }


        static IEqualityComparer<RectangleRingInfo> IEqualityComparerCreator<RectangleRingInfo>.GetComparer()
        {
            return new RectangleRingInfoEqualityComparer();
        }
    }
    public class RectangleRingInfoEqualityComparer : IEqualityComparer<RectangleRingInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public RectangleRingInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly RectangleInfoEqualityComparer baseComparer;

        public bool Equals(RectangleRingInfo? x, RectangleRingInfo? y)
        {
            //base Comparer
            return baseComparer.Equals(x, y) &&
                x!.Thickness == y!.Thickness;
        }

        public int GetHashCode([DisallowNull] RectangleRingInfo obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.Thickness);
        }
    }
}
