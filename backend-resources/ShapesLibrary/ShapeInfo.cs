using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using ShapesLibrary.Enums;
using ShapesLibrary.Exceptions;
using ShapesLibrary.Interfaces;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;
using static ShapesLibrary.Services.MathCalculations;

namespace ShapesLibrary
{
    public abstract class ShapeInfo : IShapeInfo, IDeepClonable<ShapeInfo>, IEqualityComparerCreator<ShapeInfo>
    {
        public const string MongoTotalLengthField = "TotalLength";
        public const string MongoTotalHeightField = "TotalHeight";

        public virtual double LocationX { get; set; }
        public virtual double LocationY { get; set; }
        public ShapeInfoType ShapeType { get; protected set; }
        public virtual double RotationRadians { get; protected set; }
        public string DimensionString { get => GetDimensionsString(); }
        public double TotalLength { get => GetTotalLength(); }
        public double TotalHeight { get => GetTotalHeight(); }

        protected ShapeInfo(double locationX, double locationY)
        {
            LocationX = locationX;
            LocationY = locationY;
        }

        protected virtual string GetDimensionsString()
        {
            try
            {
                return $"{GetTotalLength()} x (h){GetTotalHeight()}mm";
            }
            catch (Exception)
            {
                return $"??? x ??? mm";
            }
        }

        public abstract void RotateClockwise();
        public abstract void RotateAntiClockwise();
        public abstract ShapeInfo GetDeepClone();
        /// <summary>
        /// Returns a clone of the shape with reduced perimeter
        /// </summary>
        /// <param name="perimeterShrink">How much to shrink the perimeter</param>
        /// <param name="shiftCenterToMatchParent">Weather to shift the new shape to be centered with the old , if false the location of both shapes is the Same , if true shapes like Semicircle will get shifted to be centered with the original</param>
        /// <returns>The new Reduced Perimeter Clone</returns>
        public abstract ShapeInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent);
        /// <summary>
        /// Returns the Perimeter of the shape
        /// </summary>
        public abstract double GetPerimeter();
        public abstract double GetArea();

        public abstract RectangleInfo GetBoundingBox();
        /// <summary>
        /// Returns a size estimate for the Shape (ex. for a Circle it returns the Radius)
        /// <para>Default is half diagonal of the Bounding Box</para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual double GetSizeEstimate()
        {
            var bBox = GetBoundingBox();
            var size = bBox.GetHalfDiagonalSize();
            return size;
        }
        public abstract void Scale(double scaleFactor);
        public void ScaleFromOrigin(double scaleFactor, PointXY origin)
        {
            Scale(scaleFactor);
            //Apply the scale method to its Drawing and then translate it by the difference of the scale in X and Y axis 
            //so that its relative position remains the same ; 
            //The rectangle is scaled by a factor of ScaleFactor.
            //When scaling, both the size and position of the shapes will scale by the same factor relative to the rectangle’s center.

            //The new coordinates of the shape(NewShapeX, NewShapeY) after scaling can be calculated as:
            //NewShapeX = RectCenterX + (ShapeX−RectCenterX)×ScaleFactor
            //NewShapeX = RectCenterX + (ShapeX−RectCenterX)×ScaleFactor
            //NewShapeY = RectCenterY + (ShapeY−RectCenterY)×ScaleFactor
            //NewShapeY = RectCenterY + (ShapeY−RectCenterY)×ScaleFactor
            LocationX = origin.X + (LocationX - origin.X) * scaleFactor;
            LocationY = origin.Y + (LocationY - origin.Y) * scaleFactor;
        }
        public void Translate(double translateX, double translateY)
        {
            LocationX += translateX;
            LocationY += translateY;
        }
        public void Translate(Vector2D translationDirection)
        {
            LocationX += translationDirection.X;
            LocationY += translationDirection.Y;
        }
        public void TranslateX(double translateX)
        {
            LocationX += translateX;
        }
        public void TranslateY(double translateY)
        {
            LocationY += translateY;
        }
        /// <summary>
        /// Flips the Shape to the specified Horizontal Axis
        /// <para>If an axis is not specified it flips it to its Bounding Boxe's center</para>
        /// </summary>
        /// <param name="flipOriginX"></param>
        public virtual void FlipHorizontally(double flipOriginX = double.NaN)
        {
            if (double.IsNaN(flipOriginX)) flipOriginX = GetBoundingBox().GetLocation().X;
            double flippedX = 2 * flipOriginX - LocationX;
            LocationX = flippedX;
        }
        /// <summary>
        /// Flips the Shape to the specified Vertical Axis
        /// <para>If an axis is not specified it flips it to its Bounding Boxe's center</para>
        /// </summary>
        /// <param name="flipOriginY"></param>
        public virtual void FlipVertically(double flipOriginY = double.NaN)
        {
            if (double.IsNaN(flipOriginY)) flipOriginY = GetBoundingBox().GetLocation().Y;
            double flippedY = 2 * flipOriginY - LocationY;
            LocationY = flippedY;
        }
        public bool IsLeftOf(PointXY point)
        {
            return GetCentroid().X < point.X;
        }
        public bool IsRightOf(PointXY point)
        {
            return GetCentroid().X > point.X;
        }
        public bool IsAboveOf(PointXY point)
        {
            return GetCentroid().Y < point.Y;
        }
        public bool IsBelowOf(PointXY point)
        {
            return GetCentroid().Y > point.Y;
        }
        /// <summary>
        /// Gets the Relative Position of this Shape to the provided Point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public RelativePositioning GetPositionRelativeTo(PointXY point)
        {
            var centroid = GetCentroid();
            double dx = centroid.X - point.X;
            double dy = centroid.Y - point.Y;
            if (dy == 0 && dx == 0) return RelativePositioning.Center;

            RelativePositioning positioning = 0;

            if (dx < 0) positioning |= RelativePositioning.Left;
            else if (dx > 0) positioning |= RelativePositioning.Right;
            
            if(dy < 0) positioning |= RelativePositioning.Above;
            else if (dy > 0) positioning |= RelativePositioning.Below;

            return positioning;
        }
        /// <summary>
        /// Gets the Relative Position of this Shape to the provided Shape
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public RelativePositioning GetPoistionRelativeTo(ShapeInfo shape)
        {
            return GetPositionRelativeTo(shape.GetCentroid());
        }
        
