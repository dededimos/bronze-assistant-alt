using BronzeFactoryApplication.ViewModels.OrderRelevantViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public class ImportOrderModalViewModel : ModalViewModel
    {
        public GalaxyOrdersDisplayViewModel GalaxyOrderViewModel { get; set; }

        public ImportOrderModalViewModel(GalaxyOrdersDisplayViewModel galaxyOrdersViewModel)
        {
            Title = "lngImportOrdersModalTitle".TryTranslateKey();
            GalaxyOrderViewModel = galaxyOrdersViewModel;
        }

    }
}
