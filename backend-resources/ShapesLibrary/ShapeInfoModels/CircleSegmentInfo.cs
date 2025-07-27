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
    /// Circle Seagment , Origin Point Center of Chord
    /// </summary>
    [ShapeOrigin("ChordCenter")]
    public class CircleSegmentInfo : ShapeInfo, IPolygonSimulatable, IRingableShape, IDeepClonable<CircleSegmentInfo>, IEqualityComparerCreator<CircleSegmentInfo>
    {
        public const int MINSIMULATIONSIDES = 8;
        public const int OPTIMALSIMULATIONSIDES = 36;
        public int MinSimulationSides => MINSIMULATIONSIDES;
        public int OptimalSimulationSides => OPTIMALSIMULATIONSIDES;
        private double length;
        public double Length { get => length; set => SetTotalLength(value); }
        public override double GetTotalLength() => length;
        public override void SetTotalLength(double length)
        {
            if (this.length != length) this.length = length;
            else return;

            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                case CircleSegmentOrientation.PointingBottom:
                    sagitta = height;
                    if (Sagitta >= length / 2) //θ > 180 and length = 2 * R
                    {
                        // R must always be R>= S/2 in order for the Segment to Have meaning => check the following formula on GetChordLengthWithSagitta
                        var radius = length / 2;
                        chordLength = MathCalculations.CircleSegment.GetChordLengthWithSagitta(radius, Sagitta);
                    }
                    else //θ <= 180 and height = ChordLength
                    {
                        chordLength = length;
                    }
                    break;
                case CircleSegmentOrientation.PointingLeft:
                case CircleSegmentOrientation.PointingRight:
                    sagitta = length;
                    if (Sagitta >= height / 2d)
                    {
                        var radius = height / 2;
                        chordLength = MathCalculations.CircleSegment.GetChordLengthWithSagitta(radius, Sagitta);
                    }
                    else
                    {
                        chordLength = height;
                    }
                    break;
                default:
                    throw new Exception($"Unrecognized Orientation of Circle Chord Seagment Shape : {nameof(CircleSegmentOrientation)}");
            }
        }

        private double height;
        public double Height { get => height; set => SetTotalHeight(value); }
        public override double GetTotalHeight() => height;
        public override void SetTotalHeight(double height)
        {
            if (this.height != height) this.height = height;
            else return;

            //save length to preserve it  (change sagitta and Chord)
            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                case CircleSegmentOrientation.PointingBottom:
                    sagitta = height;
                    if (Sagitta >= length / 2d)
                    {
                        var radius = length / 2;
                        chordLength = MathCalculations.CircleSegment.GetChordLengthWithSagitta(radius, Sagitta);

                    }
                    else
                    {
                        chordLength = length;
                    }
                    break;
                case CircleSegmentOrientation.PointingLeft:
                case CircleSegmentOrientation.PointingRight:
                    sagitta = length;
                    //The Sagitta is already known here (would be the length)
                    if (Sagitta >= height / 2) //θ > 180 and height = 2 * R
                    {
                        // R must always be R>= S/2 in order for the Segment to Have meaning => check the following formula on GetChordLengthWithSagitta
                        var radius = height / 2;
                        chordLength = MathCalculations.CircleSegment.GetChordLengthWithSagitta(radius, Sagitta);
                    }
                    else //θ <= 180 and height = ChordLength
                    {
                        chordLength = height;
                    }
                    break;
                default:
                    throw new Exception($"Unrecognized Orientation of Circle Chord Seagment Shape : {nameof(CircleSegmentOrientation)}");
            }
        }

        public CircleSegmentOrientation Orientation { get; private set; }
        private double sagitta;
        public double Sagitta { get => sagitta; set => SetSagitta(value); }
        private void SetSagitta(double value)
        {
            if (Equals(sagitta, value)) return;
            else sagitta = value;
            RecalculateLengthHeight();
        }

        private double chordLength;
        public double ChordLength { get => chordLength; set => SetChordLength(value); }

        private void SetChordLength(double value)
        {
            if (Equals(chordLength, value)) return;
            else this.chordLength = value;
            RecalculateLengthHeight();
        }

        /// <summary>
        /// Recalculates the Length or Height of the Segment that are affected by a changes in the Sagitta /Chord Length or orientation
        /// </summary>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        private void RecalculateLengthHeight()
        {
            //Change length or height but do not change chord
            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                case CircleSegmentOrientation.PointingBottom:
                    height = sagitta;
                    if (MathCalculations.CircleSegment.IsThetaGreaterOrEqualThan180(ChordLength, Sagitta))
                    {
                        length = MathCalculations.CircleSegment.GetRadius(ChordLength, Sagitta) * 2d;
                    }
                    else
                    {
                        length = ChordLength;
                    }
                    break;
                case CircleSegmentOrientation.PointingLeft:
                case CircleSegmentOrientation.PointingRight:
                    length = sagitta;
                    if (MathCalculations.CircleSegment.IsThetaGreaterOrEqualThan180(ChordLength, Sagitta))
                    {
                        height = MathCalculations.CircleSegment.GetRadius(ChordLength, Sagitta) * 2d;
                    }
                    else
                    {
                        height = ChordLength;
                    }
                    break;
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }
        }

        public override CircleSegmentInfo GetDeepClone()
        {
            return (CircleSegmentInfo)this.MemberwiseClone();
        }
        public CircleSegmentInfo(double chordLength, double chordSagitta, CircleSegmentOrientation orientation = CircleSegmentOrientation.PointingTop, double locationX = 0, double locationY = 0)
            : base(locationX, locationY)
        {
            ShapeType = ShapeInfoType.CircleSegmentShapeInfo;
            //Set orientation first otherwise it throws
            Orientation = orientation;
            Sagitta = chordSagitta;
            ChordLength = chordLength;
        }
        public CircleSegmentInfo(CircleSegmentOrientation orientation, double length, double height, double locationX, double locationY) : base(locationX, locationY)
        {
            ShapeType = ShapeInfoType.CircleSegmentShapeInfo;
            //must set first orientation before the height / length
            Orientation = orientation;
            //The Sagitta needs to be Set First because the Radius needs it in order to be calculated 
            //And both Total Length and height depending on the orientation set the Sagitta 
            //Then according to orientation we execute differently the Methods
            switch (orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                case CircleSegmentOrientation.PointingBottom:
                    SetTotalHeight(height);
                    SetTotalLength(length);
                    break;
                case CircleSegmentOrientation.PointingLeft:
                case CircleSegmentOrientation.PointingRight:
                    SetTotalLength(length);
                    SetTotalHeight(height);
                    break;
                default:
                    throw new EnumValueNotSupportedException(orientation);
            }
        }

        public void SetOrientation(CircleSegmentOrientation newOrientation)
        {
            var currentOrientation = this.Orientation;
            this.Orientation = newOrientation;
            //Recalculate Length-Height If the Orientation Changed from a Vertical to a Horizontal and vice versa
            //By Recalculating Length Height 
            if (
                ((newOrientation == CircleSegmentOrientation.PointingLeft || newOrientation == CircleSegmentOrientation.PointingRight) && (currentOrientation == CircleSegmentOrientation.PointingTop || currentOrientation == CircleSegmentOrientation.PointingBottom))
                ||
                ((newOrientation == CircleSegmentOrientation.PointingBottom || newOrientation == CircleSegmentOrientation.PointingTop) && (currentOrientation == CircleSegmentOrientation.PointingLeft || currentOrientation == CircleSegmentOrientation.PointingRight))
               )
            {
                RecalculateLengthHeight();
            }
        }
        /// <summary>
        /// Returns the Opposite orientation from the one provided
        /// </summary>
        /// <param name="orientation"></param>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public static CircleSegmentOrientation GetOppositeOrientation(CircleSegmentOrientation orientation)
        {
            return orientation switch
            {
                CircleSegmentOrientation.PointingTop => CircleSegmentOrientation.PointingBottom,
                CircleSegmentOrientation.PointingBottom => CircleSegmentOrientation.PointingTop,
                CircleSegmentOrientation.PointingLeft => CircleSegmentOrientation.PointingRight,
                CircleSegmentOrientation.PointingRight => CircleSegmentOrientation.PointingLeft,
                _ => throw new EnumValueNotSupportedException(orientation),
            };
        }
        /// <summary>
        /// Returns the Opposite orientation from the current one
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public CircleSegmentOrientation GetOppositeOrientation()
        {
            return GetOppositeOrientation(Orientation);
        }
        /// <summary>
        /// Returns the Defining Circle of this Segment
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CircleInfo GetDefiningCircle()
        {
            CircleInfo circle = new(MathCalculations.CircleSegment.GetRadius(ChordLength, Sagitta));
            var small = IsSmallSegment();
            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                    circle.LocationX = this.LocationX;
                    if (small) circle.LocationY = this.LocationY + (circle.Radius - this.Sagitta);
                    else circle.LocationY = this.LocationY - (this.Sagitta - circle.Radius);
                    break;
                case CircleSegmentOrientation.PointingBottom:
                    circle.LocationX = this.LocationX;
                    if (small) circle.LocationY = this.LocationY - (circle.Radius - this.Sagitta);
                    else circle.LocationY = this.LocationY + (this.Sagitta - circle.Radius);
                    break;
                case CircleSegmentOrientation.PointingLeft:
                    if (small) circle.LocationX = this.LocationX + (circle.Radius - this.Sagitta);
                    else circle.LocationX = this.LocationX - (this.Sagitta - circle.Radius);
                    circle.LocationY = this.LocationY;
                    break;
                case CircleSegmentOrientation.PointingRight:
                    if (small) circle.LocationX = this.LocationX - (circle.Radius - this.Sagitta);
                    else circle.LocationX = this.LocationX + (this.Sagitta - circle.Radius);
                    circle.LocationY = this.LocationY;
                    break;
                case CircleSegmentOrientation.Undefined:
                default:
                    throw new Exception($"Unrecognized or Undefined {nameof(CircleSegmentOrientation)}");
            }
            return circle;
        }
        /// <summary>
        /// Returns the Radius of the Defining Circle
        /// </summary>
        /// <returns></returns>
        public double GetRadius()
        {
            return MathCalculations.CircleSegment.GetRadius(ChordLength, Sagitta);
        }
        /// <summary>
        /// Weather the Segment is the Small one from the Two Segments formed by the Defining Circle
        /// </summary>
        /// <returns></returns>
        public bool IsSmallSegment()
        {
            return Sagitta < GetRadius();
        }
        public bool IsChordHorizontal()
        {
            return Orientation is CircleSegmentOrientation.PointingTop or CircleSegmentOrientation.PointingBottom;
        }
        /// <summary>
        /// Returns the Geometrical Center of the Segment
        /// <para>The center is not the center of the Sagitta , its always displaced a little more from the tip of the segment</para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public override PointXY GetCentroid()
        {
            double radius = GetRadius();
            double theta = MathCalculations.CircleSegment.GetTheta(ChordLength, radius, Sagitta);

            // Step 3: Calculate the distance from the chord to the centroid using the formula
            double distanceToCentroid = (4 * radius * Math.Pow(Math.Sin(theta / 2), 3)) / (3 * (theta - Math.Sin(theta)));
            // Step 5: Determine the position of the centroid based on the segment's orientation
            double centroidX = LocationX;
            double centroidY = LocationY;

            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                    centroidY -= (Sagitta - distanceToCentroid);  // Move upward
                    break;
                case CircleSegmentOrientation.PointingBottom:
                    centroidY += (Sagitta - distanceToCentroid);  // Move downward
                    break;
                case CircleSegmentOrientation.PointingLeft:
                    centroidX -= (Sagitta - distanceToCentroid);  // Move left
                    break;
                case CircleSegmentOrientation.PointingRight:
                    centroidX += (Sagitta - distanceToCentroid);  // Move right
                    break;
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }

            return new PointXY(centroidX, centroidY);
        }

        /// <summary>
        /// Returns the Segment that with this one complete the Circle
        /// <para>The Remaining Segment will have the same ChordLength and Radius</para>
        /// <para>But the Sagitta will be 2*Radius - Sagitta </para>
        /// <para>The Orientation of the Remaining Segment is the Inverse of this one</para>
        /// </summary>
        /// <returns></returns>
        public CircleSegmentInfo GetOppositeSegment()
        {
            var radius = MathCalculations.CircleSegment.GetRadius(ChordLength, Sagitta);
            var oppositeSagitta = 2 * radius - this.Sagitta;
            return new(ChordLength, oppositeSagitta, GetOppositeOrientation(), this.LocationX, this.LocationY);
        }
        /// <summary>
        /// Returns the tip Point of the Arc of the Segment
        /// </summary>
        /// <returns></returns>
        public PointXY GetArcTipPoint()
        {
            return Orientation switch
            {
                CircleSegmentOrientation.PointingTop => new(this.LocationX, this.LocationY - Sagitta),
                CircleSegmentOrientation.PointingBottom => new(this.LocationX, this.LocationY + Sagitta),
                CircleSegmentOrientation.PointingLeft => new(this.LocationX - Sagitta, this.LocationY),
                CircleSegmentOrientation.PointingRight => new(this.LocationX + Sagitta, this.LocationY),
                _ => throw new EnumValueNotSupportedException(Orientation),
            };
        }
        /// <summary>
        /// Returns the Left or Top Point of the Chord depending on orientation
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public PointXY GetChordLeftTopPoint()
        {
            return Orientation switch
            {
                CircleSegmentOrientation.PointingTop or CircleSegmentOrientation.PointingBottom => new(this.LocationX - ChordLength / 2d, this.LocationY),
                CircleSegmentOrientation.PointingLeft or CircleSegmentOrientation.PointingRight => new(this.LocationX, this.LocationY - ChordLength / 2d),
                _ => throw new EnumValueNotSupportedException(Orientation),
            };
        }
        /// <summary>
        /// Returns the Right or Bottom Point of the Chord depending on orientation
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EnumValueNotSupportedException"></exception>
        public PointXY GetChordRightBottomPoint()
        {
            return Orientation switch
            {
                CircleSegmentOrientation.PointingTop or CircleSegmentOrientation.PointingBottom => new(this.LocationX + ChordLength / 2d, this.LocationY),
                CircleSegmentOrientation.PointingLeft or CircleSegmentOrientation.PointingRight => new(this.LocationX, this.LocationY + ChordLength / 2d),
                _ => throw new EnumValueNotSupportedException(Orientation),
            };
        }
        public override CircleSegmentInfo GetReducedPerimeterClone(double perimeterShrink, bool shiftCenterToMatchParent)
        {
            var shrink = perimeterShrink * 2;
            var x = this.LocationX;
            var y = this.LocationY;
            if (shiftCenterToMatchParent)
            {
                switch (Orientation)
                {
                    case CircleSegmentOrientation.PointingTop:
                        y -= perimeterShrink;
                        break;
                    case CircleSegmentOrientation.PointingBottom:
                        y += perimeterShrink;
                        break;
                    case CircleSegmentOrientation.PointingLeft:
                        x -= perimeterShrink;
                        break;
                    case CircleSegmentOrientation.PointingRight:
                        x += perimeterShrink;
                        break;
                    case CircleSegmentOrientation.Undefined:
                    default:
                        throw new EnumValueNotSupportedException(Orientation);
                }
            }
            return new CircleSegmentInfo(ChordLength - shrink * 2, Sagitta - shrink, Orientation, x, y);
        }
        public override RectangleInfo GetBoundingBox()
        {
            //Set the Bounding Box to Origin
            var boundingBox = new RectangleInfo(GetTotalLength(), GetTotalHeight(), 0, LocationX, LocationY);
            //The Center of the Bounding Box is the Center of the Sagitta (shift the Origin)
            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                    boundingBox.LocationX = this.LocationX;
                    boundingBox.LocationY = this.LocationY - Sagitta * 0.5d;
                    break;
                case CircleSegmentOrientation.PointingBottom:
                    boundingBox.LocationX = this.LocationX;
                    boundingBox.LocationY = this.LocationY + Sagitta * 0.5d;
                    break;
                case CircleSegmentOrientation.PointingLeft:
                    boundingBox.LocationX = this.LocationX - Sagitta * 0.5d;
                    boundingBox.LocationY = this.LocationY;
                    break;
                case CircleSegmentOrientation.PointingRight:
                    boundingBox.LocationX = this.LocationX + Sagitta * 0.5d;
                    boundingBox.LocationY = this.LocationY;
                    break;
                case CircleSegmentOrientation.Undefined:
                default:
                    throw new Exception($"Unrecognized or Undefined {nameof(CircleSegmentOrientation)}");
            }
            return boundingBox;
        }

        public override void Scale(double scaleFactor)
        {
            ChordLength *= scaleFactor;
            Sagitta *= scaleFactor;
        }

        public override void RotateClockwise()
        {
            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                    SetOrientation(CircleSegmentOrientation.PointingRight);
                    break;
                case CircleSegmentOrientation.PointingBottom:
                    SetOrientation(CircleSegmentOrientation.PointingLeft);
                    break;
                case CircleSegmentOrientation.PointingLeft:
                    SetOrientation(CircleSegmentOrientation.PointingTop);
                    break;
                case CircleSegmentOrientation.PointingRight:
                    SetOrientation(CircleSegmentOrientation.PointingBottom);
                    break;
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }
            RotationRadians += Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void RotateAntiClockwise()
        {
            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                    SetOrientation(CircleSegmentOrientation.PointingLeft);
                    break;
                case CircleSegmentOrientation.PointingBottom:
                    SetOrientation(CircleSegmentOrientation.PointingRight);
                    break;
                case CircleSegmentOrientation.PointingLeft:
                    SetOrientation(CircleSegmentOrientation.PointingBottom);
                    break;
                case CircleSegmentOrientation.PointingRight:
                    SetOrientation(CircleSegmentOrientation.PointingTop);
                    break;
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }
            RotationRadians -= Math.PI / 2;
            RotationRadians = MathCalculations.VariousMath.NormalizeAngle(RotationRadians);
        }
        public override void FlipHorizontally(double flipOriginX = double.NaN)
        {
            base.FlipHorizontally(flipOriginX);
            switch (Orientation)
            {
                //Orientation Not Affected by flipping
                case CircleSegmentOrientation.PointingTop:
                case CircleSegmentOrientation.PointingBottom:
                    break;
                case CircleSegmentOrientation.PointingLeft:
                    SetOrientation(CircleSegmentOrientation.PointingRight);
                    break;
                case CircleSegmentOrientation.PointingRight:
                    SetOrientation(CircleSegmentOrientation.PointingLeft);
                    break;
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }
        }
        public override void FlipVertically(double flipOriginY = double.NaN)
        {
            base.FlipVertically(flipOriginY);
            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                    SetOrientation(CircleSegmentOrientation.PointingBottom);
                    break;
                case CircleSegmentOrientation.PointingBottom:
                    SetOrientation(CircleSegmentOrientation.PointingTop);
                    break;
                //Orientation Not Affected by flipping
                case CircleSegmentOrientation.PointingLeft:
                case CircleSegmentOrientation.PointingRight:
                    break;
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }
        }

        public override double GetPerimeter()
        {
            return MathCalculations.CircleSegment.GetPerimeter(GetRadius(), ChordLength, Sagitta);
        }
        public override double GetArea()
        {
            if (Sagitta == 0 || ChordLength == 0) return 0;
            
            double radius = GetRadius();
            double theta = MathCalculations.CircleSegment.GetTheta(ChordLength, radius, Sagitta);
            double segmentArea = (radius * radius / 2) * (theta - Math.Sin(theta));
            return segmentArea;
        }

        public CircleSegmentRingInfo GetEquivalentRingShape(double ringThickness)
        {
            return new CircleSegmentRingInfo(ChordLength, this.Sagitta, ringThickness, this.Orientation, this.LocationX, this.LocationY);
        }

        IRingShape IRingableShape.GetRingShape(double ringThickness)
        {
            return GetEquivalentRingShape(ringThickness);
        }

        public static CircleSegmentInfo CircleSegmentZero() => new(CircleSegmentOrientation.PointingTop, 0, 0, 0, 0);

        static IEqualityComparer<CircleSegmentInfo> IEqualityComparerCreator<CircleSegmentInfo>.GetComparer()
        {
            return new CircleSegmentInfoEqualityComparer();
        }

        public override bool Contains(PointXY point)
        {
            var defCircle = GetDefiningCircle();
            double dx = point.X - defCircle.LocationX;
            double dy = point.Y - defCircle.LocationY;

            //Outside of Defining Circle => Outside of Segment
            if (dx * dx + dy * dy > ((defCircle.Radius * defCircle.Radius) + DoubleSafeEqualityComparer.DefaultEpsilon)) return false;

            //The Point is already calculated as Inside of Defining Circle , so we need to check that its inside the bounds of the Segment
            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                    //We need to determine that the point is above the Chord
                    return point.Y < this.LocationY + DoubleSafeEqualityComparer.DefaultEpsilon;
                case CircleSegmentOrientation.PointingBottom:
                    //We need to determine that the point is below the Chord
                    return point.Y > this.LocationY - DoubleSafeEqualityComparer.DefaultEpsilon;
                case CircleSegmentOrientation.PointingLeft:
                    //We need to determine that the point is left of the Chord
                    return point.X < this.LocationX + DoubleSafeEqualityComparer.DefaultEpsilon;
                case CircleSegmentOrientation.PointingRight:
                    //We need to determine that the point is right of the Chord
                    return point.X > this.LocationX - DoubleSafeEqualityComparer.DefaultEpsilon;
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
            var defCircle = GetDefiningCircle();
            double dx = point.X - defCircle.LocationX;
            double dy = point.Y - defCircle.LocationY;

            //Outside of Defining Circle => Outside of Segment
            if (dx * dx + dy * dy > ((defCircle.Radius * defCircle.Radius) + DoubleSafeEqualityComparer.DefaultEpsilon)) return false;

            //The Point is already calculated as Inside of Defining Circle , so we need to check that its inside the bounds of the Segment
            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                    //We need to determine that the point is above the Chord
                    return point.Y <= this.LocationY + DoubleSafeEqualityComparer.DefaultEpsilon;
                case CircleSegmentOrientation.PointingBottom:
                    //We need to determine that the point is below the Chord
                    return point.Y >= this.LocationY - DoubleSafeEqualityComparer.DefaultEpsilon;
                case CircleSegmentOrientation.PointingLeft:
                    //We need to determine that the point is left of the Chord
                    return point.X <= this.LocationX + DoubleSafeEqualityComparer.DefaultEpsilon;
                case CircleSegmentOrientation.PointingRight:
                    //We need to determine that the point is right of the Chord
                    return point.X >= this.LocationX - DoubleSafeEqualityComparer.DefaultEpsilon;
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
            if (ChordLength < DoubleSafeEqualityComparer.DefaultEpsilon || Sagitta < DoubleSafeEqualityComparer.DefaultEpsilon)
            {
                return PolygonInfo.ZeroPolygon();
            }

            //The 2 sides will always be those of the chord

            //find the non directional Thetta
            var thetta = MathCalculations.CircleSegment.GetTheta(ChordLength, GetRadius(), Sagitta);
            thetta = MathCalculations.VariousMath.NormalizeAngle(thetta); //normalize to be only positive and within 0-2π
            var oppositeAngle = 2 * Math.PI - thetta;
            //Apply transformation to thetta based on the current orientation and if this is the small or big segment
            //How to Apply the Transformation
            //1. The Angle that we have calculated above is the non-directional angle of the Segment 
            //2. The Rest 2π - nondirectional is the opposite Angle . 
            //3. The Opposite angle lies always at the middle of the other end of the Shape (we only have top,bottom ,left , right orientations)
            //4. So in order to find the start and end points of the non-directional angle we need to deduct/add half the opposite angle for the start and end based on the quadrant.
            //5. We have to also calculate the Start point in a way that its angle is smaller than that of the end to get the Clockwise order of Points
            //6. Zero angle is at the right , and we start drawing always from left to right
            //7. So the start is on the left , the end on the right
            double startAngle;
            double endAngle;
            switch (Orientation)
            {
                case CircleSegmentOrientation.PointingTop:
                    if (this.IsSmallSegment())
                    {
                        //startAngle = -Math.PI / 2 - thetta / 2d;
                        startAngle = (-Math.PI - thetta) / 2d;
                        //endAngle = -Math.PI / 2 + thetta / 2d;
                        endAngle = (-Math.PI + thetta) / 2d;
                    }
                    //Big
                    else
                    {
                        //startAngle = - 3 * Math.PI/2 + oppositeAngle / 2d;
                        startAngle = (-3 * Math.PI + oppositeAngle) / 2d;
                        //endAngle = Math.PI/2 - oppositeAngle/2;
                        endAngle = (Math.PI - oppositeAngle) / 2d;
                    }
                    break;
                case CircleSegmentOrientation.PointingBottom:
                    if (this.IsSmallSegment())
                    {
                        //startAngle = Math.PI / 2d - thetta / 2d;
                        startAngle = (Math.PI - thetta) / 2d;
                        //endAngle = (Math.PI/2d + thetta / 2d;
                        endAngle = (Math.PI + thetta) / 2d;
                    }
                    //Big
                    else
                    {
                        //startAngle = -Math.PI/2d + oppositeAngle/ 2d;
                        startAngle = (-Math.PI + oppositeAngle) / 2d;
                        //endAngle = 3 * Math.PI/2d - oppositeAngle / 2d;
                        endAngle = (3 * Math.PI - oppositeAngle) / 2d;
                    }
                    break;
                case CircleSegmentOrientation.PointingLeft:
                    if (this.IsSmallSegment())
                    {
                        startAngle = Math.PI - thetta / 2d;
                        endAngle = Math.PI + thetta / 2d;
                    }
                    //Big
                    else
                    {
                        startAngle = oppositeAngle / 2d;
                        endAngle = 2 * Math.PI - oppositeAngle / 2d;
                    }
                    break;
                case CircleSegmentOrientation.PointingRight:
                    if (this.IsSmallSegment())
                    {
                        startAngle = -thetta / 2d;
                        endAngle = +thetta / 2d;
                    }
                    //Big
                    else
                    {
                        startAngle = -Math.PI + oppositeAngle / 2d;
                        endAngle = +Math.PI - oppositeAngle / 2d;
                    }
                    break;
                default:
                    throw new EnumValueNotSupportedException(Orientation);
            }

            var vertices = MathCalculations.Circle.GetCircleArcPoints(this.GetDefiningCircle(), sides, startAngle, endAngle);
            return new PolygonInfo(vertices);
        }
    }
    public class CircleSegmentInfoEqualityComparer : IEqualityComparer<CircleSegmentInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public CircleSegmentInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly ShapeInfoBaseEqualityComparer baseComparer;

        public bool Equals(CircleSegmentInfo? x, CircleSegmentInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Orientation == y!.Orientation &&
                x.ChordLength == y.ChordLength &&
                x.Sagitta == y.Sagitta;
        }

        public int GetHashCode([DisallowNull] CircleSegmentInfo obj)
        {
            int hash = baseComparer.GetHashCode(obj);
            return HashCode.Combine(hash, obj.Orientation, obj.ChordLength, obj.Sagitta);
        }
    }
    public class CircleSegmentRingInfo : CircleSegmentInfo, IDeepClonable<CircleSegmentRingInfo>, IRingShape , IEqualityComparerCreator<CircleSegmentRingInfo>
    {
        public double Thickness { get; set; }
        public CircleSegmentRingInfo(double chordLength, double chordSagitta, double thickness, CircleSegmentOrientation orientation = CircleSegmentOrientation.PointingTop, double locationX = 0, double locationY = 0) : base(chordLength, chordSagitta, orientation, locationX, locationY)
        {
            ShapeType = ShapeInfoType.CircleSegmentRingShapeInfo;
            Thickness = thickness;
        }

        public CircleSegmentRingInfo(CircleSegmentOrientation orientation, double length, double height, double thickness, double locationX, double locationY) : base(orientation, length, height, locationX, locationY)
        {
            ShapeType = ShapeInfoType.CircleSegmentRingShapeInfo;
            Thickness = thickness;
        }
        public override CircleSegmentRingInfo GetDeepClone()
        {
            return (CircleSegmentRingInfo)this.MemberwiseClone();
        }
        public static CircleSegmentRingInfo CircleSegmentRingZero() => new(CircleSegmentOrientation.PointingTop, 0, 0, 0, 0,0);
        public CircleSegmentInfo GetOuterShape()
        {
            return new CircleSegmentInfo(ChordLength, this.Sagitta, this.Orientation, this.LocationX, this.LocationY);
        }
        IRingableShape IRingShape.GetOuterRingWholeShape()
        {
            return GetOuterShape();
        }
        public CircleSegmentInfo GetInnerRingShape()
        {
            var innerShape = GetOuterShape().GetReducedPerimeterClone(Thickness, true);
            return innerShape;
        }
        IRingableShape IRingShape.GetInnerRingWholeShape()
        {
            return GetInnerRingShape();
        }

        static IEqualityComparer<CircleSegmentRingInfo> IEqualityComparerCreator<CircleSegmentRingInfo>.GetComparer()
        {
            return new CircleSegmentRingInfoEqualityComparer();
        }
    }
    public class CircleSegmentRingInfoEqualityComparer : IEqualityComparer<CircleSegmentRingInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardPositionComparison">Weather to diregard (x,y) position comparison</param>
        public CircleSegmentRingInfoEqualityComparer(bool disregardPositionComparison = false)
        {
            baseComparer = new(disregardPositionComparison);
        }

        private readonly CircleSegmentInfoEqualityComparer baseComparer;

        public bool Equals(CircleSegmentRingInfo? x, CircleSegmentRingInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Thickness == y!.Thickness;
        }

        public int GetHashCode([DisallowNull] CircleSegmentRingInfo obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.Thickness);
        }
    }
}
