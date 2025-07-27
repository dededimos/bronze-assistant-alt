using BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels;
using CommonInterfacesBronze;
using MongoDbCommonLibrary.CommonEntities;

namespace BronzeFactoryApplication.ViewModels.HelperViewModels
{
    public partial class MongoEntityBaseUndoEditorViewModel<TEntity> : UndoEditorViewModelBase<TEntity>, IMirrorEntityEditorViewModel<TEntity>
        where TEntity : MongoDatabaseEntity, IDeepClonable<TEntity>, IEqualityComparerCreator<TEntity>, new()
    {
        public MongoDatabaseEntityEditorViewModel BaseEntity { get; }
        public MongoEntityBaseUndoEditorViewModel(Func<MongoDatabaseEntityEditorViewModel> baseEntityVmFactory)
        {
            BaseEntity = baseEntityVmFactory.Invoke();
            BaseEntity.PropertyChanged += BaseEntity_PropertyChanged;
        }

        public override TEntity CopyPropertiesToModel(TEntity model)
        {
            BaseEntity.CopyPropertiesToModel(model);
            return model;
        }

        public sealed override TEntity GetModel()
        {
            var emptyModel = ProvideEmptyModel();
            return CopyPropertiesToModel(emptyModel);
        }

        protected virtual TEntity ProvideEmptyModel() => new();

        protected override void SetModelWithoutUndoStore(TEntity model)
        {
            BaseEntity.SetModel(model);
        }


        private void BaseEntity_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(BaseEntity));
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
                BaseEntity.PropertyChanged -= BaseEntity_PropertyChanged;
                BaseEntity.Dispose();
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Just for Reference to be Used in the Designer of WPF
    /// </summary>
    public partial class MongoEntityBaseUndoEditorViewModel : MongoEntityBaseUndoEditorViewModel<MongoDatabaseEntity>
    {
        public MongoEntityBaseUndoEditorViewModel(Func<MongoDatabaseEntityEditorViewModel> baseEntityVmFactory) 
            : base(baseEntityVmFactory)
        {
        }
    }
}
