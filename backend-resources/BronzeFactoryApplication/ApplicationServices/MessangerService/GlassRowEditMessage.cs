using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.MessangerService
{
    /// <summary>
    /// A Message to signal that a GlassRow Has been Edited
    /// </summary>
    public class GlassRowEditMessage
    {
        public GlassOrderRow OldRow { get; set; }
        public GlassOrderRow NewRow { get; set; }

        public GlassRowEditMessage(GlassOrderRow oldRow, GlassOrderRow newRow)
        {
            OldRow = oldRow;
            NewRow = newRow;
        }
    }
}
