using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Supports;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SupportsViewModels
{
    public partial class MirrorSupportInfoUndefinedViewModel : MirrorSupportInfoBaseViewModel, IEditorViewModel<UndefinedMirrorSupportInfo>
    {
        public UndefinedMirrorSupportInfo CopyPropertiesToModel(UndefinedMirrorSupportInfo model)
        {
            CopyBasePropertiesToModel(model);
            return model;
        }

        public UndefinedMirrorSupportInfo GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(UndefinedMirrorSupportInfo model)
        {
            SetBaseProperties(model);
        }
    }
}
