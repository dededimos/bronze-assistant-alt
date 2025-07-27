using BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms;
using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class CircleQuadrantDimensionsPresentationOptionsVm : ShapeDimensionsPresentationOptionsVm, IEditorViewModel<CircleQuadrantDimensionsPresentationOptions>
    {
        public CircleQuadrantDimensionsPresentationOptionsVm(Func<DimensionLineOptionsVm> lineOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsFactoryVm)
            : base(MirrorDrawOptions.GetDefaultDimensionOptions<CircleQuadrantDimensionsPresentationOptions>(false), lineOptionsVmFactory, presOptionsFactoryVm)
        {
            RadiusLineOptions = lineOptionsVmFactory.Invoke();
            RadiusPresentationOptions = presOptionsFactoryVm.Invoke();

            SetModel(QuadrantOptions);
        }

        private CircleQuadrantDimensionsPresentationOptions QuadrantOptions { get => (CircleQuadrantDimensionsPresentationOptions)model; }

        public bool ShowRadiusDimension
        {
            get => QuadrantOptions.ShowRadius;
            set => SetProperty(QuadrantOptions.ShowRadius, value, QuadrantOptions, (m, v) => m.ShowRadius = v);
        }
        public double RadiusMarginFromShape
        {
            get => QuadrantOptions.RadiusMarginFromShape;
            set => SetProperty(QuadrantOptions.RadiusMarginFromShape, value, QuadrantOptions, (m, v) => m.RadiusMarginFromShape = v);
        }

        public DimensionLineOptionsVm RadiusLineOptions { get; }
        public DrawingPresentationOptionsVm RadiusPresentationOptions { get; }

        public void SetModel(CircleQuadrantDimensionsPresentationOptions model)
        {
            SetBaseModel(model);

            RadiusLineOptions.SetModel(model.RadiusLineOptions);
            RadiusPresentationOptions.SetModel(model.RadiusPresentationOptions);

            OnPropertyChanged("");
        }

        public CircleQuadrantDimensionsPresentationOptions CopyPropertiesToModel(CircleQuadrantDimensionsPresentationOptions model)
        {
            throw new NotSupportedException($"{nameof(CircleQuadrantDimensionsPresentationOptions)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public CircleQuadrantDimensionsPresentationOptions GetModel()
        {
            return QuadrantOptions;
        }
    }

}
