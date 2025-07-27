using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.MessangerService
{
    /// <summary>
    /// A message signaling 
    /// </summary>
    public class AddCabinRowsMessage 
    {
        public IEnumerable<CabinOrderRow> Rows { get; set; }

        public AddCabinRowsMessage(IEnumerable<CabinOrderRow> rows)
        {
            Rows = rows;
        }
    }
}
