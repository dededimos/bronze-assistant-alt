using BronzeFactoryApplication.ViewModels.MirrorsViewModels.SupportsViewModels;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels
{
    public partial class MirrorModuleInfoBaseViewModel : BaseViewModel
    {
        public MirrorModuleType ModuleType { get; protected set; }

        public void CopyBasePropertiesToModel(MirrorModuleInfo moduleInfo)
        {
            // nothing return
        }

        public void SetBaseProperties(MirrorModuleInfo moduleInfo)
        {
            ModuleType = moduleInfo.ModuleType;
        }
    }
}
