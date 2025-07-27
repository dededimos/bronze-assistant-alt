using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Helpers;
using BronzeArtWebApplication.Shared.Models;
using Microsoft.AspNetCore.Components;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using MirrorsModelsLibrary.StaticData;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Components.MirrorCreationComponents.AssembleMirrorComponents
{
    public partial class MirrorsCatalogueView
    {
        //[Parameter] public EventCallback<Mirror> OnMirrorSelected { get; set; }
        //[Parameter] public EventCallback ExecuteOnSelectingMirror { get; set; }

        private bool showAllSeries = false;
        private void SetShowAllSeries(bool value)
        {
            showAllSeries = value;
            SearchQuery = string.Empty; // Reset the search query when toggling the series view
            SelectedShapeString = string.Empty; // Reset the shape filter when toggling the series view
            SelectedSeriesString = string.Empty; // Reset the series filter when toggling the series view
            StateHasChanged();
        }


        private string SearchQuery { get; set; } = string.Empty;
        private string SelectedShapeString { get; set; } = string.Empty;
        private MirrorShape? SelectedShape { get => Enum.TryParse<MirrorShape>(SelectedShapeString, out MirrorShape parsedValue) ? parsedValue : null; }
        private string SelectedSeriesString { get; set; } = string.Empty;
        private MirrorSeries? SelectedSeries { get => Enum.TryParse<MirrorSeries>(SelectedSeriesString, out MirrorSeries parsedValue) ? parsedValue : null; }

        /// <summary>
        /// The Catalogue Mirrors Categorized According to their Series
        /// </summary>
        private readonly Dictionary<MirrorSeries, List<Mirror>> MirrorCategories = [];

        /// <summary>
        /// The Filtered Mirrors based on search and filters
        /// </summary>
        private Dictionary<MirrorSeries, List<Mirror>> FilteredMirrorCategories => FilterMirrorCategories();

        /// <summary>
        /// The Default Lights in the Catalogue Mirrors of each Series
        /// </summary>
        private readonly Dictionary<MirrorSeries, List<MirrorLight>> DefaultLightPerSandblastDesign = [];

        /// <summary>
        /// The Default Extras in the Catalogue Mirrors of each Series
        /// </summary>
        private readonly Dictionary<MirrorSeries, List<MirrorOption>> DefaultExtrasPerSandblastDesign = [];

        protected override void OnInitialized()
        {
            // Subscribe to User Service Property Changes 
            user.PropertyChanged += User_PropertyChanged;

            foreach (var series in Enum.GetValues<MirrorSeries>().OrderBy(s => customSeriesOrder.GetValueOrDefault(s, 500)))
            {
                // Add a new Category of Mirrors with the Specified Series
                List<Mirror> category = MirrorsStaticData.CatalogueMirrors
                    .Where(mirror => mirror.Series == series)
                    .ToList();

                // Find the Available Default Lights in this Category
                List<MirrorLight> categoryLights = category
                    .Where(mirror => mirror.Lighting?.Light != null)
                    .Select(mirror => (MirrorLight)mirror.Lighting.Light)
                    .Distinct()
                    .ToList();

                // Find the Available Extras in this Category
                List<MirrorExtra> categoryExtras = [];
                // Extract the Mirrors of the Category that Have Extras
                category.Where(mirror => mirror.Extras.Count > 0)
                    // Select the List of Extras from each Mirror and Put it into a list
                    .Select(mirror => mirror.Extras)
                    .ToList()
                    // Combine all Lists to one (All the Extras contained in this category in a single List
                    .ForEach(list => categoryExtras.AddRange(list));

                // Extract from the Mirror Extra Object only the MirrorOption Value and Remove Duplicates
                List<MirrorOption> categoryOptions = categoryExtras
                    .Select(extra => extra.Option)
                    .Distinct()
                    .ToList();

                if (category.Count != 0)
                {
                    category = category.OrderBy(m => m.Length)
                        .ThenBy(m => m.Height)
                        .ThenBy(m => m.Diameter)
                        .ThenBy(m => m.Extras.Count)
                        .ThenBy(m => m.Lighting.Light)
                        .ToList();

                    // Add the Category to The List of Categories
                    MirrorCategories.Add(series, category);
                    DefaultLightPerSandblastDesign.Add(series, categoryLights);
                    DefaultExtrasPerSandblastDesign.Add(series, categoryOptions);
                }
            }
        }

        private static Dictionary<MirrorSeries, int> customSeriesOrder = new()
        {
            { MirrorSeries.H7,  1 },
            { MirrorSeries.R7,  2 },
            { MirrorSeries.P8,  3 },
            { MirrorSeries.H8,  4 },
            { MirrorSeries.X6,  5 },
            { MirrorSeries.X4,  6 },
            { MirrorSeries._6000,  7 },
            { MirrorSeries.M3,  8 },
            { MirrorSeries.ES,  9 },
            { MirrorSeries.EL,  10 },
            { MirrorSeries.N9,  11 },
            { MirrorSeries.R9,  12 },
            { MirrorSeries.P9,  13 },
            { MirrorSeries.N6,  14 },
            { MirrorSeries.N7,  15 },
            { MirrorSeries.N1,  16 },
            { MirrorSeries.N2,  17 },
            { MirrorSeries.A9,  18 },
            { MirrorSeries.ND,  19 },
            { MirrorSeries.NC,  20 },
            { MirrorSeries.NL,  21 },
            { MirrorSeries.NS,  22 },
        };

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(BronzeUser.IsPricingVisible)
                            or nameof(BronzeUser.SelectedPriceIncreaseFactor))
            {
                StateHasChanged();
            }
        }

        private Dictionary<MirrorSeries, List<Mirror>> FilterMirrorCategories()
        {
            var filteredCategories = new Dictionary<MirrorSeries, List<Mirror>>();

            foreach (var category in MirrorCategories)
            {
                var filteredMirrors = category.Value
                    .Where(mirror =>
                        (string.IsNullOrWhiteSpace(SearchQuery) ||
                         mirror.Code.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) &&
                        (SelectedShape == null || mirror.Shape == SelectedShape) &&
                        (SelectedSeries == null ||
                         (mirror.Series != null && mirror.Series == SelectedSeries)))
                    .ToList();

                if (filteredMirrors.Count != 0)
                {
                    filteredCategories.Add(category.Key, filteredMirrors);
                }
            }

            return filteredCategories;
        }

        private void RowClickEvent(TableRowClickEventArgs<Mirror> e)
        {
            Mirror selectedMirror = e.Item;
            vm.ResetViewModel();
            vm.Shape = selectedMirror.Shape;    // Resets the ViewModel!
            vm.Light = selectedMirror.Lighting?.Light;
            vm.Length = selectedMirror.Length;
            vm.Height = selectedMirror.Height;
            vm.Diameter = selectedMirror.Diameter;
            vm.Support = selectedMirror.Support.SupportType;
            vm.FinishType = selectedMirror.Support.FinishType;

            if (vm.FinishType == SupportFinishType.Painted)
            {
                vm.PaintFinish = selectedMirror.Support.PaintFinish;
            }
            else if (vm.FinishType == SupportFinishType.Electroplated)
            {
                vm.ElectroplatedFinish = selectedMirror.Support.ElectroplatedFinish;
            }

            vm.Sandblast = selectedMirror.Sandblast;

            foreach (MirrorExtra extra in selectedMirror.Extras)
            {
                vm.AddExtra(extra.Option);
            }

            // Does not set the Code Otherwise and Rules Throw Exception
            vm.SetMirrorSeriesCode();

            dialogNav.ChangeCurrentDialog(MirrorDialog.ChooseSwitch);

            //IsDetailDialogOpen = false;
            navigationManager.NavigateTo("/AssembleMirror");
        }

        private static string GetSeriesDescription(MirrorSeries series)
        {
            // You can customize these descriptions or load them from language resources
            return series switch
            {
                MirrorSeries.H7 => string.Empty,
                MirrorSeries.N6 => string.Empty,
                MirrorSeries.M3 => string.Empty,
                MirrorSeries.H8 => string.Empty,
                MirrorSeries.X6 => string.Empty,
                MirrorSeries.X4 => string.Empty,
                MirrorSeries._6000 => string.Empty,
                MirrorSeries.N9 => string.Empty,
                MirrorSeries.N7 => string.Empty,
                MirrorSeries.A7 => string.Empty,
                MirrorSeries.A9 => string.Empty,
                MirrorSeries.M8 => string.Empty,
                MirrorSeries.ND => string.Empty,
                MirrorSeries.NC => string.Empty,
                MirrorSeries.NL => string.Empty,
                MirrorSeries.Custom => string.Empty,
                MirrorSeries.P8 => string.Empty,
                MirrorSeries.P9 => string.Empty,
                MirrorSeries.R7 => string.Empty,
                MirrorSeries.R9 => string.Empty,
                MirrorSeries.NS => string.Empty,
                MirrorSeries.N1 => string.Empty,
                MirrorSeries.N2 => string.Empty,
                MirrorSeries.EL => string.Empty,
                MirrorSeries.ES => string.Empty,
                MirrorSeries.IM => string.Empty,
                MirrorSeries.IA => string.Empty,
                MirrorSeries.N9Custom => string.Empty,
                MirrorSeries.NA => string.Empty,
                MirrorSeries.NCCustom => string.Empty,
                MirrorSeries.IC => string.Empty,
                MirrorSeries.NLCustom => string.Empty,
                MirrorSeries.IL => string.Empty,
                _ => string.Empty
            };
        }

        private void ClearFilters()
        {
            SelectedShapeString = string.Empty; 
            SelectedSeriesString = string.Empty; 
            SearchQuery = string.Empty;
        }
        private static bool IsNewSeries(MirrorSeries series)
        {
            // Logic to determine if a series is new
            // For example, you could have a list of new series or check the release date
            return false; // Example - H7 is marked as new
        }

        public void Dispose()
        {
            user.PropertyChanged -= User_PropertyChanged;
        }
    }
}
