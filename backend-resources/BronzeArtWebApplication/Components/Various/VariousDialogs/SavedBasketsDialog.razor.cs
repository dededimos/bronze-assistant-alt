using BronzeArtWebApplication.Shared.Services.SaveToStorageServices;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Components.Various.VariousDialogs
{
    public partial class SavedBasketsDialog : IDisposable
    {
        private List<BasketSave> savedBaskets = new();

        [CascadingParameter]
        public IMudDialogInstance MudDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            saveStore.SavedBasketsChanged += SaveStore_SavedBasketsChanged; ;
            saveStore.IsBusyChanged += SaveStore_IsBusyChanged;
            savedBaskets = await saveStore.GetSavedBaskets();
        }

        private async void SaveStore_SavedBasketsChanged(object sender, EventArgs e)
        {
            savedBaskets = await saveStore.GetSavedBaskets();
            await InvokeAsync(StateHasChanged);
        }

        private void SaveStore_IsBusyChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }

        public void Dispose()
        {
            saveStore.SavedBasketsChanged -= SaveStore_SavedBasketsChanged; ;
            saveStore.IsBusyChanged -= SaveStore_IsBusyChanged;
            GC.SuppressFinalize(this);
        }

        private void RestoreItemAndClose(BasketSave save)
        {
            saveStore.RestoreSavedBasket(save);
            snackBar.Add($"{Lc.Keys["BasketRestored"]}<br/>{save.BasketName}", Severity.Success, (options) => { options.VisibleStateDuration = 2;options.CloseAfterNavigation = true; });
            CloseDialog();
        }

        private async Task RemoveSavedBasket(string basketName)
        {
            await saveStore.RemoveSavedBasket(basketName);
        }

        void CloseDialog() => MudDialog.Close();
    }
}
