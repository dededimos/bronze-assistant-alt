namespace BronzeFactoryApplication.ApplicationServices.NavigationService.MainViewsNavigation
{
    public class MirrorsModuleNavigationService : INavigationService
    {
        private readonly Lazy<MirrorsModuleViewModel> vm;
        private readonly NavigationStore navStore;

        public MirrorsModuleNavigationService(Lazy<MirrorsModuleViewModel> vm, NavigationStore navStore)
        {
            this.vm = vm;
            this.navStore = navStore;
        }
        public async Task NavigateAsync()
        {
            await navStore.ChangeViewAsync(vm.Value);
        }
    }
}
