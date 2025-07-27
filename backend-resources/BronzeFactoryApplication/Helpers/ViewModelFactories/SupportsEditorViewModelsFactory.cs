using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements.Supports;

namespace BronzeFactoryApplication.Helpers.ViewModelFactories
{
    public class SupportsEditorViewModelsFactory : IAbstractFactory<IModelGetterViewModel<MirrorSupportInfo>, MirrorSupportInfo>
    {
        private readonly IServiceProvider serviceProvider;

        public SupportsEditorViewModelsFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IModelGetterViewModel<MirrorSupportInfo> Create(MirrorSupportInfo support)
        {
            switch (support)
            {
                case MirrorMultiSupports multi:
                    var multiVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorMultiSupports>>>().Invoke();
                    multiVm.SetModel(multi);
                    return multiVm;
                case MirrorVisibleFrameSupport frame:
                    var frameVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorVisibleFrameSupport>>>().Invoke();
                    frameVm.SetModel(frame);
                    return frameVm;
                case MirrorBackFrameSupport backFrame:
                    var backFrameVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorBackFrameSupport>>>().Invoke();
                    backFrameVm.SetModel(backFrame);
                    return backFrameVm;
                case UndefinedMirrorSupportInfo undefined:
                    var undefinedVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<UndefinedMirrorSupportInfo>>>().Invoke();
                    undefinedVm.SetModel(undefined);
                    return undefinedVm;
                default:
                    throw new NotSupportedException($"{typeof(MirrorSupportInfo)} of Type : {support.GetType()} is not supported by {nameof(SupportsEditorViewModelsFactory)}");
            }
        }
        public IModelGetterViewModel<MirrorSupportInfo> CreateNew(MirrorSupportType supportType)
        {
            switch (supportType)
            {
                case MirrorSupportType.Undefined:
                    var undefined = serviceProvider.GetRequiredService<Func<IEditorViewModel<UndefinedMirrorSupportInfo>>>().Invoke();
                    undefined.SetModel(new());
                    return undefined;
                case MirrorSupportType.MirrorMultiSupport:
                    var multi = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorMultiSupports>>>().Invoke();
                    multi.SetModel(new());
                    return multi;
                case MirrorSupportType.MirrorVisibleFrameSupport:
                    var frame = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorVisibleFrameSupport>>>().Invoke();
                    frame.SetModel(new());
                    return frame;
                case MirrorSupportType.MirrorBackFrameSupport:
                    var backFrame = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorBackFrameSupport>>>().Invoke();
                    backFrame.SetModel(new());
                    return backFrame;
                default:
                    throw new NotSupportedException($"{typeof(MirrorSupportInfo)} of Type : {supportType} is not supported by {nameof(SupportsEditorViewModelsFactory)}");
            }
        }
    }

}
