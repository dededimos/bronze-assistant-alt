using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class CapsuleDimensionsPresentationOptionsVm : ShapeDimensionsPresentationOptionsVm, IEditorViewModel<CapsuleDimensionsPresentationOptions>
    {
        public CapsuleDimensionsPresentationOptionsVm(Func<DimensionLineOptionsVm> lineOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsFactoryVm)
            : base(MirrorDrawOptions.GetDefaultDimensionOptions<CapsuleDimensionsPresentationOptions>(false), lineOptionsVmFactory, presOptionsFactoryVm)
        {
            DiameterLineOptions = lineOptionsVmFactory.Invoke();
            RadiusLineOptions = lineOptionsVmFactory.Invoke();
            DiameterRadiusPresentationOptions = presOptionsFactoryVm.Invoke();
            RectangleDimensionLineOptions = lineOptionsVmFactory.Invoke();
            RectangleDimensionPresentationOptions = presOptionsFactoryVm.Invoke();
            SetModel(CapsuleOptions);
        }

        private CapsuleDimensionsPresentationOptions CapsuleOptions { get => (CapsuleDimensionsPresentationOptions)model; }

        public bool ShowDiameter
        {
            get => CapsuleOptions.ShowDiameter;
            set => SetProperty(CapsuleOptions.ShowDiameter, value, CapsuleOptions, (m, v) => m.ShowDiameter = v);
        }
        public bool IsShowDiameterEnabled { get => !CapsuleOptions.ShowRadius; }

        public bool ShowRadius
        {
            get => CapsuleOptions.ShowRadius;
            set
            {
                if (SetProperty(CapsuleOptions.ShowRadius, value, CapsuleOptions, (m, v) => m.ShowRadius = v))
                {
                    OnPropertyChanged(nameof(IsShowDiameterEnabled));
                }
            }
        }

        public CapsuleRadiusDimensionPosition DiameterRadiusDimensionPosition
        {
            get => CapsuleOptions.DiameterRadiusDimensionPosition;
            set => SetProperty(CapsuleOptions.DiameterRadiusDimensionPosition, value, CapsuleOptions, (m, v) => m.DiameterRadiusDimensionPosition = v);
        }

        public double DiameterRadiusMarginFromShape
        {
            get => CapsuleOptions.DiameterRadiusMarginFromShape;
            set => SetProperty(CapsuleOptions.DiameterRadiusMarginFromShape, value, CapsuleOptions, (m, v) => m.DiameterRadiusMarginFromShape = v);
        }

        public DimensionLineOptionsVm DiameterLineOptions { get; }
        public DimensionLineOptionsVm RadiusLineOptions { get; }
        public DrawingPresentationOptionsVm DiameterRadiusPresentationOptions { get; }

        public bool ShowRectangleDimension
        {
            get => CapsuleOptions.ShowRectangleDimension;
            set => SetProperty(CapsuleOptions.ShowRectangleDimension, value, CapsuleOptions, (m, v) => m.ShowRectangleDimension = v);
        }

        public double RectangleDimensionMarginFromShape
        {
            get => CapsuleOptions.RectangleDimensionMarginFromShape;
            set => SetProperty(CapsuleOptions.RectangleDimensionMarginFromShape, value, CapsuleOptions, (m, v) => m.RectangleDimensionMarginFromShape = v);
        }

        public CapsuleRectangleDimensionPosition RectangleDimensionPosition
        {
            get => CapsuleOptions.RectangleDimensionPosition;
            set => SetProperty(CapsuleOptions.RectangleDimensionPosition, value, CapsuleOptions, (m, v) => m.RectangleDimensionPosition = v);
        }

        public DimensionLineOptionsVm RectangleDimensionLineOptions { get; }
        public DrawingPresentationOptionsVm RectangleDimensionPresentationOptions { get; }

        public void SetModel(CapsuleDimensionsPresentationOptions model)
        {
            SetBaseModel(model);
            DiameterLineOptions.SetModel(model.DiameterLineOptions);
            RadiusLineOptions.SetModel(model.RadiusLineOptions);
            DiameterRadiusPresentationOptions.SetModel(model.DiameterRadiusPresentationOptions);
            RectangleDimensionLineOptions.SetModel(model.RectangleDimensionLineOptions);
            RectangleDimensionPresentationOptions.SetModel(model.RectangleDimensionPresentationOptions);
            OnPropertyChanged("");
        }

        public CapsuleDimensionsPresentationOptions CopyPropertiesToModel(CapsuleDimensionsPresentationOptions model)
        {
            throw new NotSupportedException($"{nameof(CapsuleDimensionsPresentationOptionsVm)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public CapsuleDimensionsPresentationOptions GetModel()
        {
            return CapsuleOptions;
        }
    }
}
