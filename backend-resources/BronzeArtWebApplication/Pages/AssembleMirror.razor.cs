using BronzeArtWebApplication.Components;
using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Helpers;
using Microsoft.AspNetCore.Components;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.JSInterop;
using System.Threading;
using BronzeArtWebApplication.Shared.ViewModels;
using AKSoftware.Localization.MultiLanguages;
using BronzeArtWebApplication.Shared.Models;
using BronzeArtWebApplication.Shared.Services;
using System.Text;
using MirrorsModelsLibrary.Helpers;

namespace BronzeArtWebApplication.Pages
{
    public partial class AssembleMirror : ComponentBase
    {
        /*Notes:
         * All Image Paths & Language Strings are taken from Static Dictionaries . 
         * Usually all Descriptions are in a Dictionary with Enums as the Key for the String , Depending on the Current Culture the YML files contain all the Strings for each Language
         * Image Paths Asscociate with Each item through the Enum Key (Ex. Mirror Sandblast Designs , Mirror Options e.t.c.
         * MirrorDialog.None keeps the Story at the Last Dialog Opened . Whereas IsDialogOpen[Key] = false closes the Dialog
         */

        [Inject] private JSCallService jsCalls { get; set; }

        /// <summary>
        /// The ViewModel for the Assmebled Mirrors Properties
        /// </summary>
        [Inject] private AssembleMirrorViewModel Vm { get; set; }
        [Inject] private MirrorDialogNavigator dialogNav { get; set; }

        /// <summary>
        /// The Query string Provided for a Mirror
        /// </summary>
        [Parameter] public string MirrorQueryString { get; set; }

        /// <summary>
        /// If the App Is still Initializing (Used with Preloading Images && Showing Overlay)
        /// </summary>
        private bool isInitializing = true;

        /// <summary>
        /// When the Pages is Initialized Subscribes to Events
        /// </summary>
        protected override void OnInitialized()
        {
            // Subscribes to the Property Changes of the UserMirror
            Vm.PropertyChanged += Vm_PropertyChanged;
            user.PropertyChanged += User_PropertyChanged;
            isInitializing = false;


        }

