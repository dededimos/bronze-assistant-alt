using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SupportsViewModels
{
    public partial class MirrorMultiSupportsEditorViewModel : MirrorSupportInfoBaseViewModel, IEditorViewModel<MirrorMultiSupports>
    {
        public IEditorViewModel<MirrorSupportInstructions> TopSupportsInstructions { get; }
        public IEditorViewModel<MirrorSupportInstructions> BottomSupportsInstructions { get; }

        public MirrorMultiSupportsEditorViewModel(Func<IEditorViewModel<MirrorSupportInstructions>> vmFactory)
        {
            TopSupportsInstructions = vmFactory.Invoke();
            BottomSupportsInstructions = vmFactory.Invoke();

            TopSupportsInstructions.PropertyChanged += TopSupportsInstructions_PropertyChanged;
            BottomSupportsInstructions.PropertyChanged += BottomSupportsInstructions_PropertyChanged;
        }

        private void BottomSupportsInstructions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(BottomSupportsInstructions));
        }

        private void TopSupportsInstructions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TopSupportsInstructions));
        }

        public MirrorMultiSupports CopyPropertiesToModel(MirrorMultiSupports model)
        {
            CopyBasePropertiesToModel(model);
            model.TopSupportsInstructions = this.TopSupportsInstructions.GetModel();
            model.BottomSupportsInstructions = this.BottomSupportsInstructions.GetModel();
            return model;
        }
        public MirrorMultiSupports GetModel()
        {
            return this.CopyPropertiesToModel(new());
        }
        public void SetModel(MirrorMultiSupports model)
        {
            SetBaseProperties(model);
            this.TopSupportsInstructions.SetModel(model.TopSupportsInstructions);
            this.BottomSupportsInstructions.SetModel(model.BottomSupportsInstructions);
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
                TopSupportsInstructions.PropertyChanged -= TopSupportsInstructions_PropertyChanged;
                BottomSupportsInstructions.PropertyChanged -= BottomSupportsInstructions_PropertyChanged;
                TopSupportsInstructions.Dispose();
                BottomSupportsInstructions.Dispose();
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
