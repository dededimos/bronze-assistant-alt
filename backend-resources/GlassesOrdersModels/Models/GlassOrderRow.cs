using CommonInterfacesBronze;
using EnumsNET;
using ShowerEnclosuresModelsLibrary.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GlassesOrdersModels.Models
{
    public class GlassOrderRow : IDeepClonable<GlassOrderRow>
    {
        /// <summary>
        /// The Reference PA0 number that needs these Row of Glass(es)
        /// </summary>
        public string ReferencePA0 { get; set; } = "????";
        /// <summary>
        /// Notes for this Particular Row of Glasses
        /// </summary>
        public string Notes { get; set; } = string.Empty;
        /// <summary>
        /// How many Glasses of the Same Type are located in this Glass Row
        /// </summary>
        public int Quantity { get; set; } = 1;
        /// <summary>
        /// Which CabinRow contains this Glass Row
        /// </summary>
        public Guid CabinRowKey { get; set; } = Guid.Empty;
        /// <summary>
        /// The Cabin Structure that Uses these Glasses
        /// </summary>
        public CabinOrderRow? ParentCabinRow { get; set; }
        /// <summary>
        /// The Glass that represents the Glasses of this Row
        /// </summary>
        public Glass OrderedGlass { get; set; } = new();
        /// <summary>
        /// The String of a Special Draw (e.x. Letter K for Step)
        /// </summary>
        public string? SpecialDrawString { get; set; } = null;
        /// <summary>
        /// The Number of the Special draw (e.x. '1' for 9FK1)
        /// </summary>
        public int? SpecialDrawNumber { get; set; } = null;
        /// <summary>
        /// The Order Status for this Row
        /// </summary>
        public OrderStatus Status { get => GetStatus(); }
        /// <summary>
        /// The Filled Quantity of the Order
        /// </summary>
        public int FilledQuantity { get; set; }
        /// <summary>
        /// The Cancelled Quantity of the Order
        /// </summary>
        public int CancelledQuantity { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string RowId { get; set; } = string.Empty;
        /// <summary>
        /// Weather this Glass Row should be From Stock
        /// </summary>
        public bool IsFromStock { get; set; }

        /// <summary>
        /// Returns the OrderStatus of this Order Row Based on the Quantity Filled
        /// </summary>
        /// <returns></returns>
        private OrderStatus GetStatus()
        {
            // Mark Undefined when QTY is zero
            // Mark Cancelled when ALL are Cancelled
            // Add Cancelled if at least one is Cancelled
            // Filled QTY = 0 => Pending
            // Filled QTY < QuantityToFill => Available with Pending
            // Filled QTY >= QuantityToFill => Available
            
            OrderStatus status = OrderStatus.Undefined;
            if (Quantity is 0)
            {
                return OrderStatus.Undefined;
            }

            //Mark as cancelled
            if (CancelledQuantity != 0)
            {
                status = status.CombineFlags(OrderStatus.Cancelled);
            }
            //Find the Remaining Quantity to be Filled
            int remainingQuantityToFill = Quantity - CancelledQuantity;
            if (remainingQuantityToFill == 0) return OrderStatus.Cancelled;
            
            if (FilledQuantity is 0)
            {
                status = status.CombineFlags(OrderStatus.Pending);
            }
            else if (FilledQuantity < remainingQuantityToFill) 
            {
                status = status.CombineFlags(OrderStatus.PartiallyAvailable);
            }
            else
            {
                status = status.CombineFlags(OrderStatus.Available);
            }

            return status;

        }

        private GlassOrderRow(string referencePA0, 
                             string notes, 
                             int quantity,
                             Glass orderedGlass,
                             Guid cabinRowKey,
                             string rowId,
                             bool isFromStock,
                             int filledQuantity = 0, 
                             int cancelledQuantity = 0)
        {
            ReferencePA0 = referencePA0;
            Notes = notes;
            Quantity = quantity;
            OrderedGlass = orderedGlass;
            CabinRowKey = cabinRowKey;
            FilledQuantity = filledQuantity;
            CancelledQuantity = cancelledQuantity;
            Created = DateTime.Now;
            RowId = rowId;
            IsFromStock = isFromStock;
        }

        private GlassOrderRow()
        {

        }

        public static GlassOrderRow Empty()
        {
            return new GlassOrderRow();
        }

        /// <summary>
        /// Gets a DeepClone of the GlassOrderRow without DeepCloning the Parent thought
        /// </summary>
        /// <returns></returns>
        public GlassOrderRow GetDeepClone()
        {
            return new GlassOrderRow(ReferencePA0, Notes, Quantity, OrderedGlass.GetDeepClone(), CabinRowKey, RowId,IsFromStock,FilledQuantity, CancelledQuantity)
            {
                ParentCabinRow = null,
                OrderId = OrderId,
                Created = Created,
                LastModified = LastModified,
                SpecialDrawString = SpecialDrawString,
                SpecialDrawNumber = SpecialDrawNumber
            };
        }

        public static GlassOrderRow CreateNew(string referencePA0,
                             string notes,
                             int quantity,
                             Glass orderedGlass,
                             Guid cabinRowKey,
                             string rowId,
                             bool isFromStock,
                             int filledQuantity = 0,
                             int cancelledQuantity = 0)
        {
            return new GlassOrderRow(referencePA0,notes,quantity,orderedGlass,cabinRowKey,rowId, isFromStock, filledQuantity, cancelledQuantity);
        }

    }


}
