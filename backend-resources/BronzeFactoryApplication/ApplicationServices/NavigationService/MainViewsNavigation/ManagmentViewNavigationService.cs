using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.MainViewsNavigation
{
    public class ManagmentViewNavigationService : INavigationService
    {
        private readonly Lazy<ManagmentViewModel> vm;
        private readonly NavigationStore navStore;

        public ManagmentViewNavigationService(Lazy<ManagmentViewModel> vm, NavigationStore navStore)
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
