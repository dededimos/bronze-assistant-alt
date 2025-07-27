using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels;
using CommonHelpers.Exceptions;
using ShapesLibrary;
using ShapesLibrary.Enums;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.Helpers.ViewModelFactories
{
    public class ShapeInfoVmsFactory : IAbstractFactory<ShapeInfoBaseVm, ShapeInfo>
    {
        private readonly IServiceProvider serviceProvider;

        public ShapeInfoVmsFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ShapeInfoBaseVm Create(ShapeInfo shape)
        {
            switch (shape.ShapeType)
            {
                case ShapeInfoType.RectangleShapeInfo:
                    var rectVm = serviceProvider.GetRequiredService<Func<RectangleInfoVm>>().Invoke();
                    rectVm.SetModel((RectangleInfo)shape);
                    return rectVm;
                case ShapeInfoType.CircleShapeInfo:
                    var circleVm = serviceProvider.GetRequiredService<Func<CircleInfoVm>>().Invoke();
                    circleVm.SetModel((CircleInfo)shape);
                    return circleVm;
                case ShapeInfoType.CapsuleShapeInfo:
                    var capsuleVm = serviceProvider.GetRequiredService<Func<CapsuleInfoVm>>().Invoke();
                    capsuleVm.SetModel((CapsuleInfo)shape);
                    return capsuleVm;
                case ShapeInfoType.EllipseShapeInfo:
                    var ellipseVm = serviceProvider.GetRequiredService<Func<EllipseInfoVm>>().Invoke();
                    ellipseVm.SetModel((EllipseInfo)shape);
                    return ellipseVm;
                case ShapeInfoType.EggShapeInfo:
                    var eggVm = serviceProvider.GetRequiredService<Func<EggShapeInfoVm>>().Invoke();
                    eggVm.SetModel((EggShapeInfo)shape);
                    return eggVm;
                case ShapeInfoType.CircleSegmentShapeInfo:
                    var segmentVm = serviceProvider.GetRequiredService<Func<CircleSegmentInfoVm>>().Invoke();
                    segmentVm.SetModel((CircleSegmentInfo)shape);
                    return segmentVm;
                case ShapeInfoType.CircleQuadrantShapeInfo:
                    var quadrantVm = serviceProvider.GetRequiredService<Func<CircleQuadrantInfoVm>>().Invoke();
                    quadrantVm.SetModel((CircleQuadrantInfo)shape);
                    return quadrantVm;
                case ShapeInfoType.RegularPolygonShapeInfo:
                    var polygonVm = serviceProvider.GetRequiredService<Func<RegularPolygonInfoVm>>().Invoke();
                    polygonVm.SetModel((RegularPolygonInfo)shape);
                    return polygonVm;
                case ShapeInfoType.RectangleRingShapeInfo:
                case ShapeInfoType.CircleRingShapeInfo:
                case ShapeInfoType.CapsuleRingShapeInfo:
                case ShapeInfoType.EllipseRingShapeInfo:
                case ShapeInfoType.EggRingShapeInfo:
                case ShapeInfoType.CircleSegmentRingShapeInfo:
                case ShapeInfoType.CircleQuadrantRingShapeInfo:
                case ShapeInfoType.LineShapeInfo:
                case ShapeInfoType.TwoShapeInfo:
                case ShapeInfoType.Undefined:
                default:
                    throw new EnumValueNotSupportedException(shape.ShapeType);
            }
        }
        public ShapeInfoBaseVm Create(ShapeInfoType shapeType)
        {
            switch (shapeType)
            {
                case ShapeInfoType.RectangleShapeInfo:
                    var rectVm = serviceProvider.GetRequiredService<Func<RectangleInfoVm>>().Invoke();
                    return rectVm;
                case ShapeInfoType.CircleShapeInfo:
                    var circleVm = serviceProvider.GetRequiredService<Func<CircleInfoVm>>().Invoke();
                    return circleVm;
                case ShapeInfoType.CapsuleShapeInfo:
                    var capsuleVm = serviceProvider.GetRequiredService<Func<CapsuleInfoVm>>().Invoke();
                    return capsuleVm;
                case ShapeInfoType.EllipseShapeInfo:
                    var ellipseVm = serviceProvider.GetRequiredService<Func<EllipseInfoVm>>().Invoke();
                    return ellipseVm;
                case ShapeInfoType.EggShapeInfo:
                    var eggVm = serviceProvider.GetRequiredService<Func<EggShapeInfoVm>>().Invoke();
                    return eggVm;
                case ShapeInfoType.CircleSegmentShapeInfo:
                    var segmentVm = serviceProvider.GetRequiredService<Func<CircleSegmentInfoVm>>().Invoke();
                    return segmentVm;
                case ShapeInfoType.CircleQuadrantShapeInfo:
                    var quadrantVm = serviceProvider.GetRequiredService<Func<CircleQuadrantInfoVm>>().Invoke();
                    return quadrantVm;
                case ShapeInfoType.RegularPolygonShapeInfo:
                    var polygonVm = serviceProvider.GetRequiredService<Func<RegularPolygonInfoVm>>().Invoke();
                    return polygonVm;
                case ShapeInfoType.RectangleRingShapeInfo:
                case ShapeInfoType.CircleRingShapeInfo:
                case ShapeInfoType.CapsuleRingShapeInfo:
                case ShapeInfoType.EllipseRingShapeInfo:
                case ShapeInfoType.EggRingShapeInfo:
                case ShapeInfoType.CircleSegmentRingShapeInfo:
                case ShapeInfoType.CircleQuadrantRingShapeInfo:
                case ShapeInfoType.LineShapeInfo:
                case ShapeInfoType.TwoShapeInfo:
                case ShapeInfoType.Undefined:
                default:
                    throw new EnumValueNotSupportedException(shapeType);
            }
        }
    }
}
