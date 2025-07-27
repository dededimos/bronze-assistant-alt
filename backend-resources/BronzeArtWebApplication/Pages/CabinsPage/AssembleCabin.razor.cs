using BronzeArtWebApplication.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using BronzeArtWebApplication.Shared.Enums;
using MudBlazor;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.FreeModels;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;

namespace BronzeArtWebApplication.Pages.CabinsPage
{
    public partial class AssembleCabin : ComponentBase
    {
        [Inject] private AssembleCabinViewModel vm { get; set; }
        [Inject] private NavigationManager nm { get; set; }
        [Inject] private ISnackbar snackbar { get; set; }
        [Inject] public CabinStringFactory cabinStringFactory { get; set; }

        /// <summary>
        /// The Current Window Name Passed in the Query String(AddressBar)
        /// </summary>
        [Parameter] public string CurrentWindowName { get; set; }

        /// <summary>
        /// The Synthesis String Passed to the QueryString (Address Bar) - When trying to reach '/AssembleCabinLink'
        /// </summary>
        [Parameter] public string SynthesisString { get; set; }

        protected override void OnInitialized()
        {
            // Subscribes to the Property Changes of the CabinViewModel
            vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Notify that State Has Changed when the User Preferences/Discount Change
            if (e.PropertyName is nameof(AssembleCabinViewModel.PrimaryCabin)
                               or nameof(AssembleCabinViewModel.SecondaryCabin)
                               or nameof(AssembleCabinViewModel.TertiaryCabin))
            {
                StateHasChanged();
            }
            if (e.PropertyName is nameof(AssembleCabinViewModel.CurrentWindow))
            {
                //So that Navigation Manager does not Trigger Location Changed Twice (One from The Back Button and One from the method NavigateTo())
                //Triggers only when the Query string Parameter is Different than the Current Window ,otherwise it means we are already at the correct page
                //Means the Back/Forward Button is Pressed
                if (CurrentWindowName != vm.CurrentWindow.ToString())
                {
                    nm.NavigateTo($"/AssembleCabin/{vm.CurrentWindow.ToString()}");
                }
            }
        }

        //Helps to change Navigation based on the query string
        protected override void OnParametersSet()
        {
            //If a Synthesis Link has been pasted to the Address Bar
            if (string.IsNullOrEmpty(SynthesisString) is false)
            {
                try
                {
                    CabinSynthesis synthesis = cabinStringFactory.GenerateCabinSynthesisFromString(SynthesisString);
                    vm.PassSynthesisToViewModel(synthesis);
                    nm.NavigateTo($"/AssembleCabin/{StoryWindow.CabinPanel}");
                    snackbar.Add(lc.Keys["PassSynthesisCabinFromLink"], Severity.Success);
                }
                catch (Exception)
                {
                    nm.NavigateTo("/AssembleCabin");
                    snackbar.Add(lc.Keys["InvalidLinkCouldNotGenerateCabin"], Severity.Error);
                }
            }
            //ELSE FOLLOW NORMAL PROCEDURE
            //Change the Shown Window only if its different than what is shown already
            else if (CurrentWindowName != vm.CurrentWindow.ToString())
            {
                //If the Name on the Query String is Empty then just show the Start Window or what was previously Stored
                if (string.IsNullOrEmpty(CurrentWindowName))
                {
                    CurrentWindowName = StoryWindow.StartWindow.ToString();
                    foreach (StoryWindow key in vm.IsWindowVisible.Keys)
                    {
                        vm.IsWindowVisible[key] = false;
                        vm.OnPropertyChanged(key.ToString());
                    }
                    vm.ShowWindow(StoryWindow.StartWindow, vm.CurrentWindow);
                }
                //When the User Navigates here from the Menu ToolBar it gets him to the Last shown screen (or to start screen if nothing was shown before)
                else if (CurrentWindowName == "fromMenu")
                {
                    vm.ShowWindow(vm.CurrentWindow, vm.PreviousWindow);
                }
                else
                {
                    bool isParsable = Enum.TryParse(CurrentWindowName, true, out StoryWindow windowToShow);
                    if (isParsable)
                    {
                        foreach (StoryWindow key in vm.IsWindowVisible.Keys)
                        {
                            vm.IsWindowVisible[key] = false;
                            vm.OnPropertyChanged(key.ToString());
                        }
                        if (windowToShow is StoryWindow.CabinPanel && vm.PrimaryCabin.Model is null)
                        {
                            windowToShow = StoryWindow.StartWindow;
                            //So that the Property Change event triggers the Navigation Manager to change the Query string
                            vm.OnPropertyChanged(nameof(AssembleCabinViewModel.CurrentWindow));
                        }
                        vm.ShowWindow(windowToShow, vm.CurrentWindow);
                    }
                }
            }

            base.OnParametersSet();
        }

        #region Z1.Dispose
        public void Dispose()
        {
            //Unsubscribe from the Property Changed Event
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
        #endregion
    }
}
