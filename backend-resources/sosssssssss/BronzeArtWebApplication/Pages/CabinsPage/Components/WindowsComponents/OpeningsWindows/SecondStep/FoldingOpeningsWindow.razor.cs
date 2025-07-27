using BronzeArtWebApplication.Shared.Enums;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.CabinCategories;
using System;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents.OpeningsWindows.SecondStep
{
    public partial class FoldingOpeningsWindow : ComponentBase, IDisposable
    {
        [Parameter] public EventCallback<FoldingDoorType> OnFoldingTypeSelection { get; set; }

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change
            if (e.PropertyName == nameof(StoryWindow.FoldingOpenings))
            {
                StateHasChanged();
            }
        }

        private void HandleSelection(FoldingDoorType selectedFoldingDoorType)
        {
            switch (selectedFoldingDoorType)
            {
                case FoldingDoorType.SingleDoor44:
                    vm.ShowWindow(StoryWindow.ModelsP44, StoryWindow.FoldingOpenings);
                    break;
                case FoldingDoorType.DoubleDoorCorner46:
                    vm.ShowWindow(StoryWindow.ModelsP46, StoryWindow.FoldingOpenings);
                    break;
                case FoldingDoorType.DoubleDoorStraight48:
                    vm.ShowWindow(StoryWindow.ModelsP48, StoryWindow.FoldingOpenings);
                    break;
                case FoldingDoorType.SingleDoorSidePanel45:
                    vm.ShowWindow(StoryWindow.ModelsP45, StoryWindow.FoldingOpenings);
                    break;
                case FoldingDoorType.SingleDoorStraightPanel47:
                    vm.ShowWindow(StoryWindow.ModelsP47, StoryWindow.FoldingOpenings);
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(FoldingDoorType)} :{selectedFoldingDoorType} is not a Valid Value to Handle the Selecetion");
            }
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
