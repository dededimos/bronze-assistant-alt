using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public partial class EllipseInfoVm : ShapeInfoBaseVm , IEditorViewModel<EllipseInfo>
    {
        protected EllipseInfo Ellipse { get => (EllipseInfo)model; }
        public EllipseInfoVm()
        {
            model = EllipseInfo.ZeroEllipse();
        }

        public override EllipseInfo GetModel()
        {
            return Ellipse.GetDeepClone();
        }

        public override void OnHeightChanged(double newHeight)
        {
            OnPropertyChanged("");
        }
        public override void OnLengthChanged(double newLength)
        {
            OnPropertyChanged("");
        }
        public void SetModel(EllipseInfo ellipse)
        {
            model = ellipse.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }

        public EllipseInfo CopyPropertiesToModel(EllipseInfo model)
        {
            throw new NotImplementedException();
        }
    }
}
