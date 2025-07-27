namespace BronzeFactoryApplication.ApplicationServices.NavigationService.MainViewsNavigation
{
    public class WharehouseStockNavigationService : INavigationService
    {
        public WharehouseStockNavigationService(NavigationStore navStore , Lazy<WharehouseModuleViewModel> vm)
        {
            this.navStore = navStore;
            this.vm = vm;
        }
        private readonly NavigationStore navStore;
        private readonly Lazy<WharehouseModuleViewModel> vm;


        public async Task NavigateAsync()
        {
            await navStore.ChangeViewAsync(vm.Value);
        }
    }
}
