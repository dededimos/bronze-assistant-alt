using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using BronzeFactoryApplication.ApplicationServices.NavigationService.MainViewsNavigation;
using BronzeFactoryApplication.Helpers.Other;
using BronzeRulesPricelistLibrary;
using BronzeRulesPricelistLibrary.Builders;
using BronzeRulesPricelistLibrary.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using CommonInterfacesBronze;
using CommunityToolkit.Mvvm.Input;
using DataAccessLib.NoSQLModels;
using HandyControl.Tools.Extension;
using ShowerEnclosuresModelsLibrary.Models.RepositoryModels.ModelsSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels
{
    public partial class MenuViewModel : BaseViewModel
    {
        private readonly CabinsModuleNavigationService cabinNavigator;
        private readonly ManagmentViewNavigationService managmentNavigator;
        private readonly SearchOrdersModuleNavigationService ordersNavigator;
        private readonly VariousAppHelpersModuleNavigationService variousHelpersNavigator;
        private readonly BathroomAccessoriesNavigationService accessoriesNavigator;
        private readonly MirrorsModuleNavigationService mirrorsNavigator;
        private readonly WharehouseStockNavigationService wharehouseNavigator;

        public MenuViewModel(CabinsModuleNavigationService cabinNavigator,
                             ManagmentViewNavigationService managmentNavigator,
                             SearchOrdersModuleNavigationService ordersNavigator,
                             VariousAppHelpersModuleNavigationService variousHelpersNavigator,
                             BathroomAccessoriesNavigationService accessoriesNavigator,
                             MirrorsModuleNavigationService mirrorsNavigator ,
                             WharehouseStockNavigationService wharehouseNavigator)
        {
            this.cabinNavigator = cabinNavigator;
            this.managmentNavigator = managmentNavigator;
            this.ordersNavigator = ordersNavigator;
            this.variousHelpersNavigator = variousHelpersNavigator;
            this.accessoriesNavigator = accessoriesNavigator;
            this.mirrorsNavigator = mirrorsNavigator;
            this.wharehouseNavigator = wharehouseNavigator;
        }

        [RelayCommand]
        private async Task NavigateToCabins()
        {
            await cabinNavigator.NavigateAsync();
        }

        [RelayCommand]
        private async Task NavigateToManagment() 
        {
            await managmentNavigator.NavigateAsync();
        }

        [RelayCommand]
        private async Task NavigateToOrdersAsync()
        {
            await ordersNavigator.NavigateAsync();
        }
        [RelayCommand]
        private async Task NavigateToVariousHelpersAsync()
        {
            await variousHelpersNavigator.NavigateAsync();
        }

        [RelayCommand]
        private async Task NavigateToAccessoriesAsync()
        {
            await accessoriesNavigator.NavigateAsync();
        }
        [RelayCommand]
        private async Task NavigateToMirrorsAsync()
        {
            await mirrorsNavigator.NavigateAsync();
        }
        [RelayCommand]
        private async Task NavigateToWharehouseStockAsync()
        {
            await wharehouseNavigator.NavigateAsync();
        }
    }
}
