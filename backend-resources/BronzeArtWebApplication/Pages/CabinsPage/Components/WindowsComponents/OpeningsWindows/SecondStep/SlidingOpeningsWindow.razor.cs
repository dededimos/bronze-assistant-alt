using BronzeArtWebApplication.Shared.Enums;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using System;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents.OpeningsWindows.SecondStep
{
    public partial class SlidingOpeningsWindow : ComponentBase, IDisposable
    {
        [Parameter] public EventCallback<SlidingType> OnSlidingCategoryClick { get; set; }

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change
            if (e.PropertyName == nameof(StoryWindow.SlidingOpenings))
            {
                StateHasChanged();
            }
        }

        private void HandleSelection(SlidingType selectedSlidingType)
        {
            switch (selectedSlidingType)
            {
                case SlidingType.SingleSliding:
                    vm.ShowWindow(StoryWindow.ModelsS, StoryWindow.SlidingOpenings);
                    break;
                case SlidingType.SingleSlidingWithSidePanel:
                    vm.ShowWindow(StoryWindow.ModelsSF, StoryWindow.SlidingOpenings);
                    break;
                case SlidingType.SingleSlidingWith2SidePanels:
                    vm.ShowWindow(StoryWindow.ModelsSFF, StoryWindow.SlidingOpenings);
                    break;
                case SlidingType.DoubleSliding:
                    vm.ShowWindow(StoryWindow.Models4, StoryWindow.SlidingOpenings);
                    break;
                case SlidingType.DoubleSlidingWithSidePanel:
                    vm.ShowWindow(StoryWindow.Models4F, StoryWindow.SlidingOpenings);
                    break;
                case SlidingType.DoubleSlidingWith2SidePanels:
                    vm.ShowWindow(StoryWindow.Models4FF, StoryWindow.SlidingOpenings);
                    break;
                case SlidingType.CornerSliding:
                    vm.ShowWindow(StoryWindow.ModelsA, StoryWindow.SlidingOpenings);
                    break;
                case SlidingType.CornerSlidingWithSidePanel:
                    vm.ShowWindow(StoryWindow.ModelsAF, StoryWindow.SlidingOpenings);
                    break;
                case SlidingType.SemiCircularSliding:
                    vm.ShowWindow(StoryWindow.ModelsC, StoryWindow.SlidingOpenings);
                    break;
                case SlidingType.SemiCircularSlidingWithSidePanel:
                    vm.ShowWindow(StoryWindow.ModelsCF, StoryWindow.SlidingOpenings);
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(SlidingType)} :{selectedSlidingType} is not a Valid Value to Handle the Selecetion");
            }
        }




        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
