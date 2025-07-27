using CommonInterfacesBronze;
using DocumentFormat.OpenXml.Vml;
using MirrorsLib.Enums;
using MirrorsLib.Services;
using ShapesLibrary;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.Helpers.ViewModelFactories
{
    public class ShapeInfoEditorViewModelsFactory : IAbstractFactory<IModelGetterViewModel<ShapeInfo>, ShapeInfo>
    {
        private readonly IServiceProvider serviceProvider;

        public ShapeInfoEditorViewModelsFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IModelGetterViewModel<ShapeInfo> Create(ShapeInfo shape)
        {
            switch (shape)
            {
                case RectangleRingInfo rectRing:
                    var rectRingVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<RectangleRingInfo>>>().Invoke();
                    rectRingVm.SetModel(rectRing);
                    return rectRingVm;
                case RectangleInfo rect:
                    var rectVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<RectangleInfo>>>().Invoke();
                    rectVm.SetModel(rect);
                    return rectVm;
                case CircleRingInfo circleRing:
                    var circleRingVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<CircleRingInfo>>>().Invoke();
                    circleRingVm.SetModel(circleRing);
                    return circleRingVm;
                case CircleInfo circle:
                    var circleVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<CircleInfo>>>().Invoke();
                    circleVm.SetModel(circle);
                    return circleVm;
                case CapsuleInfo capsule:
                    var capsuleVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<CapsuleInfo>>>().Invoke();
                    capsuleVm.SetModel(capsule);
                    return capsuleVm;
                case EllipseInfo ellipse:
                    var ellipseVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<EllipseInfo>>>().Invoke();
                    ellipseVm.SetModel(ellipse);
                    return ellipseVm;
                case CircleQuadrantInfo quadrant:
                    var quadrantVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<CircleQuadrantInfo>>>().Invoke();
                    quadrantVm.SetModel(quadrant);
                    return quadrantVm;
                case CircleSegmentInfo segment:
                    var segmentVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<CircleSegmentInfo>>>().Invoke();
                    segmentVm.SetModel(segment);
                    return segmentVm;
                case EggShapeInfo egg:
                    var eggVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<EggShapeInfo>>>().Invoke();
                    eggVm.SetModel(egg);
                    return eggVm;
                case RegularPolygonInfo regularPolygon:
                    var polygonVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<RegularPolygonInfo>>>().Invoke();
                    polygonVm.SetModel(regularPolygon);
                    return polygonVm;
                case UndefinedShapeInfo undefined:
                    var undefinedVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<UndefinedShapeInfo>>>().Invoke();
                    undefinedVm.SetModel(undefined);
                    return undefinedVm;
                default:
                    throw new NotSupportedException($"{typeof(ShapeInfo).Name} of Type : {shape.GetType().Name} is not supported by {this.GetType().Name}");
            }
        }
    }
}
