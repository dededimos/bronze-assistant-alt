using BronzeFactoryApplication.ViewModels.CabinsViewModels;
using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels;
using CommonInterfacesBronze;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels
{
    /// <summary>
    /// The Base of all ViewModels
    /// </summary>
    public partial class BaseViewModel : ObservableObject , IBaseViewModel
    {
        #region 1.BusyPrompts
        protected const string LOADING = "Loading...";
        protected const string GENERATING = "Generating...";
        protected const string SAVING = "Saving...";
        protected const string CLOSING = "Closing...";
        #endregion

        [ObservableProperty]
        private string title = string.Empty;
        /// <summary>
        /// Weather this can be Disposed or acts as a Singleton
        /// </summary>
        public virtual bool IsDisposable { get => true; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;
        public bool IsNotBusy { get => !IsBusy; }

        [ObservableProperty]
        private string busyPrompt = LOADING;

        /// <summary>
        /// Weather it has Initilized
        /// </summary>
        public virtual bool Initilized { get; set; }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (isSupressingPropertyNotifications)
            {
                return;
            }
            else base.OnPropertyChanged(e);
        }

        private bool isSupressingPropertyNotifications = false;
        protected void SuppressPropertyNotifications() => isSupressingPropertyNotifications = true;
        protected void ResumePropertyNotifications() => isSupressingPropertyNotifications = false;
        /// <summary>
        /// Executes the Action with Property notifications Suppressed , Resumes notifications after action is executed.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="notifyPropChangesOnBlockFinish">Informs all properties have changed when the code block finishes execution</param>
        protected void SupressedPropertyNotificationsBlock(Action action,bool notifyPropChangesOnBlockFinish = true)
        {
            SuppressPropertyNotifications();
            action.Invoke();
            ResumePropertyNotifications();
            if (notifyPropChangesOnBlockFinish) OnPropertyChanged("");
        }

        public void Dispose()
        {
            Dispose(true);
            //item already disposed above code , supress finalize
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initilizes the ViewModel Asynchronously
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        protected virtual Task InitilizeAsync()
        {
            throw new NotSupportedException($"{this.GetType().Name} does not Support Sync or Async Initilization");
        }

        /// <summary>
        /// Initilizes the ViewModel
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        protected virtual void Initilize()
        {
            throw new NotSupportedException($"{this.GetType().Name} does not Support Sync or Async Initilization");
        }

        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;
        public virtual void Dispose(bool disposing)
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
            //base.Dispose(disposing);
        }
    }
}
