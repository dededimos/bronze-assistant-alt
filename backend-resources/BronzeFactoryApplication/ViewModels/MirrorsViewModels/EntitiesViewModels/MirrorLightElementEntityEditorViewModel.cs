using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsRepositoryMongoDB.Entities;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorLightElementEntityEditorViewModel : MirrorElementEntityBaseEditorViewModel<MirrorLightElementEntity>, IMirrorEntityEditorViewModel<MirrorLightElementEntity>
    {
        public IEditorViewModel<MirrorLightInfo> LightInfo { get; set; }

        public MirrorLightElementEntityEditorViewModel(
            Func<IEditorViewModel<MirrorLightInfo>> lightInfoVmFactory,
            Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory,
            IEditModelModalsGenerator editModalsGenerator) : base(baseEntityFactory, editModalsGenerator)
        {
            LightInfo = lightInfoVmFactory.Invoke();
            LightInfo.PropertyChanged += LightInfo_PropertyChanged;
        }

        public override MirrorLightElementEntity CopyPropertiesToModel(MirrorLightElementEntity model)
        {
            base.CopyPropertiesToModel(model);
            LightInfo.CopyPropertiesToModel(model.LightInfo);
            return model;
        }
        protected override void SetModelWithoutUndoStore(MirrorLightElementEntity model)
        {
            //Call the MirrorElementEntity base to set the Db Entity and the MirrorElements Properties
            base.SetModelWithoutUndoStore(model);
            LightInfo.SetModel(model.LightInfo);
        }


        private void LightInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(LightInfo));
        }
        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                LightInfo.PropertyChanged -= LightInfo_PropertyChanged;
                LightInfo.Dispose();
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }
}
