using BronzeArtWebApplication.Shared.Enums;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using System;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents.OpeningsWindows.SecondStep
{
    public partial class FixedPanelOpeningsWindow : ComponentBase, IDisposable
    {
        [Parameter] public EventCallback<FixedPanelType> OnFixedPanelSelection { get; set; }

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change
            if (e.PropertyName == nameof(StoryWindow.FixedPanelOpenings))
            {
                StateHasChanged();
            }
        }

        private void HandleSelection(FixedPanelType selectedPanelType)
        {
            switch (selectedPanelType)
            {
                case FixedPanelType.WallPanel:
                    vm.ShowWindow(StoryWindow.ModelsW, StoryWindow.FixedPanelOpenings);
                    break;
                case FixedPanelType.FreeStandingPanel:
                    vm.ShowWindow(StoryWindow.ModelsE, StoryWindow.FixedPanelOpenings);
                    break;
                case FixedPanelType.PanelWithFlipper81:
                    vm.ShowWindow(StoryWindow.Models81, StoryWindow.FixedPanelOpenings);
                    break;
                case FixedPanelType.DoubleWallPanel82:
                    vm.ShowWindow(StoryWindow.Models82, StoryWindow.FixedPanelOpenings);
                    break;
                case FixedPanelType.DoubleAngularPanel84:
                    vm.ShowWindow(StoryWindow.Models84, StoryWindow.FixedPanelOpenings);
                    break;
                case FixedPanelType.DoublePanel85:
                    vm.ShowWindow(StoryWindow.Models85, StoryWindow.FixedPanelOpenings);
                    break;
                case FixedPanelType.TriplePanel88:
                    vm.ShowWindow(StoryWindow.Models88, StoryWindow.FixedPanelOpenings);
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(FixedPanelType)} :{selectedPanelType} is not a Valid Value to Handle the Selecetion");
            }
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
