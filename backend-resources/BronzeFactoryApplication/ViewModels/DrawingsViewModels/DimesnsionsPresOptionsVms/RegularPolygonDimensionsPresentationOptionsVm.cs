using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class RegularPolygonDimensionsPresentationOptionsVm : ShapeDimensionsPresentationOptionsVm, IEditorViewModel<RegularPolygonDimensionsPresentationOptions>
    {
        public RegularPolygonDimensionsPresentationOptionsVm(Func<DimensionLineOptionsVm> lineOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsFactoryVm)
            : base(MirrorDrawOptions.GetDefaultDimensionOptions<RegularPolygonDimensionsPresentationOptions>(false), lineOptionsVmFactory, presOptionsFactoryVm)
        {
            RadiusLineOptions = lineOptionsVmFactory.Invoke();
            RadiusPresentationOptions = presOptionsFactoryVm.Invoke();
            HelpCirclePresentationOptions = presOptionsFactoryVm.Invoke();
            SetModel(PolygonOptions);
        }

        private RegularPolygonDimensionsPresentationOptions PolygonOptions { get => (RegularPolygonDimensionsPresentationOptions)model; }

        public bool ShowRadius
        {
            get => PolygonOptions.ShowRadius;
            set => SetProperty(PolygonOptions.ShowRadius, value, PolygonOptions, (m, v) => m.ShowRadius = v);
        }
        public double RadiusMarginFromShape
        {
            get => PolygonOptions.RadiusMarginFromShape;
            set => SetProperty(PolygonOptions.RadiusMarginFromShape, value, PolygonOptions, (m, v) => m.RadiusMarginFromShape = v);
        }

        public DimensionLineOptionsVm RadiusLineOptions { get; }
        public DrawingPresentationOptionsVm RadiusPresentationOptions { get; }
        public DrawingPresentationOptionsVm HelpCirclePresentationOptions { get; }

        public void SetModel(RegularPolygonDimensionsPresentationOptions model)
        {
            SetBaseModel(model);

            RadiusLineOptions.SetModel(model.RadiusLineOptions);
            RadiusPresentationOptions.SetModel(model.RadiusPresentationOptions);
            HelpCirclePresentationOptions.SetModel(model.HelpCirclePresentationOptions);

            OnPropertyChanged("");
        }

        public RegularPolygonDimensionsPresentationOptions CopyPropertiesToModel(RegularPolygonDimensionsPresentationOptions model)
        {
            throw new NotSupportedException($"{nameof(RegularPolygonDimensionsPresentationOptions)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public RegularPolygonDimensionsPresentationOptions GetModel()
        {
            return PolygonOptions;
        }
    }

}
