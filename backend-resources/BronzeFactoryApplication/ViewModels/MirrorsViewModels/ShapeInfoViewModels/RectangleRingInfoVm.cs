using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public partial class RectangleRingInfoVm : ShapeInfoBaseVm, IEditorViewModel<RectangleRingInfo>
    {
        protected RectangleRingInfo RectangleRing { get => (RectangleRingInfo)model; }
        public RectangleRingInfoVm()
        {
            model = RectangleRingInfo.RectangleRingZero();
        }

        public double Thickness
        {
            get => RectangleRing.Thickness;
            set
            {
                if (SetProperty(RectangleRing.Thickness, value, RectangleRing, (m, v) => RectangleRing.Thickness = v))
                {
                    OnPropertyChanged(nameof(DimensionsString));
                }
            }
        }
        public override RectangleRingInfo GetModel()
        {
            return RectangleRing.GetDeepClone();
        }
        public void SetModel(RectangleRingInfo ring)
        {
            model = ring.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }
        public RectangleRingInfo CopyPropertiesToModel(RectangleRingInfo model)
        {
            model.LocationX = LocationX;
            model.LocationY = LocationY;
            model.Height = Height;
            model.Length = Length;
            model.Thickness = Thickness;
            return model;
        }
    }
}
