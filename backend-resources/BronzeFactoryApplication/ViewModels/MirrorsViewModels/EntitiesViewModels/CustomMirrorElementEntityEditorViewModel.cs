using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MirrorsRepositoryMongoDB.Entities;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class CustomMirrorElementEntityEditorViewModel : MirrorElementEntityBaseEditorViewModel<CustomMirrorElementEntity>, IMirrorEntityEditorViewModel<CustomMirrorElementEntity>
    {
        [ObservableProperty]
        private LocalizedString customElementType = LocalizedString.Undefined();


        public CustomMirrorElementEntityEditorViewModel(
            Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory,
            IEditModelModalsGenerator editModalsGenerator) : base(baseEntityFactory, editModalsGenerator)
        {

        }

        public override CustomMirrorElementEntity CopyPropertiesToModel(CustomMirrorElementEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.CustomElementType = this.CustomElementType.GetDeepClone();
            return model;
        }
        protected override void SetModelWithoutUndoStore(CustomMirrorElementEntity model)
        {
            //Call the MirrorElementEntity base to set the Db Entity and the MirrorElements Properties
            base.SetModelWithoutUndoStore(model);
            this.CustomElementType = model.CustomElementType.GetDeepClone();
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
