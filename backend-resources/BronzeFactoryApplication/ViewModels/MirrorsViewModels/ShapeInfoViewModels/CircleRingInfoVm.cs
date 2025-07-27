using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public partial class CircleRingInfoVm : ShapeInfoBaseVm, IEditorViewModel<CircleRingInfo>
    {
        protected CircleRingInfo CircleRing { get => (CircleRingInfo)model; }
        public CircleRingInfoVm()
        {
            model = CircleRingInfo.ZeroCircleRing();
        }

        public double Radius
        {
            get => CircleRing.Radius;
            set
            {
                if (SetProperty(CircleRing.Radius, value, CircleRing, (m, v) => CircleRing.Radius = v))
                {
                    OnPropertyChanged(nameof(Height));
                    OnPropertyChanged(nameof(Length));
                    OnPropertyChanged(nameof(DimensionsString));
                }
            }
        }
        public double Thickness
        {
            get => CircleRing.Thickness;
            set
            {
                if (SetProperty(CircleRing.Thickness, value, CircleRing, (m, v) => CircleRing.Thickness = v))
                {
                    OnPropertyChanged(nameof(DimensionsString));
                }
            }
        }

        public override void OnHeightChanged(double newHeight)
        {
            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(Radius));
            OnPropertyChanged(nameof(DimensionsString));
        }
        public override void OnLengthChanged(double newLength)
        {
            OnPropertyChanged(nameof(Height));
            OnPropertyChanged(nameof(Radius));
            OnPropertyChanged(nameof(DimensionsString));
        }

        public void SetModel(CircleRingInfo ring)
        {
            model = ring.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }

        public CircleRingInfo CopyPropertiesToModel(CircleRingInfo model)
        {
            model.Radius = Radius;
            model.Thickness = Thickness;
            model.LocationX = LocationX;
            model.LocationY = LocationY;

            return model;
        }

        public override CircleRingInfo GetModel()
        {
            return CircleRing.GetDeepClone();
        }
    }
}
