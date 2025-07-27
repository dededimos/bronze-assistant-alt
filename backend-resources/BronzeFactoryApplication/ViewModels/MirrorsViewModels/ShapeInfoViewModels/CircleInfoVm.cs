using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public partial class CircleInfoVm : ShapeInfoBaseVm, IEditorViewModel<CircleInfo>
    {
        protected CircleInfo Circle { get => (CircleInfo)model; }
        public CircleInfoVm()
        {
            model = CircleInfo.ZeroCircle();
        }

        public double Radius
        {
            get => Circle.Radius;
            set
            {
                if (SetProperty(Circle.Radius, value, Circle, (m, v) => Circle.Radius = v))
                {
                    OnPropertyChanged(nameof(Height));
                    OnPropertyChanged(nameof(Length));
                    OnPropertyChanged(nameof(Diameter));
                    OnPropertyChanged(nameof(DimensionsString));
                }
            }
        }
        public double Diameter
        {
            get => Circle.Diameter;
            set
            {
                if (SetProperty(Circle.Diameter,value,Circle,(m,v)=> Circle.Radius = v/2d))
                {
                    OnPropertyChanged(nameof(Height));
                    OnPropertyChanged(nameof(Length));
                    OnPropertyChanged(nameof(Radius));
                    OnPropertyChanged(nameof(DimensionsString));
                }
            }
        }

        public override void OnHeightChanged(double newHeight)
        {
            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(Radius));
            OnPropertyChanged(nameof(Diameter));
            OnPropertyChanged(nameof(DimensionsString));
        }
        public override void OnLengthChanged(double newLength)
        {
            OnPropertyChanged(nameof(Height));
            OnPropertyChanged(nameof(Radius));
            OnPropertyChanged(nameof(Diameter));
            OnPropertyChanged(nameof(DimensionsString));
        }

        public void SetModel(CircleInfo circle)
        {
            model = circle.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }

        public CircleInfo CopyPropertiesToModel(CircleInfo model)
        {
            model.Radius = Radius;
            model.LocationX = LocationX;
            model.LocationY = LocationY;
            return model;
        }

        public override CircleInfo GetModel()
        {
            return Circle.GetDeepClone();
        }
    }
}
