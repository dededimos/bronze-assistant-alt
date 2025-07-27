using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.MessangerService
{
    /// <summary>
    /// A Message including information about the Edits on a CabinOrderRow
    /// </summary>
    public class CabinRowEditMessage
    {
        public CabinOrderRow OldRow { get; set; }
        public CabinOrderRow NewRow { get; set; }

        public CabinRowEditMessage(CabinOrderRow oldRow, CabinOrderRow newRow)
        {
            OldRow = oldRow;
            NewRow = newRow;
        }
    }
}