        public PointXY GetLocation() => new(LocationX, LocationY);
        /// <summary>
        /// Returns the Centroid of the Shape (Geometrical Center)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual PointXY GetCentroid()
        {
            throw new NotImplementedException($"{nameof(GetCentroid)} is not Implemented for {this.GetType().Name}");
        }

        public static UndefinedShapeInfo Undefined() => new();
        public static IEqualityComparer<ShapeInfo> GetComparer()
        {
            return new ShapeInfoEqualityComparer();
        }

        public abstract double GetTotalLength();

        public abstract double GetTotalHeight();

        public abstract void SetTotalLength(double length);

        public abstract void SetTotalHeight(double height);

        public bool AreBoundingBoxesIntersecting(ShapeInfo other)
        {
            var rect1 = this.GetBoundingBox();
            var rect2 = other.GetBoundingBox();

            //Checking the overlap on the X and Y axis , if they overlap in both axis then they intersect
            //This method is valid ONLY for Axis Aligned Rectangles!!! (a.k.a : non rotated or rotated by the same amount )
            //The comparisons work either way , if rect1 is to the left of rect2 or rect2 is to the left of rect1
            bool overlapX = rect1.RightX > rect2.LeftX && rect1.LeftX < rect2.RightX;
            bool overlapY = rect1.BottomY > rect2.TopY && rect1.TopY < rect2.BottomY;

            return overlapX && overlapY;
        }

        /// <summary>
        /// Weather this Shape Contains the provided Point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract bool Contains(PointXY point);
        /// <summary>
        /// Weather this Shape Contains the provided Shape
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public abstract bool Contains(ShapeInfo shape);
        /// <summary>
        /// Weather this Shape Intersects with the provided Point (Point on its real Boundary)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract bool IntersectsWithPoint(PointXY point);
        /// <summary>
        /// Weather this Shape Intersects with another Shape
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public abstract bool IntersectsWithShape(ShapeInfo shape);

        public virtual bool ContainsSimpleRectangle(RectangleInfo rect)
        {
            throw new NotSupportedContainmentException(this, rect, "***SIMPLE RECTANGLE CONTAINMENT***");
        }

        /// <summary>
        /// Returns the Projection Info of the Shape onto the provided Direction
        /// </summary>
        /// <param name="axis">The Unit Vector Direction of the Projection Axis</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedProjectionException"></exception>
        public virtual ProjectionInfo GetProjectionOntoAxis(Vector2D axis)
        {
            throw new NotSupportedProjectionException(this, axis);
        }

