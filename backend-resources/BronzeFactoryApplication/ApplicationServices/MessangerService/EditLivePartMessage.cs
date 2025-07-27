using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.MessangerService
{
    /// <summary>
    /// A message indicating a part Should be Edited
    /// </summary>
    public class EditLivePartMessage
    {
        public PartSpot SpotToEdit { get; set; }
        public CabinPart PartToEdit { get; set; }
        public PartsViewModel Sender { get; set; }

        /// <summary>
        /// Creates a message indicating a part Should be Edited
        /// </summary>
        /// <param name="spotToEdit">The Spot Where the Part to be Edited is placed</param>
        /// <param name="partToEdit">The Part to Edit</param>
        /// <param name="sender">The sender of the Message</param>
        public EditLivePartMessage(PartSpot spotToEdit, CabinPart partToEdit, PartsViewModel sender)
        {
            SpotToEdit = spotToEdit;
            PartToEdit = partToEdit;
            Sender = sender;
        }

    }

}
