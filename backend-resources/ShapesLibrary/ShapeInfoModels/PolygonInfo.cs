using CommonHelpers.Comparers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using ShapesLibrary.Attributes;
using ShapesLibrary.Exceptions;
using ShapesLibrary.Interfaces;
using ShapesLibrary.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Formats.Asn1.AsnWriter;

namespace ShapesLibrary.ShapeInfoModels
{
    [ShapeOrigin("PolygonCentroid")]
    public class PolygonInfo : ShapeInfo, IDeepClonable<PolygonInfo>, IEqualityComparerCreator<PolygonInfo>
    {
        public PolygonInfo(IList<PointXY> vertices) : base(0, 0)
        {
            if (vertices.Count <= 2) throw new ArgumentException("A Polygon must have at least 3 Vertices", nameof(vertices));
            ShapeType = Enums.ShapeInfoType.PolygonShapeInfo;
            //Find the centroid of the provided vertices and set the correct location on the backing fields (so to not trigger the SetLocation Methods)
            var centroid = MathCalculations.Polygons.GetCentroid(vertices);
            locationX = centroid.X;
            locationY = centroid.Y;
            this.vertices = new(vertices);
            edges = new Lazy<List<PositionVector>>(() => MathCalculations.Polygons.GetPolygonEdges(this));
            edgesNormalAxesNormalized = new Lazy<List<Vector2D>>(() => MathCalculations.Polygons.GetPolygonEdgesNormalAxesNormalized(this));
        }

        protected List<PointXY> vertices = [];
        public IReadOnlyList<PointXY> Vertices { get => vertices; }

        private Lazy<List<PositionVector>> edges;
        private Lazy<List<Vector2D>> edgesNormalAxesNormalized;
        /// <summary>
        /// Resets the Lazy property so that the edges are recalculated BUT ONLY WHEN AND IF NEEDED
        /// </summary>
        private void ReInitializeEdgesAndAxes()
        {
            edges = new Lazy<List<PositionVector>>(() => MathCalculations.Polygons.GetPolygonEdges(this));
            edgesNormalAxesNormalized = new Lazy<List<Vector2D>>(() => MathCalculations.Polygons.GetPolygonEdgesNormalAxesNormalized(this));
        }
        
        public override PointXY GetCentroid()
        {
            return GetLocation();
        }
        /// <summary>
        /// The Edges of the Polygon 
        /// </summary>
        public IReadOnlyList<PositionVector> Edges => edges.Value;
        /// <summary>
        /// The Axes on which lie the Edges of the Polygon
        /// </summary>
        public IReadOnlyList<Vector2D> EdgesNormalAxesNormalized => edgesNormalAxesNormalized.Value;


        private double locationX;
        public override double LocationX { get => locationX; set => SetLocationX(value); }
        private void SetLocationX(double value)
        {
            if (this.locationX != value)
            {
                var dx = value - this.locationX;
                this.locationX = value;
                vertices = vertices.Select(v => new PointXY(v.X + dx, v.Y)).ToList();
                ReInitializeEdgesAndAxes();
            }
        }

        private double locationY;
        public override double LocationY { get => locationY; set => SetLocationY(value); }
        private void SetLocationY(double value)
        {
            if (this.locationY != value)
            {
                var dy = value - this.locationY;
                this.locationY = value;
                vertices = vertices.Select(v => new PointXY(v.X, v.Y + dy)).ToList();
                ReInitializeEdgesAndAxes();
            }
        }

        public int NumberOfSides { get => vertices.Count; }

