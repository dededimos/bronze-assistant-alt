using System;

namespace BronzeArtWebApplication.Shared.Services.OtherServices
{
    /// <summary>
    /// A service to mark that the Application is Busy
    /// </summary>
    public class BusyStateService
    {
        /// <summary>
        /// Weather the Application is Busy
        /// </summary>
        public bool IsBusy { get; private set; }
        /// <summary>
        /// The Text to Display While Busy
        /// </summary>
        public string IsBusyText { get; private set; }

        public event Action OnBusyStateChanged;

        /// <summary>
        /// Set the Busy State of the consumer of the Service
        /// </summary>
        /// <param name="isBusy"></param>
        public void SetBusyState(bool isBusy,string busyText = "Loading...")
        {
            IsBusy = isBusy;
            if (IsBusy) IsBusyText = busyText;
            OnBusyStateChanged?.Invoke();
        }
    }

}
