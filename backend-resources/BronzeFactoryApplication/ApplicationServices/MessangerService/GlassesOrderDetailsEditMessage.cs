using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.MessangerService
{
    /// <summary>
    /// A Message to Edit the Details of an Glasses Order
    /// </summary>
    public class GlassesOrderDetailsEditMessage
    {
        public string OrderId { get; set; }
        public string Notes { get; set; }

        public GlassesOrderDetailsEditMessage(string orderId , string notes)
        {
            OrderId = orderId;
            Notes = notes;
        }
    }
}