        /// <summary>
        /// Returns the Order of the Polygon's Vertices
        /// <para>1 for Counterclockwise</para>
        /// <para>-1 for Clockwise</para>
        /// <para>0 for degenerate (all points Collinear)</para>
        /// </summary>
        /// <returns></returns>
        public int GetPolygonWinding()
        {
            double sum = 0;
            int count = Vertices.Count;

            for (int i = 0; i < count; i++)
            {
                PointXY a = Vertices[i];
                PointXY b = Vertices[(i + 1) % count];
                sum += (a.X * b.Y) - (b.X * a.Y);
            }

            if (sum > DoubleSafeEqualityComparer.DefaultEpsilon)
                return 1; // Counterclockwise
            else if (sum < -DoubleSafeEqualityComparer.DefaultEpsilon)
                return -1; // Clockwise
            else
                return 0; // Undefined or degenerate
        }
        /// <summary>
        /// Returns the maximum distance from the centroid to any vertex.
        /// </summary>
        public double GetMaxVertexDistance()
        {
            PointXY centroid = GetCentroid();
            double maxDistance = 0.0;

            foreach (var vertex in Vertices)
            {
                Vector2D vector = new Vector2D(centroid, vertex);
                double distance = vector.Magnitude();
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
            }

            return maxDistance;
        }
        /// <summary>
        /// Finds the closest point on the polygon's boundary to the given point.
        /// </summary>
        /// <param name="point">The point to find the closest point to.</param>
        /// <returns>The closest PointXY on the polygon's boundary.</returns>
        public PointXY GetClosestPoint(PointXY point)
        {
            double minDistanceSquared = double.MaxValue;
            PointXY closestPoint = Vertices[0];

            for (int i = 0; i < Vertices.Count; i++)
            {
                PointXY a = Vertices[i];
                PointXY b = Vertices[(i + 1) % Vertices.Count];

                // Find the closest point on the edge AB to the point
                PointXY closest = MathCalculations.Points.GetClosestPointOnSegmentFromPoint(point, a, b);

                // Calculate squared distance to avoid unnecessary square roots
                double distanceSquared = (closest.X - point.X) * (closest.X - point.X) + (closest.Y - point.Y) * (closest.Y - point.Y);

                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    closestPoint = closest;
                }
            }

            return closestPoint;
        }

        /// <summary>
        /// Calculates if a Point lies inside the Polygon
        /// <para>Uses the Winding Algorith to Detemine through the Cross Products</para>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool Contains(PointXY point)
        {
            return MathCalculations.Polygons.IsPointInPolygonWinding(this,point);
        }
        public override bool ContainsSimpleRectangle(RectangleInfo rect)
        {
            return Contains(rect.TopLeftVertex) &&
                Contains(rect.TopRightVertex) &&
                Contains(rect.BottomLeftVertex) &&
                Contains(rect.BottomRightVertex);
        }
        /// <summary>
        /// Checks weather this Polygon already contains another Shape
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedContainmentException"></exception>
        public override bool Contains(ShapeInfo shape)
        {
            return shape switch
            {
                CircleInfo circle => ContainsCircle(circle),
                RectangleInfo rect => rect.IsSimpleRectangle ? ContainsSimpleRectangle(rect) : ContainsPolygon(rect.GetPolygonSimulation(rect.OptimalSimulationSides)),
                CapsuleInfo capsule => ContainsCapsule(capsule),
                PolygonInfo polygon => ContainsPolygon(polygon),
                //last one because circle - Capsule are already simulatable
                IPolygonSimulatable simulation => ContainsPolygon(simulation.GetOptimalPolygonSimulation()),
                _ => throw new NotSupportedContainmentException(this, shape),
            };
        }

        /// <summary>
        /// Calculates if a Circle lies inside the Polygon
        /// <para>1.Checks if the Circle Center is inside the Polygon</para>
        /// <para>2.If the distance from each edge is more than the Radius of the Circle</para>
        /// </summary>
        /// <param name="circle"></param>
        /// <returns></returns>
        private bool ContainsCircle(CircleInfo circle)
        {
            var circleCenter = circle.GetLocation();
            // Check if the Circle Center is inside the Polygon
            if (!Contains(circleCenter)) return false;

            // Check if the Squared distance from each edge is more than the Radius of the Circle
            double circleRadiusSquared = Math.Pow(circle.Radius, 2);

            foreach (PositionVector edge in this.Edges)
            {
                double squaredDistance = MathCalculations.Vectors.GetSquaredDistanceOfPointFromVector(edge, circleCenter);
                if (squaredDistance <= circleRadiusSquared + DoubleSafeEqualityComparer.DefaultEpsilon)
                {
                    return false;
                }
            }

            return true;
        }
        private bool ContainsCapsule(CapsuleInfo capsule)
        {
            return ContainsCircle(capsule.GetCapsuleCircle(true)) &&
                ContainsCircle(capsule.GetCapsuleCircle(false)) &&
                ContainsPolygon(capsule.GetMiddleRectangle().GetPolygonSimulation(4));
        }
        private bool ContainsPolygon(PolygonInfo polygon)
        {
            // Check if all the vertices of the provided polygon are contained within this polygon
            foreach (var vertex in polygon.Vertices)
            {
                if (!Contains(vertex))
                {
                    return false;
                }
            }

            return true;
        }


