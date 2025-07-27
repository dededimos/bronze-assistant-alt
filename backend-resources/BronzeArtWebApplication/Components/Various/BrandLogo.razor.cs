using BronzeArtWebApplication.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;

namespace BronzeArtWebApplication.Components.Various
{
    public partial class BrandLogo :ComponentBase , IDisposable
    {
        [Parameter] public string Class { get; set; }
        [Parameter] public string Style { get; set; }
        [Parameter] public string ClassBronzeLogo { get; set; }
        [Parameter] public string StyleBronzeLogo { get; set; }
        [Parameter] public bool IsVisibleBronzeLogoRetail { get; set; }

        protected override void OnInitialized()
        {
            user.PropertyChanged += User_PropertyChanged;
            
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.SelectedAppMode)
                               or nameof(BronzeUser.SelectedRetailTheme))
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