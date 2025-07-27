using BronzeFactoryApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService
{
    /// <summary>
    /// An Object Store - Saving the Current Navigation ViewModel
    /// </summary>
    public class NavigationStore
    {
        private BaseViewModel? currentViewModel;
        /// <summary>
        /// The Current View ViewModel
        /// </summary>
        public BaseViewModel CurrentViewModel
        {
            get => currentViewModel ?? throw new ArgumentNullException(nameof(currentViewModel));
            private set 
            {
                if (currentViewModel != value)
                {
                    if (currentViewModel is not null && currentViewModel.IsDisposable) currentViewModel.Dispose();
                    currentViewModel = value;
                    RaiseCurrentViewModelChanged(); 
                }
            }
        }

        public async Task ChangeViewAsync(BaseViewModel newView)
        {
            if (currentViewModel != newView)
            {
                if (currentViewModel is not null and IOperationOnNavigatingAway oper) await oper.OnNavigatingAwayOperation();
                CurrentViewModel = newView;
            }
        }


        public event EventHandler? CurrentViewModelChanged;
        /// <summary>
        /// Informs Subscribers the Current Viewmodel HasChanged
        /// </summary>
        private void RaiseCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
