using BronzeArtWebApplication.Shared.Enums;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using System;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents.SeriesWindows
{
    public partial class CabinSeriesWindow : ComponentBase, IDisposable
    {
        [Parameter] public EventCallback<CabinSeries> OnSeriesClick { get; set; }

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change
            if (e.PropertyName == nameof(StoryWindow.SeriesPrimary))
            {
                StateHasChanged();
            }
        }

        private void HandleSelection(CabinSeries selectedSeries)
        {
            switch (selectedSeries)
            {
                case CabinSeries.Bronze6000:
                    vm.ShowWindow(StoryWindow.SeriesB6000, StoryWindow.SeriesPrimary);
                    break;
                case CabinSeries.Inox304:
                    vm.ShowWindow(StoryWindow.SeriesInox304, StoryWindow.SeriesPrimary);
                    break;
                case CabinSeries.Smart:
                    vm.ShowWindow(StoryWindow.SeriesSmart, StoryWindow.SeriesPrimary);
                    break;
                case CabinSeries.Hotel8000:
                    vm.ShowWindow(StoryWindow.SeriesHotel8000, StoryWindow.SeriesPrimary);
                    break;
                case CabinSeries.Niagara6000:
                    vm.ShowWindow(StoryWindow.SeriesNiagara6000, StoryWindow.SeriesPrimary);
                    break;
                case CabinSeries.Free:
                    vm.ShowWindow(StoryWindow.SeriesFree, StoryWindow.SeriesPrimary);
                    break;
                default:
                    vm.ShowWindow(StoryWindow.None, StoryWindow.SeriesPrimary);
                    break;
            }
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
