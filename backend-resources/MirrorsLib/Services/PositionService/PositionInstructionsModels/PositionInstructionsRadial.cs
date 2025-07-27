using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.Services.PositionService.Enums;
using ShapesLibrary;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;
using static ShapesLibrary.Services.MathCalculations;

namespace MirrorsLib.Services.PositionService.PositionInstructionsModels
{
    public class PositionInstructionsRadial : PositionInstructionsBase, IDeepClonable<PositionInstructionsRadial>
    {
        public double AngleFromCenterDeg { get; set; }
        public double AngleFromCenterRad { get => AngleFromCenterDeg * Math.PI / 180; }
        public RadialDistancing RDistancing { get; set; }
        public double RadialDistance { get; set; }

        public PositionInstructionsRadial()
        {
            InstructionsType = PositionInstructionsType.RadialInstructions;
        }
        public override PositionInstructionsRadial GetDeepClone()
        {
            var clone = (PositionInstructionsRadial)this.MemberwiseClone();
            return clone;
        }

        public override PointXY GetPosition(ShapeInfo shape)
        {
            return shape switch
            {
                EllipseInfo ellipse => GetPosition(ellipse),
                CircleSegmentInfo segment => GetPosition(segment),
                CircleQuadrantInfo quadrant => GetPosition(quadrant),
                CircleInfo circle => GetPosition(circle),
                RectangleInfo rectangle => GetPosition(rectangle),
                CapsuleInfo capsule => GetPosition(capsule),
                EggShapeInfo egg => GetPosition(egg),
                RegularPolygonInfo regularPolygon => GetPosition(regularPolygon),
                PolygonInfo polygon => GetPosition(polygon),
                _ => throw new NotSupportedException($"Shape : {shape.GetType().Name} is not Supported"),
            };
        }