        protected override void OnParametersSet()
        {
            if (string.IsNullOrEmpty(MirrorQueryString) is false)
            {
                try
                {
                    navigationManager.NavigateTo("/AssembleMirror");
                    Vm.PassMirrorToVM(MirrorStringFactory.GenerateMirrorFromString(MirrorQueryString));
                    snackbar.Add(languageContainer.Keys["PassMirrorFromLink"], Severity.Success);
                }
                catch (Exception)
                {
                    snackbar.Add(languageContainer.Keys["InvalidLinkCouldNotGenerateMirror"],Severity.Error);
                }
            }
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.CombinedDiscount)               //Changes Visiblity in Table Prices for Prices
                               or nameof(BronzeUser.SelectedPriceIncreaseFactor)    //Changes Visiblity in Table Prices for Prices
                               or nameof(BronzeUser.SelectedVatFactor)              //Changes Visiblity in Table Prices for Vat FootRows 
                               or nameof(BronzeUser.SelectedRetailTheme))           //Changes Visiblity in Table Prices for Vat FootRows
            {
                StateHasChanged();
            }
        }

        /// <summary>
        /// Fires When a Property of the UserMirror Changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //When the Shape of the Mirror Changes Reset the Rest
            if (e.PropertyName == nameof(AssembleMirrorViewModel.Shape))
            {
                Vm.ResetExceptShape();
            }

            // Run AutoCorrect and Inform User for Changes
            if (e.PropertyName is nameof(AssembleMirrorViewModel.Support) ||
                e.PropertyName is nameof(AssembleMirrorViewModel.Light) ||
                e.PropertyName is nameof(AssembleMirrorViewModel.Support))
            {
                CorrectStateInformUser();
            }

            //Notify that State Has Changed when the Mirror changes or CombinedDiscount in the ViewModel
            if (e.PropertyName is "mirror")
            {
                StateHasChanged();
            }
        }

        /// <summary>
        /// Resets Choices
        /// </summary>
        private void ResetChoices()
        {
            //Reset ViewModel
            Vm.ResetViewModel();
            // New Up the Mirror
            dialogNav.ChangeCurrentDialog(MirrorDialog.ChooseShape);
        }


        #region 1.Styles for Components

        //Main Body Option Buttons
        private Variant optionButtonVariant = Variant.Filled;
        private Color ButtonIconColor = Color.Primary;

        #endregion

        #region 2.PAGE Navigation Methods

        /// <summary>
        /// Goes to Printing Page
        /// </summary>
        private void Printpage()
        {
            if (Vm.ValidateMirrorInputs().IsValid)
            {
                snackbar.Clear(); //Clear any SnackBars otherwise they get printed along with the offer
                navigationManager.NavigateTo("/PrintingOffer");
            }
            else
            {

            }
        }

        /// <summary>
        /// Navigates to the Mirror Draw Page
        /// </summary>
        private void GoToMirroDrawPage()
        {
            //Restrict Navigation if The Draw cannot be made : 
            if (Vm.Series is MirrorSeries.NS 
                          or MirrorSeries.N1 
                          or MirrorSeries.N2 
                          or MirrorSeries.ES 
                          or MirrorSeries.EL 
                          or MirrorSeries.P8 
                          or MirrorSeries.P9 
                          or MirrorSeries.ND )
            {
                MessageBoxOptions options = new()
                {
                    Message = languageContainer.Keys["MirrorDrawNotAvailableMessage"],
                    Title = languageContainer.Keys["MirrorDrawNotAvailableTitle"],
                };
                ds.ShowMessageBox(options);
                return;
            }


            if (Vm.ValidateMirrorInputs(false).IsValid)
            {
                navigationManager.NavigateTo("/MirrorDrawPage");
            }
            else
            {
                MessageBoxOptions options = new()
                {
                    Message = languageContainer.Keys["MirrorNotCompleteMessage"],
                    Title = languageContainer.Keys["MirrorNotCompleteTitle"],
                };
                ds.ShowMessageBox(options);
            }
        }

        #endregion

        /// <summary>
        /// Validates the Current Mirror Selection and Informs the User with a snackbar for Any Changes
        /// </summary>
        private void CorrectStateInformUser()
        {
            //Validate Mirror and Act Upon any Incompatible Choices made
            var errorCodesCorrected = Vm.ValidateAndReturnErrorCodes();
            if (errorCodesCorrected != null && errorCodesCorrected.Any())
            {
                foreach (var errorCode in errorCodesCorrected)
                {
                    snackbar.Add(languageContainer.Keys[errorCode], Severity.Info);
                }
            }
        }

        /// <summary>
        /// Copies the Current Link to the Clipboard
        /// </summary>
        private async void CopyLinkToClipboard()
        {
            try
            {
                //Get the Base URI
                string baseURI = await jsCalls.GetBaseUri();
                StringBuilder builder = new(baseURI);
                builder.Append("/AssembleMirror/");
                
                //Convert The Mirror Various Properties Into a String and Pass them along with the BaseUri
                builder.Append(MirrorStringFactory.GenerateStringFromMirror(Vm.GetMirrorObject()));
                await jsCalls.WriteTextToClipboardAsync(builder.ToString());
                
                //Inform the User the Copy was Succesfull
                snackbar.Add($"{languageContainer.Keys["LinkCopiedToClipboard"]}-{builder.ToString().Substring(0,20)}...", Severity.Success);
            }
            catch (Exception)
            {
                snackbar.Add(languageContainer.Keys["CouldNotCopyLinkToClipboard"], Severity.Error);
            }
        }

        /// <summary>
        /// Disposes the Component when we change Pages
        /// Otherwise each time a new instance is made but the old coexists in the background (Memory Leak)
        /// </summary>
        public void Dispose()
        {
            Vm.PropertyChanged -= Vm_PropertyChanged;
            user.PropertyChanged -= User_PropertyChanged;
        }
    }
}
