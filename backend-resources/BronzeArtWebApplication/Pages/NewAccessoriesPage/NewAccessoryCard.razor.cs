using BathAccessoriesModelsLibrary;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BronzeArtWebApplication.Pages.NewAccessoriesPage
{
#nullable enable
    public partial class NewAccessoryCard
    {
        [Parameter]
        public BathroomAccessory Accessory { get; set; } = new();
        [Parameter]
        public bool ShowDimensions { get; set; }
        [Parameter]
        public List<string> AppearingDimensionsCodes { get; set; } = [];
        
        [Parameter]
        public string? ShownAccessoryFinishCode { get; set; }

        [Parameter]
        public bool ShowName { get; set; }
        [Parameter]
        public bool ShowSeries { get; set; }
        [Parameter]
        public bool ShowFinishes { get; set; }
        [Parameter]
        public bool ShowPrices { get; set; }
        [Parameter]
        public bool ShowStock { get; set; }
        [Parameter]
        public string Class { get; set; } = string.Empty;

        private string? mousedOverFinishCode = null;

        private void MouseOverFinish(string finishCode)
        {
            mousedOverFinishCode = finishCode;
        }
    }
}
