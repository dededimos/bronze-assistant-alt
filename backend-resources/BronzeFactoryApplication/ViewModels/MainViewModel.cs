using Azure;
using BronzeFactoryApplication.ApplicationServices.DataService;
using BronzeFactoryApplication.ApplicationServices.NavigationService;
using BronzeFactoryApplication.ApplicationServices.NavigationService.MainViewsNavigation;
using BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation;
using BronzeFactoryApplication.ApplicationServices.SettingsService;
using BronzeFactoryApplication.Properties;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using CommonHelpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAccessLib.MongoDBAccess;
using HandyControl.Themes;
using HandyControl.Tools.Extension;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BronzeFactoryApplication.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly NavigationStore navigationStore;
        private readonly MainMenuNavigationService menuNavigator;
        private readonly CabinsModuleNavigationService cabinsNavigator;
        private readonly ManagmentViewNavigationService managmentNavigator;
        private readonly SearchOrdersModuleNavigationService ordersNavigator;
        private readonly MirrorsModuleNavigationService mirrorsNavigator;
        private readonly WharehouseStockNavigationService wharehouseNavigator;
        private readonly IDialogService dialogService;
        private readonly IGeneralSettingsProvider generalSettingsProvider;
        private readonly BathroomAccessoriesNavigationService accessoriesNavigator;
        
        [ObservableProperty]
        private OperationResult? initilizationResult;

        public OperationProgressViewModel OperationProgressVm { get; }
        /// <summary>
        /// The Current Active ViewModel
        /// </summary>
        public BaseViewModel CurrentViewModel => navigationStore.CurrentViewModel;

        /// <summary>
        /// Weather the Main Menu is Active and can be interacted with
        /// </summary>
        public bool IsNavigationMenuActive { get => navigationStore.CurrentViewModel is not MenuViewModel && ModalsContainer.FrontModal is null; }
        /// <summary>
        /// Weather the CabinNavigation is Enabled currently
        /// </summary>
        public bool CanNavigateToCabins { get => navigationStore.CurrentViewModel is not ShowerCabinsModuleViewModel; }
        public bool CanNavigateToAccessories { get => navigationStore.CurrentViewModel is not AccessoriesModuleViewModel;}
        public bool CanNavigateToMirrors { get => navigationStore.CurrentViewModel is not MirrorsModuleViewModel; }
        public bool CanNavigateToWharehouse { get => navigationStore.CurrentViewModel is not WharehouseModuleViewModel; }

        /// <summary>
        /// The Viewmodel Containing all Currently Opened Modals
        /// </summary>
        public ModalsContainerViewModel ModalsContainer { get; }


        public MainViewModel(
            NavigationStore navigationStore,
            MainMenuNavigationService menuNavigator,
            CabinsModuleNavigationService cabinsNavigator,
            ManagmentViewNavigationService managmentNavigator,
            SearchOrdersModuleNavigationService ordersNavigator,
            IDialogService dialogService,
            IGeneralSettingsProvider generalSettingsProvider,
            ModalsContainerViewModel modalsContainer,
            BathroomAccessoriesNavigationService accessoriesNavigator,
            OperationProgressViewModel operationProgressVm,
            MirrorsModuleNavigationService mirrorsNavigator,
            WharehouseStockNavigationService wharehouseNavigator)
        {
            //Needed to Navigate Between the Views and Get the Current Shown View's ViewModel
            this.navigationStore = navigationStore;
            this.ModalsContainer = modalsContainer;
            this.accessoriesNavigator = accessoriesNavigator;
            this.OperationProgressVm = operationProgressVm;
            //Navigates between the Views
            this.menuNavigator = menuNavigator;
            this.cabinsNavigator = cabinsNavigator;
            this.managmentNavigator = managmentNavigator;
            this.ordersNavigator = ordersNavigator;

            //Hook to Navigation Changes and Front Modal Changes
            navigationStore.CurrentViewModelChanged += NavigationStore_CurrentViewModelChanged;
            ModalsContainer.PropertyChanged += OnFrontModalChanged;

            //Needed to show Seperate Window Modals
            this.dialogService = dialogService;
            this.generalSettingsProvider = generalSettingsProvider;
            this.mirrorsNavigator = mirrorsNavigator;
            this.wharehouseNavigator = wharehouseNavigator;
        }

        /// <summary>
        /// Inform the Navigation Bar Visibility Should Changed Based on weather a modal is Open
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFrontModalChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(ModalsContainerViewModel.FrontModal))
            {
                OnPropertyChanged(nameof(IsNavigationMenuActive));
            }
        }

        /// <summary>
        /// Informs Navigation Has Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigationStore_CurrentViewModelChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(CurrentViewModel)); // Notifies that current View Has Changed (the change happens in the NavStore)
            OnPropertyChanged(nameof(IsNavigationMenuActive)); //Notifies if the Navigation Bar should remain Active or not (Main Menu does not have a navigation Bar)
            NavigateToCabinsCommand.NotifyCanExecuteChanged(); //Notifies weather the Navigation Bars Cabins Icon should be able to Execute (If we are in Cabins it Cannot)
            NavigateToAccessoriesCommand.NotifyCanExecuteChanged();
            NavigateToMirrorsCommand.NotifyCanExecuteChanged();
            NavigateToWharehouseCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanNavigateToCabins))]
        private async Task NavigateToCabins()
        {
            await cabinsNavigator.NavigateAsync();
        }

        [RelayCommand]
        private async Task NavigateToMenu()
        {
            await menuNavigator.NavigateAsync();
        }

        [RelayCommand]
        private async Task NavigateToManagment()
        {
            await managmentNavigator.NavigateAsync();
        }

        [RelayCommand]
        private async Task NavigateToSearchOrders()
        {
            await ordersNavigator.NavigateAsync();
        }

        [RelayCommand(CanExecute = nameof(CanNavigateToAccessories))]
        private async Task NavigateToAccessoriesAsync()
        {
            await accessoriesNavigator.NavigateAsync();
        }

        [RelayCommand(CanExecute = nameof(CanNavigateToMirrors))]
        private async Task NavigateToMirrorsAsync()
        {
            await mirrorsNavigator.NavigateAsync();
        }

        [RelayCommand(CanExecute = nameof(CanNavigateToWharehouse))]
        private async Task NavigateToWharehouseAsync()
        {
            await wharehouseNavigator.NavigateAsync();
        }

        [RelayCommand]
        private async Task ChangeTheme(string? themeIdentifier)
        {
            try
            {
                if (themeIdentifier is null)
                {
                    throw new ArgumentNullException(nameof(themeIdentifier), $"Could not Execute {nameof(ChangeThemeCommand)}");
                }
                if (themeIdentifier is "Dark" or "Light")
                {
                    //Check if Theme is Already the Same
                    if (themeIdentifier == await generalSettingsProvider.GetSelectedTheme())
                    {
                        MessageService.Information.AlreadySelectedTheme();
                        return;
                    }
                    else
                    {
                        await generalSettingsProvider.SetSelectedTheme(themeIdentifier);

                        Log.Information("Selected theme Changed : {themeIdentifier}", themeIdentifier);

                        if (themeIdentifier is "Dark")
                        {
                            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                            ThemeManager.Current.AccentColor = new SolidColorBrush(Color.FromRgb(50, 108, 243));
                        }
                        else
                        {
                            ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                            ThemeManager.Current.AccentColor = new SolidColorBrush(Color.FromRgb(50, 108, 243));
                        }

                        ////Check if User wants to Restart Now
                        //MessageBoxResult result = MessageService.Questions.ApplicationRestartThemeChange();
                        //if (result is MessageBoxResult.OK)
                        //{
                        //    //Get the Path to Restart the Application
                        //    var currentExecutablePath = Process.GetCurrentProcess().MainModule?.FileName ?? null;
                        //    Process.Start(currentExecutablePath ?? throw new Exception("Current Executable File Not Found or Access was Denied"));
                        //    Application.Current.Shutdown();
                        //}
                    }
                }
                else
                {
                    throw new NotSupportedException($"Selected Theme is Not Supported : '{themeIdentifier}' ");
                }
            }
            catch (NotSupportedException nsex)
            {
                Log.Warning(nsex.Message);
                MessageService.Information.NotSupportedTheme();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error While Trying to Execute {nameof(ChangeTheme)}");
            }
        }

        [RelayCommand]
        /// <summary>
        /// Changes The Language and Restarts or Not The Application
        /// </summary>
        /// <param name="languageIdentifier"></param>
        private async Task ChangeLanguage(string? languageIdentifier)
        {
            try
            {
                if (languageIdentifier is null)
                {
                    throw new ArgumentNullException(nameof(languageIdentifier), $"Could not Execute {nameof(ChangeLanguageCommand)}");
                }
                if (languageIdentifier is "el-GR" or "en-EN")
                {
                    //Check if language already selected
                    if (languageIdentifier == await generalSettingsProvider.GetSelectedLanguage())
                    {
                        MessageService.Information.AlreadySelectedLanguage();
                        return;
                    }
                    else
                    {
                        await generalSettingsProvider.SetSelectedLanguage(languageIdentifier);
                        Log.Information("Selected Language Changed : {languageIdentifier}", languageIdentifier);

                        //Check if user wants to restart Now
                        MessageBoxResult result = MessageService.Questions.ApplicationRestartLanguageChange();
                        if (result is MessageBoxResult.OK)
                        {
                            await Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                //Does not Work Correctly => Mongo Db Throws timeout Exception
                                //var currentExecutablePath = Process.GetCurrentProcess().MainModule?.FileName ?? null;
                                //Process.Start(currentExecutablePath ?? throw new Exception("Current Executable File Not Found or Access was Denied"));
                                Application.Current.Shutdown();
                            });
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException($"Selected Language is Not Supported : '{languageIdentifier}' ");
                }
            }
            catch (NotSupportedException nsex)
            {
                Log.Warning(nsex.Message);
                MessageService.Information.NotSupportedLanguage();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error While Trying to Execute {nameof(ChangeLanguage)}");
            }
        }

        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                navigationStore.CurrentViewModelChanged -= NavigationStore_CurrentViewModelChanged;
                ModalsContainer.PropertyChanged -= OnFrontModalChanged;
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }
}
