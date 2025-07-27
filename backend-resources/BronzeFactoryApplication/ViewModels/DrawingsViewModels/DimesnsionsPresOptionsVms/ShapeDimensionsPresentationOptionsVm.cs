using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class ShapeDimensionsPresentationOptionsVm : BaseViewModel
    {
        public ShapeDimensionsPresentationOptionsVm(ShapeDimensionsPresentationOptions model,
            Func<DimensionLineOptionsVm> dimLineOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsVmFactory)
        {
            this.model = model;
            HeightLineOptions = dimLineOptionsVmFactory.Invoke();
            HeightPresentationOptions = presOptionsVmFactory.Invoke();
            LengthLineOptions = dimLineOptionsVmFactory.Invoke();
            LengthPresentationOptions = presOptionsVmFactory.Invoke();
            //SetBaseModel(model); (called by derived classes not needed)
        }
        protected ShapeDimensionsPresentationOptions model;

        public bool ShowHeightDimension
        {
            get => model.ShowHeight;
            set
            {
                if (SetProperty(model.ShowHeight, value, model, (m, v) => m.ShowHeight = v))
                {
                    OnShowHeightDimensionChanged();
                }
            }
        }
        public virtual void OnShowHeightDimensionChanged()
        {
            return;
        }

        public RectangleHeightDimensionPosition HeightPosition
        {
            get => model.HeightPosition;
            set => SetProperty(model.HeightPosition, value, model, (m, v) => m.HeightPosition = v);
        }

        public double HeightMarginFromShape
        {
            get => model.HeightMarginFromShape;
            set => SetProperty(model.HeightMarginFromShape, value, model, (m, v) => m.HeightMarginFromShape = v);
        }

        public DimensionLineOptionsVm HeightLineOptions { get; }
        public DrawingPresentationOptionsVm HeightPresentationOptions { get; }

        public bool ShowLengthDimension
        {
            get => model.ShowLength;
            set
            {
                if (SetProperty(model.ShowLength, value, model, (m, v) => m.ShowLength = v))
                {
                    OnShowLengthDimensionChanged();
                }
            }
        }
        public virtual void OnShowLengthDimensionChanged()
        {
            return;
        }

        public double LengthMarginFromShape
        {
            get => model.LengthMarginFromShape;
            set => SetProperty(model.LengthMarginFromShape, value, model, (m, v) => m.LengthMarginFromShape = v);
        }

        public RectangleLengthDimensionPosition LengthPosition
        {
            get => model.LengthPosition;
            set => SetProperty(model.LengthPosition, value, model, (m, v) => m.LengthPosition = v);
        }

        public DimensionLineOptionsVm LengthLineOptions { get; }
        public DrawingPresentationOptionsVm LengthPresentationOptions { get; }

        protected void SetBaseModel(ShapeDimensionsPresentationOptions model)
        {
            if (this.model != model) this.model = model;
            HeightLineOptions.SetModel(model.HeightLineOptions);
            HeightPresentationOptions.SetModel(model.HeightPresentationOptions);
            LengthLineOptions.SetModel(model.LengthLineOptions);
            LengthPresentationOptions.SetModel(model.LengthPresentationOptions);
        }

    }


}
