using BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class EllipseDimensionsPresentationOptionsVm : ShapeDimensionsPresentationOptionsVm, IEditorViewModel<EllipseDimensionsPresentationOptions>
    {
        public EllipseDimensionsPresentationOptionsVm(Func<DimensionLineOptionsVm> lineOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsFactoryVm)
            : base(MirrorDrawOptions.GetDefaultDimensionOptions<EllipseDimensionsPresentationOptions>(false), lineOptionsVmFactory, presOptionsFactoryVm)
        {
            RadiusMajorLineOptions = lineOptionsVmFactory.Invoke();
            RadiusMinorLineOptions = lineOptionsVmFactory.Invoke();
            RadiusMajorPresentationOptions = presOptionsFactoryVm.Invoke();
            RadiusMinorPresentationOptions = presOptionsFactoryVm.Invoke();
            SetModel(EllipseOptions);
        }

        private EllipseDimensionsPresentationOptions EllipseOptions { get => (EllipseDimensionsPresentationOptions)model; }

        public bool ShowRadiusMajor
        {
            get => EllipseOptions.ShowRadiusMajor;
            set => SetProperty(EllipseOptions.ShowRadiusMajor, value, EllipseOptions, (m, v) => m.ShowRadiusMajor = v);
        }
        public bool ShowRadiusMinor
        {
            get => EllipseOptions.ShowRadiusMinor;
            set => SetProperty(EllipseOptions.ShowRadiusMinor, value, EllipseOptions, (m, v) => m.ShowRadiusMinor = v);
        }

        public EllipseRadiusDimensionPosition RadiusDimensionPosition 
        { 
            get => EllipseOptions.RadiusDimensionPosition;
            set => SetProperty(EllipseOptions.RadiusDimensionPosition, value, EllipseOptions, (m, v) => m.RadiusDimensionPosition = v);
        }

        public DimensionLineOptionsVm RadiusMajorLineOptions { get; }
        public DimensionLineOptionsVm RadiusMinorLineOptions { get; }
        public DrawingPresentationOptionsVm RadiusMajorPresentationOptions { get; }
        public DrawingPresentationOptionsVm RadiusMinorPresentationOptions { get; }

        public void SetModel(EllipseDimensionsPresentationOptions model)
        {
            SetBaseModel(model);
            RadiusMajorLineOptions.SetModel(model.RadiusMajorLineOptions);
            RadiusMinorLineOptions.SetModel(model.RadiusMinorLineOptions);
            RadiusMajorPresentationOptions.SetModel(model.RadiusMajorPresentationOptions);
            RadiusMinorPresentationOptions.SetModel(model.RadiusMinorPresentationOptions);
            OnPropertyChanged("");
        }

        public EllipseDimensionsPresentationOptions CopyPropertiesToModel(EllipseDimensionsPresentationOptions model)
        {
            throw new NotSupportedException($"{nameof(EllipseDimensionsPresentationOptions)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public EllipseDimensionsPresentationOptions GetModel()
        {
            return EllipseOptions;
        }
    }

}
