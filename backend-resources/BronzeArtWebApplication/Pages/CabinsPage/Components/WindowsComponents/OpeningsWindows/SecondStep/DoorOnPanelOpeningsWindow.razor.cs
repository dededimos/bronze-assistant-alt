using BronzeArtWebApplication.Shared.Enums;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using System;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents.OpeningsWindows.SecondStep
{
    public partial class DoorOnPanelOpeningsWindow : ComponentBase, IDisposable
    {
        [Parameter] public EventCallback<DoorOnPanelType> OnDoorTypeSelection { get; set; }

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change
            if (e.PropertyName == nameof(StoryWindow.DoorOnPanelOpenings))
            {
                StateHasChanged();
            }
        }

        private void HandleSelection(DoorOnPanelType selectedDoorType)
        {
            switch (selectedDoorType)
            {
                case DoorOnPanelType.Door34:
                    vm.ShowWindow(StoryWindow.ModelsHB34, StoryWindow.DoorOnPanelOpenings);
                    break;
                case DoorOnPanelType.DoorSidePanel35:
                    vm.ShowWindow(StoryWindow.ModelsHB35, StoryWindow.DoorOnPanelOpenings);
                    break;
                case DoorOnPanelType.DoubleCornerDoor37:
                    vm.ShowWindow(StoryWindow.ModelsHB37, StoryWindow.DoorOnPanelOpenings);
                    break;
                case DoorOnPanelType.DoorStraightPanel40:
                    vm.ShowWindow(StoryWindow.ModelsHB40, StoryWindow.DoorOnPanelOpenings);
                    break;
                case DoorOnPanelType.DoubleDoor43:
                    vm.ShowWindow(StoryWindow.ModelsHB43, StoryWindow.DoorOnPanelOpenings);
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(DoorOnPanelType)} :{selectedDoorType} is not a Valid Value to Handle the Selecetion");
            }
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