        /// <summary>
        /// Returns the Position point of the Item in a circle
        /// </summary>
        /// <param name="circle"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private PointXY GetPosition(CircleInfo circle)
        {
            var distance = RDistancing switch
            {
                RadialDistancing.FromCenterToRadiusEnd => RadialDistance,
                RadialDistancing.FromRadiusEndToCenter => circle.Radius - RadialDistance,
                _ => throw new NotSupportedException($"{nameof(RadialDistancing)} value of : {RDistancing} is not supported"),
            };

            return Points.GetPointAtDistanceFromPoint(distance, AngleFromCenterRad, circle.LocationX, circle.LocationY);
        }
        private PointXY GetPosition(EllipseInfo ellipse)
        {
            double distance;
            switch (RDistancing)
            {
                case RadialDistancing.FromCenterToRadiusEnd:
                    distance = RadialDistance;
                    break;
                case RadialDistancing.FromRadiusEndToCenter:
                    //Get the Radial distance of the Ellipse at this Point 
                    var ellipseRadialDistance = Ellipse.GetEllipseRadialDistance(ellipse.RadiusMajor, ellipse.RadiusMinor, AngleFromCenterRad);
                    distance = ellipseRadialDistance - RadialDistance;
                    break;
                default:
                    throw new NotSupportedException($"{nameof(RadialDistancing)} value of : {RDistancing} is not supported");
            }
            return Points.GetPointAtDistanceFromPoint(distance, AngleFromCenterRad, ellipse.LocationX, ellipse.LocationY);
        }
        private PointXY GetPosition(CircleSegmentInfo segment)
        {
            switch (RDistancing)
            {
                case RadialDistancing.FromCenterToRadiusEnd:
                    //Find the point on which the item will be placed .It at RadialDistance away from the center of the chord at a certain AngleFromCenter
                    return MathCalculations.Points.GetPointAtDistanceFromPoint(RadialDistance, AngleFromCenterRad, segment.LocationX, segment.LocationY);
                case RadialDistancing.FromRadiusEndToCenter:
                    //To measure the distance from the end we will have to find the point on the arc which is the boundary
                    //To Find it we need the Line Equation from the circle segment center to the arc of the segment (with the given angle)
                    //We also need the Circle's equation that forms the center . This way we find the intersection of the line with the cricle thus the point on the arc of the segment
                    var lineEquation = Line.GetLineEquation(segment.LocationX, segment.LocationY, AngleFromCenterRad, true);
                    //To Find the Circle Equation we need to know the angle of the chords bisector with the X axis this , depending on the chords orientation its easy:
                    var bisectorAngle = segment.Orientation switch
                    {
                        ShapesLibrary.Enums.CircleSegmentOrientation.PointingTop => Math.PI / 2,
                        ShapesLibrary.Enums.CircleSegmentOrientation.PointingBottom => -Math.PI / 2,
                        ShapesLibrary.Enums.CircleSegmentOrientation.PointingLeft => 0,
                        ShapesLibrary.Enums.CircleSegmentOrientation.PointingRight => Math.PI,
                        _ => throw new EnumValueNotSupportedException(segment.Orientation),
                    };
                    var circleEquation = CircleSegment.GetParentCircleEquation(segment.ChordLength, segment.Sagitta, new(segment.LocationX, segment.LocationY), bisectorAngle, true);

                    //With the Line and the Circle Equations known we can find the points of intersection between the two 
                    PointXY[] intersectionPoints = Circle.GetIntersectionsWithLine(circleEquation, lineEquation);
                    //The Solution for the intersection is certain to have 2 solutions because the line passes through the center of the chord
                    //so depending on the orientation of the segment we can find the correct point from the solutions
                    if (intersectionPoints.Length == 0) throw new InvalidOperationException("Intersection points cannot be zero please refer this to a developer");
                    if (intersectionPoints.Length == 1) return intersectionPoints[0];
                    //else
                    var intersectionPoint1 = intersectionPoints[0];
                    var intersectionPoint2 = intersectionPoints[1];
                    switch (segment.Orientation)
                    {
                        case ShapesLibrary.Enums.CircleSegmentOrientation.PointingTop:
                            //Take the point with the biggest y , if ys are equal take the smallest or biggest x according to the provided angle of positioning
                            //This way we take into account the case where the line cuts the chord at two places and according to the provided angle from the instructions we take the correct point
                            if (intersectionPoint1.Y != intersectionPoint2.Y) return intersectionPoints.MinBy(p => p.Y);
                            else if (AngleFromCenterDeg >= 90 && AngleFromCenterDeg <= 180) return intersectionPoints.MaxBy(p => p.X);
                            else return intersectionPoints.MinBy(p => p.X);
                        case ShapesLibrary.Enums.CircleSegmentOrientation.PointingBottom:
                            if (intersectionPoint1.Y != intersectionPoint2.Y) return intersectionPoints.MaxBy(p => p.Y);
                            else if (AngleFromCenterDeg >= 270 && AngleFromCenterDeg <= 360) return intersectionPoints.MinBy(p => p.X);
                            else return intersectionPoints.MaxBy(p => p.X);
                        case ShapesLibrary.Enums.CircleSegmentOrientation.PointingLeft:
                            if (intersectionPoint1.X != intersectionPoint2.X) return intersectionPoints.MinBy(p => p.X);
                            else if (AngleFromCenterDeg >= 0 && AngleFromCenterDeg <= 90) return intersectionPoints.MaxBy(p => p.Y);
                            else return intersectionPoints.MinBy(p => p.Y);
                        case ShapesLibrary.Enums.CircleSegmentOrientation.PointingRight:
                            if (intersectionPoint1.X != intersectionPoint2.X) return intersectionPoints.MaxBy(p => p.X);
                            else if (AngleFromCenterDeg >= 90 && AngleFromCenterDeg <= 180) return intersectionPoints.MaxBy(p => p.Y);
                            else return intersectionPoints.MinBy(p => p.Y);
                        case ShapesLibrary.Enums.CircleSegmentOrientation.Undefined:
                        default:
                            throw new EnumValueNotSupportedException(segment.Orientation);
                    }
                default:
                    throw new EnumValueNotSupportedException(RDistancing);
            }
        }
        private PointXY GetPosition(CircleQuadrantInfo shapeInfo)
        {
            return GetPosition(shapeInfo.GetCircle());
        }
        private PointXY GetPosition(RectangleInfo rectangle)
        {
            var circumscribedCircle = rectangle.GetCircumscribedCircle();
            return GetPosition(circumscribedCircle);
        }
        private PointXY GetPosition(CapsuleInfo capsule)
        {
            var normalizedAngle = MathCalculations.VariousMath.NormalizeAngle(AngleFromCenterRad);
            //Radial Positioning In Capsule refers to the Circles of the Capsule .
            //Depending on the given angle in the Instructions and the orientation of the Capsule we can find the correct circle
            var isFirstCircle = capsule.Orientation switch
            {
                ShapesLibrary.Enums.CapsuleOrientation.Horizontal => (normalizedAngle <= Math.PI || normalizedAngle > 3 * Math.PI / 2),
                ShapesLibrary.Enums.CapsuleOrientation.Vertical => normalizedAngle < Math.PI,
                _ => throw new EnumValueNotSupportedException(capsule.Orientation),
            };
            var circleInfo = capsule.GetCapsuleCircle(isFirstCircle);
            return GetPosition(circleInfo);
        }
        private PointXY GetPosition(EggShapeInfo egg)
        {
            //As per the Capsule method we need to find weather the angle refers to the circle or the ellipse of the EggShape
            //and return accordingly the Radial Positioning
            var normalizedAngle = MathCalculations.VariousMath.NormalizeAngle(this.AngleFromCenterRad);
            ShapeInfo ellipseOrCircle = egg.Orientation switch
            {
                ShapesLibrary.Enums.EggOrientation.VerticalPointingTop => normalizedAngle < Math.PI ? egg.GetEggEllipse() : egg.GetEggCircle(),
                ShapesLibrary.Enums.EggOrientation.VerticalPointingBottom => normalizedAngle >= Math.PI ? egg.GetEggEllipse() : egg.GetEggCircle(),
                ShapesLibrary.Enums.EggOrientation.HorizontalPointingRight => normalizedAngle > Math.PI / 2 || normalizedAngle <= 3 * Math.PI / 2 ? egg.GetEggEllipse() : egg.GetEggCircle(),
                ShapesLibrary.Enums.EggOrientation.HorizontalPointingLeft => normalizedAngle <= Math.PI / 2 || normalizedAngle > 3 * Math.PI / 2 ? egg.GetEggEllipse() : egg.GetEggCircle(),
                _ => throw new EnumValueNotSupportedException(egg.Orientation),
            };
            return GetPosition(ellipseOrCircle);
        }
        private PointXY GetPosition(RegularPolygonInfo polygon)
        {
            var periscribedCircle = polygon.GetCircumscribedCircle();
            return GetPosition(periscribedCircle);
        }
        private PointXY GetPosition(PolygonInfo polygon)
        {
            var bBox = polygon.GetBoundingBox().GetCircumscribedCircle();
            return GetPosition(bBox);
        }
    }
    public class PositionInstructionsRadialEqualityComparer : IEqualityComparer<PositionInstructionsRadial>
    {
        public bool Equals(PositionInstructionsRadial? x, PositionInstructionsRadial? y)
        {
            var baseComparer = new PositionInstructionsBaseBaseEqualityComparer();

            return baseComparer.Equals(x, y) &&
                x!.AngleFromCenterDeg == y!.AngleFromCenterDeg &&
                x.RDistancing == y.RDistancing &&
                x.RadialDistance == y.RadialDistance;
        }

        public int GetHashCode([DisallowNull] PositionInstructionsRadial obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
