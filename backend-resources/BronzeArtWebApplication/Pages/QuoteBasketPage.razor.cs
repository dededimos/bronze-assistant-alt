using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using BronzeArtWebApplication.Components.Various.VariousDialogs;
using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Services;
using BronzeArtWebApplication.Shared.ViewModels.AccessoriesPageViewModels;
using BronzeRulesPricelistLibrary.Models.Priceables.AccessoriesPriceables;
using ClosedXML.Excel;
using CommonHelpers;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Pages
{
    public partial class QuoteBasketPage : IDisposable
    {

        private readonly Variant editTemplateVariant = Variant.Outlined;
        private readonly Margin editTemplateMargin = Margin.Dense;
        private bool isOverlayOpen;
        private bool imagesDisabled;
        private bool _drawerOpen = false;
        private bool includeSheetsInQuote = false;
        private decimal extraToolsTotalDiscount;
        private int indexOfJustSavedEditedLine = -1;
        private int indexOfJustScrappedEditedLine = -1;
        private Timer timerHideCheckmark;
        private Timer timerHideXCheckmark;
        private bool isBusy;
        private TaskProgressReport taskProgress = new(0,0,"");
        /// <summary>
        /// To Cancel Operations
        /// </summary>
        private CancellationTokenSource cts;
        /// <summary>
        /// Cancel a running Task
        /// </summary>
        private void CancelRunningTask() => cts?.Cancel();
        
        private MudAutocomplete<BathroomAccessory> searchByCodeComplete;

        private UserAccessoriesOptions foundFromSearch;
        protected override void OnInitialized()
        {
            //The First TimeoutInfinite tells after how much time to execute the Callback . In the begining we never want to execute it checkmarks are hidden
            //The Second TimeoutInfinite tells how much time it needs to reexecute the timer , which is never , we only execute it once
            timerHideCheckmark = new(HideCheckMarkAfterTimePasses, null, Timeout.Infinite, Timeout.Infinite);
            timerHideXCheckmark = new(HideXCheckMarkAfterTimePasses, null, Timeout.Infinite, Timeout.Infinite);
            saveService.BasketRestored += SaveService_BasketRestored;
            Basket.PropertyChanged += Basket_PropertyChanged;
        }

        private void Basket_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }

        private void SaveService_BasketRestored(object sender, EventArgs e)
        {
            StateHasChanged();
        }

        /// <summary>
        /// Closes the Edit Row and Scrap Changes if the Overlay has already closed (User Clicked outside of edit Line)
        /// </summary>
        private void ScrapChangesIfOverlayAlreadyClosed()
        {
            if (isOverlayOpen == false)
            {
                var indexOfEditedItem = Basket.ItemUnderEdit != null ? Basket.Products.IndexOf(Basket.ItemUnderEdit) : -1;
                if (indexOfEditedItem != -1)
                {
                    DisplayXCheckMarkAfterEdit(indexOfEditedItem);
                }
                Basket.FinishEditItem(true);
                StateHasChanged();
            }
        }
        private void SaveEditsAndClose()
        {
            var indexOfEditedItem = Basket.ItemUnderEdit != null ? Basket.Products.IndexOf(Basket.ItemUnderEdit) : -1;
            if (indexOfEditedItem != -1)
            {
                DisplayCheckMarkAfterEdit(indexOfEditedItem);
            }
            Basket.FinishEditItem(false);
            isOverlayOpen = false;
        }

        private void DisplayCheckMarkAfterEdit(int indexOfEditedLine)
        {
            indexOfJustSavedEditedLine = indexOfEditedLine;
            StateHasChanged();
            //hide after 2s
            timerHideCheckmark.Change(2000, Timeout.Infinite);
        }
        private void DisplayXCheckMarkAfterEdit(int indexOfEditedLine)
        {
            indexOfJustScrappedEditedLine = indexOfEditedLine;
            timerHideXCheckmark.Change(2000, Timeout.Infinite);
            StateHasChanged();
        }
        private void HideCheckMarkAfterTimePasses(object state)
        {
            InvokeAsync(() =>
            {
                indexOfJustSavedEditedLine = -1;
                StateHasChanged();
            });
        }
        private void HideXCheckMarkAfterTimePasses(object state)
        {
            InvokeAsync(() =>
            {
                indexOfJustScrappedEditedLine = -1;
                StateHasChanged();
            });
        }

        private void RevertEditsAndClose()
        {
            var indexOfEditedItem = Basket.ItemUnderEdit != null ? Basket.Products.IndexOf(Basket.ItemUnderEdit) : -1;
            if (indexOfEditedItem != -1)
            {
                DisplayXCheckMarkAfterEdit(indexOfEditedItem);
            }
            Basket.FinishEditItem(true);
            isOverlayOpen = false;
        }


        /// <summary>
        /// Opens the Edit Overlay
        /// </summary>
        private void OpenOverlay()
        {
            isOverlayOpen = true;
            StateHasChanged();
        }
        /// <summary>
        /// Sets the Edit Row
        /// </summary>
        /// <param name="item">The Item under the Row Being Edited</param>
        private void SetEditItem(BasketItemViewModel item)
        {
            //Close Side Menu if Open
            _drawerOpen = false;
            Basket.StartEditItem(item);
            OpenOverlay();
        }

        private async Task SaveCurrentBasket()
        {
            var result = await message.OpenInputBasketNameAsync(Basket.CurrentBasketName);
            if (result == string.Empty)
            {
                return;
            }
            else
            {
                Basket.CurrentBasketName = result;
                var isSaved = await saveService.SaveBasket();
                if (isSaved)
                {
                    snackbar.Add(Lc.Keys["SaveSuccess"], Severity.Success, (options) => options.CloseAfterNavigation = true);
                }
                else
                {
                    snackbar.Add(Lc.Keys["SaveFailed"], Severity.Warning, (options) => options.CloseAfterNavigation = true);
                }
            }
        }
        private async Task RemoveOnUserConfirm(BasketItemViewModel item)
        {
            var result = await message.QuestionAsync(Lc.Keys["RemoveBasketItem"], Lc.Keys["RemoveBasketItemMessage"], Lc.Keys[IMessageService.DialogOk], Lc.Keys[IMessageService.DialogCancel]);
            if (result == MessageResult.Ok)
            {
                Basket.RemoveProduct(item);
            }
            // close any edits if still open
            isOverlayOpen = false;
        }
        private async Task DeleteBasket()
        {
            var result = await message.QuestionAsync(Lc.Keys["Info"], Lc.Keys["DeleteBasketQuestion"], Lc.Keys[IMessageService.DialogYes], Lc.Keys[IMessageService.DialogNo]);
            if (result == MessageResult.Cancel) return;
            else Basket.Products.Clear();
        }
        private async Task ClearRetailDiscounts()
        {
            var result = await message.QuestionAsync(Lc.Keys["Info"], Lc.Keys["ClearDiscountsQuestion"], Lc.Keys[IMessageService.DialogYes], Lc.Keys[IMessageService.DialogNo]);
            if (result == MessageResult.Cancel) { return; }
            else Basket.ClearRetailDiscounts();
        }
        private async Task DisableRulesClearDiscSetCatPrice()
        {
            var messageTxt = $"1.{Lc.Keys["DisablesRules"]}{Environment.NewLine}2.{Lc.Keys["SetsCataloguePrice"]}{Environment.NewLine}3.{Lc.Keys["ClearsDiscounts"]}{Environment.NewLine}{Environment.NewLine}{Lc.Keys["DoYouWantToContinue"]}";
            var result = await message.QuestionAsync(Lc.Keys["Info"], messageTxt, Lc.Keys[IMessageService.DialogYes], Lc.Keys[IMessageService.DialogNo]);
            if (result == MessageResult.Cancel) { return; }
            else Basket.DisableRulesApplyCataloguePriceToAll(true);
        }
        private async Task RevertsPricingToDefaultsWholesale()
        {
            var messageTxt = $"{Lc.Keys["RevertsPricingToDefault"]}{Environment.NewLine}{Environment.NewLine}{Lc.Keys["DoYouWantToContinue"]}";
            var result = await message.QuestionAsync(Lc.Keys["Info"], messageTxt, Lc.Keys[IMessageService.DialogYes], Lc.Keys[IMessageService.DialogNo]);
            if (result == MessageResult.Cancel) { return; }
            else Basket.RevertAllItemsToDefaultsWholeSale();
        }
        private async Task ApplyDiscountToAllItems()
        {
            var msg = $"{Lc.Keys["ApplySelectedDiscountToAllItems"]}({extraToolsTotalDiscount:0.00}%) ?{Environment.NewLine}{Environment.NewLine}{Lc.Keys["DoYouWantToContinue"]}";
            var result = await message.QuestionAsync(Lc.Keys["Info"], msg, Lc.Keys[IMessageService.DialogYes], Lc.Keys[IMessageService.DialogNo]);
            if (result == MessageResult.Cancel) { return; }
            if (user.SelectedAppMode == BronzeAppMode.Retail)
            {
                Basket.ApplyRetailTotalDiscountToAll(extraToolsTotalDiscount);
            }
            else if (user.SelectedAppMode == BronzeAppMode.Wholesale)
            {
                Basket.ApplyWholeSaleDiscountToAll(extraToolsTotalDiscount);
            }
            //Otherwise changes do not propagate on print template.
            await InvokeAsync(StateHasChanged);
        }
        private async Task ApplyNewAccessoriesOptions()
        {
            if (foundFromSearch is null)
            {
                await message.InfoAsync(Lc.Keys["Info"], Lc.Keys["SelectedOptionsAreEmptyPrompt"]);
                return;
            }

            var msg = $"{Lc.Keys["ApplyNewOptionsQuestion1"]}{foundFromSearch.DescriptionInfo.Name}{Environment.NewLine}{Lc.Keys["ApplyNewOptionsQuestion2"]}{Environment.NewLine}{Lc.Keys["ApplyNewOptionsQuestion3"]}{Environment.NewLine}{Lc.Keys["ApplyNewOptionsQuestion4"]}{Environment.NewLine}{Lc.Keys["ApplyNewOptionsQuestion5"]}{Environment.NewLine}{Lc.Keys["ApplyNewOptionsQuestion6"]}";
            var result = await message.QuestionAsync(Lc.Keys["Info"], msg, Lc.Keys[IMessageService.DialogYes], Lc.Keys[IMessageService.DialogNo]);
            if (result == MessageResult.Cancel) { return; }
            Basket.ChangeSelectedOptions(foundFromSearch);
        }
        private async Task AddAccessoryFromSearch(BathroomAccessory a)
        {
            if (a is null)
            {
                //message.Info(Lc.Keys["Info"], Lc.Keys["PleaseSelectAnAccessoryFirstPrompt"]);
                return;
            }

            //Reset the Autocomplete so that the value is fresh after adding (otherwise its stack in the searched item)
            await searchByCodeComplete.ResetAsync();
            //Open the Dialog
            await message.OpenAddToBasketDialogAsync(a, a.AvailableFinishes.FirstOrDefault(f => f.Finish.Id == a.BasicFinish.Id) ?? AccessoryFinish.Empty());
            //Delay 200ms to let the autocomplete finish its updating (bacause in the end of its updating it gains focus)
            await Task.Delay(200);
            //De focus the autocomplete
            //await searchByCodeComplete.BlurAsync();
            //Focus the input of quantity inside the add item dialog
            await Js.FocusInputInsideParent(AddProductsToBasketDialog.QuantityInputControlId);
        }

        private async Task CopyTableToClipBoard()
        {
            if (Basket.ItemUnderEdit is not null)
            {
                await message.InfoAsync(Lc.Keys["Info"], Lc.Keys["CloseEditsBeforeCopyMessage"]);
                return;
            }

            //Close any Line Animations if they are there
            HideCheckMarkAfterTimePasses(null);
            HideXCheckMarkAfterTimePasses(null);

            int[] columnsToRemove = Array.Empty<int>();
            int[] rowsToRemove = Array.Empty<int>();
            int totalRows = (Basket.Products.Count + 2 + 1); //2 header rows , 1 footer row 
            int lastRowIndex = totalRows - 1;

            if (user.SelectedAppMode == BronzeAppMode.Guest)
            {
                if (imagesDisabled)
                {
                    columnsToRemove = new int[] { 0, 2 };
                }
                else
                {
                    columnsToRemove = new int[] { 0 };
                }
                rowsToRemove = new int[] { 0, lastRowIndex };
            }
            else
            {
                if (imagesDisabled)
                {
                    // first column with buttons , images column , last column with info
                    columnsToRemove = new int[] { 0, 2, 10 };
                }
                else
                {
                    columnsToRemove = new int[] { 0, 10 };
                }
                rowsToRemove = new int[] { 0, lastRowIndex };
            }
            try
            {
                await Js.WriteTableToClipboard("basketTable", columnsToRemove, rowsToRemove);
                snackbar.Add(Lc.Keys["TableCopied"], Severity.Success, (o) => o.CloseAfterNavigation = true);
            }
            catch (Exception ex)
            {
                snackbar.Add("Error While Copying Table", Severity.Warning, (o) => o.CloseAfterNavigation = true);
                Console.WriteLine(ex.Message);
            }
        }

        private async Task GenerateExcelReport()
        {
            try
            {
                isBusy = true;
                cts = new();
                CancellationToken cancellationToken = cts.Token;
                using var wb = new XLWorkbook();
                string filename = $"BronzeArtQuote-{DateTime.Now:dd-MM-yyyy}";
                IProgress<TaskProgressReport> progress = new Progress<TaskProgressReport>(report =>
                {
                    taskProgress = report;
                    InvokeAsync(StateHasChanged);
                });

                if (user.SelectedAppMode == BronzeAppMode.Wholesale)
                {
                    await wholesaleReport.GenerateReport(Basket.Products.ToList(), wb, Basket.CurrentBasketNotes,progress: progress,cancellationToken:cancellationToken);
                }
                else if (user.SelectedAppMode == BronzeAppMode.Retail && Basket.PricesEnabled)
                {
                    await retailReport.GenerateReport(Basket.Products.ToList(), wb, Basket.CurrentBasketNotes, user.VatFactor, progress: progress, cancellationToken: cancellationToken);
                }
                else
                {
                    await guestReport.GenerateReport(Basket.Products.ToList(), wb, Basket.CurrentBasketNotes, progress: progress, cancellationToken: cancellationToken);
                    filename = $"BronzeArtRequest-{DateTime.Now:dd-MM-yyyy}";
                }
                wb.Author = "BronzeWebApplicationB2B";

                using var stream = new MemoryStream();
                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(new(1, 0, $"{Lc.Keys["SavingFile..."]}"));
                await Task.Delay(50);

                wb.SaveAs(stream);
                var content = stream.ToArray();
                await Js.TriggerExcelFileDownload(filename, Convert.ToBase64String(content));

                progress?.Report(new(1, 1, $"{Lc.Keys["FileSaved"]}"));
                await Task.Delay(50);
            }
            catch (OperationCanceledException)
            {
                await message.InfoAsync(Lc.Keys["Info"], Lc.Keys["TaskCancelled"]);
            }
            catch (Exception ex)
            {
                await message.ErrorAsync(ex);
            }
            //Not Busy , reset Cancellation Source
            finally
            {
                isBusy = false;
                cts.Dispose();
                cts = null;
                StateHasChanged();
            }
        }

        private async Task GenerateExcelReportALLAccessories()
        {
            try
            {
                cts = new();
                CancellationToken cancellationToken = cts.Token;
                using var wb = new XLWorkbook();
                IProgress<TaskProgressReport> progress = new Progress<TaskProgressReport>(report =>
                {
                    taskProgress = report;
                    InvokeAsync(StateHasChanged);
                });
                if (user.SelectedAppMode == BronzeAppMode.Wholesale && user.IsPowerUser)
                {
                    isBusy = true;
                    var priceables = await Basket.GeneratePriceablesForAllAccessories(progress, cancellationToken);
                    await priceableReport.GenerateReport(priceables, wb, progress : progress,cancellationToken: cancellationToken);
                }
                else
                {
                    return;
                }
                wb.Author = "BronzeWebApplicationB2B";

                using var stream = new MemoryStream();
                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(new(1, 0, $"{Lc.Keys["SavingFile..."]}"));
                await Task.Delay(50);
                
                wb.SaveAs(stream);
                var content = stream.ToArray();
                await Js.TriggerExcelFileDownload($"AllAccessories-{DateTime.Now:dd-MM-yyyy}", Convert.ToBase64String(content));
                
                progress?.Report(new(1, 1, $"{Lc.Keys["FileSaved"]}"));
                await Task.Delay(50);
            }
            catch (OperationCanceledException)
            {
                await message.InfoAsync(Lc.Keys["Info"], Lc.Keys["TaskCancelled"]);
            }
            catch (Exception ex)
            {
                await message.ErrorAsync(ex);
            }
            //Not Busy , reset Cancellation Source
            finally 
            { 
                isBusy = false; 
                cts.Dispose(); 
                cts = null; 
                StateHasChanged(); 
            }
        }


        public void Dispose()
        {
            saveService.BasketRestored -= SaveService_BasketRestored;
            Basket.PropertyChanged -= Basket_PropertyChanged;
            timerHideCheckmark.Dispose();
            timerHideXCheckmark.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
