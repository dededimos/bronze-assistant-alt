using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.MainViewsNavigation
{
    public class SearchOrdersModuleNavigationService : INavigationService
    {
        private readonly Lazy<OrdersModuleViewModel> vm;
        private readonly NavigationStore navStore;

        public SearchOrdersModuleNavigationService(Lazy<OrdersModuleViewModel> vm, NavigationStore navStore)
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
