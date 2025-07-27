using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using System;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents.OpeningsWindows
{
    public partial class OpeningsWindow : ComponentBase, IDisposable
    {
        [Parameter] public EventCallback<OpeningCategory> OnOpeningCategoryClick { get; set; }

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change
            if (e.PropertyName == nameof(StoryWindow.OpeningPrimary))
            {
                StateHasChanged();
            }
        }

        private void HandleSelection(OpeningCategory selectedCategory)
        {
            switch (selectedCategory)
            {
                case OpeningCategory.Sliding:
                    vm.ShowWindow(StoryWindow.SlidingOpenings, StoryWindow.OpeningPrimary);
                    break;
                case OpeningCategory.Folding:
                    vm.ShowWindow(StoryWindow.FoldingOpenings, StoryWindow.OpeningPrimary);
                    break;
                case OpeningCategory.StandardDoor:
                    vm.ShowWindow(StoryWindow.StandardDoorOpenings, StoryWindow.OpeningPrimary);
                    break;
                case OpeningCategory.DoorOnPanel:
                    vm.ShowWindow(StoryWindow.DoorOnPanelOpenings, StoryWindow.OpeningPrimary);
                    break;
                case OpeningCategory.FixedPanels:
                    vm.ShowWindow(StoryWindow.FixedPanelOpenings, StoryWindow.OpeningPrimary);
                    break;
                case OpeningCategory.BathtubPanels:
                    vm.ShowWindow(StoryWindow.ModelsBathtub, StoryWindow.OpeningPrimary);
                    break;
                //Open Nothing if not Recognized
                default:
                    vm.ShowWindow(StoryWindow.None, StoryWindow.OpeningPrimary);
                    break;
            }
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
