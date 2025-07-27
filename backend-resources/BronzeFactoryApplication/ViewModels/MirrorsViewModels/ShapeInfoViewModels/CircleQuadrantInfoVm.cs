using CommonInterfacesBronze;
using Microsoft.Graph.Models.CallRecords;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public partial class CircleQuadrantInfoVm : ShapeInfoBaseVm , IEditorViewModel<CircleQuadrantInfo>
    {
        protected CircleQuadrantInfo Quadrant { get => (CircleQuadrantInfo)model; }
        public CircleQuadrantInfoVm()
        {
            model = CircleQuadrantInfo.ZeroQuadrant();
        }

        public double Radius
        {
            get => Quadrant.Radius;
            set
            {
                if (SetProperty(Quadrant.Radius, value, Quadrant, (m, v) => Quadrant.Radius = v))
                {
                    OnPropertyChanged(nameof(Height));
                    OnPropertyChanged(nameof(Length));
                    OnPropertyChanged(nameof(DimensionsString));
                }
            }
        }
        public override void OnHeightChanged(double newHeight)
        {
            OnPropertyChanged(nameof(Radius));
            OnPropertyChanged(nameof(Length));
            OnPropertyChanged(nameof(DimensionsString));
        }
        public override void OnLengthChanged(double newLength)
        {
            OnPropertyChanged(nameof(Radius));
            OnPropertyChanged(nameof(Height));
            OnPropertyChanged(nameof(DimensionsString));
        }
        public void SetModel(CircleQuadrantInfo quadrant)
        {
            model = quadrant.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }
        public override CircleQuadrantInfo GetModel()
        {
            return Quadrant.GetDeepClone();
        }

        public CircleQuadrantInfo CopyPropertiesToModel(CircleQuadrantInfo model)
        {
            throw new NotImplementedException();
        }
    }
}
