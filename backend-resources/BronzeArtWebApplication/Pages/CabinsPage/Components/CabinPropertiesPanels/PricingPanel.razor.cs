using BronzeArtWebApplication.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.CabinPropertiesPanels
{
    public partial class PricingPanel : ComponentBase, IDisposable
    {
        protected override void OnInitialized()
        {
            user.PropertyChanged += User_PropertyChanged;
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.SelectedPrimaryDiscountCabin)
                               or nameof(BronzeUser.SelectedSecondaryDiscountCabin)
                               or nameof(BronzeUser.SelectedTertiaryDiscountCabin))
            {
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            user.PropertyChanged -= User_PropertyChanged;
        }
    }
}
