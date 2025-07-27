using BathAccessoriesModelsLibrary;
using BronzeArtWebApplication.Shared.Helpers;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Pages.NewAccessoriesPage
{
    public partial class AccessoriesPage
    {
        /// <summary>
        /// Gets the Search Term to Provide auto scrolling to the element
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery(Name = RoutesStash.SearchTermCodeParamName)]
        public string SearchTermCode { get; set; } = string.Empty;

        /// <summary>
        /// The Selected Finish Filter
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery(Name = RoutesStash.FinishFilterParamName)]
        public string FinishFilterCode { get; set; } = string.Empty;

        /// <summary>
        /// The Selected Secondary Type Filter
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery(Name = RoutesStash.SecondaryTypeFilterParamName)]
        public string SecondaryTypeFilterCode { get; set; } = string.Empty;

        /// <summary>
        /// The Selected Series Filter
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery(Name = RoutesStash.SeriesFilterParamName)]
        public string SeriesFilterCode { get; set; } = string.Empty;

        /// <summary>
        /// Defines which Trait Type has been Chosen for Navigation
        /// </summary>
        [Parameter]
        public string TraitTypeString { get; set; }
        /// <summary>
        /// Defines which Trait by Code has been Chosen For Navigation
        /// </summary>
        [Parameter]
        public string TraitCodeString { get; set; }

        private TypeOfTrait? TraitType 
        { 
            get
            {
                if (string.IsNullOrEmpty(TraitTypeString)) return null;
                else if (Enum.TryParse(TraitTypeString,out TypeOfTrait traitType)) return traitType;
                else return TypeOfTrait.Undefined;
            } 
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!string.IsNullOrEmpty(SearchTermCode))
            {
                //scrolls to the element with the provided Code (the element has its Id as the Code)
                await Js.ScrollElementIntoViewAsync(SearchTermCode);
                SearchTermCode = string.Empty;
            }
        }
    }
}
