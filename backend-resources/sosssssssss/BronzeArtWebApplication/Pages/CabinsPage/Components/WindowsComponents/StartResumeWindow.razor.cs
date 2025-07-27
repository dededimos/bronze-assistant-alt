using BronzeArtWebApplication.Shared.Enums;
using Microsoft.AspNetCore.Components;
using System;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents
{
    public partial class StartResumeWindow : ComponentBase, IDisposable
    {
        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change
            if (e.PropertyName is nameof(StoryWindow.StartWindow))
            {
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
