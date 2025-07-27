using BronzeArtWebApplication.Shared.Enums;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using System;
using System.Collections.Generic;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoCabins;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents.SeriesWindows
{
    public partial class ModelSelectionWindow : ComponentBase, IDisposable
    {
        [Parameter] public EventCallback<CabinModelEnum> OnModelSelection { get; set; }

        /// <summary>
        /// According to the Window that need to be Shown a Model List is Populated on the Initilized Lifecycle Method
        /// </summary>
        [Parameter] public StoryWindow ModelWindowToRender { get; set; }

        //The ModelsList to Populate
        private List<CabinModelEnum> modelsList;

        /// <summary>
        /// The Model Selected
        /// </summary>
        private CabinModelEnum? selectedModel;
        private bool isDialogVisible;

        protected override void OnInitialized()
        {
            switch (ModelWindowToRender)
            {
                case StoryWindow.SeriesB6000:
                    modelsList = ModelsB6000;
                    break;
                case StoryWindow.SeriesFree:
                    modelsList = ModelsFree;
                    break;
                case StoryWindow.SeriesHotel8000:
                    modelsList = ModelsHotel8000;
                    break;
                case StoryWindow.SeriesInox304:
                    modelsList = ModelsInox304;
                    break;
                case StoryWindow.SeriesNiagara6000:
                    modelsList = ModelsNiagara6000;
                    break;
                case StoryWindow.SeriesSmart:
                    modelsList = new() { CabinModelEnum.ModelWS };
                    break;
                default:
                    throw new ArgumentException($"Invalid {nameof(StoryWindow)} :{ModelWindowToRender} is not Valid Parameter for {nameof(ModelSelectionWindow)} Component");
            }

            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change Name
            if (e.PropertyName == ModelWindowToRender.ToString())
            {
                StateHasChanged();
            }
        }

        private void HandleModelSelection(CabinModelEnum selectedModel)
        {
            //DEPRECATED -- THE DRAW CHOOSING IS ENOUGH
            //vm.PrimaryCabin.Model = selectedModel; //Pass the Selected Model as Primary

            // Open Dialog to select available Draw for the Selected Model
            this.selectedModel = selectedModel;
            isDialogVisible = true;
        }

        private void HandleDrawNoSelection(CabinDrawNumber selectedDrawNo)
        {
            vm.SelectedCabinDraw = selectedDrawNo;
            vm.ShowWindow(StoryWindow.CabinPanel, ModelWindowToRender);
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
