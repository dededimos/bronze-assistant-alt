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
    /// Egg shape Infromation , Origin Point Center of Circle/Ellipse
    /// </summary>
    [ShapeOrigin("CircleCenter")]
    public class EggShapeInfo : ShapeInfo, IPolygonSimulatable, IDeepClonable<EggShapeInfo>, IRingableShape, IEqualityComparerCreator<EggShapeInfo>
    {
        public const int MINSIMULATIONSIDES = 8;
        public const int OPTIMALSIMULATIONSIDES = 50;
        public int OptimalSimulationSides => OPTIMALSIMULATIONSIDES;
        public EggOrientation Orientation { get; private set; }
        public double CircleDiameter { get => CircleRadius * 2d; }
        public double CircleRadius { get; set; }
        public double EllipseDiameter { get => EllipseRadius * 2d; }
        public double EllipseRadius { get; set; }
        /// <summary>
        /// How many times bigger is the Ellipse Radius from the Circle Radius
        /// </summary>
        public double PreferedElongation { get; set; } = 2;
        public bool UsesElongationCoefficient { get; set; } = true;
        public int MinSimulationSides => MINSIMULATIONSIDES;

        public EggShapeInfo(double circleRadius, double ellipseRadius, EggOrientation orientation = EggOrientation.VerticalPointingTop, double locationX = 0, double locationY = 0) : base(locationX, locationY)
        {
            ShapeType = ShapeInfoType.EggShapeInfo;
            CircleRadius = circleRadius;
            EllipseRadius = ellipseRadius;
            Orientation = orientation;
        }
        /// <summary>
        /// Creates an Egg with the Defined Dimensions and Orientation
        /// </summary>
        /// <param name="orientation">The Orientation of the Egg</param>
        /// <param name="length">The Length of the Egg Shape</param>
        /// <param name="height">The Height of the Egg Shape</param>
        /// <param name="locationX"></param>
        /// <param name="locationY"></param>
        public EggShapeInfo(EggOrientation orientation, double length, double height, double locationX, double locationY) : base(locationX, locationY)
        {
            ShapeType = ShapeInfoType.EggShapeInfo;
            //First orientation in order to set the rest
            Orientation = orientation;
            SetTotalLength(length);
            SetTotalHeight(height);
        }
        public override EggShapeInfo GetDeepClone()
        {
            return (EggShapeInfo)this.MemberwiseClone();
        }

        public static EggShapeInfo ZeroEgg() => new(0, 0);

        public void SetOrientation(EggOrientation orientation) => Orientation = orientation;

        public override EggShapeInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            return new EggShapeInfo(this.CircleRadius - perimeterShrink, this.EllipseRadius - perimeterShrink, this.Orientation, this.LocationX, this.LocationY);
        }

        public override RectangleInfo GetBoundingBox()
        {
            var boundingBox = new RectangleInfo(this.GetTotalLength(), this.GetTotalHeight(), 0, this.LocationX, this.LocationY);
            //The Location of the Bounding box is shifted to that of the Egg Shape , their Origin Point is not at the same coordinates
            //The Coordinates are shifted according the Orientation
            switch (Orientation)
            {
                case EggOrientation.VerticalPointingTop:
                    boundingBox.LocationX = this.LocationX;
                    //The Y is shifted upwards/downwards from the previous based on weather the Ellipse Radius is bigger or smaller than the Rectangle Half Height
                    boundingBox.LocationY = this.LocationY - (EllipseRadius - boundingBox.Height / 2d);
                    break;
                case EggOrientation.VerticalPointingBottom:
                    //Check first case this is similar draw if you have to
                    boundingBox.LocationX = this.LocationX;
                    boundingBox.LocationY = this.LocationY + (EllipseRadius - boundingBox.Height / 2d);
                    break;
                case EggOrientation.HorizontalPointingRight:
                    //Check first case this is similar draw if you have to
                    boundingBox.LocationY = this.LocationY;
                    boundingBox.LocationX = this.LocationX + (EllipseRadius - boundingBox.Length / 2d);
                    break;
                case EggOrientation.HorizontalPointingLeft:
                    //Check first case this is similar draw if you have to
                    boundingBox.LocationY = this.LocationY;
                    boundingBox.LocationX = this.LocationX - (EllipseRadius - boundingBox.Length / 2d);
                    break;
                case EggOrientation.Undefined:
                default:
                    throw new Exception($"Not recognized or Undefined {nameof(EggOrientation)}");
            }
            return boundingBox;
        }

        /// <summary>
        /// Returns the Ellipse Shape that has been used to Create this Egg Shape
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public EllipseInfo GetEggEllipse()
        {
            var ellipseOrientation = Orientation switch
            {
                EggOrientation.VerticalPointingTop or EggOrientation.VerticalPointingBottom => EllipseOrientation.Vertical,
                EggOrientation.HorizontalPointingRight or EggOrientation.HorizontalPointingLeft => EllipseOrientation.Horizontal,
                _ => throw new Exception($"Unrecognized {nameof(EggOrientation)} value : {Orientation}"),
            };
            var ellipse = new EllipseInfo(CircleRadius, EllipseRadius, ellipseOrientation, this.LocationX, this.LocationY);
            return ellipse;
        }
        /// <summary>
        /// Returns the Circle Shape that has been used to create this Egg Shape
        /// </summary>
        /// <returns></returns>
        public CircleInfo GetEggCircle()
        {
            return new CircleInfo(CircleRadius, this.LocationX, this.LocationY);
        }

        /// <summary>
        /// Returns the XY position of the Tip of the Ellipse
        /// </summary>
        /// <returns></returns>
        public PointXY GetEllipseTipPoint()
        {
            return Orientation switch
            {
                EggOrientation.VerticalPointingTop => new(this.LocationX, this.LocationY - EllipseRadius),
                EggOrientation.VerticalPointingBottom => new(this.LocationX, this.LocationY + EllipseRadius),
                EggOrientation.HorizontalPointingRight => new(this.LocationX + EllipseRadius, this.LocationY),
                EggOrientation.HorizontalPointingLeft => new(this.LocationX - EllipseRadius, this.LocationY),
                _ => throw new EnumValueNotSupportedException(Orientation),
            };
        }
        /// <summary>
        /// Returns the XY position of the Tip of the Circle
        /// </summary>
        /// <returns></returns>
        public PointXY GetCircleTipPoint()
        {
            return Orientation switch
            {
                EggOrientation.VerticalPointingTop => new(this.LocationX, this.LocationY + CircleRadius),
                EggOrientation.VerticalPointingBottom => new(this.LocationX, this.LocationY - CircleRadius),
                EggOrientation.HorizontalPointingRight => new(this.LocationX - CircleRadius, this.LocationY),
                EggOrientation.HorizontalPointingLeft => new(this.LocationX + CircleRadius, this.LocationY),
                _ => throw new EnumValueNotSupportedException(Orientation),
            };
        }

        public override double GetTotalHeight()
        {
            return Orientation switch
            {
                EggOrientation.VerticalPointingTop or EggOrientation.VerticalPointingBottom => CircleRadius + EllipseRadius,
                EggOrientation.HorizontalPointingRight or EggOrientation.HorizontalPointingLeft => CircleDiameter,
                _ => throw new EnumValueNotSupportedException(Orientation),
            };
        }
        public override double GetTotalLength()
        {
            return Orientation switch
            {
                EggOrientation.VerticalPointingTop or EggOrientation.VerticalPointingBottom => CircleDiameter,
                EggOrientation.HorizontalPointingRight or EggOrientation.HorizontalPointingLeft => CircleRadius + EllipseRadius,
                _ => throw new EnumValueNotSupportedException(Orientation),
            };
        }

        public override void SetTotalHeight(double height)
        {
            if (UsesElongationCoefficient)
            {
                switch (Orientation)
                {
                    case EggOrientation.VerticalPointingTop:
                    case EggOrientation.VerticalPointingBottom:
                        if (PreferedElongation <= 0) throw new InvalidOperationException($"{nameof(PreferedElongation)} must be greater than zero");
                        CircleRadius = height / (PreferedElongation + 1);
                        break;
                    case EggOrientation.HorizontalPointingRight:
                    case EggOrientation.HorizontalPointingLeft:
                        CircleRadius = height / 2d;
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Orientation);
                }
                EllipseRadius = CircleRadius * PreferedElongation;
            }
            else
            {
                switch (Orientation)
                {
                    case EggOrientation.VerticalPointingTop:
                    case EggOrientation.VerticalPointingBottom:
                        //Keep Circle Radius the Same and change only Ellipse
                        EllipseRadius = height - CircleRadius;
                        break;
                    case EggOrientation.HorizontalPointingRight:
                    case EggOrientation.HorizontalPointingLeft:
                        //Save the Length to keep it the same
                        var length = GetTotalLength();
                        //Change the Circle Radius and restore the Length of the Egg by changing the Ellipse Radius
                        CircleRadius = height / 2d;
                        EllipseRadius = length - CircleRadius;
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Orientation);
                }
            }
        }
        public override void SetTotalLength(double length)
        {
            if (UsesElongationCoefficient)
            {
                switch (Orientation)
                {
                    case EggOrientation.VerticalPointingTop:
                    case EggOrientation.VerticalPointingBottom:
                        CircleRadius = length / 2d;
                        break;
                    case EggOrientation.HorizontalPointingRight:
                    case EggOrientation.HorizontalPointingLeft:
                        if (PreferedElongation <= 0) throw new InvalidOperationException($"{nameof(PreferedElongation)} must be greater than zero");
                        CircleRadius = length / (PreferedElongation + 1);
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Orientation);
                }
                EllipseRadius = CircleRadius * PreferedElongation;
            }
            else
            {
                switch (Orientation)
                {
                    case EggOrientation.VerticalPointingTop:
                    case EggOrientation.VerticalPointingBottom:
                        //Save the Length to keep it the same
                        var height = GetTotalHeight();
                        //Change the Circle Radius and restore the Height of the Egg by changing the Ellipse Radius
                        CircleRadius = length / 2d;
                        EllipseRadius = height - CircleRadius;
                        break;
                    case EggOrientation.HorizontalPointingRight:
                    case EggOrientation.HorizontalPointingLeft:
                        //Keep Circle Radius the Same and change only Ellipse
                        EllipseRadius = length - CircleRadius;
                        break;
                    default:
                        throw new EnumValueNotSupportedException(Orientation);
                }
            }
        }
        /// <summary>
        /// Calculates the centroid of the egg shape, consisting of a half-circle and a half-ellipse.
        /// </summary>
        /// <returns>A PointXY representing the coordinates of the centroid of the egg shape.</returns>
        public override PointXY GetCentroid()
        {
            // Step 1: Calculate the centroids of the half-circle and half-ellipse
            double centroidCircle = (4 * CircleRadius) / (3 * Math.PI); // Distance from the flat edge of the half-circle
            double centroidEllipse = (4 * EllipseRadius) / (3 * Math.PI); // Distance from the flat edge of the half-ellipse

            // Step 2: Calculate the areas of the half-circle and half-ellipse
            double areaCircle = 0.5 * Math.PI * CircleRadius * CircleRadius;
            double areaEllipse = 0.5 * Math.PI * CircleRadius * EllipseRadius; // CircleRadius is the semi-minor axis, EllipseRadius is the semi-major axis

            // Step 3: Compute the combined centroid using the weighted average formula
            double combinedCentroidDistance = (centroidCircle * areaCircle + centroidEllipse * areaEllipse) / (areaCircle + areaEllipse);

            // Step 4: Determine the position of the centroid based on the egg's orientation
            double centroidX = LocationX;
            double centroidY = LocationY;

            switch (Orientation)
            {
                case EggOrientation.VerticalPointingTop:
                    centroidY -= combinedCentroidDistance; // Move upward from the circle's center
                    break;
                case EggOrientation.VerticalPointingBottom:
                    centroidY += combinedCentroidDistance; // Move downward from the circle's center
                    break;
                case EggOrientation.HorizontalPointingRight:
                    centroidX += combinedCentroidDistance; // Move to the right from the circle's center
                    break;
                case EggOrientation.HorizontalPointingLeft:
                    centroidX -= combinedCentroidDistance; // Move to the left from the circle's center
                    break;
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }

            return new PointXY(centroidX, centroidY);
        }

        public override void Scale(double scaleFactor)
        {
            CircleRadius *= scaleFactor;
            EllipseRadius *= scaleFactor;
        }

        public override void RotateClockwise()
        {
            Orientation = Orientation switch
            {
                EggOrientation.VerticalPointingTop => EggOrientation.HorizontalPointingRight,
                EggOrientation.VerticalPointingBottom => EggOrientation.HorizontalPointingLeft,
                EggOrientation.HorizontalPointingRight => EggOrientation.VerticalPointingBottom,
                EggOrientation.HorizontalPointingLeft => EggOrientation.VerticalPointingTop,
                _ => throw new Exception($"Unrecognized Orientation of Egg Shape : {nameof(EggOrientation)}"),
            };
            RotationRadians += Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void RotateAntiClockwise()
        {
            Orientation = Orientation switch
            {
                EggOrientation.VerticalPointingTop => EggOrientation.HorizontalPointingLeft,
                EggOrientation.VerticalPointingBottom => EggOrientation.HorizontalPointingRight,
                EggOrientation.HorizontalPointingRight => EggOrientation.VerticalPointingTop,
                EggOrientation.HorizontalPointingLeft => EggOrientation.VerticalPointingBottom,
                _ => throw new Exception($"Unrecognized Orientation of Egg Shape : {nameof(EggOrientation)}"),
            };
            RotationRadians -= Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void FlipHorizontally(double flipOriginX = double.NaN)
        {
            base.FlipHorizontally(flipOriginX);
            switch (Orientation)
            {
                //Orientation not affected by flipping
                case EggOrientation.VerticalPointingTop:
                case EggOrientation.VerticalPointingBottom:
                    break;
                case EggOrientation.HorizontalPointingRight:
                    SetOrientation(EggOrientation.HorizontalPointingLeft);
                    break;
                case EggOrientation.HorizontalPointingLeft:
                    SetOrientation(EggOrientation.HorizontalPointingRight);
                    break;
                case EggOrientation.Undefined:
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }
        }
        public override void FlipVertically(double flipOriginY = double.NaN)
        {
            base.FlipVertically(flipOriginY);
            switch (Orientation)
            {
                case EggOrientation.VerticalPointingTop:
                    SetOrientation(EggOrientation.VerticalPointingBottom);
                    break;
                case EggOrientation.VerticalPointingBottom:
                    SetOrientation(EggOrientation.VerticalPointingTop);
                    break;
                //Orientation not affected by flipping
                case EggOrientation.HorizontalPointingRight:
                case EggOrientation.HorizontalPointingLeft:
                    break;
                case EggOrientation.Undefined:
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }
        }

        public override double GetPerimeter()
        {
            return MathCalculations.Egg.GetPerimeter(CircleRadius, EllipseRadius);
        }
        public override double GetArea()
        {
            // Area of the half-circle
            double areaCircle = 0.5 * Math.PI * CircleRadius * CircleRadius;

            // Area of the half-ellipse
            double areaEllipse = 0.5 * Math.PI * CircleRadius * EllipseRadius;

            // Total area of the egg shape
            return areaCircle + areaEllipse;
        }
        public EggShapeRingInfo GetEquivalentRingShape(double ringThickness)
        {
            return new EggShapeRingInfo(this.CircleRadius, this.EllipseRadius, ringThickness, this.Orientation, this.LocationX, this.LocationY);
        }
        IRingShape IRingableShape.GetRingShape(double ringThickness)
        {
            return GetEquivalentRingShape(ringThickness);
        }

        static IEqualityComparer<EggShapeInfo> IEqualityComparerCreator<EggShapeInfo>.GetComparer()
        {
            return new EggShapeInfoEqualityComparer();
        }

        public override bool Contains(PointXY point)
        {
            //Normalizes the point as if the origin was 0,0 in the center of the Egg
            double dx = point.X - LocationX;
            double dy = point.Y - LocationY;

            //We check if the point maybe on the Ellipse or Maybe on the Circle
            //Because we automatically remove the half boundaries by checking ys and xs of the point
            //Then we only have to check weather its inside the Ellipse or inside the circle
            switch (Orientation)
            {
                case EggOrientation.VerticalPointingTop:
                    if (dy < DoubleSafeEqualityComparer.DefaultEpsilon) //point may be ONLY inside Ellipse
                        return GetEggEllipse().Contains(point);
                    else  //point may be ONLY inside Circle
                        return GetEggCircle().Contains(point);
                case EggOrientation.VerticalPointingBottom:
                    if (dy > -DoubleSafeEqualityComparer.DefaultEpsilon) //point may be ONLY inside Ellipse
                        return GetEggEllipse().Contains(point);
                    else //point may be ONLY inside Circle
                        return GetEggCircle().Contains(point);
                case EggOrientation.HorizontalPointingRight:
                    if (dx > -DoubleSafeEqualityComparer.DefaultEpsilon) //point may be ONLY inside Ellipse
                        return GetEggEllipse().Contains(point);
                    else //point may be ONLY inside Circle
                        return GetEggCircle().Contains(point);
                case EggOrientation.HorizontalPointingLeft:
                    if (dx < DoubleSafeEqualityComparer.DefaultEpsilon) //point may be ONLY inside Ellipse
                        return GetEggEllipse().Contains(point);
                    else //point may be ONLY inside Circle
                        return GetEggCircle().Contains(point);
                default:
                    throw new EnumValueNotSupportedException(Orientation);
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
            //Normalizes the point as if the origin was 0,0 in the center of the Egg
            double dx = point.X - LocationX;
            double dy = point.Y - LocationY;

            //We check if the point maybe on the Ellipse or Maybe on the Circle
            //Because we automatically remove the half boundaries by checking ys and xs of the point
            //Then we only have to check weather its inside the Ellipse or inside the circle
            switch (Orientation)
            {
                case EggOrientation.VerticalPointingTop:
                    if (dy <= DoubleSafeEqualityComparer.DefaultEpsilon) //point may be ONLY inside Ellipse
                        return GetEggEllipse().IntersectsWithPoint(point);
                    else  //point may be ONLY inside Circle
                        return GetEggCircle().IntersectsWithPoint(point);
                case EggOrientation.VerticalPointingBottom:
                    if (dy >= -DoubleSafeEqualityComparer.DefaultEpsilon) //point may be ONLY inside Ellipse
                        return GetEggEllipse().IntersectsWithPoint(point);
                    else //point may be ONLY inside Circle
                        return GetEggCircle().IntersectsWithPoint(point);
                case EggOrientation.HorizontalPointingRight:
                    if (dx >= -DoubleSafeEqualityComparer.DefaultEpsilon) //point may be ONLY inside Ellipse
                        return GetEggEllipse().IntersectsWithPoint(point);
                    else //point may be ONLY inside Circle
                        return GetEggCircle().IntersectsWithPoint(point);
                case EggOrientation.HorizontalPointingLeft:
                    if (dx <= DoubleSafeEqualityComparer.DefaultEpsilon) //point may be ONLY inside Ellipse
                        return GetEggEllipse().IntersectsWithPoint(point);
                    else //point may be ONLY inside Circle
                        return GetEggCircle().IntersectsWithPoint(point);
                default:
                    throw new EnumValueNotSupportedException(Orientation);
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

        public PolygonInfo GetPolygonSimulation(int sides)
        {
            if (sides < MinSimulationSides) throw new SimulationSidesOutOfRangeException(this);
            //Need to Simulate the Circle Part , The Ellipse Part 
            //And remove the last and first vertices from the retrieved points each time to avoid duplication
            double startCircleAngle;
            double startEllipseAngle;
            double endCircleAngle;
            double endEllipseAngle;

            switch (Orientation)
            {
                case EggOrientation.VerticalPointingTop:
                    startCircleAngle = 0;
                    endCircleAngle = Math.PI;
                    startEllipseAngle = Math.PI;
                    endEllipseAngle = 2 * Math.PI;
                    break;
                case EggOrientation.VerticalPointingBottom:
                    startCircleAngle = -Math.PI;
                    endCircleAngle = 0;
                    startEllipseAngle = 0;
                    endEllipseAngle = Math.PI;
                    break;
                case EggOrientation.HorizontalPointingRight:
                    startCircleAngle = -3 * Math.PI / 2d;
                    endCircleAngle = -Math.PI / 2d;
                    startEllipseAngle = -Math.PI / 2d;
                    endEllipseAngle = Math.PI / 2d;
                    break;
                case EggOrientation.HorizontalPointingLeft:
                    startCircleAngle = -Math.PI / 2d;
                    endCircleAngle = Math.PI / 2d;
                    startEllipseAngle = Math.PI / 2d;
                    endEllipseAngle = 3 * Math.PI / 2d;
                    break;
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }
            //The two vertices of the Egg are the same for both the circle and the Ellipse 
            //So we put sides/2 + 2 to each of the Functions returning points . The First and Last returned vertices will be those anyways

            var isOdd = sides % 2 != 0;
            // Calculate how to distribute sides between the two Shapes.
            int circlePoints, ellipsePoints;

            if (isOdd)
            {
                // If odd, assign one extra side to the Ellipse.
                sides--; // Adjust for odd number by subtracting one.
                circlePoints = (sides / 2);
                ellipsePoints = sides / 2 + 1;
            }
            else
            {
                // If even, distribute equally between both circles.
                circlePoints = sides / 2;
                ellipsePoints = circlePoints;
            }

            var circle = this.GetEggCircle();
            var ellipse = this.GetEggEllipse();
            var circleVertices = MathCalculations.Circle.GetCircleArcPoints(circle, circlePoints, startCircleAngle, endCircleAngle);
            var ellipseVertices = MathCalculations.Ellipse.GetEllipseArcPoints(ellipse, ellipsePoints, startEllipseAngle, endEllipseAngle);

            //remove last and first points from one of the shapes
            // remove first and last vertices from ellipseVertices
            ellipseVertices.RemoveAt(0);
            ellipseVertices.RemoveAt(ellipseVertices.Count - 1);

            //Circle always drawn First
            return new PolygonInfo([.. circleVertices, .. ellipseVertices]);
        }
    }

    public class EggShapeInfoEqualityComparer : IEqualityComparer<EggShapeInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public EggShapeInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly ShapeInfoBaseEqualityComparer baseComparer;

        public bool Equals(EggShapeInfo? x, EggShapeInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Orientation == y!.Orientation &&
                x.CircleRadius == y.CircleRadius &&
                x.EllipseRadius == y.EllipseRadius &&
                x.PreferedElongation == y.PreferedElongation &&
                x.UsesElongationCoefficient == y.UsesElongationCoefficient;
        }

        public int GetHashCode([DisallowNull] EggShapeInfo obj)
        {
            int hash = baseComparer.GetHashCode(obj);
            return HashCode.Combine(hash, obj.Orientation, obj.CircleRadius, obj.EllipseRadius, obj.PreferedElongation, obj.UsesElongationCoefficient);
        }
    }


    [ShapeOrigin("CircleCenter")]
    public class EggShapeRingInfo : EggShapeInfo, IDeepClonable<EggShapeRingInfo>, IRingShape, IEqualityComparerCreator<EggShapeRingInfo>
    {
        public double Thickness { get; set; }
        public EggShapeRingInfo(double circleRadius, double ellipseRadius, double thickness, EggOrientation orientation = EggOrientation.VerticalPointingTop, double locationX = 0, double locationY = 0)
            : base(circleRadius, ellipseRadius, orientation, locationX, locationY)
        {
            ShapeType = ShapeInfoType.EggRingShapeInfo;
            Thickness = thickness;
        }

        public EggShapeRingInfo(EggOrientation orientation, double length, double height, double thickness, double locationX, double locationY)
            : base(orientation, length, height, locationX, locationY)
        {
            ShapeType = ShapeInfoType.EggRingShapeInfo;
            Thickness = thickness;
        }

        public override EggShapeRingInfo GetDeepClone()
        {
            return (EggShapeRingInfo)this.MemberwiseClone();
        }
        public static EggShapeRingInfo ZeroEggRing() => new(0, 0,0);

        public EggShapeInfo GetInnerRingWholeShape()
        {
            return new EggShapeInfo(this.CircleRadius - this.Thickness, this.EllipseRadius - this.Thickness, this.Orientation, this.LocationX, this.LocationY);
        }

        public EggShapeInfo GetOuterRingWholeShape()
        {
            return new EggShapeInfo(this.CircleRadius, this.EllipseRadius, this.Orientation, this.LocationX, this.LocationY);
        }

        IRingableShape IRingShape.GetInnerRingWholeShape()
        {
            return GetInnerRingWholeShape();
        }

        IRingableShape IRingShape.GetOuterRingWholeShape()
        {
            return GetOuterRingWholeShape();
        }

        static IEqualityComparer<EggShapeRingInfo> IEqualityComparerCreator<EggShapeRingInfo>.GetComparer()
        {
            return new EggShapeRingInfoEqualityComparer();
        }
    }
    public class EggShapeRingInfoEqualityComparer : IEqualityComparer<EggShapeRingInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public EggShapeRingInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly EggShapeInfoEqualityComparer baseComparer;

        public bool Equals(EggShapeRingInfo? x, EggShapeRingInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Thickness == y!.Thickness;
        }

        public int GetHashCode([DisallowNull] EggShapeRingInfo obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.Thickness);
        }
    }
}
