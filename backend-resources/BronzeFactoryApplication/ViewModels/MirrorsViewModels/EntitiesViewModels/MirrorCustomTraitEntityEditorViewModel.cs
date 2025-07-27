using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MirrorsLib.Repositories;
using MirrorsRepositoryMongoDB.Entities;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorCustomTraitEntityEditorViewModel : MirrorElementTraitEntityEditorBaseViewModel<CustomMirrorTraitEntity> , IMirrorEntityEditorViewModel<CustomMirrorTraitEntity>
    {
        public MirrorCustomTraitEntityEditorViewModel(Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory,
                                                      IMirrorsDataProvider dataProvider,
                                                      IEditModelModalsGenerator editModalsGenerator) : base(baseEntityFactory, dataProvider, editModalsGenerator)
        {
            
        }
        [ObservableProperty]
        private LocalizedString customTraitType = LocalizedString.Undefined();

        public override CustomMirrorTraitEntity CopyPropertiesToModel(CustomMirrorTraitEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.CustomTraitType = this.CustomTraitType.GetDeepClone();
            return model;
        }

        protected override void SetModelWithoutUndoStore(CustomMirrorTraitEntity model)
        {
            base.SetModelWithoutUndoStore(model);
            this.CustomTraitType = model.CustomTraitType.GetDeepClone();
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
