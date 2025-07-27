using CommonInterfacesBronze;
using DrawingLibrary.Models.PresentationOptions;
using ShapesLibrary.Services;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels
{
    public partial class DimensionLineOptionsVm : BaseViewModel, IEditorViewModel<DimensionLineOptions>
    {
        private DimensionLineOptions model = new();

        public double ArrowThickness
        {
            get => model.ArrowThickness;
            set => SetProperty(model.ArrowThickness,value,model,(m,v)=> m.ArrowThickness = v);
        }

        public double ArrowLength
        {
            get => model.ArrowLength;
            set => SetProperty(model.ArrowLength, value, model, (m, v) => m.ArrowLength = v);
        }

        public bool IncludeStartArrow
        {
            get => model.IncludeStartArrow;
            set => SetProperty(model.IncludeStartArrow, value, model, (m, v) => m.IncludeStartArrow = v);
        }
        public bool IsIncludeStartArrowEnabled { get => !IncludeEndArrow; }

        public bool IncludeEndArrow
        {
            get => model.IncludeEndArrow;
            set
            {
                if (SetProperty(model.IncludeEndArrow, value, model, (m, v) => m.IncludeEndArrow = v))
                {
                    if (model.IncludeEndArrow)
                    {
                        //Change the Start Arrow to true whenever the End Arrow is Included
                        //This way always a Dimension with end Arrow has also starting Arrow
                        IncludeStartArrow = true;
                    }
                    OnPropertyChanged(nameof(IsIncludeStartArrowEnabled));
                }
            }
        }
        public bool IsIncludeEndArrowEnabled { get => !IsTwoLinesDimension; }

        public string DimensionUnitString
        {
            get => model.DimensionUnitString;
            set => SetProperty(model.DimensionUnitString, value, model, (m, v) => m.DimensionUnitString = v);
        }

        public string DimensionTextPrefix
        {
            get => model.DimensionTextPrefix;
            set => SetProperty(model.DimensionTextPrefix, value, model, (m, v) => m.DimensionTextPrefix = v);
        }

        public double TextMarginFromDimension
        {
            get => model.TextMarginFromDimension;
            set => SetProperty(model.TextMarginFromDimension, value, model, (m, v) => m.TextMarginFromDimension = v);
        }

        public double OneEndLineLength
        {
            get => model.OneEndLineLength;
            set => SetProperty(model.OneEndLineLength, value, model, (m, v) => m.OneEndLineLength = v);
        }

        public double StartRotationAngle
        {
            get => MathCalculations.VariousMath.RadiansToDegrees(model.StartRotationAngle);
            set => SetProperty(model.StartRotationAngle, value, model, (m, v) => m.StartRotationAngle = MathCalculations.VariousMath.DegreesToRadians(v));
        }

        public int DimensionValueRoundingDecimals
        {
            get => model.DimensionValueRoundingDecimals;
            set => SetProperty(model.DimensionValueRoundingDecimals, value, model, (m, v) => m.DimensionValueRoundingDecimals = v);
        }

        public bool IsTwoLinesDimension
        {
            get => model.IsTwoLinesDimension;
            set 
            {
                if (SetProperty(model.IsTwoLinesDimension, value, model, (m, v) => m.IsTwoLinesDimension = v))
                {
                    if (model.IsTwoLinesDimension)
                    {
                        IncludeEndArrow = true;
                    }
                    OnPropertyChanged(nameof(IsIncludeEndArrowEnabled));
                }
            }
        }

        public double TwoLinesDimensionArrowLengthThresholdMultiplier
        {
            get => model.TwoLinesDimensionArrowLengthThresholdMultiplier;
            set => SetProperty(model.TwoLinesDimensionArrowLengthThresholdMultiplier, value, model, (m, v) => m.TwoLinesDimensionArrowLengthThresholdMultiplier = v);
        }
        public bool CenterTextOnTwoLineDimension
        {
            get => model.CenterTextOnTwoLineDimension;
            set => SetProperty(model.CenterTextOnTwoLineDimension, value, model, (m, v) => m.CenterTextOnTwoLineDimension = v);
        }

        public DimensionLineOptions CopyPropertiesToModel(DimensionLineOptions model)
        {
            throw new NotSupportedException($"{nameof(DimensionLineOptionsVm)} does not support {nameof(CopyPropertiesToModel)} method");
        }

        public DimensionLineOptions GetModel()
        {
            return model;
        }

        public void SetModel(DimensionLineOptions model)
        {
            this.model = model;
            OnPropertyChanged("");
        }
    }


}
