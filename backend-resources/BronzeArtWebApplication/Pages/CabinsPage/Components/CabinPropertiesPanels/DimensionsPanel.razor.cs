using BronzeArtWebApplication.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoCabins;
using System;
using ShowerEnclosuresModelsLibrary.Enums;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using ShowerEnclosuresModelsLibrary.Helpers;
using MudBlazor;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.CabinPropertiesPanels
{
    public partial class DimensionsPanel : ComponentBase
    {
        [Inject] public IJSRuntime js { get; set; }

        /// <summary>
        /// The Currently Focused Dimension
        /// </summary>
        private string focusedDimension;
        /// <summary>
        /// The Currenty Focused Synthesis Model
        /// </summary>
        private CabinSynthesisModel? focusedSynthesisModel;

        private bool isLength1Focused;
        private bool IsLength1Focused
        {
            get { return isLength1Focused; }
            set
            {
                if (value != isLength1Focused)
                {
                    isLength1Focused = value;
                    //Set the Focused Dimension Name when the Length1 is Being Focused
                    if (value == true)
                    {
                        IsLength2Focused = false;
                        IsLength3Focused = false;
                        //Get the Name from the DescKeys Dictionary
                        focusedDimension = vm.PrimaryCabin?.Model is not null ? PrimaryCabinLengthDescKey[(CabinModelEnum)vm.PrimaryCabin.Model] : "";
                        focusedSynthesisModel = CabinSynthesisModel.Primary;
                    }
                    if (value == false)
                    {
                        focusedDimension = "";
                        focusedSynthesisModel = null;
                    }
                }
            }
        }

        private bool isLength2Focused;
        private bool IsLength2Focused
        {
            get { return isLength2Focused; }
            set
            {
                if (value != isLength2Focused)
                {
                    isLength2Focused = value;
                    //Set the Focused Dimension Name when the Length1 is Being Focused
                    if (value == true)
                    {
                        IsLength1Focused = false;
                        IsLength3Focused = false;
                        //Get the Name from the DescKeys Dictionary
                        focusedDimension = vm.SecondaryCabin?.Model is not null ? SecondaryCabinLengthDescKey[(CabinModelEnum)vm.SecondaryCabin.Model] : "";
                        focusedSynthesisModel = CabinSynthesisModel.Secondary;
                    }
                    if (value == false)
                    {
                        focusedDimension = "";
                        focusedSynthesisModel = null;
                    }
                }
            }
        }

        private bool isLength3Focused;
        private bool IsLength3Focused
        {
            get { return isLength3Focused; }
            set
            {
                if (value != isLength3Focused)
                {
                    isLength3Focused = value;
                    //Set the Focused Dimension Name when the Length1 is Being Focused
                    if (value == true)
                    {
                        IsLength1Focused = false;
                        IsLength2Focused = false;
                        //Get the Name from the DescKeys Dictionary
                        focusedDimension = vm.TertiaryCabin?.Model is not null ? TertiaryCabinLengthDescKey[(CabinModelEnum)vm.TertiaryCabin.Model] : "";
                        focusedSynthesisModel = CabinSynthesisModel.Tertiary;
                    }
                    if (value == false)
                    {
                        focusedDimension = "";
                        focusedSynthesisModel = null;
                    }
                }
            }
        }

        //References for the Basic DimensionsMenu PopUp
        private MudMenu menuPrimary = new();
        private MudMenu menuSecondary = new();
        private MudMenu menuTertiary = new();
        private MudMenu menuHeight = new();
        private EventArgs args = new(); // Helps as a parameter to Open the Menus


        //EXPLANATION OF IS FLIPPED WITH DIRECTION
        //1.The IsFlipped always starts as false (which means the sketches always start NOTFLIPPED)
        //2.And All the Default Directions have the Default Sketches
        //3.The Direction cannot change from within code other than the user selection
        //4.Whenever the User presses to change Direction IsFlipped becomes false or true
        //5.Thus changing the Direction to the Appropriate Opposite or Undefined value

        /// <summary>
        /// Wheather the Sketch and SVG of the Current Draw Are Flipped from the Original
        /// </summary>
        private bool _isFlipped;
        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                //Is Flipped is also checked on initilization so that it matches the Direction of Cabin
                if (value != _isFlipped)
                {
                    _isFlipped = value;
                    //If Flipped Change also the Synthesis Direction (The Property Changes of the Primary Cabins e.t.c. will inform for the changes)
                    vm.ChangeSynthesisDirection();
                }
            }
        }

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
            //If the Current Direction is not the Default Direction then the Photo Should be Flipped
            if (DefaultPrimaryCabinDirection[vm.SelectedCabinDraw] != vm.PrimaryCabin.Direction)
            {
                _isFlipped = true; //We must change only the backing field so that the setter does not run
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(AssembleCabinViewModel.PrimaryCabin)
                               or nameof(AssembleCabinViewModel.NumberOfActiveCabinSides))
            {
                StateHasChanged();
            }
        }

        //Wheather the Step Dialog is Open
        private bool isStepDimensionsDialogVisible;
        private void HandleClickStepDimensions()
        {
            isStepDimensionsDialogVisible = true;
        }

        /// <summary>
        /// On Dialog Closure Removes steps from the Primary/Secondary/Tertiary if both Length/Height are zero
        /// </summary>
        private void RemoveZeroValueSteps()
        {
            if (vm.PrimaryCabin.CabinObject?.HasStep == true)
            {
                if (vm.PrimaryCabin.InputStepLength is 0 or null || vm.PrimaryCabin.InputStepHeight is 0 or null)
                {
                    vm.PrimaryCabin.RemoveStep();
                }
            }
            if (vm.SecondaryCabin.CabinObject?.HasStep == true)
            {
                if (vm.SecondaryCabin.InputStepLength is 0 or null || vm.SecondaryCabin.InputStepHeight is 0 or null)
                {
                    vm.SecondaryCabin.RemoveStep();
                }
            }
            if (vm.TertiaryCabin.CabinObject?.HasStep == true)
            {
                if (vm.TertiaryCabin.InputStepLength is 0 or null || vm.TertiaryCabin.InputStepHeight is 0 or null)
                {
                    vm.TertiaryCabin.RemoveStep();
                }
            }
        }


        //DEPRECATED
        //async Task SetFocusToOnContainerClick(string idToSetFocusTo, string idOfContainerClicked)
        //{
        //    await js.InvokeVoidAsync("SetFocusToOnContainerClick", idToSetFocusTo, idOfContainerClicked);
        //}

    }
}
