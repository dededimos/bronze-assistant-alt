using MirrorsLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShapesLibrary.Enums;
using CommonHelpers.Exceptions;
using ShapesLibrary;
using ShapesLibrary.ShapeInfoModels;

namespace MirrorsLib.Helpers
{
    public static class MirrorHelperExtensions
    {
        /// <summary>
        /// Returns a Default Shape Info Object based on the Provided Mirror Shape
        /// </summary>
        /// <param name="shape">The Shape for which to create a Shape Info Object</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static ShapeInfo ToShapeInfoObject(this BronzeMirrorShape shape)
        {
            return shape switch
            {
                BronzeMirrorShape.UndefinedMirrorShape => ShapeInfo.Undefined(),
                BronzeMirrorShape.RectangleMirrorShape => RectangleInfo.ZeroRectangle(),
                BronzeMirrorShape.CircleMirrorShape => CircleInfo.ZeroCircle(),
                BronzeMirrorShape.CapsuleMirrorShape => CapsuleInfo.CapsuleZero(),
                BronzeMirrorShape.EllipseMirrorShape => EllipseInfo.ZeroEllipse(),
                BronzeMirrorShape.CircleSegmentMirrorShape => CircleSegmentInfo.CircleSegmentZero(),
                BronzeMirrorShape.CircleQuadrantMirrorShape => CircleQuadrantInfo.ZeroQuadrant(),
                BronzeMirrorShape.EggMirrorShape => EggShapeInfo.ZeroEgg(),
                BronzeMirrorShape.RegularPolygonMirrorShape => RegularPolygonInfo.ZeroRegularPolygon(),
                _ => throw new NotSupportedException($"Mirror Shape Type : {shape} is not Supported by {nameof(ShapeInfo)} class"),
            };
        }
        public static ShapeInfoType ToShapeInfoType(this BronzeMirrorShape mirrorShape)
        {
            return mirrorShape switch
            {
                BronzeMirrorShape.RectangleMirrorShape => ShapeInfoType.RectangleShapeInfo,
                BronzeMirrorShape.CircleMirrorShape => ShapeInfoType.CircleShapeInfo,
                BronzeMirrorShape.CapsuleMirrorShape => ShapeInfoType.CapsuleShapeInfo,
                BronzeMirrorShape.EllipseMirrorShape => ShapeInfoType.EllipseShapeInfo,
                BronzeMirrorShape.CircleSegmentMirrorShape => ShapeInfoType.CircleSegmentShapeInfo,
                BronzeMirrorShape.CircleQuadrantMirrorShape => ShapeInfoType.CircleQuadrantShapeInfo,
                BronzeMirrorShape.EggMirrorShape => ShapeInfoType.EggShapeInfo,
                BronzeMirrorShape.RegularPolygonMirrorShape => ShapeInfoType.RegularPolygonShapeInfo,
                BronzeMirrorShape.UndefinedMirrorShape => ShapeInfoType.Undefined,
                _ => throw new EnumValueNotSupportedException(mirrorShape)
            };
        }
        /// <summary>
        /// Returns the <see cref="BronzeMirrorShape"> matching the <see cref="ShapeInfoType"/> argument
        /// </summary>
        /// <param name="shapeInfoType">The Type of Shape to match to a mirror</param>
        /// <returns></returns>
        public static BronzeMirrorShape ToBronzeMirrorShape(this ShapeInfoType shapeInfoType)
        {
            return shapeInfoType switch
            {
                ShapeInfoType.RectangleShapeInfo => BronzeMirrorShape.RectangleMirrorShape,
                ShapeInfoType.CircleShapeInfo => BronzeMirrorShape.CircleMirrorShape,
                ShapeInfoType.CapsuleShapeInfo => BronzeMirrorShape.CapsuleMirrorShape,
                ShapeInfoType.EllipseShapeInfo => BronzeMirrorShape.EllipseMirrorShape,
                ShapeInfoType.EggShapeInfo => BronzeMirrorShape.EggMirrorShape,
                ShapeInfoType.CircleSegmentShapeInfo => BronzeMirrorShape.CircleSegmentMirrorShape,
                ShapeInfoType.CircleQuadrantShapeInfo => BronzeMirrorShape.CircleQuadrantMirrorShape,
                _ => throw new EnumValueNotSupportedException(shapeInfoType),
            };
        }
        public static MirrorOrientedShape ToMirrorOrientedShape(this ShapeInfo shapeInfo)
        {
            return shapeInfo switch
            {
                RectangleInfo => MirrorOrientedShape.RectangleMirrorShape,
                CircleInfo => MirrorOrientedShape.CircleMirrorShape,
                CapsuleInfo capsule => capsule.Orientation == CapsuleOrientation.Horizontal ? MirrorOrientedShape.HorizontalCapsuleMirrorShape : MirrorOrientedShape.VerticalCapsuleMirrorShape,
                EllipseInfo ellipse => ellipse.Orientation == EllipseOrientation.Horizontal ? MirrorOrientedShape.HorizontalEllipseMirrorShape : MirrorOrientedShape.VerticalEllipseMirrorShape,
                CircleQuadrantInfo quadrant => quadrant.QuadrantPart.ToMirrorOrientedShape(),
                CircleSegmentInfo segment => segment.Orientation.ToMirrorOrientedShape(),
                EggShapeInfo egg => egg.Orientation.ToMirrorOrientedShape(),
                RegularPolygonInfo => MirrorOrientedShape.RegularPolygonMirrorShape,
                UndefinedShapeInfo => MirrorOrientedShape.Undefined,
                _ => throw new NotSupportedException($"{shapeInfo.GetType().Name} is not a Supported Mirror Shape Type"),
            };
        }
        public static MirrorOrientedShape ToMirrorOrientedShape(this EggOrientation orientation)
        {
            return orientation switch
            {
                EggOrientation.VerticalPointingTop => MirrorOrientedShape.TopEggMirrorShape,
                EggOrientation.VerticalPointingBottom => MirrorOrientedShape.BottomEggMirrorShape,
                EggOrientation.HorizontalPointingRight => MirrorOrientedShape.RightEggMirrorShape,
                EggOrientation.HorizontalPointingLeft => MirrorOrientedShape.LeftEggMirrorShape,
                _ => throw new EnumValueNotSupportedException(orientation),
            };
        }
        public static MirrorOrientedShape ToMirrorOrientedShape(this CircleSegmentOrientation orientation)
        {
            return orientation switch
            {
                CircleSegmentOrientation.PointingTop => MirrorOrientedShape.TopSegmentMirrorShape,
                CircleSegmentOrientation.PointingBottom => MirrorOrientedShape.BottomSegmentMirrorShape,
                CircleSegmentOrientation.PointingLeft => MirrorOrientedShape.LeftSegmentMirrorShape,
                CircleSegmentOrientation.PointingRight => MirrorOrientedShape.RightSegmentMirrorShape,
                _ => throw new EnumValueNotSupportedException(orientation),
            };
        }
        public static MirrorOrientedShape ToMirrorOrientedShape(this CircleQuadrantPart quadrantPart)
        {
            return quadrantPart switch
            {
                CircleQuadrantPart.TopLeft => MirrorOrientedShape.TopLeftQuadrantMirrorShape,
                CircleQuadrantPart.TopRight => MirrorOrientedShape.TopRightQuadrantMirrorShape,
                CircleQuadrantPart.BottomLeft => MirrorOrientedShape.BottomLeftQuadrantMirrorShape,
                CircleQuadrantPart.BottomRight => MirrorOrientedShape.BottomRightQuadrantMirrorShape,
                _ => throw new EnumValueNotSupportedException(quadrantPart),
            };
        }
        public static BronzeMirrorShape ToBronzeMirrorShape(this MirrorOrientedShape orientedShape)
        {
            return orientedShape switch
            {
                MirrorOrientedShape.RectangleMirrorShape => BronzeMirrorShape.RectangleMirrorShape,
                MirrorOrientedShape.CircleMirrorShape => BronzeMirrorShape.CircleMirrorShape,
                MirrorOrientedShape.HorizontalEllipseMirrorShape => BronzeMirrorShape.EllipseMirrorShape,
                MirrorOrientedShape.VerticalEllipseMirrorShape => BronzeMirrorShape.EllipseMirrorShape,
                MirrorOrientedShape.HorizontalCapsuleMirrorShape => BronzeMirrorShape.CapsuleMirrorShape,
                MirrorOrientedShape.VerticalCapsuleMirrorShape => BronzeMirrorShape.CapsuleMirrorShape,
                MirrorOrientedShape.TopEggMirrorShape => BronzeMirrorShape.EggMirrorShape,
                MirrorOrientedShape.BottomEggMirrorShape => BronzeMirrorShape.EggMirrorShape,
                MirrorOrientedShape.LeftEggMirrorShape => BronzeMirrorShape.EggMirrorShape,
                MirrorOrientedShape.RightEggMirrorShape => BronzeMirrorShape.EggMirrorShape,
                MirrorOrientedShape.TopSegmentMirrorShape => BronzeMirrorShape.CircleSegmentMirrorShape,
                MirrorOrientedShape.BottomSegmentMirrorShape => BronzeMirrorShape.CircleSegmentMirrorShape,
                MirrorOrientedShape.LeftSegmentMirrorShape => BronzeMirrorShape.CircleSegmentMirrorShape,
                MirrorOrientedShape.RightSegmentMirrorShape => BronzeMirrorShape.CircleSegmentMirrorShape,
                MirrorOrientedShape.TopRightQuadrantMirrorShape => BronzeMirrorShape.CircleQuadrantMirrorShape,
                MirrorOrientedShape.TopLeftQuadrantMirrorShape => BronzeMirrorShape.CircleQuadrantMirrorShape,
                MirrorOrientedShape.BottomLeftQuadrantMirrorShape => BronzeMirrorShape.CircleQuadrantMirrorShape,
                MirrorOrientedShape.BottomRightQuadrantMirrorShape => BronzeMirrorShape.CircleQuadrantMirrorShape,
                MirrorOrientedShape.RegularPolygonMirrorShape => BronzeMirrorShape.RegularPolygonMirrorShape,
                _ => BronzeMirrorShape.UndefinedMirrorShape,
            };
        }
    }
}
