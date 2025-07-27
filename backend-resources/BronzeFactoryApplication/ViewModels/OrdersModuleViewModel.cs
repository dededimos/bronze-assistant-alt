using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using BronzeFactoryApplication.ApplicationServices.SettingsService.SearchOrdersViewSettingsService;
using BronzeFactoryApplication.Helpers.Converters;
using BronzeFactoryApplication.Helpers.ViewModelFactories;
using BronzeFactoryApplication.ViewModels.OrderRelevantViewModels;
using DataAccessLib;
using DataAccessLib.NoSQLModels;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using GlassesOrdersModels.Models.SubModels;
using HandyControl.Tools.Extension;
using SpecificationFilterLibrary;
using SpecificationFilterLibrary.SpecificationModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using static SpecificationFilterLibrary.Helpers.SpecificationFilterHelpers;

namespace BronzeFactoryApplication.ViewModels
{
    /// <summary>
    /// Pulls Options and Initilizes at the begining pulling Small Orders
    /// The User Can Select an Order or Use the Search Boxes to Bring Glasses / Cabions to View
    /// There are Base Specification Filters which control the Filtering for Glasses/Cabins initilized at the constructor
    /// There are also extra filters that need to be used as extended filters in the Future (A Popu must be used or something of the sort)
    /// The User can Update the QTY of each Glass , either By Marking its row as Filled/Unfilled or by Controlling explicitly the partial Qunatity
    /// The Updates as clicked happen concurently and Seamphores control them 
    /// When leaving the View all the Semaphores must be waited until all are released , as well as when a partial quantity update needs to be done
    /// </summary>
    public partial class OrdersModuleViewModel : BaseViewModel , IOperationOnNavigatingAway
    {
        private readonly IGlassOrderRepository glassRepo;
        private readonly GlassOrderRowViewModelFactory glassRowsFactory;
        private readonly OpenEditGlassRowQuantityModalService editQuantityModalService;
        private readonly ISearchOrdersViewSettingsProvider settingsProvider;
        private readonly ObjectPropertyFilter<GlassOrderRowEntity> glassRowFilterGenerator;
        private readonly ObjectPropertyFilter<CabinRowEntity> cabinRowFilterGenerator;

        /// <summary>
        /// The Semaphore Locks used per Glass Row being Updated , the Updates are concurrent but each row should update at any given time atomically not concurrently
        /// </summary>
        private readonly ConcurrentDictionary<string,SemaphoreSlim> _glassRowsUpdateLocks = new(); //A Concurrent Dictionarry of Locks and RowIds
        /// <summary>
        /// The Number of Semaphores that are Currently active , The View Will not Change not Until all the Semaphores are released
        /// This number should be Incremented and Deincremented with the help of the Interlock Class , so that the operation is thread safe;
        /// </summary>
        private int numberOfNonReleasedSemaphores;

        /// <summary>
        /// The Log for the GlassRowsQuery Observable Collection - Used so to lock the collection while updated from another Thread
        /// </summary>
        private readonly object _glassRowsQueryLock = new();
        /// <summary>
        /// The Log for the CabinRowsQuery Observable Collection - Used so to lock the collection while updated from another Thread
        /// </summary>
        private readonly object _cabinRowsQueryLock = new();

        [ObservableProperty]
        private SearchOrdersViewSettings settings = new();

        [ObservableProperty]
        private ObservableCollection<GlassesOrderSmall> glassOrdersSmall = [];
        [ObservableProperty]
        private GlassesOrderSmall? selectedOrder;

        [ObservableProperty]
        private ObservableCollection<GlassesOrder> orders = new();


