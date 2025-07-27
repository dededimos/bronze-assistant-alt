using BronzeArtWebApplication.Shared.Enums;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using System;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents.OpeningsWindows.SecondStep
{
    public partial class StandardDoorOpeningsWindow : ComponentBase, IDisposable
    {
        [Parameter] public EventCallback<StandardDoorType> OnDoorTypeSelection { get; set; }

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change
            if (e.PropertyName == nameof(StoryWindow.StandardDoorOpenings))
            {
                StateHasChanged();
            }
        }

        private void HandleSelection(StandardDoorType selectedDoorType)
        {
            switch (selectedDoorType)
            {
                case StandardDoorType.Door3151:
                    vm.ShowWindow(StoryWindow.ModelsB3151, StoryWindow.StandardDoorOpenings);
                    break;
                case StandardDoorType.DoorSidePanel3252:
                    vm.ShowWindow(StoryWindow.ModelsB3252, StoryWindow.StandardDoorOpenings);
                    break;
                case StandardDoorType.DoubleCornerDoor3353:
                    vm.ShowWindow(StoryWindow.ModelsB3353, StoryWindow.StandardDoorOpenings);
                    break;
                case StandardDoorType.DoorStraightPanel3859:
                    vm.ShowWindow(StoryWindow.ModelsB3859, StoryWindow.StandardDoorOpenings);
                    break;
                case StandardDoorType.DoubleDoor4161:
                    vm.ShowWindow(StoryWindow.ModelsB4161, StoryWindow.StandardDoorOpenings);
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(StandardDoorType)} :{selectedDoorType} is not a Valid Value to Handle the Selecetion");
            }
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
