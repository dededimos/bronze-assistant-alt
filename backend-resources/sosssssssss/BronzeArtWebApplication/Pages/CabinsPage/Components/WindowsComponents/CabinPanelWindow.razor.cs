using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Models;
using BronzeArtWebApplication.Shared.Services;
using BronzeArtWebApplication.Shared.ViewModels;
using CommonHelpers.Utilities.CustomComparers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BronzeArtWebApplication.Shared.Helpers.StaticInfoCabins;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.WindowsComponents
{
    public partial class CabinPanelWindow : ComponentBase, IDisposable
    {
        [Inject] public JSCallService jsCalls { get; set; }
        [Inject] public ISnackbar snackbar { get; set; }
        [Inject] public CabinStringFactory cabinStringFactory { get; set; }

        private readonly List<(string, string)> validationErrorCodes = new();

        /// <summary>
        /// Wheather the Photo is selected (Negates the Draw)
        /// </summary>
        private bool isPhotoActive;

        /// <summary>
        /// Wheather the Draw is Selected (Negates the Photo)
        /// </summary>
        private bool isDrawActive;

        protected override void OnInitialized()
        {
            vm.PropertyChanged += Vm_PropertyChanged;
            user.PropertyChanged += User_PropertyChanged;
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.SelectedAppMode)
                               or nameof(BronzeUser.SelectedRetailTheme)
                               or nameof(BronzeUser.IsPowerUser))
            {
                StateHasChanged();
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //The Enum Value Name is Used as the Property Change
            if (e.PropertyName is nameof(StoryWindow.CabinPanel)
                               or nameof(AssembleCabinViewModel.PrimaryCabin)
                               or nameof(AssembleCabinViewModel.SecondaryCabin)
                               or nameof(AssembleCabinViewModel.TertiaryCabin))
            {
                //This Forces Showing Photo or Draw
                if (vm.PrimaryCabin.Model is CabinModelEnum.Model9C)
                {
                    isPhotoActive = true;
                    isDrawActive = false;
                }
                else
                {
                    isPhotoActive = false;
                    isDrawActive = true;
                }
                StateHasChanged();
            }
        }

        /// <summary>
        /// Weather the Cabins Selection is Valid , if it is It also Calculates Glasses
        /// </summary>
        /// <returns>Terue if valid , false if not</returns>
        private bool IsValidSelection()
        {
            bool isValid = false;

            bool isValidPrimary = vm.PrimaryCabin.IsSelectionValid();
            List<(string, string)> errorsPrimary = vm.PrimaryCabin.GetValidationErrorCodes().Select(error => (vm.PrimaryCabin.Model != null ? PrimaryCabinLengthDescKey[(CabinModelEnum)vm.PrimaryCabin.Model] : "", error)).ToList();

            //When Secondary & Tertiary models are null there are not Inside Draw Selection so their models are Valid whatever they have
            bool isValidSecondary = vm.SecondaryCabin.Model != null ? vm.SecondaryCabin.IsSelectionValid() : true;
            //Get Errors when non Valid
            List<(string, string)> errorsSecondary = isValidSecondary ? new() : vm.SecondaryCabin.GetValidationErrorCodes().Select(error => (vm.SecondaryCabin.Model != null ? SecondaryCabinLengthDescKey[(CabinModelEnum)vm.SecondaryCabin.Model] : "", error)).ToList();

            bool isValidTertiary = vm.TertiaryCabin.Model != null ? vm.TertiaryCabin.IsSelectionValid() : true;
            //Get Errors when non Valid
            List<(string, string)> errorsTertiary = isValidTertiary ? new() : vm.TertiaryCabin.GetValidationErrorCodes().Select(error => (vm.TertiaryCabin.Model != null ? TertiaryCabinLengthDescKey[(CabinModelEnum)vm.TertiaryCabin.Model] : "", error)).ToList(); ;

            isValid = isValidPrimary && isValidSecondary && isValidTertiary;
            validationErrorCodes.Clear();
            if (isValid is false)
            {
                validationErrorCodes.AddRange(errorsPrimary);
                validationErrorCodes.AddRange(errorsSecondary);
                validationErrorCodes.AddRange(errorsTertiary);
            }
            return isValid;
        }

        /// <summary>
        /// Goes to Printing Page
        /// </summary>
        private async void PrintpageAsync()
        {
            await SaveSynthesisToRecentsAsync(); //Save Current Synthesis to Recents
            snackbar.Clear(); // Clears any present snackbars , otherwise they print in the Offer
            navigationManager.NavigateTo("/PrintingOfferCabins");
        }

        /// <summary>
        /// Copies the Current Link to the Clipboard
        /// </summary>
        private async void CopyLinkToClipboardAsync()
        {
            //Save Copied
            try
            {
                //Get the Base URI
                string baseURI = await jsCalls.GetBaseUri();
                StringBuilder builder = new(baseURI);
                builder.Append("/AssembleCabinLink/");

                //Convert The Mirror Various Properties Into a String and Pass them along with the BaseUri
                builder.Append(cabinStringFactory.GenerateStringFromSynthesis(vm.Synthesis));
                await jsCalls.WriteTextToClipboardAsync(builder.ToString());

                //Inform the User the Copy was Succesfull
                snackbar.Add($"{lc.Keys["LinkCopiedToClipboard"]}-{builder.ToString().Substring(0, 20)}...", Severity.Success);
                await SaveSynthesisToRecentsAsync();
            }
            catch (Exception)
            {
                snackbar.Add(lc.Keys["CouldNotCopyLinkToClipboard"], Severity.Error);
            }
        }

        /// <summary>
        /// Saves the Current Synthesis to the Recents in Local Storage
        /// </summary>
        /// <returns></returns>
        private async Task SaveSynthesisToRecentsAsync(string storageListName = "RecentSynthesis", bool withMessage = false)
        {
            try
            {
                //Sorted List of the recent Values
                IComparer<DateTime> decendingDateComparer = new DateDescendingComparer();
                SortedList<DateTime, string> recentSynthesis = new(decendingDateComparer);

                //Set the New Recent item made
                string newRecent = cabinStringFactory.GenerateStringFromSynthesis(vm.Synthesis);

                //Try to get what its already in Storage else List will just be empty.
                if (await storage.ContainKeyAsync(storageListName))
                {
                    recentSynthesis = await storage.GetItemAsync<SortedList<DateTime, string>>(storageListName);
                }

                // If Value Already Exists do not Add 
                if (recentSynthesis.ContainsValue(newRecent))
                {
                    if (withMessage)
                    {
                        snackbar.Add(lc.Keys["SavedSelectionAlreadyExists"], Severity.Warning);
                    }
                    return;
                }


                //If recent has less than 20 items add the new recent
                if (recentSynthesis.Count < 25)
                {
                    recentSynthesis.Add(DateTime.Now, newRecent);
                }
                else // remove the oldest and add the newest
                {
                    recentSynthesis.Remove(recentSynthesis.Keys.Min());
                    recentSynthesis.Add(DateTime.Now, newRecent);
                }
                await storage.SetItemAsync(storageListName, recentSynthesis);
                if (withMessage)
                {
                    snackbar.Add(lc.Keys["CabinSelectionSaved"], Severity.Success);
                }
            }
            catch (Exception ex)
            {
                if (withMessage)
                {
                    snackbar.Add(lc.Keys["FailedToSaveCabinSelection"], Severity.Error);
                }
                Console.WriteLine("Failed to Save Print to Recent/Saved Items");
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Copies the Table to the clipboard
        /// </summary>
        /// <returns></returns>
        private async Task CopyTable()
        {
            int[] columnsToRemove = Array.Empty<int>();
            int[] rowsToRemove = Array.Empty<int>();
            int numberOfItems = vm.GetProductsList().Count;
            if (user.SelectedAppMode == BronzeAppMode.Guest)
            {
                columnsToRemove = new int[] { };
                rowsToRemove = new int[] { };
            }
            else if (user.SelectedAppMode == BronzeAppMode.Retail)
            {
                if (user.RetailTheme == RetailModeTheme.Lakiotis)
                {
                    int lastRow = numberOfItems + 1;//1 header row
                    int lastRowIndex = lastRow - 1;
                    rowsToRemove = new int[] { lastRowIndex };
                }
                else
                {
                    int lastRow = numberOfItems + 1 + 3;//1 header row , 3 footerRows
                    int lastRowIndex = lastRow - 1;
                    rowsToRemove = new int[] { lastRowIndex, lastRowIndex - 1, lastRowIndex - 2 };
                    if (user.CombinedDiscountCabin != 0)
                    {
                        columnsToRemove = new int[] { 5 }; //info Row when there is discount there is one more row with written off pricing
                    }
                    else
                    {
                        columnsToRemove = new int[] { 4 }; //info Row
                    }
                }
            }
            else if (user.SelectedAppMode == BronzeAppMode.Wholesale)
            {
                if (user.RetailTheme != RetailModeTheme.Lakiotis && !user.IsPowerUser)
                {
                    int lastRow = numberOfItems + 1 + 2;//1 header row , 2 footerRows
                    int lastRowIndex = lastRow - 1;
                    rowsToRemove = new int[] { lastRowIndex, lastRowIndex - 1 };
                    // no columns here
                }
                else if (user.RetailTheme == RetailModeTheme.Lakiotis || user.IsPowerUser)
                {
                    int lastRow = numberOfItems + 1 + 2;//1 header row , 2 footerRows
                    int lastRowIndex = lastRow - 1;
                    rowsToRemove = new int[] { lastRowIndex, lastRowIndex - 1 };

                    columnsToRemove = new int[] { 4,5 }; //info Row
                }
            }

            try
            {
                await jsCalls.CopyTableOfElementToClipboard(CabinsPricingTable.TableId, columnsToRemove, rowsToRemove);
                snackbar.Add(lc.Keys["TableCopied"], Severity.Success, (o) => o.CloseAfterNavigation = true);
            }
            catch (Exception ex)
            {
                snackbar.Add("Failed to Copy Table", Severity.Warning, (o) => o.CloseAfterNavigation = true);
                Console.WriteLine("Failed to Copy Table");
                Console.WriteLine(ex.Message);
            }
        }

        public void Dispose()
        {
            vm.PropertyChanged -= Vm_PropertyChanged;
            user.PropertyChanged -= User_PropertyChanged;
        }
    }
}
