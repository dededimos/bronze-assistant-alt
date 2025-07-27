using BronzeArtWebApplication.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace BronzeArtWebApplication.Shared
{
    public partial class LoginDisplay : ComponentBase , IDisposable
    {

        [Parameter] public string LogoutString { get; set; }
        [Parameter] public string LoginString { get; set; }

        private void BeginLogout(MouseEventArgs args)
        {
            Navigation.NavigateToLogout("authentication/logout");
        }

        protected override void OnInitialized()
        {
            bronzeUser.PropertyChanged += BronzeUser_PropertyChanged;
        }

        private void BronzeUser_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.IsWhiteLabeled))
            {
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            bronzeUser.PropertyChanged -= BronzeUser_PropertyChanged;
        }

    }
}
