
using BathAccessoriesModelsLibrary;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Pages.NewAccessoriesPage.VariousComponents
{
    public partial class AccessoryDimensionsTable
    {
        [Parameter]
        public BathroomAccessory Accessory { get; set; } = BathroomAccessory.Empty();

        [Parameter]
        public AccessoryFinish SelectedFinish { get; set; } = AccessoryFinish.Empty();

        [Parameter]
        public string HoveredDimensionPhotoUrl { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> HoveredDimensionPhotoUrlChanged { get; set; }

        public async Task OnHoveredDimensionPhotoUrlChanged(string hoveredPhotoUrl)
        {
            if (HoveredDimensionPhotoUrl != hoveredPhotoUrl)
            {
                HoveredDimensionPhotoUrl = hoveredPhotoUrl;
                await HoveredDimensionPhotoUrlChanged.InvokeAsync(HoveredDimensionPhotoUrl);
            }
        }
    }
}