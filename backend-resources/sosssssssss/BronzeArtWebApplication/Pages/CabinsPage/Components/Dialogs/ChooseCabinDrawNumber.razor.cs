using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using ShowerEnclosuresModelsLibrary.Enums;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.Dialogs
{
    public partial class ChooseCabinDrawNumber : ComponentBase
    {
        [Parameter] public bool IsVisible { get; set; }

        [Parameter] public CabinModelEnum? SelectedModel { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
        [Parameter] public EventCallback<CabinDrawNumber> OnChoosingDrawClick { get; set; }
        [Parameter] public EventCallback OnPreviousClick { get; set; }
    }
}
