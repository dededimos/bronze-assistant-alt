using BronzeFactoryApplication.ViewModels.HelperViewModels;
using MirrorsLib;
using MirrorsLib.MirrorsOrderModels;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOrdersViewModels
{
    public partial class MirrorOrderRowUndoViewModel : UndoEditorViewModelBase<MirrorOrderRow>
    {
        public MirrorOrderRowUndoViewModel(Func<MirrorOrderRowViewModel> rowVmFactory,
                                           Func<MirrorSynthesisEditorWithDrawViewModel> mirrorFactory)
        {
            Row = rowVmFactory.Invoke();
            MirrorToEdit = mirrorFactory.Invoke();
            Row.PropertyChanged += Row_PropertyChanged;
            MirrorToEdit.MirrorEditor.MirrorChanged += MirrorEditor_MirrorChanged;
        }

        private void MirrorEditor_MirrorChanged(object? sender, EventArgs e)
        {
            Row.Mirror = MirrorToEdit.MirrorEditor.GetModel();
        }
        private void Row_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Row));
        }

        public MirrorOrderRowViewModel Row { get; }
        public MirrorSynthesisEditorWithDrawViewModel MirrorToEdit { get; }

        public override MirrorOrderRow CopyPropertiesToModel(MirrorOrderRow model)
        {
            Row.CopyPropertiesToModel(model);
            return model;
        }
        public override MirrorOrderRow GetModel()
        {
            return CopyPropertiesToModel(new());
        }
        protected override void SetModelWithoutUndoStore(MirrorOrderRow model)
        {
            Row.SetModel(model);
            MirrorToEdit.MirrorEditor.SetModel(model.RowItem ?? MirrorSynthesis.DefaultSynthesis());
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
                Row.PropertyChanged -= Row_PropertyChanged;
                MirrorToEdit.MirrorEditor.MirrorChanged -= MirrorEditor_MirrorChanged;
                Row.Dispose();
                MirrorToEdit.Dispose();
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
