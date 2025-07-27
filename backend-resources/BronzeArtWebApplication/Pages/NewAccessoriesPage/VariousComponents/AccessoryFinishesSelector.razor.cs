using BathAccessoriesModelsLibrary;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Pages.NewAccessoriesPage.VariousComponents
{
    public partial class AccessoryFinishesSelector
    {
        [Parameter]
        public List<AccessoryFinish> Finishes { get; set; } = new();

        [Parameter]
        public AccessoryFinish SelectedFinish { get; set; } = AccessoryFinish.Empty();

        [Parameter]
        public EventCallback<AccessoryFinish> SelectedFinishChanged { get; set; }

        public async Task OnSelectedFinishChanged(AccessoryFinish newSelectedFinish)
        {
            if (SelectedFinish != newSelectedFinish)
            {
                SelectedFinish = newSelectedFinish;
                await SelectedFinishChanged.InvokeAsync(SelectedFinish);
            }
        }
    }
}