using BathAccessoriesModelsLibrary;
using BronzeArtWebApplication.Shared.Layouts;
using BronzeArtWebApplication.Shared.ViewModels.AccessoriesPageViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Pages.NewAccessoriesPage
{
    public partial class AccessoriesView : IDisposable
    {
        private List<BathroomAccessory> _previousAccessoriesList = new();

        [Parameter]
        public List<BathroomAccessory> Accessories { get; set; }
        [Parameter]
        public bool ShowSeriesCard { get; set; }
        [Parameter]
        public bool ShowDimensionsCard { get; set; }
        [Parameter]
        public bool ShowNameCard { get; set; }
        [Parameter]
        public bool ShowFinishes { get; set; }

        [Parameter]
        public bool FilterBySecondaryType { get; set; } = false;
        [Parameter]
        public bool FilterByFinishType { get; set; } = false;
        [Parameter]
        public string ShownAccessoryFinishCode { get; set; } = null;

        private List<BathroomAccessory> FilteredAccessories 
        {
            get 
            {
                List<BathroomAccessory> filteredList = Accessories;
                if (FilterBySecondaryType && !string.IsNullOrWhiteSpace(SelectedSecondaryTypeFilter))
                {
                    filteredList = filteredList.Where(a=> a.SecondaryType.Code == SelectedSecondaryTypeFilter).ToList();
                }
                if (FilterByFinishType && !string.IsNullOrWhiteSpace(SelectedFinishFilter)) 
                {
                    filteredList = filteredList.Where(a => a.AvailableFinishes.Select(af=> af.Finish.Code).Any(c=> c== SelectedFinishFilter)).ToList();
                }
                return filteredList;
            }
        }
        private string SelectedSecondaryTypeFilter { get; set; } = string.Empty;
        private string SelectedFinishFilter { get; set; } = string.Empty;

        /// <summary>
        /// The Number of Items from the Total that can currently show
        /// </summary>
        private int itemsToShow = 35;
        private int remainingItems = 0;

        /// <summary>
        /// Returns the Accessories Grouped into a Dictionary with Keys being the Name of each Secondary Type
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,List<BathroomAccessory>> AccessoriesBySecondaryType()
        {
            return FilteredAccessories.GroupBy(a => a.SecondaryType.Trait).ToDictionary(g=>g.Key,g=>g.ToList());
        }

        public void ResetFilters()
        {
            SelectedSecondaryTypeFilter = string.Empty;
            SelectedFinishFilter = string.Empty;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (_previousAccessoriesList != Accessories)
            {
                itemsToShow = 35;
                _previousAccessoriesList = Accessories;
                remainingItems = FilteredAccessories.Count - itemsToShow;
                await Js.ScrollToTop(AccessoriesLayout.bodyElementId);
            }
            ResetFilters();
        }

        protected override void OnInitialized()
        {
            Basket.PropertyChanged += Basket_PropertyChanged;
        }

        private void Basket_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BasketViewModel.PricesEnabled))
            {
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            Basket.PropertyChanged -= Basket_PropertyChanged;
            GC.SuppressFinalize(this);
        }
    }
}