        #region A. GLASS QUERY PROPERTIES
        [ObservableProperty]
        private ObservableCollection<GlassOrderRowViewModel> glassRowsQuery = new();
        [ObservableProperty]
        private bool isGlassesBusy;
        [ObservableProperty]
        private GlassOrderRowViewModel? selectedGlassRow;
        [ObservableProperty]
        private bool matchGlassRowToCabin = true;
        [ObservableProperty]
        private bool areCabinsGrouped = false;
        /// <summary>
        /// The Page of the Current Glass Query
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasMorePagesGlassQuery))]
        [NotifyPropertyChangedFor(nameof(HasPreviousPageGlassQuery))]
        [NotifyPropertyChangedFor(nameof(IsGlassesPagerVisible))]
        private int pageOfCurrentGlassesQuery = 0;
        /// <summary>
        /// The Number of Pages of the Current Glass Query
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasMorePagesGlassQuery))]
        private int pagesOfCurrentGlassQuery = 0;
        public bool HasMorePagesGlassQuery { get => PageOfCurrentGlassesQuery < PagesOfCurrentGlassQuery; }
        public bool HasPreviousPageGlassQuery { get => PageOfCurrentGlassesQuery >= 2; }
        public bool IsGlassesPagerVisible { get => PageOfCurrentGlassesQuery > 0; }
        #endregion

        #region B. CABINS QUERY PROPERTIES
        [ObservableProperty]
        private ObservableCollection<CabinOrderRow> cabinRowsQuery = new();
        [ObservableProperty]
        private bool isCabinsBusy;
        [ObservableProperty]
        private CabinOrderRow? selectedCabinRow;
        [ObservableProperty]
        private bool matchCabinRowToGlass = true;
        [ObservableProperty]
        private bool areGlassesGrouped = true;
        /// <summary>
        /// The Page of the Current Cabins Query
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasMorePagesCabinsQuery))]
        [NotifyPropertyChangedFor(nameof(HasPreviousPageCabinsQuery))]
        [NotifyPropertyChangedFor(nameof(IsCabinPagerVisible))]
        private int pageOfCurrentCabinsQuery = 0;
        /// <summary>
        /// The Number of Pages of the Current Cabins Query
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasMorePagesCabinsQuery))]
        private int pagesOfCurrentCabinsQuery = 0;
        public bool HasMorePagesCabinsQuery { get => PageOfCurrentCabinsQuery < PagesOfCurrentCabinsQuery; }
        public bool HasPreviousPageCabinsQuery { get => PageOfCurrentCabinsQuery >= 2; } 
        public bool IsCabinPagerVisible { get => PageOfCurrentCabinsQuery > 0; }
        #endregion

        /// <summary>
        /// All Queries search for Orders only from this Date and Onwards
        /// </summary>
        [ObservableProperty]
        private DateTime fromFilterDate = DateTime.Now.AddDays(-60);
        /// <summary>
        /// All Queries search for Order only up to This Date
        /// </summary>
        [ObservableProperty]
        private DateTime toFilterDate = DateTime.Now;

        public FilterViewModel FilterGlasses { get; set; }
        public FilterViewModel FilterCabins { get; set; }

        #region CONSTRUCTOR
        public OrdersModuleViewModel(IGlassOrderRepository glassRepo, 
                                    Func<FilterViewModel> filtersVmFactory,
                                    GlassOrderRowViewModelFactory glassRowsFactory,
                                    OpenEditGlassRowQuantityModalService editQuantityModalService,
                                    ISearchOrdersViewSettingsProvider settingsProvider)
        {
            BusyPrompt = "Retrieving Results...";
            this.glassRepo = glassRepo;
            this.glassRowsFactory = glassRowsFactory;
            this.editQuantityModalService = editQuantityModalService;
            this.settingsProvider = settingsProvider;
            glassRowFilterGenerator = new(FilterViewModel.ExcludedGlassProperties);
            cabinRowFilterGenerator = new(FilterViewModel.ExcludedCabinProperties);

            //Create the Filters and Set the Default Selections
            FilterGlasses = filtersVmFactory.Invoke();
            FilterGlasses.SetAvailableProperties(glassRowFilterGenerator.FilterableProperties.Select(fp => fp.property).ToArray());
            ResetGlassFilters();

            FilterCabins = filtersVmFactory.Invoke();
            FilterCabins.SetAvailableProperties(cabinRowFilterGenerator.FilterableProperties.Select(fp => fp.property).ToArray());
            ResetCabinFilters();

            // Enables Updating Observable Collection on a non UI Thread (provided that a lock is used)
            // This must be called on the UI Thread and after the collection has been assigned to the items control
            // In Our Case the 
            BindingOperations.EnableCollectionSynchronization(GlassRowsQuery, _glassRowsQueryLock);
            BindingOperations.EnableCollectionSynchronization(CabinRowsQuery, _cabinRowsQueryLock);
        }
        #endregion

        [RelayCommand]
        protected override async Task InitilizeAsync()
        {
            if (Initilized) return;
            
            IsBusy = true;
            try
            {
                Settings = await settingsProvider.GetSettingsAsync();
                await GetSmallOrdersAsync();
                Initilized = true;
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #region A. QUERY ORDER COMMANDS
        /// <summary>
        /// A Command Loading the Retrieve Small Orders from the Database
        /// </summary>
        /// <returns></returns>
        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task LoadSmallOrdersAsync()
        {
            IsBusy = true;
            try
            {
                await GetSmallOrdersAsync();
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// A Command Restoring the Defaults for the Small Orders Settings
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task SmallOrdersRestoreDefaultsAsync()
        {
            Settings = await settingsProvider.GetDefaultsAsync();
        }

        /// <summary>
        /// Retrieves the Selected Order from the Database
        /// </summary>
        /// <returns></returns>
        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task GetSelectedOrderAsync()
        {
            if (SelectedOrder is null) return;
            var spec = new ExpressionSpecification<CabinRowEntity>(o => o.OrderId == SelectedOrder.OrderId);
            //Set the Glasses Query Page to 1
            PageOfCurrentCabinsQuery = 1;
            await QueryCabinsAsync(spec, 1, true,false);
        }
        #endregion

        #region B. QUERY GLASSES COMMANDS
        /// <summary>
        /// Loads the Results of a Search Query in Glasses (always starts from page1)
        /// </summary>
        /// <returns></returns>
        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task QueryGlassesAsync()
        {
            //Set the Glasses Query Page to 1
            PageOfCurrentGlassesQuery = 1;
            PageOfCurrentCabinsQuery = 0;
            //Execute the Query
            await QueryGlassesAsync(PageOfCurrentGlassesQuery, true);
        }

        [RelayCommand]
        private async Task GetPendingGlassesAsync()
        {
            //Set the Glasses Query Page to 1
            PageOfCurrentGlassesQuery = 0;
            PageOfCurrentCabinsQuery = 0;
            //Execute the Query

            IsBusy = true;
            IsCabinsBusy = true;
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    GlassRowsQuery.Clear();
                    CabinRowsQuery.Clear();
                });
                await foreach (var result in glassRepo.GetPendingGlassesAsync(FromFilterDate,ToFilterDate))
                {
                    foreach (var glassRow in result)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //Add the Row to the Query Table
                            GlassRowsQuery.Add(glassRowsFactory.Create(glassRow));
                        });
                        await Task.Delay(1);
                    }
                    //get the Cabin Rows and add the two (Distinct to not add Duplicates)
                    var cabinsToAdd = result.Select(gr => gr.ParentCabinRow).Where(c=> c is not null).DistinctBy(r=> r!.CabinKey);
                    foreach (var cabinRow in cabinsToAdd)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //Add the CabinRow to the Query Table
                            if (cabinRow != null) CabinRowsQuery.Add(cabinRow);
                        });
                        await Task.Delay(1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                IsCabinsBusy = false;
            }
        }

        private async Task QueryGlassesAsync(int page, bool findNumberOfPages = false)
        {
            IsBusy = true;
            IsCabinsBusy = true;
            try
            {
                var spec = CreateSpecificationGlasses();

                //Find the Number of Pages
                if (findNumberOfPages) PagesOfCurrentGlassQuery = await glassRepo.GetGlassQueryPagesAsync(FromFilterDate,ToFilterDate ,spec);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    GlassRowsQuery.Clear();
                    CabinRowsQuery.Clear();
                });
                await foreach (var result in glassRepo.QueryGlassesAsync(FromFilterDate, ToFilterDate, spec, page))
                {
                    foreach (var glassRow in result)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //Add the Row to the Query Table
                            GlassRowsQuery.Add(glassRowsFactory.Create(glassRow));
                        });
                        await Task.Delay(1);
                    }
                    //get the Cabin Rows and add the two (Distinct to not add Duplicates)
                    var cabinsToAdd = result.Select(gr => gr.ParentCabinRow).Where(c => c is not null).DistinctBy(r => r!.CabinKey); ;
                    foreach (var cabinRow in cabinsToAdd)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //Add the CabinRow to the Query Table
                            if (cabinRow != null) CabinRowsQuery.Add(cabinRow);
                        });
                        await Task.Delay(1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                IsCabinsBusy = false;
            }
        }

        /// <summary>
        /// Loads the Results of the Next Page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task LoadNextGlassPageAsync()
        {
            PageOfCurrentGlassesQuery++;
            if (PageOfCurrentGlassesQuery > PagesOfCurrentGlassQuery)
            {
                MessageService.Warning($"Cannot Load {PagesOfCurrentGlassQuery} , the Total Pages of the Query are {PagesOfCurrentGlassQuery}", "Failure".TryTranslateKey());
            }
            else
            {
                await QueryGlassesAsync(PageOfCurrentGlassesQuery);
            }
        }
        /// <summary>
        /// Loads the Results of the Previous Page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task LoadPreviousGlassPageAsync()
        {
            if (PageOfCurrentGlassesQuery < 2)
            {
                MessageService.Warning($"Cannot Load Previous Page ,You are Already in the First Page", "Failure".TryTranslateKey());
            }
            else
            {
                PageOfCurrentGlassesQuery--;
                await QueryGlassesAsync(PageOfCurrentGlassesQuery);
            }
        }

        #endregion

        #region C. QUERY CABINS COMMANDS

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task QueryCabinsAsync()
        {
            //Set the Glasses Query Page to 1
            PageOfCurrentCabinsQuery = 1;
            PageOfCurrentGlassesQuery = 0;
            //Execute the Query
            await QueryCabinsAsync(CreateSpecificationCabins(),PageOfCurrentCabinsQuery, true);
        }
        private async Task QueryCabinsAsync(Specification<CabinRowEntity> spec, int page, bool findNumberOfPages = false , bool useDateFilters = true)
        {
            IsBusy = true;
            IsCabinsBusy = true;
            try
            {
                DateTime from;
                DateTime to;
                if (useDateFilters)
                {
                    from = FromFilterDate;
                    to = ToFilterDate;
                }
                else
                {
                    from = DateTime.Now.AddYears(-20);
                    to = DateTime.Now.AddYears(20);
                }
                //Find the Number of Pages
                if (findNumberOfPages) PagesOfCurrentCabinsQuery = await glassRepo.GetCabinQueryPagesAsync(from, to, spec);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    GlassRowsQuery.Clear();
                    CabinRowsQuery.Clear();
                });
                await foreach (var result in glassRepo.QueryCabinsAsync(from, to, spec, page))
                {
                    foreach (var cabinRow in result)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //Add the Cabin Rows to the Query Table
                            CabinRowsQuery.Add(cabinRow);
                        });
                        await Task.Delay(1);
                    }

                    //Get all the Glasses and Add them Also to the Query Table
                    var glassesToAdd = result.SelectMany(cr => cr.GlassesRows);
                    foreach (var glassRow in glassesToAdd)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            GlassRowsQuery.Add(glassRowsFactory.Create(glassRow));
                        });
                        await Task.Delay(1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                IsCabinsBusy = false;
            }
        }

        /// <summary>
        /// Loads the Results of the Next Page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task LoadNextCabinsPageAsync()
        {
            PageOfCurrentCabinsQuery++;
            if (PageOfCurrentCabinsQuery > PagesOfCurrentCabinsQuery)
            {
                MessageService.Warning($"Cannot Load {PagesOfCurrentCabinsQuery} , the Total Pages of the Query are {PagesOfCurrentCabinsQuery}", "Failure".TryTranslateKey());
            }
            else
            {
                await QueryCabinsAsync(CreateSpecificationCabins(),PageOfCurrentCabinsQuery,false);
            }
        }
        /// <summary>
        /// Loads the Results of the Previous Page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task LoadPreviousCabinsPageAsync()
        {
            if (PageOfCurrentCabinsQuery < 2)
            {
                MessageService.Warning($"Cannot Load Previous Page ,You are Already in the First Page", "Failure".TryTranslateKey());
            }
            else
            {
                PageOfCurrentCabinsQuery--;
                await QueryCabinsAsync(CreateSpecificationCabins(),PageOfCurrentCabinsQuery,false);
            }
        }
        #endregion

        [RelayCommand]
        private void SetDateFilters(string spanOfDays)
        {
            int span = int.TryParse(spanOfDays, out int days) ? days : 0;
            FromFilterDate = DateTime.Now.AddDays(-days);
            ToFilterDate = DateTime.Now;
        }

        /// <summary>
        /// Will mark a Glass Row as Filled , if the Database operation fails the Row will return to its previous state
        /// </summary>
        /// <param name="row">Row to Update</param>
        /// <returns></returns>
        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task FillUnfillGlassRowAsync(GlassOrderRowViewModel row)
        {
            //Create or get the already made Semaphore (One semaphore per RowId and one Concurrent Execution per RowId)
            //Initial Count 1 and Max Count 1 (Concurrent Executions for the Same id is 1!)
            Interlocked.Increment(ref numberOfNonReleasedSemaphores); //Mark one of the semaphores has been locked
            SemaphoreSlim seamaphore = _glassRowsUpdateLocks.GetOrAdd(row.GetModel().RowId, new SemaphoreSlim(1, 1));

            // Await to get the lock if its already taken by another call
            await seamaphore.WaitAsync();
            // Store the Initial Filled Qty

            //Store the Starting FilledQuantity so to restore if exception is thrown
            int startingFilledQuantity = row.FilledQuantity;
            row.FilledQuantity = row.IsFilled ? 0 : row.Quantity;

            try
            {
                // Update the Row with the new Quantity
                await glassRepo.UpdateGlassRowAsync(row.GetModel());
            }
            catch (Exception ex)
            {
                // Revert to the Starting Filled Quantity 
                row.FilledQuantity = startingFilledQuantity;
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                // Release the Lock
                seamaphore.Release();
                // Mark that : one of the semaphores has been released
                Interlocked.Decrement(ref numberOfNonReleasedSemaphores);
            }
        }

        /// <summary>
        /// Generates Filter Specifications for a Glasses Query
        /// </summary>
        /// <returns></returns>
        private Specification<GlassOrderRowEntity> CreateSpecificationGlasses()
        {
            var spec1 = glassRowFilterGenerator.GetFilter(FilterGlasses.SelectedPropertyToFilter1,
                                           FilterGlasses.ParsedOperatorAndValueInput1.restText,
                                           FilterGlasses.ParsedOperatorAndValueInput1.o);
            var spec2 = glassRowFilterGenerator.GetFilter(FilterGlasses.SelectedPropertyToFilter2,
                                           FilterGlasses.ParsedOperatorAndValueInput2.restText,
                                           FilterGlasses.ParsedOperatorAndValueInput2.o);
            var spec3 = glassRowFilterGenerator.GetFilter(FilterGlasses.SelectedPropertyToFilter3,
                                           FilterGlasses.ParsedOperatorAndValueInput3.restText,
                                           FilterGlasses.ParsedOperatorAndValueInput3.o);
            //If all specs are SatisfiedSpecification (meaning all inputs are null or empty) then just return 
            //a spec that is always true but inludes one of the properties . Otherwise the MongoServer does not accept the Expression
            bool areAllSatisfiedSpecs = spec1 is SatisfiedSpecification<GlassOrderRowEntity> &&
                                        spec2 is SatisfiedSpecification<GlassOrderRowEntity> &&
                                        spec3 is SatisfiedSpecification<GlassOrderRowEntity>;

            return areAllSatisfiedSpecs ? new ExpressionSpecification<GlassOrderRowEntity>((x) => x.OrderId != "") : spec1.And(spec2).And(spec3);
        }
        /// <summary>
        /// Generates Specifications for a Cabins Query
        /// </summary>
        /// <returns></returns>
        private Specification<CabinRowEntity> CreateSpecificationCabins()
        {
            //Combine all The Three Filters into One

            var spec1 = cabinRowFilterGenerator.GetFilter(FilterCabins.SelectedPropertyToFilter1,
                                            FilterCabins.ParsedOperatorAndValueInput1.restText,
                                            FilterCabins.ParsedOperatorAndValueInput1.o);
            var spec2 = cabinRowFilterGenerator.GetFilter(FilterCabins.SelectedPropertyToFilter2,
                                            FilterCabins.ParsedOperatorAndValueInput2.restText,
                                            FilterCabins.ParsedOperatorAndValueInput2.o);
            var spec3 = cabinRowFilterGenerator.GetFilter(FilterCabins.SelectedPropertyToFilter3,
                                            FilterCabins.ParsedOperatorAndValueInput3.restText,
                                            FilterCabins.ParsedOperatorAndValueInput3.o);

            //If all specs are SatisfiedSpecification (meaning all inputs are null or empty) then just return 
            //a spec that is always true but inludes one of the properties . Otherwise the MongoServer does not accept the Expression
            bool areAllSatisfiedSpecs = spec1 is SatisfiedSpecification<CabinRowEntity> &&
                                        spec2 is SatisfiedSpecification<CabinRowEntity> &&
                                        spec3 is SatisfiedSpecification<CabinRowEntity>;

            return areAllSatisfiedSpecs ? new ExpressionSpecification<CabinRowEntity>((x) => x.OrderId != "") : spec1.And(spec2).And(spec3);
        }

        [RelayCommand]
        private async Task OpenEditQuantityModalAsync(GlassOrderRowViewModel row)
        {
            //Do not open if Concurrent updates are running
            await WaitUpdatesAsync();

            editQuantityModalService.OpenModal(row);
        }

        #region AAA.SELECT FILTER COMMANDS
        [RelayCommand]
        private void SelectGlassesFilter1(PropertyInfo propertyToFilter)
        {
            if (propertyToFilter.PropertyType == typeof(DateTime))
            {
                FilterGlasses.Property1ConstraintValue = DateTime.Now.Date.ToString();
            }
            else
            {
                FilterGlasses.Property1ConstraintValue = string.Empty;
            }
            FilterGlasses.SelectedPropertyToFilter1 = propertyToFilter;
        }
        [RelayCommand]
        private void SelectGlassesFilter2(PropertyInfo propertyToFilter)
        {
            if (propertyToFilter.PropertyType == typeof(DateTime))
            {
                FilterGlasses.Property2ConstraintValue = DateTime.Now.Date.ToString();
            }
            else
            {
                FilterGlasses.Property2ConstraintValue = string.Empty;
            }
            FilterGlasses.SelectedPropertyToFilter2 = propertyToFilter;
        }
        [RelayCommand]
        private void SelectGlassesFilter3(PropertyInfo propertyToFilter)
        {
            if (propertyToFilter.PropertyType == typeof(DateTime))
            {
                FilterGlasses.Property3ConstraintValue = DateTime.Now.Date.ToString();
            }
            else
            {
                FilterGlasses.Property3ConstraintValue = string.Empty;
            }
            FilterGlasses.SelectedPropertyToFilter3 = propertyToFilter;
        }
        [RelayCommand]
        private void SelectCabinsFilter1(PropertyInfo propertyToFilter)
        {
            if (propertyToFilter.PropertyType == typeof(DateTime))
            {
                FilterCabins.Property1ConstraintValue = DateTime.Now.Date.ToString();
            }
            else
            {
                FilterCabins.Property1ConstraintValue = string.Empty;
            }
            FilterCabins.SelectedPropertyToFilter1 = propertyToFilter;
        }
        [RelayCommand]
        private void SelectCabinsFilter2(PropertyInfo propertyToFilter)
        {
            if (propertyToFilter.PropertyType == typeof(DateTime))
            {
                FilterCabins.Property2ConstraintValue = DateTime.Now.Date.ToString();
            }
            else
            {
                FilterCabins.Property2ConstraintValue = string.Empty;
            }
            FilterCabins.SelectedPropertyToFilter2 = propertyToFilter;
        }
        [RelayCommand]
        private void SelectCabinsFilter3(PropertyInfo propertyToFilter)
        {
            if (propertyToFilter.PropertyType == typeof(DateTime))
            {
                FilterCabins.Property3ConstraintValue = DateTime.Now.Date.ToString();
            }
            else
            {
                FilterCabins.Property3ConstraintValue = string.Empty;
            }
            FilterCabins.SelectedPropertyToFilter3 = propertyToFilter;
        }
        [RelayCommand]
        private void ResetGlassFilters()
        {
            FilterGlasses.SelectedPropertyToFilter1 = null;
            FilterGlasses.SelectedPropertyToFilter2 = null;
            FilterGlasses.SelectedPropertyToFilter3 = null;
            FilterGlasses.SelectedPropertyToFilter1 = FilterGlasses.AvailableProperties.First(p => p.Name == nameof(GlassOrderRowEntity.ReferencePA0));
            FilterGlasses.SelectedPropertyToFilter2 = FilterGlasses.AvailableProperties.First(p => p.Name == nameof(GlassOrderRowEntity.OrderId));
            FilterGlasses.SelectedPropertyToFilter3 = FilterGlasses.AvailableProperties.First(p => p.Name == nameof(GlassOrderRowEntity.OrderedGlass.Thickness));
            FilterGlasses.Property1ConstraintValue = string.Empty;
            FilterGlasses.Property2ConstraintValue = string.Empty;
            FilterGlasses.Property3ConstraintValue = string.Empty;
        }
        [RelayCommand]
        private void ResetCabinFilters()
        {
            FilterCabins.SelectedPropertyToFilter1 = null;
            FilterCabins.SelectedPropertyToFilter2 = null;
            FilterCabins.SelectedPropertyToFilter3 = null;
            FilterCabins.SelectedPropertyToFilter1 = FilterCabins.AvailableProperties.First(p => p.Name == nameof(CabinRowEntity.ReferencePA0));
            FilterCabins.SelectedPropertyToFilter2 = FilterCabins.AvailableProperties.First(p => p.Name == nameof(CabinRowEntity.OrderId));
            FilterCabins.SelectedPropertyToFilter3 = FilterCabins.AvailableProperties.First(p => p.Name == nameof(CabinRowEntity.OrderedCabin.Model));
            FilterCabins.Property1ConstraintValue = string.Empty;
            FilterCabins.Property2ConstraintValue = string.Empty;
            FilterCabins.Property3ConstraintValue = string.Empty;
        }
        #endregion

        /// <summary>
        /// Wait until all the Semaphores have been released to change the View
        /// </summary>
        /// <returns></returns>
        public async Task OnNavigatingAwayOperation()
        {
            await WaitUpdatesAsync();
        }

        /// <summary>
        /// Awaits for the Semaphores to Release from all their Pending Operations
        /// </summary>
        /// <returns></returns>
        private async Task WaitUpdatesAsync()
        {
            if (numberOfNonReleasedSemaphores != 0)
            {
                int maxWaitTime = 4000;
                var watch = Stopwatch.StartNew();
                try
                {
                    IsBusy = true;
                    await Task.Run(async() =>
                    {

                        while (numberOfNonReleasedSemaphores != 0 || watch.ElapsedMilliseconds <= maxWaitTime)
                        {
                            //wait for the seamphores to be released 100ms Wait between each check
                            await Task.Delay(100);
                        }

                        //Throw if the wait was more than 4sec
                        if (watch.ElapsedMilliseconds > maxWaitTime)
                        {
                            throw new Exception($"Timeout {maxWaitTime / 1000}sec passed while waiting for updates to End...");
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageService.LogAndDisplayException(ex);
                }
                finally
                {
                    IsBusy = false;
                    watch.Stop();
                }
            }
        }

        /// <summary>
        /// Gets the Small Orders from the Database
        /// </summary>
        /// <returns></returns>
        private async Task GetSmallOrdersAsync()
        {
            var result = await glassRepo.GetOrdersSmallAsync(Settings.MaxResultsGetSmallOrders);
            GlassOrdersSmall = new(result);
        }

        #region Save To File Commands
        [RelayCommand]
        private async Task SaveGlassListToExcelAsync()
        {
            BusyPrompt = SAVING;
            IsBusy = true;
            try
            {
                if (GlassRowsQuery.Any())
                {
                    var fileName = await Task.Run(() =>
                    {
                        var fileName = ExcelService.ReportXls.SaveAsXlsReport(GlassRowsQuery.Select(x => x.GetModel()));
                        return fileName;
                    });
                    if (MessageService.Questions.ExcelSavedAskOpenFile(fileName) == MessageBoxResult.OK)
                    {
                        //Open the file if users reply is positive
                        Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
                    }
                }
                else MessageService.Info("No Rows To Save","Empty Rows");
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                BusyPrompt = LOADING;
            }
        }

        [RelayCommand]
        private async Task SaveCabinsListToExcelAsync()
        {
            BusyPrompt = SAVING;
            IsBusy = true;
            try
            {
                if (CabinRowsQuery.Any())
                {
                    var fileName = await Task.Run(() =>
                    {
                        var fileName = ExcelService.ReportXls.SaveAsXlsReport(CabinRowsQuery);
                        return fileName;
                    });
                    if (MessageService.Questions.ExcelSavedAskOpenFile(fileName) == MessageBoxResult.OK)
                    {
                        //Open the file if users reply is positive
                        Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
                    }
                }
                else MessageService.Info("No Rows To Save", "Empty Rows");
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                BusyPrompt = LOADING;
            }
        }

        #endregion

    }

    public partial class FilterViewModel : BaseViewModel
    {
        public readonly static string[] ExcludedGlassProperties = new string[]
        {
            nameof(GlassOrderRowEntity.CabinRowKey),
            nameof(GlassOrderRowEntity.Id),
            nameof(GlassOrderRowEntity.LastModified),
            nameof(GlassOrderRowEntity.CancelledQuantity),
            nameof(GlassOrderRowEntity.SpecialDrawNumber),
            nameof(GlassOrderRowEntity.SpecialDrawString),
            $"{nameof(GlassOrderRowEntity.OrderedGlass)}.{nameof(GlassOrderRowEntity.OrderedGlass.HasRounding)}",
        };
        public readonly static string[] ExcludedCabinProperties = new string[]
        {
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.PartsList)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.Constraints)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.Extras)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.Opening)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.SynthesisModel)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.Direction)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.IsReversible)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.TypeDiscriminator)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.Id)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.LastModified)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.Created)}",
            $"{nameof(CabinRowEntity.OrderedCabin)}.{nameof(CabinRowEntity.OrderedCabin.IsCodeOverriden)}",
            nameof(CabinRowEntity.SynthesisKey),
            nameof(CabinRowEntity.CabinKey),
            nameof(CabinRowEntity.Id),
            nameof(CabinRowEntity.LastModified)
        };

        /// <summary>
        /// All the Available Properties
        /// </summary>
        private PropertyInfo[] availableProperties = Array.Empty<PropertyInfo>();
        /// <summary>
        /// All the Available Properties that have not yet been Selected
        /// </summary>
        public PropertyInfo[] AvailableProperties 
        {
            //Return only the Properties that are not already Selected
            get => availableProperties.Where(p => 
                    p.Name != SelectedPropertyToFilter1?.Name && 
                    p.Name != SelectedPropertyToFilter2?.Name && 
                    p.Name != SelectedPropertyToFilter3?.Name)
                    .ToArray();
        }

        /// <summary>
        /// The Property that has been Selected For Filtering (From the available Selectable Properties for Filtering of this FilterViewModel)
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AvailableProperties))]
        private PropertyInfo? selectedPropertyToFilter1;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AvailableProperties))]
        private PropertyInfo? selectedPropertyToFilter2;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AvailableProperties))]
        private PropertyInfo? selectedPropertyToFilter3;

        #region 2. INPUTS (FILTER-CONSTRAINT)
        /// <summary>
        /// The Input Text against which values will be filtered (What is given as input in the Filter from a User (Includes operators))
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParsedOperatorAndValueInput1))]
        private string property1ConstraintValue = string.Empty;
        /// <summary>
        /// The Input Text against which values will be filtered (What is given as input in the Filter from a User (Includes operators))
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParsedOperatorAndValueInput2))]
        private string property2ConstraintValue = string.Empty;
        /// <summary>
        /// The Input Text against which values will be filtered (What is given as input in the Filter from a User (Includes operators))
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParsedOperatorAndValueInput3))]
        private string property3ConstraintValue = string.Empty;
        #endregion

        #region 3.PARSED INPUTS (OPERATOR-TEXT)
        public (ComparisonOperator o, string restText) ParsedOperatorAndValueInput1
        {
            get => ParseFilterOperator(Property1ConstraintValue);
        }
        public (ComparisonOperator o, string restText) ParsedOperatorAndValueInput2
        {
            get => ParseFilterOperator(Property2ConstraintValue);
        }
        public (ComparisonOperator o, string restText) ParsedOperatorAndValueInput3
        {
            get => ParseFilterOperator(Property3ConstraintValue);
        } 
        #endregion

        /// <summary>
        /// Sets the Available Properties for Selection
        /// </summary>
        /// <param name="properties">The Properties to Set</param>
        public void SetAvailableProperties(PropertyInfo[] properties)
        {
            LanguageConverter converter = new();
            availableProperties = properties.OrderBy(p=> converter.Convert(p.Name,typeof(object),string.Empty,CultureInfo.InvariantCulture).ToString()).ToArray();
            OnPropertyChanged(nameof(AvailableProperties));
        }
    }



}
