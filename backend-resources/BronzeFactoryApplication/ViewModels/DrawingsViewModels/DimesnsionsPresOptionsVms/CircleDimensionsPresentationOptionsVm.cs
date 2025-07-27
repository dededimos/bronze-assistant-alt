using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class CircleDimensionsPresentationOptionsVm : ShapeDimensionsPresentationOptionsVm, IEditorViewModel<CircleDimensionsPresentationOptions>
    {
        public CircleDimensionsPresentationOptionsVm(Func<DimensionLineOptionsVm> lineOptionsVmFactory,
                                                     Func<DrawingPresentationOptionsVm> presOptionsFactoryVm)
            : base(MirrorDrawOptions.GetDefaultDimensionOptions<CircleDimensionsPresentationOptions>(false), lineOptionsVmFactory, presOptionsFactoryVm)
        {
            DiameterLineOptions = lineOptionsVmFactory.Invoke();
            RadiusLineOptions = lineOptionsVmFactory.Invoke();
            DiameterRadiusPresentationOptions = presOptionsFactoryVm.Invoke();
            SetModel(CircleOptions);
        }

        private CircleDimensionsPresentationOptions CircleOptions { get => (CircleDimensionsPresentationOptions)model; }

        public bool ShowDiameter
        {
            get => CircleOptions.ShowDiameter;
            set => SetProperty(CircleOptions.ShowDiameter, value, CircleOptions, (m, v) => m.ShowDiameter = v);
        }
        
        public bool ShowRadius
        {
            get => CircleOptions.ShowRadius;
            set
            {
                if (SetProperty(CircleOptions.ShowRadius, value, CircleOptions, (m, v) => m.ShowRadius = v))
                {
                    
                }
            }
        }

        public bool ShowDiameterRadiusInsideShape
        {
            get => CircleOptions.ShowDiameterRadiusInsideShape;
            set => SetProperty(CircleOptions.ShowDiameterRadiusInsideShape, value, CircleOptions, (m, v) => m.ShowDiameterRadiusInsideShape = v);
        }

        public double RadiusDiameterPositionAngleRadians
        {
            get => CircleOptions.RadiusDiameterPositionAngleRadians;
            set => SetProperty(CircleOptions.RadiusDiameterPositionAngleRadians, value, CircleOptions, (m, v) => m.RadiusDiameterPositionAngleRadians = v);
        }

        public double DiameterRadiusMarginFromShape
        {
            get => CircleOptions.DiameterRadiusMarginFromShape;
            set => SetProperty(CircleOptions.DiameterRadiusMarginFromShape, value, CircleOptions, (m, v) => m.DiameterRadiusMarginFromShape = v);
        }

        public DimensionLineOptionsVm DiameterLineOptions { get; }
        public DimensionLineOptionsVm RadiusLineOptions { get; }
        public DrawingPresentationOptionsVm DiameterRadiusPresentationOptions { get; }


        public CircleDimensionsPresentationOptions CopyPropertiesToModel(CircleDimensionsPresentationOptions model)
        {
            throw new NotSupportedException($"{nameof(CircleDimensionsPresentationOptionsVm)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public CircleDimensionsPresentationOptions GetModel()
        {
            return CircleOptions;
        }

        public void SetModel(CircleDimensionsPresentationOptions model)
        {
            SetBaseModel(model);
            DiameterLineOptions.SetModel(model.DiameterLineOptions);
            RadiusLineOptions.SetModel(model.RadiusLineOptions);
            DiameterRadiusPresentationOptions.SetModel(model.DiameterRadiusPresentationOptions);
            OnPropertyChanged("");
        }
    }

}
