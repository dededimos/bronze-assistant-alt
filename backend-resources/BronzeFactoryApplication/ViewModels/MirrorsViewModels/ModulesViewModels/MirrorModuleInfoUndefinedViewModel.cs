using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class MirrorModuleInfoUndefinedViewModel : MirrorModuleInfoBaseViewModel, IEditorViewModel<UndefinedMirrorModuleInfo>
    {
        public UndefinedMirrorModuleInfo CopyPropertiesToModel(UndefinedMirrorModuleInfo model)
        {
            return model;
        }

        public UndefinedMirrorModuleInfo GetModel()
        {
            return new UndefinedMirrorModuleInfo();
        }

        public void SetModel(UndefinedMirrorModuleInfo model)
        {
            SetBaseProperties(model);
        }
    }
}
