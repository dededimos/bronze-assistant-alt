using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class EggDimensionsPresentationOptionsVm : ShapeDimensionsPresentationOptionsVm, IEditorViewModel<EggDimensionsPresentationOptions>
    {
        public EggDimensionsPresentationOptionsVm(Func<DimensionLineOptionsVm> lineOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsFactoryVm)
            : base(MirrorDrawOptions.GetDefaultDimensionOptions<EggDimensionsPresentationOptions>(false), lineOptionsVmFactory, presOptionsFactoryVm)
        {
            CircleRadiusLineOptions = lineOptionsVmFactory.Invoke();
            EllipseRadiusLineOptions = lineOptionsVmFactory.Invoke();
            CircleRadiusPresentationOptions = presOptionsFactoryVm.Invoke();
            EllipseRadiusPresentationOptions = presOptionsFactoryVm.Invoke();
            SetModel(EggOptions);
        }

        private EggDimensionsPresentationOptions EggOptions { get => (EggDimensionsPresentationOptions)model; }

        public bool ShowCircleRadius
        {
            get => EggOptions.ShowCircleRadius;
            set => SetProperty(EggOptions.ShowCircleRadius, value, EggOptions, (m, v) => m.ShowCircleRadius = v);
        }
        public bool ShowEllipseRadius
        {
            get => EggOptions.ShowEllipseRadius;
            set => SetProperty(EggOptions.ShowEllipseRadius, value, EggOptions, (m, v) => m.ShowEllipseRadius = v);
        }

        public EggCircleRadiusDimensionPosition CircleRadiusDimensionsPosition
        {
            get => EggOptions.CircleRadiusDimensionsPosition;
            set => SetProperty(EggOptions.CircleRadiusDimensionsPosition, value, EggOptions, (m, v) => m.CircleRadiusDimensionsPosition = v);
        }

        public DimensionLineOptionsVm EllipseRadiusLineOptions { get; }
        public DimensionLineOptionsVm CircleRadiusLineOptions { get; }
        public DrawingPresentationOptionsVm EllipseRadiusPresentationOptions { get; }
        public DrawingPresentationOptionsVm CircleRadiusPresentationOptions { get; }

        public void SetModel(EggDimensionsPresentationOptions model)
        {
            SetBaseModel(model);

            EllipseRadiusLineOptions.SetModel(model.EllipseRadiusLineOptions);
            CircleRadiusLineOptions.SetModel(model.CircleRadiusLineOptions);
            EllipseRadiusPresentationOptions.SetModel(model.EllipseRadiusPresentationOptions);
            CircleRadiusPresentationOptions.SetModel(model.CircleRadiusPresentationOptions);

            OnPropertyChanged("");
        }

        public EggDimensionsPresentationOptions CopyPropertiesToModel(EggDimensionsPresentationOptions model)
        {
            throw new NotSupportedException($"{nameof(EggDimensionsPresentationOptions)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public EggDimensionsPresentationOptions GetModel()
        {
            return EggOptions;
        }
    }
}
