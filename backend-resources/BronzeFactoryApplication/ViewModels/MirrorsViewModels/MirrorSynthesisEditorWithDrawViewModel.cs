using BronzeFactoryApplication.ViewModels.DrawingsViewModels;
using CommonHelpers.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public partial class MirrorSynthesisEditorWithDrawViewModel :BaseViewModel
    {
        public MirrorSynthesisEditorWithDrawViewModel(Func<MirrorSynthesisEditorViewModel> synthesisVmFactory,
                                                      Func<MirrorSynthesisDrawingViewModel> drawsVmFactory)
        {
            MirrorEditor = synthesisVmFactory.Invoke();
            MirrorEditor.MirrorChanged += MirrorEditor_MirrorChanged;
            MirrorDrawing = drawsVmFactory.Invoke();

            //The Action must run in the Current Dispatacher because it changes the UI from the ViewModels binded to it.
            actionTimer = new(400, () => Application.Current.Dispatcher.Invoke(() => MirrorDrawing.GenerateNewDrawing(MirrorEditor.GetModel())));
        }

        private void MirrorEditor_MirrorChanged(object? sender, EventArgs e)
        {
            if (actionTimer.IsRunning)
            {
                actionTimer.Reset();
            }
            else
            {
                actionTimer.Start();
            }
        }

        /// <summary>
        /// A timer to delay the drawing generation until the Mirror PropChange event stops firing
        /// </summary>
        private readonly ActionTimer actionTimer;
        public MirrorSynthesisEditorViewModel MirrorEditor { get; set; }
        public MirrorSynthesisDrawingViewModel MirrorDrawing { get; set; }

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
                MirrorEditor.MirrorChanged -= MirrorEditor_MirrorChanged;
                actionTimer.Stop();
                actionTimer.Dispose();
                MirrorEditor.Dispose();
                MirrorDrawing.Dispose();
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
