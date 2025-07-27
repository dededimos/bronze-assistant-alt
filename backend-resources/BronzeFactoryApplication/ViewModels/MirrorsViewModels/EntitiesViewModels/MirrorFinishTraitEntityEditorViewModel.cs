using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.DrawingsViewModels;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using MirrorsLib.Repositories;
using MirrorsRepositoryMongoDB.Entities;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorFinishElementEntityEditorViewModel : MirrorElementEntityBaseEditorViewModel<MirrorFinishElementEntity>, IMirrorEntityEditorViewModel<MirrorFinishElementEntity>
    {
        public MirrorFinishElementEntityEditorViewModel(Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory,
                                                      Func<DrawBrushVm> brushVmFactory,
                                                      IEditModelModalsGenerator editModalsGenerator) : base(baseEntityFactory, editModalsGenerator)
        {
            FinishBrush = brushVmFactory.Invoke();
            FinishBrush.PropertyChanged += FinishBrush_PropertyChanged;
        }

        private void FinishBrush_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(FinishBrush));
        }

        public DrawBrushVm FinishBrush { get; set; }

        public override MirrorFinishElementEntity CopyPropertiesToModel(MirrorFinishElementEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.FinishColorBrush = new DrawBrushDTO(FinishBrush.GetModel());
            return model;
        }
        protected override void SetModelWithoutUndoStore(MirrorFinishElementEntity model)
        {
            base.SetModelWithoutUndoStore(model);
            FinishBrush.SetModel(model.FinishColorBrush.ToDrawBrush());
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
                FinishBrush.PropertyChanged -= FinishBrush_PropertyChanged;
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
