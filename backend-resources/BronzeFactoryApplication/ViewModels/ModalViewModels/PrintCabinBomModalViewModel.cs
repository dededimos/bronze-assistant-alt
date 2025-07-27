using BronzeFactoryApplication.Helpers.Other;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class PrintCabinBomModalViewModel : ModalViewModel
    {
        private readonly Func<CabinBomViewModel> bomVmFactory;

        [ObservableProperty]
        private CabinBomViewModel bom;

        [ObservableProperty]
        private ObservableCollection<CabinBomViewModel> boms = new();

        public PrintCabinBomModalViewModel(Func<CabinBomViewModel> bomVmFactory)
        {
            Title = "lngBomTitle".TryTranslateKey();
            this.bomVmFactory = bomVmFactory;
            bom = bomVmFactory.Invoke();
        }

        public void AddCabinBom(CabinOrderRow cabinRow)
        {
            var newBom = bomVmFactory.Invoke();
            newBom.SetCabinRow(cabinRow);
            Boms.Add(newBom);
        }

        [RelayCommand]
        private void PrintBoms()
        {
            WPFHelpers.PrintCabinBoms(Boms);
        }
        [RelayCommand]
        private void PrintSingleBom(FrameworkElement element)
        {
            if(Boms.Any())
            WPFHelpers.PrintFrameWorkElement(element);
        }

    }
}
