using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;
using static ShapesLibrary.Services.MathCalculations;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public partial class CircleSegmentInfoVm : ShapeInfoBaseVm , IEditorViewModel<CircleSegmentInfo>
    {
        protected CircleSegmentInfo Segment { get => (CircleSegmentInfo)model; }
        public CircleSegmentInfoVm()
        {
            model = CircleSegmentInfo.CircleSegmentZero();
        }

        public double Sagitta
        {
            get => Segment.Sagitta;
            set
            {
                if (SetProperty(Segment.Sagitta, value, Segment, (m, v) => Segment.Sagitta = v))
                {
                    OnPropertyChanged("");
                }
            }
        }
        public double ChordLength
        {
            get => Segment.ChordLength;
            set
            {
                if (SetProperty(Segment.ChordLength, value, Segment, (m, v) => Segment.ChordLength = v))
                {
                    OnPropertyChanged("");
                }
            }
        }

        public override void OnHeightChanged(double newHeight)
        {
            OnPropertyChanged("");
        }
        public override void OnLengthChanged(double newLength)
        {
            OnPropertyChanged("");
        }
        public void SetModel(CircleSegmentInfo segment)
        {
            model = segment.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }

        public CircleSegmentInfo CopyPropertiesToModel(CircleSegmentInfo model)
        {
            throw new NotImplementedException();
        }

        public override CircleSegmentInfo GetModel()
        {
            return Segment.GetDeepClone();
        }
    }
}
