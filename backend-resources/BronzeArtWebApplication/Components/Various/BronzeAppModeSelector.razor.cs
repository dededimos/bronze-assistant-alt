using BronzeArtWebApplication.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Components.Various
{
    /// <summary>
    /// The Components from which the User Selects or Views the Current Mode of the App
    /// </summary>
    public partial class BronzeAppModeSelector : ComponentBase, IDisposable
    {
        private bool isOptionsDialogVisible;

        /// <summary>
        /// Wheather the App Mode Dialog Will Refer to Cabins or Mirrors
        /// </summary>
        [Parameter] public bool IsSelectionForCabins { get; set; } = false;
        [Parameter] public bool IsSelectionForAccessories { get; set; }

        protected override void OnInitialized()
        {
            user.PropertyChanged += User_PropertyChanged;
        }

        //Gets Informed When Selected Mode Changes
        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.IsRetailFunctionAvailable)
                               or nameof(BronzeUser.IsWholesaleFunctionAvailable)
                               or nameof(BronzeUser.IsPowerUser)
                               or nameof(BronzeUser.SelectedAppMode)
                               or nameof(BronzeUser.SelectedPriceIncreaseFactor)
                               or nameof(BronzeUser.SelectedPriceIncreaseFactorCabins)
                               or nameof(BronzeUser.MaximumRetailDiscount)
                               or nameof(BronzeUser.MinimumRetailDiscount))
            {
                StateHasChanged();
            }
        }

        private async Task OpenOptionsDialog()
        {
            if (IsSelectionForAccessories)
            {
                await ms.InfoAsync("No Options Available", "There are No options available for the Accessories Pages");
            }
            else
            {
                isOptionsDialogVisible = !isOptionsDialogVisible;
            }
        }


        public void Dispose()
        {
            user.PropertyChanged -= User_PropertyChanged;
            GC.SuppressFinalize(this);
        }
    }
}
