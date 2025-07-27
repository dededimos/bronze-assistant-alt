using EnumsNET;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;

namespace GlassesOrdersModels.Models
{
    public class CabinOrderRow
    {
        public string ReferencePA0 { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public Cabin OrderedCabin { get; set; } = Cabin.Empty();
        public List<GlassOrderRow> GlassesRows { get; set; } = new();
        public bool HasGlassFromStock { get => GlassesRows.Any(r => r.IsFromStock); }
        public Guid SynthesisKey { get; set; }
        public Guid CabinKey { get; set; }
        public OrderStatus Status { get => GlassesOrder.GetCombinedStatus(GlassesRows.Select(r => r.Status)); }
        public string OrderId { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string RowId { get; set; } = string.Empty;
        
        /// <summary>
        /// Constructs a Cabin Row from the Provided Arguments
        /// </summary>
        /// <param name="referencePA0">The PA0 Number</param>
        /// <param name="notes">Notes for this particular Row</param>
        /// <param name="quantity">The Number of Pieces of the Ordered Cabin</param>
        /// <param name="orderedCabin">The Ordered Cabin object</param>
        /// <param name="glassRows">The Glass Rows asscociated with this Row</param>
        /// <param name="synthesisKey">The key associating this row with a synthesis</param>
        /// <param name="cabinKey">The key of reference of this row</param>
        private CabinOrderRow(string referencePA0, 
                             string notes, 
                             int quantity,
                             Cabin orderedCabin,
                             IEnumerable<GlassOrderRow> glassRows,
                             Guid synthesisKey,
                             Guid cabinKey,
                             string rowId)
        {
            ReferencePA0 = referencePA0;
            Notes = notes;
            Quantity = quantity;
            OrderedCabin = orderedCabin;
            SynthesisKey = synthesisKey;
            CabinKey = cabinKey;
            RowId = rowId;
            GlassesRows = new(glassRows);
            Created = DateTime.Now;

            foreach (var row in GlassesRows)
            {
                row.ParentCabinRow = this;
            }
        }

        private CabinOrderRow()
        {

        }

        public static CabinOrderRow Empty()
        {
            return new CabinOrderRow();
        }

        public static CabinOrderRow CreateNew(string referencePA0,
            string notes,
            int quantity,
            Cabin orderedCabin,
            IEnumerable<GlassOrderRow> glassRows,
            Guid synthesisKey,
            Guid cabinKey,
            string rowId)
        {
            return new CabinOrderRow(referencePA0,notes,quantity,orderedCabin,glassRows,synthesisKey,cabinKey,rowId);
        }


    }


}
