using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels.MirrorModuleWithElementInfoVms;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.SandblastViewModels;
using CommonInterfacesBronze;
using DocumentFormat.OpenXml.Math;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorExtras;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsModelsLibrary.Enums;
using static ShapesLibrary.Services.MathCalculations;

namespace BronzeFactoryApplication.Helpers.ViewModelFactories
{
    public class ModuleEditorViewModelsFactory : IAbstractFactory<IModelGetterViewModel<MirrorModuleInfo>, MirrorModuleInfo>
    {
        private readonly IServiceProvider serviceProvider;

        public ModuleEditorViewModelsFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IModelGetterViewModel<MirrorModuleInfo> Create(MirrorModuleInfo module)
        {
            switch (module)
            {
                case BluetoothModuleInfo bluetooth:
                    var bluetoothVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<BluetoothModuleInfo>>>().Invoke();
                    bluetoothVm.SetModel(bluetooth);
                    return bluetoothVm;
                case MagnifierSandblastedModuleInfo sandblastedMagn:
                    var sandblastedMagnVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<MagnifierSandblastedModuleInfo>>>().Invoke();
                    sandblastedMagnVm.SetModel(sandblastedMagn);
                    return sandblastedMagnVm;
                case MagnifierModuleInfo magn:
                    var magnVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<MagnifierModuleInfo>>>().Invoke();
                    magnVm.SetModel(magn);
                    return magnVm;
                case MirrorBackLidModuleInfo lid:
                    var lidVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorBackLidModuleInfo>>>().Invoke();
                    lidVm.SetModel(lid);
                    return lidVm;
                case MirrorLampModuleInfo lamp:
                    var lampVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorLampModuleInfo>>>().Invoke();
                    lampVm.SetModel(lamp);
                    return lampVm;
                case ResistancePadModuleInfo resistance:
                    var resistanceVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<ResistancePadModuleInfo>>>().Invoke();
                    resistanceVm.SetModel(resistance);
                    return resistanceVm;
                case RoundedCornersModuleInfo corners:
                    var cornersVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<RoundedCornersModuleInfo>>>().Invoke();
                    cornersVm.SetModel(corners);
                    return cornersVm;
                case ScreenModuleInfo screen:
                    var screenVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<ScreenModuleInfo>>>().Invoke();
                    screenVm.SetModel(screen);
                    return screenVm;
                case TouchButtonModuleInfo touch:
                    var touchVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<TouchButtonModuleInfo>>>().Invoke();
                    touchVm.SetModel(touch);
                    return touchVm;
                case TransformerModuleInfo transformer:
                    var transformerVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<TransformerModuleInfo>>>().Invoke();
                    transformerVm.SetModel(transformer);
                    return transformerVm;
                case MirrorProcessModuleInfo process:
                    var processVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorProcessModuleInfo>>>().Invoke();
                    processVm.SetModel(process);
                    return processVm;
                case UndefinedMirrorModuleInfo undefined:
                    var undefinedVm = serviceProvider.GetRequiredService<Func<IEditorViewModel<UndefinedMirrorModuleInfo>>>().Invoke();
                    undefinedVm.SetModel(undefined);
                    return undefinedVm;
                default:
                    throw new NotSupportedException($"{nameof(MirrorModuleInfo)} of Type : {module.GetType().Name} is not supported by {this.GetType().Name}");
            }
        }
        public IModelGetterViewModel<MirrorModuleInfo> CreateNew(MirrorModuleType moduleType)
        {
            switch (moduleType)
            {
                case MirrorModuleType.Undefined:
                    var undefined = serviceProvider.GetRequiredService<Func<IEditorViewModel<UndefinedMirrorModuleInfo>>>().Invoke();
                    undefined.SetModel(new());
                    return undefined;
                case MirrorModuleType.BluetoothModuleType:
                    var bluetooth = serviceProvider.GetRequiredService<Func<IEditorViewModel<BluetoothModuleInfo>>>().Invoke();
                    bluetooth.SetModel(new());
                    return bluetooth;
                case MirrorModuleType.MagnifierSandblastedModuleType:
                    var sandMagn = serviceProvider.GetRequiredService<Func<IEditorViewModel<MagnifierSandblastedModuleInfo>>>().Invoke();
                    sandMagn.SetModel(new());
                    return sandMagn;
                case MirrorModuleType.MagnifierModuleType:
                    var magn = serviceProvider.GetRequiredService<Func<IEditorViewModel<MagnifierModuleInfo>>>().Invoke();
                    magn.SetModel(new());
                    return magn;
                case MirrorModuleType.MirrorBackLidModuleType:
                    var lid = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorBackLidModuleInfo>>>().Invoke();
                    lid.SetModel(new());
                    return lid;
                case MirrorModuleType.MirrorLampModuleType:
                    var lamp = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorLampModuleInfo>>>().Invoke();
                    lamp.SetModel(new());
                    return lamp;
                case MirrorModuleType.ResistancePadModuleType:
                    var resistance = serviceProvider.GetRequiredService<Func<IEditorViewModel<ResistancePadModuleInfo>>>().Invoke();
                    resistance.SetModel(new());
                    return resistance;
                case MirrorModuleType.RoundedCornersModuleType:
                    var roundedCorners = serviceProvider.GetRequiredService<Func<IEditorViewModel<RoundedCornersModuleInfo>>>().Invoke();
                    roundedCorners.SetModel(new());
                    return roundedCorners;
                case MirrorModuleType.ScreenModuleType:
                    var screen = serviceProvider.GetRequiredService<Func<IEditorViewModel<ScreenModuleInfo>>>().Invoke();
                    screen.SetModel(new());
                    return screen;
                case MirrorModuleType.TouchButtonModuleType:
                    var touch = serviceProvider.GetRequiredService<Func<IEditorViewModel<TouchButtonModuleInfo>>>().Invoke();
                    touch.SetModel(new());
                    return touch;
                case MirrorModuleType.TransformerModuleType:
                    var transformer = serviceProvider.GetRequiredService<Func<IEditorViewModel<TransformerModuleInfo>>>().Invoke();
                    transformer.SetModel(new());
                    return transformer;
                case MirrorModuleType.ProcessModuleType:
                    var process = serviceProvider.GetRequiredService<Func<IEditorViewModel<MirrorProcessModuleInfo>>>().Invoke();
                    process.SetModel(new());
                    return process;
                default:
                    throw new NotSupportedException($"{typeof(MirrorModuleInfo)} of Type : {moduleType} is not supported by {this.GetType().Name}");
            }
        }
    }

}
