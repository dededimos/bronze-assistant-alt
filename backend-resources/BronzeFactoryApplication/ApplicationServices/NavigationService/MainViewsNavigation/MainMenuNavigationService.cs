using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.MainViewsNavigation;

public class MainMenuNavigationService : INavigationService
{
    private readonly NavigationStore navStore;
    private readonly Func<MenuViewModel> menuVmFactory;

    public MainMenuNavigationService(NavigationStore navigationStore, Func<MenuViewModel> menuVmFactory)
    {
        navStore = navigationStore;
        this.menuVmFactory = menuVmFactory;
    }

    /// <summary>
    /// Navigate to Main Menu
    /// </summary>
    public async Task NavigateAsync()
    {
        await navStore.ChangeViewAsync(menuVmFactory.Invoke());
    }
}
