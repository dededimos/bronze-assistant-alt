using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements.Sandblasts;

namespace BronzeFactoryApplication.Helpers.ViewModelFactories
{
    public class SandblastEditorViewModelsFactory : IAbstractFactory<IModelGetterViewModel<MirrorSandblastInfo>, MirrorSandblastInfo>
    {
        private readonly IServiceProvider serviceProvider;

        public SandblastEditorViewModelsFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IModelGetterViewModel<MirrorSandblastInfo> Create(MirrorSandblastInfo sandblast)
        {
            switch (sandblast)
            {
                case LineSandblast line:
                    var lineVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<LineSandblast>>>().Invoke();
                    lineVm.SetModel(line);
                    return lineVm;
                case HoledRectangleSandblast holedRect:
                    var holedRectVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<HoledRectangleSandblast>>>().Invoke();
                    holedRectVm.SetModel(holedRect);
                    return holedRectVm;
                case TwoLineSandblast twoLine:
                    var twoLineVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<TwoLineSandblast>>>().Invoke();
                    twoLineVm.SetModel(twoLine);
                    return twoLineVm;
                case CircularSandblast circle:
                    var circleVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<CircularSandblast>>>().Invoke();
                    circleVm.SetModel(circle);
                    return circleVm;
                case UndefinedSandblastInfo undefined:
                    var undefinedVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<UndefinedSandblastInfo>>>().Invoke();
                    undefinedVm.SetModel(undefined);
                    return undefinedVm;
                default:
                    throw new NotSupportedException($"{typeof(MirrorSandblastInfo)} of Type : {sandblast.GetType()} is not supported by {nameof(SandblastEditorViewModelsFactory)}");
            }
        }
        public IModelGetterViewModel<MirrorSandblastInfo> CreateNew(MirrorSandblastType sandblastType)
        {
            switch (sandblastType)
            {
                case MirrorSandblastType.Undefined:
                    var undefined = serviceProvider.GetRequiredService<Func<IEditorViewModel<UndefinedSandblastInfo>>>().Invoke();
                    undefined.SetModel(new());
                    return undefined;
                case MirrorSandblastType.LineSandblast:
                    var line = serviceProvider.GetRequiredService<Func<IEditorViewModel<LineSandblast>>>().Invoke();
                    line.SetModel(new());
                    return line;
                case MirrorSandblastType.HoledRectangleSandblast:
                    var holedRect = serviceProvider.GetRequiredService<Func<IEditorViewModel<HoledRectangleSandblast>>>().Invoke();
                    holedRect.SetModel(new());
                    return holedRect;
                case MirrorSandblastType.TwoLineSandblast:
                    var twoLine = serviceProvider.GetRequiredService<Func<IEditorViewModel<TwoLineSandblast>>>().Invoke();
                    twoLine.SetModel(new());
                    return twoLine;
                case MirrorSandblastType.CircularSandblast:
                    var circular = serviceProvider.GetRequiredService<Func<IEditorViewModel<CircularSandblast>>>().Invoke();
                    circular.SetModel(new());
                    return circular;
                default:
                    throw new NotSupportedException($"{typeof(MirrorSandblastInfo)} of Type : {sandblastType} is not supported by {nameof(SandblastEditorViewModelsFactory)}");
            }
        }
    }

}
