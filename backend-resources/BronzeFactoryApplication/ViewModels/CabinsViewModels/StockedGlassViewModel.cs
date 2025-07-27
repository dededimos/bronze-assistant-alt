using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.CabinsViewModels
{
    public partial class StockedGlassViewModel : BaseViewModel
    {
        [ObservableProperty]
        private GlassViewModel glass = new();
        [ObservableProperty]
        private int quantity;
        [ObservableProperty]
        private string notes = string.Empty;
        [ObservableProperty]
        private string rowId = string.Empty;
        [ObservableProperty]
        private DateTime created = DateTime.Now;
        [ObservableProperty]
        private DateTime lastModified = DateTime.Now;
        
        public StockedGlassRow GetStockedGlass()
        {
            StockedGlassRow row = new(RowId, Glass.GetGlass(), Quantity, Notes, Created, LastModified);
            return row;
        }
        public StockedGlassViewModel(StockedGlassRow stockedGlassToSet)
        {
            this.Glass.SetGlass(stockedGlassToSet.Glass, false);
            this.Quantity = stockedGlassToSet.Quantity;
            this.Notes = stockedGlassToSet.Notes;
            this.RowId = stockedGlassToSet.RowId;
            this.Created = stockedGlassToSet.Created;
            this.LastModified = stockedGlassToSet.LastModified;
        }
    }
}
