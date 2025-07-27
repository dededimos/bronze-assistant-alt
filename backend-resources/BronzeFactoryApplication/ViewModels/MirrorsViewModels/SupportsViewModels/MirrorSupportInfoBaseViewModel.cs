using MirrorsLib.Enums;
using MirrorsLib.MirrorElements.Supports;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SupportsViewModels
{
    public partial class MirrorSupportInfoBaseViewModel : BaseViewModel
    {
        public MirrorSupportType SupportType { get; protected set; }

        [ObservableProperty]
        private double minDistanceFromSandblast;
        [ObservableProperty]
        private double minDistanceFromOtherModules;
        [ObservableProperty]
        private bool isModulesBoundary;

        public void CopyBasePropertiesToModel(MirrorSupportInfo supportInfo)
        {
            supportInfo.MinDistanceFromSandblast = MinDistanceFromSandblast;
            supportInfo.MinDistanceFromOtherModules = MinDistanceFromOtherModules;
            supportInfo.IsModulesBoundary = this.IsModulesBoundary;
        }

        public void SetBaseProperties(MirrorSupportInfo supportInfo)
        {
            SupportType = supportInfo.SupportType;
            MinDistanceFromSandblast = supportInfo.MinDistanceFromSandblast;
            MinDistanceFromOtherModules = supportInfo.MinDistanceFromOtherModules;
            IsModulesBoundary = supportInfo.IsModulesBoundary;
        }
    }
}
