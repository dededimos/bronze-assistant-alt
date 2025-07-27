using BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms;
using CommonInterfacesBronze;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class MirrorGlassDrawOptionsVm : BaseViewModel, IEditorViewModel<MirrorDrawOptions>
    {
        public MirrorGlassDrawOptionsVm(Func<DrawingPresentationOptionsVm> presOptionsVmFactory,
            Func<LineSandblastInfoDimensionOptionsVm> lineSandblastInfoVmFactory,
            Func<TwoLineSandblastInfoDimensionOptionsVm> twoLineSandblastInfoVmFactory,
            Func<HoledRectangleSandblastInfoDimensionOptionsVm> holedRectSandblastInfoVmFactory,
            Func<CircularSandblastInfoDimensionOptionsVm> circularSandblastInfoVmFactory,
            Func<RectangleDimensionsPresentationOptionsVm> rectDimOptionsVmFactory,
            Func<CircleDimensionsPresentationOptionsVm> circleDimOptionsVmFactory,
            Func<CapsuleDimensionsPresentationOptionsVm> capsuleDimOptionsVmFactory,
            Func<EllipseDimensionsPresentationOptionsVm> ellipseOptionsVmFactory,
            Func<EggDimensionsPresentationOptionsVm> eggOptionsVmFactory,
            Func<CircleSegmentDimensionsPresentationOptionsVm> circleSegmentOptionsVmFactory,
            Func<CircleQuadrantDimensionsPresentationOptionsVm> quadrantOptionsVmFactory,
            Func<RegularPolygonDimensionsPresentationOptionsVm> regularPolygonOptionsVmFactory)
        {
            SandblastPresentationOptions = presOptionsVmFactory.Invoke();
            LineSandblastDimensionsOptions = lineSandblastInfoVmFactory.Invoke();
            TwoLineSandblastDimensionsOptions = twoLineSandblastInfoVmFactory.Invoke();
            HoledRectangleSandblastDimensionsOptions = holedRectSandblastInfoVmFactory.Invoke();
            CircularSandblastDimensionsOptions = circularSandblastInfoVmFactory.Invoke();
            MirrorGlassPresentationOptions = presOptionsVmFactory.Invoke();
            RectangleMirrorsDimensionOptions = rectDimOptionsVmFactory.Invoke();
            CircleMirrorsDimensionOptions = circleDimOptionsVmFactory.Invoke();
            CapsuleMirrorsPresentationOptions = capsuleDimOptionsVmFactory.Invoke();
            EllipseMirrorsDimensionsOptions = ellipseOptionsVmFactory.Invoke();
            EggMirrorsDimensionsOptions = eggOptionsVmFactory.Invoke();
            CircleSegmentMirrorsDimensionsOptions = circleSegmentOptionsVmFactory.Invoke();
            CircleQuadrantMirrorsDimensionsOptions = quadrantOptionsVmFactory.Invoke();
            RegularPolygonMirrorsDimensionsOptions = regularPolygonOptionsVmFactory.Invoke();
            SetModel(this.model);
        }

        private MirrorDrawOptions model = MirrorDrawOptions.GetDefaultOptions(false);

        public DrawingPresentationOptionsVm SandblastPresentationOptions { get; }
        public LineSandblastInfoDimensionOptionsVm LineSandblastDimensionsOptions { get; }
        public TwoLineSandblastInfoDimensionOptionsVm TwoLineSandblastDimensionsOptions { get; }
        public HoledRectangleSandblastInfoDimensionOptionsVm HoledRectangleSandblastDimensionsOptions { get; }
        public CircularSandblastInfoDimensionOptionsVm CircularSandblastDimensionsOptions { get; }

        public DrawingPresentationOptionsVm MirrorGlassPresentationOptions { get; }

        public RectangleDimensionsPresentationOptionsVm RectangleMirrorsDimensionOptions { get; }
        public CircleDimensionsPresentationOptionsVm CircleMirrorsDimensionOptions { get; }
        public CapsuleDimensionsPresentationOptionsVm CapsuleMirrorsPresentationOptions { get; }
        public EllipseDimensionsPresentationOptionsVm EllipseMirrorsDimensionsOptions { get; }
        public EggDimensionsPresentationOptionsVm EggMirrorsDimensionsOptions { get; }
        public CircleSegmentDimensionsPresentationOptionsVm CircleSegmentMirrorsDimensionsOptions { get; }
        public CircleQuadrantDimensionsPresentationOptionsVm CircleQuadrantMirrorsDimensionsOptions { get; }
        public RegularPolygonDimensionsPresentationOptionsVm RegularPolygonMirrorsDimensionsOptions { get; }

        public MirrorDrawOptions CopyPropertiesToModel(MirrorDrawOptions model)
        {
            throw new NotSupportedException($"{nameof(MirrorGlassDrawOptionsVm)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public MirrorDrawOptions GetModel()
        {
            return model;
        }

        public void SetModel(MirrorDrawOptions model)
        {
            if (this.model != model) this.model = model;
            SandblastPresentationOptions.SetModel(model.SandblastPresentationOptions);
            LineSandblastDimensionsOptions.SetModel(model.LineSandblastDimensionsOptions);
            TwoLineSandblastDimensionsOptions.SetModel(model.TwoLineSandblastDimensionsOptions);
            HoledRectangleSandblastDimensionsOptions.SetModel(model.HoledRectangleSandblastDimensionsOptions);
            CircularSandblastDimensionsOptions.SetModel(model.CircularSandblastDimensionsOptions);
            MirrorGlassPresentationOptions.SetModel(model.GlassPresentationOptions);
            RectangleMirrorsDimensionOptions.SetModel(model.RectangleMirrorsDimensionOptions);
            CircleMirrorsDimensionOptions.SetModel(model.CircleMirrorsDimensionOptions);
            CapsuleMirrorsPresentationOptions.SetModel(model.CapsuleMirrorsPresentationOptions);
            EllipseMirrorsDimensionsOptions.SetModel(model.EllipseMirrorsDimensionsOptions);
            EggMirrorsDimensionsOptions.SetModel(model.EggMirrorsDimensionsOptions);
            CircleSegmentMirrorsDimensionsOptions.SetModel(model.CircleSegmentMirrorsDimensionsOptions);
            CircleQuadrantMirrorsDimensionsOptions.SetModel(model.CircleQuadrantMirrorsDimensionsOptions);
            RegularPolygonMirrorsDimensionsOptions.SetModel(model.RegularPolygonMirrorsDimensionsOptions);
            OnPropertyChanged("");
        }
    }

}
