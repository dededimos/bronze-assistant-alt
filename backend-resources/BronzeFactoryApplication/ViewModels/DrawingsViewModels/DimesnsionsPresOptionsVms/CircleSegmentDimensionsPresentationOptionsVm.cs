using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class CircleSegmentDimensionsPresentationOptionsVm : ShapeDimensionsPresentationOptionsVm, IEditorViewModel<CircleSegmentDimensionsPresentationOptions>
    {
        public CircleSegmentDimensionsPresentationOptionsVm(Func<DimensionLineOptionsVm> lineOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsFactoryVm)
            : base(MirrorDrawOptions.GetDefaultDimensionOptions<CircleSegmentDimensionsPresentationOptions>(false), lineOptionsVmFactory, presOptionsFactoryVm)
        {
            RadiusLineOptions = lineOptionsVmFactory.Invoke();
            DiameterLineOptions = lineOptionsVmFactory.Invoke();
            ChordLengthLineOptions = lineOptionsVmFactory.Invoke();
            RadiusPresentationOptions = presOptionsFactoryVm.Invoke();
            DiameterPresentationOptions = presOptionsFactoryVm.Invoke();
            ChordLengthPresentationOptions = presOptionsFactoryVm.Invoke();
            CircleExtensionPresentationOptions = presOptionsFactoryVm.Invoke();

            SetModel(SegmentOptions);
        }

        private CircleSegmentDimensionsPresentationOptions SegmentOptions { get => (CircleSegmentDimensionsPresentationOptions)model; }

        public bool ShowCircleRadius
        {
            get => SegmentOptions.ShowCircleRadius;
            set
            {
                if (SetProperty(SegmentOptions.ShowCircleRadius, value, SegmentOptions, (m, v) => m.ShowCircleRadius = v))
                {
                    OnPropertyChanged(nameof(IsShowDiameterEnabled));
                }
            }
        }
        public bool ShowDiameter
        {
            get => SegmentOptions.ShowDiameter;
            set => SetProperty(SegmentOptions.ShowDiameter, value, SegmentOptions, (m, v) => m.ShowDiameter = v);
        }
        public bool IsShowDiameterEnabled { get; set; }

        public bool ShowChordLength
        {
            get => SegmentOptions.ShowChordLength;
            set => SetProperty(SegmentOptions.ShowChordLength, value, SegmentOptions, (m, v) => m.ShowChordLength = v);
        }

        public double RadiusDiameterMarginFromShape
        {
            get => SegmentOptions.RadiusDiameterMarginFromShape;
            set => SetProperty(SegmentOptions.RadiusDiameterMarginFromShape, value, SegmentOptions, (m, v) => m.RadiusDiameterMarginFromShape = v);
        }

        public double ChordLengthMarginFromShape
        {
            get => SegmentOptions.ChordLengthMarginFromShape;
            set => SetProperty(SegmentOptions.ChordLengthMarginFromShape, value, SegmentOptions, (m, v) => m.ChordLengthMarginFromShape = v);
        }

        public DimensionLineOptionsVm RadiusLineOptions { get; }
        public DimensionLineOptionsVm DiameterLineOptions { get; }
        public DimensionLineOptionsVm ChordLengthLineOptions { get; }
        public DrawingPresentationOptionsVm RadiusPresentationOptions { get; }
        public DrawingPresentationOptionsVm DiameterPresentationOptions { get; }
        public DrawingPresentationOptionsVm ChordLengthPresentationOptions { get; }
        public DrawingPresentationOptionsVm CircleExtensionPresentationOptions { get; }

        public void SetModel(CircleSegmentDimensionsPresentationOptions model)
        {
            SetBaseModel(model);

            RadiusLineOptions.SetModel(model.RadiusLineOptions);
            DiameterLineOptions.SetModel(model.DiameterLineOptions);
            ChordLengthLineOptions.SetModel(model.ChordLengthLineOptions);
            RadiusPresentationOptions.SetModel(model.RadiusPresentationOptions);
            DiameterPresentationOptions.SetModel(model.DiameterPresentationOptions);
            ChordLengthPresentationOptions.SetModel(model.ChordLengthPresentationOptions);
            CircleExtensionPresentationOptions.SetModel(model.CircleExtensionPresentationOptions);

            OnPropertyChanged("");
        }

        public CircleSegmentDimensionsPresentationOptions CopyPropertiesToModel(CircleSegmentDimensionsPresentationOptions model)
        {
            throw new NotSupportedException($"{nameof(CircleSegmentDimensionsPresentationOptions)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public CircleSegmentDimensionsPresentationOptions GetModel()
        {
            return SegmentOptions;
        }
    }
}
