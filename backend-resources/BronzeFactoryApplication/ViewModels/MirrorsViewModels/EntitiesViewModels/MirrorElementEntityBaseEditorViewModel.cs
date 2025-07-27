using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsRepositoryMongoDB.Entities;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorElementEntityBaseEditorViewModel<TElementEntity> : MongoEntityBaseUndoEditorViewModel<TElementEntity>, IMirrorEntityEditorViewModel<TElementEntity>
        where TElementEntity : MirrorElementEntity, IDeepClonable<TElementEntity>, IEqualityComparerCreator<TElementEntity>, new()
    {
        public MirrorElementEntityBaseEditorViewModel(Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory, IEditModelModalsGenerator editModalsGenerator)
            : base(baseEntityFactory)
        {
            this.editModalsGenerator = editModalsGenerator;
        }

        private readonly IEditModelModalsGenerator editModalsGenerator;

        [ObservableProperty]
        private string code = string.Empty;
        [ObservableProperty]
        private string shortCode = string.Empty;
        [ObservableProperty]
        private string minimalCode = string.Empty;
        [ObservableProperty]
        private string photoUrl = string.Empty;
        [ObservableProperty]
        private string photoUrl2 = string.Empty;
        [ObservableProperty]
        private string iconUrl = string.Empty;
        [ObservableProperty]
        private bool isOverriddenElement;
        [ObservableProperty]
        private LocalizedString name = LocalizedString.Undefined();
        [ObservableProperty]
        private LocalizedString description = LocalizedString.Undefined();
        [ObservableProperty]
        private LocalizedString extendedDescription = LocalizedString.Undefined();

        public override TElementEntity CopyPropertiesToModel(TElementEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.Code = this.Code;
            model.ShortCode = this.ShortCode;
            model.MinimalCode = this.MinimalCode;
            model.PhotoUrl = this.PhotoUrl;
            model.PhotoUrl2 = this.PhotoUrl2;
            model.IconUrl = this.IconUrl;
            model.IsDefaultElement = !this.IsOverriddenElement;
            model.LocalizedDescriptionInfo = LocalizedDescription.Create(this.Name.GetDeepClone(), this.Description.GetDeepClone(), this.ExtendedDescription.GetDeepClone());
            return model;
        }
        protected override void SetModelWithoutUndoStore(TElementEntity model)
        {
            base.SetModelWithoutUndoStore(model);
            this.Code = model.Code;
            this.ShortCode = model.ShortCode;
            this.MinimalCode = model.MinimalCode;
            this.PhotoUrl = model.PhotoUrl;
            this.PhotoUrl2 = model.PhotoUrl2;
            this.IconUrl = model.IconUrl;
            this.IsOverriddenElement = !model.IsDefaultElement;
            this.Name = model.LocalizedDescriptionInfo.Name.GetDeepClone();
            this.Description = model.LocalizedDescriptionInfo.Description.GetDeepClone();
            this.ExtendedDescription = model.LocalizedDescriptionInfo.ExtendedDescription.GetDeepClone();
        }

        [RelayCommand]
        private void EditLocalizedString(LocalizedString localizedStringToEdit)
        {
            EditModelMessage<LocalizedString> message = new(localizedStringToEdit, this);
            editModalsGenerator.OpenEditModal(message);
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
