using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.MainViewsNavigation
{
    public class BathroomAccessoriesNavigationService : INavigationService
    {
        private readonly Lazy<AccessoriesModuleViewModel> vm;
        private readonly NavigationStore navStore;

        public BathroomAccessoriesNavigationService(Lazy<AccessoriesModuleViewModel> vm, NavigationStore navStore)
        {
            this.vm = vm;
            this.navStore = navStore;
        }

        public async Task NavigateAsync()
        {
            await navStore.ChangeViewAsync(vm.Value);
        }
    }

    // Must change all Menu Nav Services with this
    public class NavigationService<T> where T : BaseViewModel
    {
        protected readonly NavigationStore navStore;
        protected readonly T vm;

        public NavigationService(NavigationStore navStore, T vm)
        {
            this.navStore = navStore;
            this.vm = vm;
        }

        public async Task NavigateAsync()
        {
            await navStore.ChangeViewAsync(vm);
        }
    }
}
