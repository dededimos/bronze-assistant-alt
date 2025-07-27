using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.MessangerService
{
    public class GlassesOrderSelectedMessage : ValueChangedMessage<GlassesOrder>
    {
        /// <summary>
        /// The Class the Initiated the Message
        /// </summary>
        public Type SenderType { get; set; }

        public GlassesOrderSelectedMessage(GlassesOrder value , Type senderType) : base(value)
        {
            SenderType = senderType;
        }
    }
}
