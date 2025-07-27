using BronzeFactoryApplication.ViewModels.OrderRelevantViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.Helpers.ViewModelFactories
{
    public class GlassOrderRowViewModelFactory
    {
        private readonly Func<GlassOrderRowViewModel> vmFactory;

        public GlassOrderRowViewModelFactory(Func<GlassOrderRowViewModel> vmFactory)
        {
            this.vmFactory = vmFactory;
        }

        public GlassOrderRowViewModel Create(GlassOrderRow row)
        {
            var vm = vmFactory.Invoke();
            vm.InitilizeViewModel(row);
            return vm;
        }
    }
}
