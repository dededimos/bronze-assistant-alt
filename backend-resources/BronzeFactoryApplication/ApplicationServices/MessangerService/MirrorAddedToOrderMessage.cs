using MirrorsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.MessangerService
{
    /// <summary>
    /// A Message informing that a Mirror was added to an Order
    /// </summary>
    public class MirrorAddedToOrderMessage
    {
        public MirrorAddedToOrderMessage(MirrorSynthesis mirrorAddedToOrder)
        {
            MirrorAddedToOrder = mirrorAddedToOrder;
        }
        public MirrorSynthesis MirrorAddedToOrder { get; set; }
    }
}
