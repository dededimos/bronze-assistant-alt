using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements.Sandblasts;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SandblastViewModels
{
    /// <summary>
    /// The Base View Model for sandblasts
    /// </summary>
    public partial class MirrorSandblastInfoBaseViewModel : BaseViewModel 
    {
        [ObservableProperty]
        private double thickness;
        [ObservableProperty]
        private double minDistanceFromSupport;
        [ObservableProperty]
        private double minDistanceFromOtherModules;
        [ObservableProperty]
        private bool isModulesBoundary;
        [ObservableProperty]
        private bool isSupportsBoundary;

        public MirrorSandblastType SandblastType { get; protected set; }

        public void CopyBasePropertiesToModel(MirrorSandblastInfo sandblastInfo)
        {
            sandblastInfo.Thickness = this.Thickness;
            sandblastInfo.MinDistanceFromSupport = this.MinDistanceFromSupport;
            sandblastInfo.MinDistanceFromOtherModules = this.MinDistanceFromOtherModules;
            sandblastInfo.IsModulesBoundary = this.IsModulesBoundary;
            sandblastInfo.IsSupportsBoundary = this.IsSupportsBoundary;
                
        }

        public void SetBaseProperties(MirrorSandblastInfo sandblastInfo)
        {
            this.IsSupportsBoundary = sandblastInfo.IsSupportsBoundary;
            this.IsModulesBoundary = sandblastInfo.IsModulesBoundary;
            this.Thickness = sandblastInfo.Thickness;
            this.MinDistanceFromSupport = sandblastInfo.MinDistanceFromSupport;
            this.MinDistanceFromOtherModules = sandblastInfo.MinDistanceFromOtherModules;
        }
    }

    public partial class MirrorSandblastInfoUndefinedViewModel : MirrorSandblastInfoBaseViewModel, IEditorViewModel<UndefinedSandblastInfo>
    {
        public MirrorSandblastInfoUndefinedViewModel()
        {
            SandblastType = MirrorSandblastType.Undefined;
        }
        public UndefinedSandblastInfo CopyPropertiesToModel(UndefinedSandblastInfo model)
        {
            CopyBasePropertiesToModel(model);
            return model;
        }

        public UndefinedSandblastInfo GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(UndefinedSandblastInfo model)
        {
            this.Thickness = model.Thickness;
        }
    }

}
