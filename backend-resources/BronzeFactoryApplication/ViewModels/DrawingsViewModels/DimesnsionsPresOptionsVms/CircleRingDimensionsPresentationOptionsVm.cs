using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class CircleRingDimensionsPresentationOptionsVm : ShapeDimensionsPresentationOptionsVm, IEditorViewModel<CircleRingDimensionsPresentationOptions>
    {
        public CircleRingDimensionsPresentationOptionsVm(Func<DimensionLineOptionsVm> lineOptionsVmFactory,
                                                         Func<DrawingPresentationOptionsVm> presOptionsVmFactory)
            : base(MirrorDrawOptions.GetDefaultDimensionOptions<CircleRingDimensionsPresentationOptions>(false), lineOptionsVmFactory, presOptionsVmFactory)
        {
            ThicknessLineOptions = lineOptionsVmFactory.Invoke();
            ThicknessPresentationOptions = presOptionsVmFactory.Invoke();
            SetModel(CircleRingOptions);
        }

        private CircleRingDimensionsPresentationOptions CircleRingOptions { get => (CircleRingDimensionsPresentationOptions)model; }

        public bool ShowThickness
        {
            get => CircleRingOptions.ShowThickness;
            set => SetProperty(CircleRingOptions.ShowThickness, value, CircleRingOptions, (m, v) => m.ShowThickness = v);
        }

        public DimensionLineOptionsVm ThicknessLineOptions { get; }
        public DrawingPresentationOptionsVm ThicknessPresentationOptions { get; }


        public CircleRingDimensionsPresentationOptions CopyPropertiesToModel(CircleRingDimensionsPresentationOptions model)
        {
            throw new NotSupportedException($"{nameof(CircleRingDimensionsPresentationOptionsVm)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public CircleRingDimensionsPresentationOptions GetModel()
        {
            return CircleRingOptions;
        }

        public void SetModel(CircleRingDimensionsPresentationOptions model)
        {
            SetBaseModel(model);
            ThicknessLineOptions.SetModel(model.ThicknessLineOptions);
            ThicknessPresentationOptions.SetModel(model.ThicknessPresentationOptions);
            OnPropertyChanged("");
        }
    }

}
