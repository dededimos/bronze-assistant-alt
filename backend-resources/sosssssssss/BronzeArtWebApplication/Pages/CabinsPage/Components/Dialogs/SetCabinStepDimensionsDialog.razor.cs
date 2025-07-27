using BronzeArtWebApplication.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoCabins;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.Dialogs
{
    public partial class SetCabinStepDimensionsDialog : ComponentBase, IDisposable
    {
        /// <summary>
        /// Helper Method to Set the Parameter instead of using its setter...
        /// </summary>
        /// <param name="newIsVisible"></param>
        /// <returns></returns>
        protected async Task OnIsVisibleChangedAsync(bool newIsVisible)
        {
            if (IsVisible != newIsVisible)
            {
                IsVisible = newIsVisible;
                await IsVisibleChanged.InvokeAsync(newIsVisible);
                if (!newIsVisible)
                {
                    await OnClosingDialog.InvokeAsync();
                }
            }
        }

        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
        [Parameter] public EventCallback OnClosingDialog { get; set; }

        /// <summary>
        /// The Component must know how many MudItems it will Render so that we can Manipulate the
        /// xs,sm,md,lg attributes -- Passed as a Parameter otherwise the lg/md are not calculated on First Render
        /// </summary>
        [Parameter] public int ActiveCabinSides { get; set; }

        #region 1.Focused Dimensions
        //When the User Focuses in one of the Numeric Fileds or Sliders the Focused Dimension Name is Changed
        //The Focused Dimension Named is Passed to the Measure Draw Components Highlighting the Dimension Arrow
        private bool isStepLength1Focused;
        private bool IsStepLength1Focused
        {
            get { return isStepLength1Focused; }
            set
            {
                if (value != isStepLength1Focused)
                {
                    isStepLength1Focused = value;
                    //Set the Focused Dimension Name when the Length1 is Being Focused
                    if (value == true)
                    {
                        focusedDimensionPrimary = "StepLength";
                    }
                    if (value == false)
                    {
                        focusedDimensionPrimary = "";
                    }
                }
            }
        }

        private bool isStepHeight1Focused;
        private bool IsStepHeight1Focused
        {
            get { return isStepHeight1Focused; }
            set
            {
                if (value != isStepHeight1Focused)
                {
                    isStepHeight1Focused = value;
                    //Set the Focused Dimension Name when the Length1 is Being Focused
                    if (value == true)
                    {
                        focusedDimensionPrimary = "StepHeight";
                    }
                    if (value == false)
                    {
                        focusedDimensionPrimary = "";
                    }
                }
            }
        }

        private bool isStepLength2Focused;
        private bool IsStepLength2Focused
        {
            get { return isStepLength2Focused; }
            set
            {
                if (value != isStepLength2Focused)
                {
                    isStepLength2Focused = value;
                    //Set the Focused Dimension Name when the Length1 is Being Focused
                    if (value == true)
                    {
                        focusedDimensionSecondary = "StepLength";
                    }
                    if (value == false)
                    {
                        focusedDimensionSecondary = "";
                    }
                }
            }
        }

        private bool isStepHeight2Focused;
        private bool IsStepHeight2Focused
        {
            get { return isStepHeight2Focused; }
            set
            {
                if (value != isStepHeight2Focused)
                {
                    isStepHeight2Focused = value;
                    //Set the Focused Dimension Name when the Length1 is Being Focused
                    if (value == true)
                    {
                        focusedDimensionSecondary = "StepHeight";
                    }
                    if (value == false)
                    {
                        focusedDimensionSecondary = "";
                    }
                }
            }
        }

        private bool isStepLength3Focused;
        private bool IsStepLength3Focused
        {
            get { return isStepLength3Focused; }
            set
            {
                if (value != isStepLength3Focused)
                {
                    isStepLength3Focused = value;
                    //Set the Focused Dimension Name when the Length1 is Being Focused
                    if (value == true)
                    {
                        focusedDimensionTertiary = "StepLength";
                    }
                    if (value == false)
                    {
                        focusedDimensionTertiary = "";
                    }
                }
            }
        }

        private bool isStepHeight3Focused;
        private bool IsStepHeight3Focused
        {
            get { return isStepHeight3Focused; }
            set
            {
                if (value != isStepHeight3Focused)
                {
                    isStepHeight3Focused = value;
                    //Set the Focused Dimension Name when the Length1 is Being Focused
                    if (value == true)
                    {
                        focusedDimensionTertiary = "StepHeight";
                    }
                    if (value == false)
                    {
                        focusedDimensionTertiary = "";
                    }
                }
            }
        }

        private string focusedDimensionPrimary; //The Dimension Being Focused
        private string focusedDimensionSecondary; //The Dimension Being Focused
        private string focusedDimensionTertiary; //The Dimension Being Focused

        #endregion

        /// <summary>
        /// The Component must know which Models cannot take step so to render a display message the  
        /// (bool,bool,bool) = Primary.CanHaveStep , Secondary.CanHaveStep e.t.c.
        /// </summary>
        private (bool, bool, bool) canHaveStep;

        //If the Photos & Steps Should should be Flipped
        private bool _isFlipped;

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
            base.OnInitialized();
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(AssembleCabinViewModel.PrimaryCabin))
            {
                //If the Current Direction is not the Default Direction then the Photo Should be Flipped
                if (DefaultPrimaryCabinDirection[vm.SelectedCabinDraw] != vm.PrimaryCabin.Direction)
                {
                    _isFlipped = true;
                }
                else
                {
                    _isFlipped = false;
                }
                StateHasChanged();
            }
        }

        protected override void OnParametersSet()
        {
            //Set which parts of the CabinDraw can take Step the rest will render a message they cannot
            //canHaveStep = ShowerEnclosuresModelsLibrary.Helpers.HelperMethods.CabinDrawCanHaveStep[vm.SelectedCabinDraw];
            canHaveStep = (vm.PrimaryCabin?.CabinObject?.Constraints?.CanHaveStep ?? false,
                           vm.SecondaryCabin?.CabinObject?.Constraints?.CanHaveStep ?? false,
                           vm.TertiaryCabin?.CabinObject?.Constraints?.CanHaveStep ?? false);
            //If the Current Direction is not the Default Direction then the Photo Should be Flipped
            if (DefaultPrimaryCabinDirection[vm.SelectedCabinDraw] != vm.PrimaryCabin.Direction)
            {
                _isFlipped = true;
            }
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
        }
    }
}
