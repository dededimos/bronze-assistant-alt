using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public partial class EggShapeInfoVm : ShapeInfoBaseVm , IEditorViewModel<EggShapeInfo>
    {
        protected EggShapeInfo Egg { get => (EggShapeInfo)model; }
        public EggShapeInfoVm()
        {
            model = EggShapeInfo.ZeroEgg();
        }

        public bool UsesElongationCoefficient
        {
            get => Egg.UsesElongationCoefficient;
            set => SetProperty(Egg.UsesElongationCoefficient, value, Egg, (m, v) => m.UsesElongationCoefficient = v);
        }


        public double PreferedElongation
        {
            get => Egg.PreferedElongation;
            set => SetProperty(Egg.PreferedElongation, value, Egg, (m, v) => m.PreferedElongation = v);
        }
        public double CircleRadius
        {
            get => Egg.CircleRadius;
            set
            {
                if (SetProperty(Egg.CircleRadius, value, Egg, (m, v) => m.CircleRadius = v))
                {
                    OnPropertyChanged(nameof(Height));
                    OnPropertyChanged(nameof(Length));
                    OnPropertyChanged(nameof(EllipseRadius));
                    OnPropertyChanged(nameof(DimensionsString));
                }
            }
        }
        public double EllipseRadius
        {
            get => Egg.EllipseRadius;
            set
            {
                if (SetProperty(Egg.EllipseRadius, value, Egg, (m, v) => m.EllipseRadius = v))
                {
                    OnPropertyChanged(nameof(Height));
                    OnPropertyChanged(nameof(Length));
                    OnPropertyChanged(nameof(CircleRadius));
                    OnPropertyChanged(nameof(DimensionsString));
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

        public void SetModel(EggShapeInfo egg)
        {
            model = egg.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }
        public override EggShapeInfo GetModel()
        {
            return Egg.GetDeepClone();
        }

        public EggShapeInfo CopyPropertiesToModel(EggShapeInfo model)
        {
            throw new NotImplementedException();
        }
    }
}
