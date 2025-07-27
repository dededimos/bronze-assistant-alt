using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.Services.CodeBuldingService;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOptionsViewModels
{
    public partial class MirrorApplicationOptionsBaseViewModel : BaseViewModel
    {
        public string OptionsType { get; protected set; } = nameof(MirrorApplicationOptionsBase);

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
    public partial class MirrorApplicationOptionsUndefinedViewModel : MirrorApplicationOptionsBaseViewModel, IEditorViewModel<MirrorApplicationOptionsBase>
    {
        public MirrorApplicationOptionsUndefinedViewModel()
        {
            OptionsType = nameof(MirrorApplicationOptionsBase);
        }

        public MirrorApplicationOptionsBase CopyPropertiesToModel(MirrorApplicationOptionsBase model)
        {
            return model;
        }

        public MirrorApplicationOptionsBase GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(MirrorApplicationOptionsBase model)
        {
            return;
        }
    }
}