        IShapeInfo IDeepClonable<IShapeInfo>.GetDeepClone()
        {
            return this.GetDeepClone();
        }
    }
    public class ShapeInfoEqualityComparer : IEqualityComparer<ShapeInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public ShapeInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            this.disregardPositionComparison = disregardPositionComparison;
        }

        private readonly bool disregardPositionComparison;

        public bool Equals(ShapeInfo? x, ShapeInfo? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            if (x.GetType() != y.GetType()) return false;

            switch (x)
            {
                case RectangleRingInfo rectRing:
                    var rectRingComparer = new RectangleRingInfoEqualityComparer(disregardPositionComparison);
                    return rectRingComparer.Equals(rectRing, (RectangleRingInfo)y);
                case RectangleInfo rect:
                    var rectComparer = new RectangleInfoEqualityComparer(disregardPositionComparison);
                    return rectComparer.Equals(rect, (RectangleInfo)y);
                case CircleRingInfo circleRing:
                    var circleRingComparer = new CircleRingInfoEqualityComparer(disregardPositionComparison);
                    return circleRingComparer.Equals(circleRing, (CircleRingInfo)y);
                case CircleInfo circle:
                    var circleComparer = new CircleInfoEqualityComparer(disregardPositionComparison);
                    return circleComparer.Equals(circle, (CircleInfo)y);
                case CapsuleRingInfo capsuleRing:
                    var capsuleRingComparer = new CapsuleRingInfoEqualityComparer(disregardPositionComparison);
                    return capsuleRingComparer.Equals(capsuleRing, (CapsuleRingInfo)y);
                case CapsuleInfo capsule:
                    var capsuleComparer = new CapsuleInfoEqualityComparer(disregardPositionComparison);
                    return capsuleComparer.Equals(capsule, (CapsuleInfo)y);
                case EllipseRingInfo ellipseRing:
                    var ellipseRingComparer = new EllipseRingInfoEqualityComparer(disregardPositionComparison);
                    return ellipseRingComparer.Equals(ellipseRing, (EllipseRingInfo)y);
                case EllipseInfo ellipse:
                    var ellipseComparer = new EllipseInfoEqualityComparer(disregardPositionComparison);
                    return ellipseComparer.Equals(ellipse, (EllipseInfo)y);
                case CircleQuadrantRingInfo quadrantRing:
                    var quadrantRingComparer = new CircleQuadrantRingInfoEqualityComparer(disregardPositionComparison);
                    return quadrantRingComparer.Equals(quadrantRing, (CircleQuadrantRingInfo)y);
                case CircleQuadrantInfo quadrant:
                    var quadrantComparer = new CircleQuadrantInfoEqualityComparer(disregardPositionComparison);
                    return quadrantComparer.Equals(quadrant, (CircleQuadrantInfo)y);
                case CircleSegmentRingInfo segmentRing:
                    var segmentRingComparer = new CircleSegmentRingInfoEqualityComparer(disregardPositionComparison);
                    return segmentRingComparer.Equals(segmentRing, (CircleSegmentRingInfo)y);
                case CircleSegmentInfo segment:
                    var segmentComparer = new CircleSegmentInfoEqualityComparer(disregardPositionComparison);
                    return segmentComparer.Equals(segment, (CircleSegmentInfo)y);
                case EggShapeRingInfo eggRing:
                    var eggRingComparer = new EggShapeRingInfoEqualityComparer(disregardPositionComparison);
                    return eggRingComparer.Equals(eggRing, (EggShapeRingInfo)y);
                case EggShapeInfo egg:
                    var eggComparer = new EggShapeInfoEqualityComparer(disregardPositionComparison);
                    return eggComparer.Equals(egg, (EggShapeInfo)y);
                case LineInfo line:
                    var lineComparer = new LineInfoEqualityComparer(disregardPositionComparison);
                    return lineComparer.Equals(line, (LineInfo)y);
                case PolygonInfo polygon:
                    var polygonComparer = new PolygonInfoEqualityComparer(disregardPositionComparison);
                    return polygonComparer.Equals(polygon, (PolygonInfo)y);
                case CompositeShapeInfo composite:
                    var compositeComparer = new CompositeShapeInfoEqualityComparer(disregardPositionComparison);
                    return compositeComparer.Equals(composite, (CompositeShapeInfo)y);
                case UndefinedShapeInfo: return true;
                default:
                    throw new NotSupportedException($"The Provided {nameof(ShapeInfo)} type is not Supported for Equality Comparison: {x.GetType().Name}");
            }
        }

        public int GetHashCode([DisallowNull] ShapeInfo obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            
            switch (obj)
            {
                case RectangleRingInfo rectRing:
                    var rectRingComparer = new RectangleRingInfoEqualityComparer(disregardPositionComparison);
                    return rectRingComparer.GetHashCode(rectRing);
                case RectangleInfo rect:
                    var rectComparer = new RectangleInfoEqualityComparer(disregardPositionComparison);
                    return rectComparer.GetHashCode(rect);
                case CircleRingInfo circleRing:
                    var circleRingComparer = new CircleRingInfoEqualityComparer(disregardPositionComparison);
                    return circleRingComparer.GetHashCode(circleRing);
                case CircleInfo circle:
                    var circleComparer = new CircleInfoEqualityComparer(disregardPositionComparison);
                    return circleComparer.GetHashCode(circle);
                case CapsuleRingInfo capsuleRing:
                    var capsuleRingComparer = new CapsuleRingInfoEqualityComparer(disregardPositionComparison);
                    return capsuleRingComparer.GetHashCode(capsuleRing);
                case CapsuleInfo capsule:
                    var capsuleComparer = new CapsuleInfoEqualityComparer(disregardPositionComparison);
                    return capsuleComparer.GetHashCode(capsule);
                case EllipseRingInfo ellipseRing:
                    var ellipseRingComparer = new EllipseRingInfoEqualityComparer(disregardPositionComparison);
                    return ellipseRingComparer.GetHashCode(ellipseRing);
                case EllipseInfo ellipse:
                    var ellipseComparer = new EllipseInfoEqualityComparer(disregardPositionComparison);
                    return ellipseComparer.GetHashCode(ellipse);
                case CircleQuadrantRingInfo quadrantRing:
                    var quadrantRingComparer = new CircleQuadrantRingInfoEqualityComparer(disregardPositionComparison);
                    return quadrantRingComparer.GetHashCode(quadrantRing);
                case CircleQuadrantInfo quadrant:
                    var quadrantComparer = new CircleQuadrantInfoEqualityComparer(disregardPositionComparison);
                    return quadrantComparer.GetHashCode(quadrant);
                case CircleSegmentRingInfo segmentRing:
                    var segmentRingComparer = new CircleSegmentRingInfoEqualityComparer(disregardPositionComparison);
                    return segmentRingComparer.GetHashCode(segmentRing);
                case CircleSegmentInfo segment:
                    var segmentComparer = new CircleSegmentInfoEqualityComparer(disregardPositionComparison);
                    return segmentComparer.GetHashCode(segment);
                case EggShapeRingInfo eggRing:
                    var eggRingComparer = new EggShapeRingInfoEqualityComparer(disregardPositionComparison);
                    return eggRingComparer.GetHashCode(eggRing);
                case EggShapeInfo egg:
                    var eggComparer = new EggShapeInfoEqualityComparer(disregardPositionComparison);
                    return eggComparer.GetHashCode(egg);
                case LineInfo line:
                    var lineComparer = new LineInfoEqualityComparer(disregardPositionComparison);
                    return lineComparer.GetHashCode(line);
                case PolygonInfo polygon:
                    var polygonComparer = new PolygonInfoEqualityComparer(disregardPositionComparison);
                    return polygonComparer.GetHashCode(polygon);
                case CompositeShapeInfo composite:
                    var compositeComparer = new CompositeShapeInfoEqualityComparer(disregardPositionComparison);
                    return compositeComparer.GetHashCode(composite);
                case UndefinedShapeInfo: return 17;
                default:
                    throw new NotSupportedException($"The Provided {nameof(ShapeInfo)} type is not Supported for HashCodeGeneration: {obj.GetType().Name}");
            }
        }
    }
    public class ShapeInfoBaseEqualityComparer : IEqualityComparer<ShapeInfo>
    {
        private readonly bool disregardPositionComparison;

        public ShapeInfoBaseEqualityComparer(bool disregardPositionComparison = false)
        {
            this.disregardPositionComparison = disregardPositionComparison;
        }

        public bool Equals(ShapeInfo? x, ShapeInfo? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return (disregardPositionComparison ||
                (x.LocationX == y.LocationX &&
                x.LocationY == y.LocationY));
        }

        public int GetHashCode([DisallowNull] ShapeInfo obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            int hash = 17;
            hash = hash * 23 + obj.ShapeType.GetHashCode();
            if (!disregardPositionComparison)
            {
                hash = hash * 23 + obj.LocationX.GetHashCode();
                hash = hash * 23 + obj.LocationY.GetHashCode();
            }
            return hash;
        }
    }
    public class UndefinedShapeInfo : ShapeInfo
    {
        public UndefinedShapeInfo():base(0,0)
        {
            ShapeType = ShapeInfoType.Undefined;
        }

        public override RectangleInfo GetBoundingBox()
        {
            return new RectangleInfo(0,0);
        }

        public override UndefinedShapeInfo GetDeepClone()
        {
            return new();
        }

        

        public override void RotateClockwise()
        {
            return;
        }
        public override void RotateAntiClockwise()
        {
            return;
        }

        public override double GetPerimeter()
        {
            return 0;
        }
        public override double GetArea()
        {
            return 0;
        }

        public override ShapeInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            return ShapeInfo.Undefined();
        }

        public override void Scale(double scaleFactor)
        {
            return;
        }

        public override double GetTotalLength()
        {
            return 0;
        }

        public override double GetTotalHeight()
        {
            return 0;
        }

        public override void SetTotalLength(double length)
        {
            return;
        }

        public override void SetTotalHeight(double height)
        {
            return;
        }

        public override bool Contains(PointXY point)
        {
            throw new NotSupportedContainmentException(this,point);
        }
        public override bool Contains(ShapeInfo shape)
        {
            throw new NotSupportedContainmentException(this, shape);
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
}
