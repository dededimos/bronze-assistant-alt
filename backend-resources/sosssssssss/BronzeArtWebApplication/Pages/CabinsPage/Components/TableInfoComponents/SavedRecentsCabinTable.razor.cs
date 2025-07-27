using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShowerEnclosuresModelsLibrary.Helpers;
using ShowerEnclosuresModelsLibrary.Models;
using Blazored.LocalStorage;
using System;
using CommonHelpers.Utilities.CustomComparers;
using System.Diagnostics;
using MudBlazor;
using BronzeArtWebApplication.Shared.ViewModels;
using BronzeArtWebApplication.Shared.Enums;

namespace BronzeArtWebApplication.Pages.CabinsPage.Components.TableInfoComponents
{
    public partial class SavedRecentsCabinTable : ComponentBase
    {
        [Inject] private ILocalStorageService storage { get; set; }
        [Inject] private AssembleCabinViewModel vm { get; set; }
        [Inject] private NavigationManager nm { get; set; }
        [Inject] private CabinStringFactory cabinStringFactory { get; set; }

        [Parameter] public string Class { get; set; }
        [Parameter] public string Style { get; set; }
        [Parameter] public string TableHeightPX { get; set; }

        private readonly Typo tablesTypo = Typo.caption;
        private readonly string headerTextStyle = "font-weight:500;color:white;";
        private readonly string cellTextStyle = "color:white;";
        private readonly string textClass = "noselect";

        /// <summary>
        /// The Strings of the Recent Synthesis Strings
        /// </summary>
        private SortedList<DateTime, string> recentSynthesisStrings = new(new DateDescendingComparer());

        /// <summary>
        /// The Strings of the Saved Synthesis Strings
        /// </summary>
        private SortedList<DateTime, string> savedSynthesisStrings = new(new DateDescendingComparer());

        private readonly List<CabinSynthesis> recentSynthesis = new();
        private readonly List<CabinSynthesis> savedSynthesis = new();

        private bool isBusy;
        private bool isInErrorRecents;
        private bool isInErrorSaved;
        private int activePanelIndex;

        public bool IsRecentsEmpty { get => recentSynthesis.Count < 1; }
        public bool IsSavedEmpty { get => savedSynthesis.Count < 1; }

        protected override async Task OnParametersSetAsync()
        {
            isBusy = true;
            await Task.Run(async () =>
            {
                await Task.Delay(1000);
                try
                {
                    if (await storage.ContainKeyAsync("RecentSynthesis"))
                    {
                        //The Deserilizer cannot take a comparer from before ? (Mayube its somewhere in the Options , so we have to add the items to the sorted list again to appear in Desdcending Order)
                        var storedList = await storage.GetItemAsync<SortedList<DateTime, string>>("RecentSynthesis");
                        foreach (var item in storedList)
                        {
                            recentSynthesisStrings.Add(item.Key, item.Value);
                        }
                        foreach (var item in recentSynthesisStrings.Values)
                        {
                            if (cabinStringFactory.TryGenerateCabinSynthesisFromString(item, out CabinSynthesis synthesis))
                            {
                                recentSynthesis.Add(synthesis);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to Load Recents");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(" ");
                    isInErrorRecents = true;
                }

                try
                {
                    if (await storage.ContainKeyAsync("SavedSynthesis"))
                    {
                        var storedList = await storage.GetItemAsync<SortedList<DateTime, string>>("SavedSynthesis");
                        foreach (var item in storedList)
                        {
                            savedSynthesisStrings.Add(item.Key, item.Value);
                        }
                        foreach (var item in savedSynthesisStrings.Values)
                        {
                            if (cabinStringFactory.TryGenerateCabinSynthesisFromString(item, out CabinSynthesis synthesis))
                            {
                                savedSynthesis.Add(synthesis);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to Load Saved");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(" ");
                    isInErrorSaved = true;
                }
            });
            isBusy = false;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                StateHasChanged();
            }
            base.OnAfterRender(firstRender);
        }

        private void HandleSelection(TableRowClickEventArgs<CabinSynthesis> e)
        {
            try
            {
                vm.PassSynthesisToViewModel(e.Item);
                nm.NavigateTo($"/AssembleCabin/{StoryWindow.CabinPanel}");
                snackbar.Add(lc.Keys["CabinSynthesisFromRecentTable"], Severity.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                nm.NavigateTo("/AssembleCabin");
                snackbar.Add(lc.Keys["ErrorGeneratingSynthesisFromRecentTable"], Severity.Error);
            }

        }

        private async void HandleDeletion(string storageListName, CabinSynthesis deletedItem)
        {
            isBusy = true;
            await Task.Run(async () =>
            {
                if (await storage.ContainKeyAsync(storageListName))
                {
                    var storedList = await storage.GetItemAsync<SortedList<DateTime, string>>(storageListName);
                    foreach (var item in storedList)
                    {
                        savedSynthesisStrings.Add(item.Key, item.Value);
                    }
                    foreach (var item in savedSynthesisStrings.Values)
                    {
                        if (cabinStringFactory.TryGenerateCabinSynthesisFromString(item, out CabinSynthesis synthesis))
                        {
                            savedSynthesis.Add(synthesis);
                        }
                    }
                }
            });
            isBusy = false;
        }

    }
}
