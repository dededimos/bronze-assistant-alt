using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.MainViewsNavigation;

public class CabinsModuleNavigationService : INavigationService
{
    private readonly Lazy<ShowerCabinsModuleViewModel> vm;
    private readonly NavigationStore navStore;

    public CabinsModuleNavigationService(Lazy<ShowerCabinsModuleViewModel> vm, NavigationStore navStore)
    {
        this.vm = vm;
        this.navStore = navStore;
    }

    /// <summary>
    /// Navigates to the Shower Cabins Module
    /// </summary>
    public async Task NavigateAsync()
    {
        await navStore.ChangeViewAsync(vm.Value);
    }
}