        public override RectangleInfo GetBoundingBox()
        {
            var leftX = vertices.Min(v => v.X);
            var rightX = vertices.Max(v => v.X);
            var topY = vertices.Max(v => v.Y);
            var bottomY = vertices.Min(v => v.Y);
            return new RectangleInfo(leftX, topY, rightX, bottomY);
        }
        public override double GetPerimeter()
        {
            double perimeter = 0;
            var vertexCount = vertices.Count;

            for (int i = 0; i < vertexCount; i++)
            {
                PointXY currentVertex = vertices[i];
                //ensures that when at the last iteration the next vertex will be vertices[0] as the division will return a modulo "0"
                //whereas in all other occasions vertexCound is bigger than i+1 and the modulo is (i+1).
                PointXY nextVertex = vertices[(i + 1) % vertexCount];
                perimeter += MathCalculations.Points.GetDistanceBetweenPoints(currentVertex, nextVertex);
            }
            return perimeter;
        }
        public override double GetArea()
        {
            //shoelace formula !
            double area = 0;
            int count = Vertices.Count;

            for (int i = 0; i < count; i++)
            {
                PointXY a = Vertices[i];
                PointXY b = Vertices[(i + 1) % count];
                area += (a.X * b.Y) - (b.X * a.Y);
            }

            return Math.Abs(area) / 2.0;
        }
        public override ShapeInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            throw new NotSupportedException($"{nameof(PolygonInfo)} does not support {nameof(GetReducedPerimeterClone)}");
        }
        public override ProjectionInfo GetProjectionOntoAxis(Vector2D axis)
        {
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            foreach (var vertex in Vertices)
            {
                double projection = axis.Dot(vertex);
                if (projection < min)
                    min = projection;
                if (projection > max)
                    max = projection;
            }
            return new ProjectionInfo(min,max);
        }
        /// <summary>
        /// Returns the Closest Normalized Axis that passes from and Edge and the Point 
        /// <para>This is the direction that will produce the closest distance from the point to the Polygon</para>
        /// <para>If the projection of the point to the edge is within the edge , then the axis is also perpendicular to the edge</para>
        /// <para>Method Helps in SAT when needing to find the Closest Axis to a circle</para>
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2D GetClosestNormalizedAxisFromPointToPolygon(PointXY point)
        {
            double minDistanceSquared = double.PositiveInfinity;
            Vector2D closestNormalizedNormalAxis = Vector2D.Zero;
            foreach (var edge in Edges) 
            { 
                //compute the SQUARED distance from the Point to the Edge
                var distanceSquared = MathCalculations.Points.GetSquaredDistanceBetweenPointAndSegment(point, edge.Start, edge.End);

                //if the distance is smaller than the ones found before then update the closest axis
                if (distanceSquared < minDistanceSquared)
                {
                    minDistanceSquared = distanceSquared;
                    //Set also the Closest Normal axis (normalized)
                    closestNormalizedNormalAxis = edge.Vector.GetNormal().Normalize();
                }
            }
            //Return the perpendicular Normalized axis to an edge that is closest to the point
            return closestNormalizedNormalAxis;
        }

        public override void RotateAntiClockwise()
        {
            vertices = vertices.Select(v => MathCalculations.Points.RotatePointAroundOrigin(v, this.GetLocation(), -Math.PI / 2d)).ToList();
            ReInitializeEdgesAndAxes();
            RotationRadians -= Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void RotateClockwise()
        {
            vertices = vertices.Select(v => MathCalculations.Points.RotatePointAroundOrigin(v, this.GetLocation(), Math.PI / 2d)).ToList();
            ReInitializeEdgesAndAxes();
            RotationRadians += Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void Scale(double scaleFactor)
        {
            //Calculate the centroid of the polygon.
            var centroid = this.GetLocation();
            //Translate the vertices so that the centroid is at (0,0):
            //v′= (v.X−xc,v.Y−yc)
            //v′= (v.X−xc​,v.Y−yc​)
            List<PointXY> translatedVertices = vertices.Select(v => new PointXY(v.X - centroid.X, v.Y - centroid.Y)).ToList();

            //Scale the translated vertices:
            //v′′= (v′.X∗scaleFactor,v′.Y∗scaleFactor)
            //v′′= (v′.X∗scaleFactor,v′.Y∗scaleFactor)
            translatedVertices = translatedVertices.Select(v => new PointXY(v.X * scaleFactor, v.Y * scaleFactor)).ToList();

            //Translate the scaled vertices back to their original centroid:
            //vfinal = (v′′.X + xc,v′′.Y + yc)
            //vfinal​ = (v′′.X + xc​,v′′.Y + yc​)
            vertices = translatedVertices.Select(v => new PointXY(v.X + centroid.X, v.Y + centroid.Y)).ToList();
        }

        public override double GetTotalHeight()
        {
            return GetBoundingBox().Height;
        }
        public override double GetTotalLength()
        {
            return GetBoundingBox().Length;
        }
        public override void SetTotalHeight(double height)
        {
            throw new NotSupportedException($"Non special cases of {nameof(PolygonInfo)} do not support {nameof(SetTotalHeight)} Functions");
            //var bBox = GetBoundingBox();
            //if (bBox.Height == 0)
            //{
            //    //create a regular Polygon
            //    var reg = CreateRegularPolygon(height, vertices.Count, locationX, locationY);
            //    vertices = [.. reg.Vertices];
            //}
            //else
            //{
            //    var scale = height / bBox.Height;
            //    Scale(scale);
            //}
        }
        public override void SetTotalLength(double length)
        {
            throw new NotSupportedException($"Non special cases of {nameof(PolygonInfo)} do not support {nameof(SetTotalLength)} Functions");
            //var bBox = GetBoundingBox();
            //if (bBox.Length == 0)
            //{
            //    //create a regular Polygon
            //    var reg = CreateRegularPolygon(length, vertices.Count, locationX, locationY);
            //    vertices = [.. reg.Vertices];
            //}
            //else
            //{
            //    var scale = length / bBox.Length;
            //    Scale(scale);
            //}
        }
        public override PolygonInfo GetDeepClone()
        {
            var clone = (PolygonInfo)this.MemberwiseClone();
            clone.vertices = new(vertices);
            return clone;
        }

        static IEqualityComparer<PolygonInfo> IEqualityComparerCreator<PolygonInfo>.GetComparer()
        {
            return new PolygonInfoEqualityComparer();
        }
        /// <summary>
        /// Creates a regular Polygon with the specified total Length/Height and number of sides
        /// </summary>
        /// <param name="lengthHeight">The Length/Height of the Polygon</param>
        /// <param name="numberOfSides">The Number of Sides of the Polygon</param>
        /// <param name="locationX">The CenterX of the Polygon</param>
        /// <param name="locationY">The CenterY of the Polygon</param>
        /// <returns>The Polygon</returns>

        public static PolygonInfo ZeroPolygon() => new([new(0, 0), new(0, 0), new(0, 0)]);

        public override bool IntersectsWithPoint(PointXY point)
        {
            return MathCalculations.Polygons.IsPointIntersectingPolygonWinding(this, point);
        }
        public override bool IntersectsWithShape(ShapeInfo shape)
        {
            return shape switch
            {
                RectangleRingInfo rectRing => rectRing.IntersectsWithShape(this),
                CircleRingInfo circleRing => circleRing.IntersectsWithShape(this),
                CircleInfo circle => IntersectsWithCircle(circle),
                PolygonInfo polygon => IntersectsWithPolygon(polygon),
                IPolygonSimulatable simulatable => IntersectsWithPolygon(simulatable.GetOptimalPolygonSimulation()),
                _ => throw new NotSupportedIntersectionException(this, shape),
            };
        }
        public bool IntersectsWithPolygon(PolygonInfo otherPolygon)
        {
            //Using the SAT (Separating Axis Theorem) to check if the Polygons intersect
            //The SAT states that if there is a line that can separate the two polygons then they do not intersect
            //So we project each of the Polygons on Axis that are perpendicular to the edges of the Polygons
            //Projecting a Polygon on an Axis means finding the points where the perpendicular of each vertex intersects the Axis
            //These Points will then form a line (one for each polygon)
            //We check weather those lines overlap or not

            //As soon as ONE of those axis indicates no overlap in the Projection's points then the Polygons do not intersect
            //If ALL the Axis indicate overlap then the Polygons intersect
            foreach (var edge in Edges)
            {
                var axis = edge.Vector.GetNormal(); //Gets the perpendicular Vector direction of the edge
                if (AreProjectionsOverlappingOnAxis(otherPolygon, axis) is false)
                {
                    return false;  //exits and returns there is no intersection (only one axis needs to be found that is not overlapping) 
                }
            }

            foreach (var edge in otherPolygon.Edges)
            {
                var axis = edge.Vector.GetNormal(); //Gets the perpendicular Vector direction of the edge
                if (AreProjectionsOverlappingOnAxis(otherPolygon, axis) is false)
                {
                    return false;  //exits and returns there is no intersection (only one axis needs to be found that is not overlapping) 
                }
            }

            //If only overlaps are found then the Polygons intersect
            return true;
        }
        public bool IntersectsWithCircle(CircleInfo cirlce)
        {
            //Create the axis of SAT theorem
            List<Vector2D> axes = [];
            axes.AddRange(EdgesNormalAxesNormalized);
            
            //Get the Closest Normal Axis to the Circle's Center
            var closestAxis = GetClosestNormalizedAxisFromPointToPolygon(cirlce.GetCentroid());
            axes.Add(closestAxis);

            foreach (var axis in axes)
            {
                var projectionPolygon = GetProjectionOntoAxis(axis);
                var projectionCircle = cirlce.GetProjectionOntoAxis(axis);

                if (projectionPolygon.GetOverlap(projectionCircle) <= 0)
                {
                    //At least one found axis that does not overlap , means no intersection
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks weather the Projections of the Polygons on the provided Axis overlap
        /// </summary>
        /// <param name="otherPolygon"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private bool AreProjectionsOverlappingOnAxis(PolygonInfo otherPolygon, Vector2D axis)
        {
            //Project Polygon 1
            //Initilize the min Max Values to be able to use them in the comparisons
            var min1 = double.MaxValue;
            var max1 = double.MinValue;

            foreach (var vertex in this.Vertices)
            {
                var projection = axis.Dot(vertex);
                min1 = Math.Min(min1, projection);//replaces the initilized value and then sets the min projection
                max1 = Math.Max(max1, projection);//replaces the initilized value and then sets the max projection
            }

            var min2 = double.MaxValue;
            var max2 = double.MinValue;

            foreach (var vertex in otherPolygon.Vertices)
            {
                //The Dot Product gives us an indication of the projection of the vertex on the axis (instead of getting an actual point we need only to get this one)
                //The Projection gets computed by the Dot Product , instead of computing the whole projection we only find the DOT
                var projectionScalar = axis.Dot(vertex);
                min2 = Math.Min(min2, projectionScalar);//replaces the initilized value and then sets the min projection
                max2 = Math.Max(max2, projectionScalar);//replaces the initilized value and then sets the max projection
            }

            //check for overlap 
            //if the Max Point of the First is Smaller than the Min Point of the Second then there is no overlap , and vice versa
            //Account for floating point errors with Epsilon
            return !(max1 < (min2 + DoubleSafeEqualityComparer.DefaultEpsilon) || max2 < (min1 + DoubleSafeEqualityComparer.DefaultEpsilon));
        }
    }
    public class PolygonInfoEqualityComparer : IEqualityComparer<PolygonInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public PolygonInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly ShapeInfoBaseEqualityComparer baseComparer;

        public bool Equals(PolygonInfo? x, PolygonInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Vertices.SequenceEqual(y!.Vertices);
        }

        public int GetHashCode([DisallowNull] PolygonInfo obj)
        {
            int hash = baseComparer.GetHashCode(obj);
            foreach (var vertex in obj.Vertices)
            {
                unchecked
                {
                    hash = HashCode.Combine(hash, vertex);
                }
            }
            return hash;
        }
    }

    public class RegularPolygonInfo : PolygonInfo, IDeepClonable<RegularPolygonInfo> , IEqualityComparerCreator<RegularPolygonInfo>
    {
        /// <summary>
        /// Creates a Regular Polygon with one of its sides parallel to X Axis , unless a rotation is given
        /// </summary>
        /// <param name="circumscribedCircle">The Circle in which the polygon is inscribed to</param>
        /// <param name="numberOfSides">The Number of sides of the Polygon</param>
        /// <param name="rotation">The rotation of the polygon</param>
        public RegularPolygonInfo(CircleInfo circumscribedCircle, int numberOfSides, double rotation = 0) : base([new(0, 0), new(0, 0), new(0, 0)])
        {
            ShapeType = Enums.ShapeInfoType.RegularPolygonShapeInfo;
            vertices = MathCalculations.Circle.GetInscribedPolygonVertices(circumscribedCircle, numberOfSides, rotation);
        }
        /// <summary>
        /// Creates a Regular Polygon with one of its sides parallel to X Axis , unless a rotation is given
        /// </summary>
        /// <param name="radius">The Radius of the Circumscribed Circle</param>
        /// <param name="numberOfSides">The Number of sides of the Polygon</param>
        /// <param name="rotation">The rotation of the polygon</param>
        /// <param name="locationX">The location X of the Polygon</param>
        /// <param name="locationY">The location Y of the Polygon</param>
        public RegularPolygonInfo(double radius, int numberOfSides, double rotation = 0, double locationX = 0, double locationY = 0) : this(new CircleInfo(radius, locationX, locationY), numberOfSides, rotation) { }

        private double rotationRadians;
        public override double RotationRadians { get => MathCalculations.VariousMath.NormalizeAngle(rotationRadians); }
        public double CircumscribedRadius { get => GetCircumscribedCircle().Radius; }

        public override void RotateClockwise()
        {
            base.RotateClockwise();
            rotationRadians += Math.PI / 2d;
        }
        public override void RotateAntiClockwise()
        {
            base.RotateAntiClockwise();
            rotationRadians -= Math.PI / 2d;
        }

        public override RegularPolygonInfo GetDeepClone()
        {
            return (RegularPolygonInfo)base.GetDeepClone();
        }
        public CircleInfo GetCircumscribedCircle()
        {
            var edgeSize = MathCalculations.Points.GetDistanceBetweenPoints(vertices[0], vertices[1]);
            var radius = MathCalculations.Polygons.GetCircumscribedRadiusFromEdgeSize(edgeSize, vertices.Count);
            return new CircleInfo(radius, LocationX, LocationY);
        }

        public override void SetTotalHeight(double height)
        {
            var initialTotalHeight = GetTotalHeight();
            //handle zero case
            //If the height is zero then change the vertices to match e polygon with a random Radius and the same number of sides and Centroid / rotation
            if (initialTotalHeight == 0 || double.IsNaN(initialTotalHeight))
            {
                var randomCircumscribedCircle = new CircleInfo(100, LocationX, LocationY);
                vertices = MathCalculations.Circle.GetInscribedPolygonVertices(randomCircumscribedCircle, vertices.Count, rotationRadians);
                //This will change the total height to a random one so that the transformation can happen
            }

            //find the scale needed to be applied to the previous height in order to get the new height
            var scale = height / GetTotalHeight();
            //This will bring the vertices to the new height (but will also change length)
            Scale(scale);
        }
        public override void SetTotalLength(double length)
        {
            var initialTotalLength = GetTotalLength();
            //handle zero case
            //If the length is zero then change the vertices to match e polygon with a random Radius and the same number of sides and Centroid / rotation
            if (initialTotalLength == 0 || double.IsNaN(initialTotalLength))
            {
                var randomCircumscribedCircle = new CircleInfo(100, LocationX, LocationY);
                vertices = MathCalculations.Circle.GetInscribedPolygonVertices(randomCircumscribedCircle, vertices.Count, rotationRadians);
                //This will change the total length to a random one so that the transformation can happen
            }

            //find the scale needed to be applied to the previous length in order to get the new length
            var scale = length / GetTotalLength();
            //This will bring the vertices to the new length (but will also change length)
            Scale(scale);
        }

        public static RegularPolygonInfo ZeroRegularPolygon() => new(CircleInfo.ZeroCircle(), 3, 0);

        static IEqualityComparer<RegularPolygonInfo> IEqualityComparerCreator<RegularPolygonInfo>.GetComparer()
        {
            return new PolygonInfoEqualityComparer();
        }
    }
}
