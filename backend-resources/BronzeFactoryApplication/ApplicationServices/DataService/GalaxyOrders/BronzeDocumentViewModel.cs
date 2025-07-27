using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ApplicationServices.DataService.GalaxyOrders
{
    public partial class BronzeDocumentViewModel : BaseViewModel
    {
        private readonly BronzeDocument document;

        public string DocumentNumber { get => document.DocumentNumber; }
        public string DocumentSeriesNumber { get => document.DocumentSeriesNumber; }
        public DateTime Date { get => document.Date; }
        public string ClientName { get => document.ClientName; }
        public string ClientAddress { get => document.ClientAddress; }
        public decimal StartingTotalValue { get => document.StartingTotalValue; }
        public decimal DiscountValue { get => document.DiscountValue; }
        public decimal NetTotalValue { get => document.NetTotalValue; }
        public OrderTransformationState StateOfTransformation { get => document.StateOfTransformation; }

        public ObservableCollection<BronzeProductRowViewModel> Rows { get; }

        public BronzeDocumentViewModel(BronzeDocument document , ValidatorCabinCode cabinCodeValidator)
        {
            this.document = document;
            Rows = new(document.Rows.Select(r => new BronzeProductRowViewModel(r, cabinCodeValidator)));
        }
    }

}
