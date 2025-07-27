using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.DataService.GalaxyOrders
{
    public partial class BronzeProductRowViewModel : BaseViewModel
    {
        /// <summary>
        /// The Product Row Represented by this ViewModel
        /// </summary>
        private BronzeProductRow row;

        public string Code { get => row.Code; }
        public string Description { get => row.Description; }
        public double Quantity { get => row.Quantity; }
        public decimal StartingPrice { get => row.StartingPrice; }
        public decimal DiscountPercent { get => row.DiscountPercent; }
        public decimal NetPrice { get => row.NetPrice; }

        //Special Charachteristics (If Cabin 1-3 Length-Height , Middle is Metal-and-Glass Finish)
        public string Charachteristic1 { get => IsCabin && decimal.TryParse(row.Charachteristic1, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal c1) ? c1.ToString("F0") : row.Charachteristic1; }
        public string Charachteristic2 { get => row.Charachteristic2; }
        public string Charachteristic3 { get => IsCabin && decimal.TryParse(row.Charachteristic3, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal c3) ? c3.ToString("F0") : row.Charachteristic3; }

        /// <summary>
        /// Weather this is a Cabin Row
        /// </summary>
        public bool IsCabin { get;}

        /// <summary>
        /// Weather this Row has Special Charachteristics
        /// </summary>
        public bool HasCharachteristics { get => !string.IsNullOrEmpty(Charachteristic1); }

        /// <summary>
        /// Weather this Row has been selected
        /// </summary>
        [ObservableProperty]
        private bool isSelected;

        public BronzeProductRowViewModel(BronzeProductRow row , ValidatorCabinCode cabinCodeValidator)
        {
            this.row = row;
            IsCabin = cabinCodeValidator.Validate(row.Code).IsValid;
        }
    }
}
