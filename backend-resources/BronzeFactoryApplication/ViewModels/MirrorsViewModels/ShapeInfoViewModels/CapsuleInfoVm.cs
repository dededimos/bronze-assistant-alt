using CommonInterfacesBronze;
using ShapesLibrary.ShapeInfoModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels
{
    public partial class CapsuleInfoVm : ShapeInfoBaseVm , IEditorViewModel<CapsuleInfo>
    {
        protected CapsuleInfo Capsule { get => (CapsuleInfo)model; }
        public CapsuleInfoVm()
        {
            model = CapsuleInfo.CapsuleZero();
        }

        public override CapsuleInfo GetModel()
        {
            return Capsule.GetDeepClone();
        }

        public override void OnHeightChanged(double newHeight)
        {
            OnPropertyChanged("");
        }
        public override void OnLengthChanged(double newLength)
        {
            OnPropertyChanged("");
        }

        public void SetModel(CapsuleInfo capsule)
        {
            model = capsule.GetDeepClone();
            OnPropertyChanged(string.Empty);
        }

        public CapsuleInfo CopyPropertiesToModel(CapsuleInfo model)
        {
            throw new NotImplementedException();
        }
    }

}
