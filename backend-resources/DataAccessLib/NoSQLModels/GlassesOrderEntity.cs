using GlassesOrdersModels.Models;
using MongoDB.Bson;
using MongoDB.Driver.Core.Operations;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.NoSQLModels
{
    public class GlassesOrderEntity : DbEntity
    {
        public string OrderId { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.Undefined;
        public int GlassesCount { get; set; }
        public int CabinsCount { get; set; }
        public int PA0Count { get; set; }
        public List<CabinRowEntity> CabinRows { get; set; } = new();
        public List<GlassOrderRowEntity> GlassRows { get; set; } = new();
        

        public GlassesOrderEntity()
        {

        }

        public GlassesOrderEntity(GlassesOrder glassesOrder)
        {
            OrderId = glassesOrder.OrderId;
            Notes = glassesOrder.Notes;
            GlassesCount = glassesOrder.GlassesCount;
            CabinsCount = glassesOrder.CabinsCount;
            PA0Count = glassesOrder.PA0Count;
            Status = glassesOrder.Status;
            CabinRows = glassesOrder.CabinRows.Select(r=> new CabinRowEntity(r)).ToList();
            GlassRows = glassesOrder.GlassRows.Select(r => GlassOrderRowEntity.CreateEntity(r)).ToList();
        }

        public GlassesOrder ToGlassesOrder()
        {
            //The Glasses Order only has a GlassRows Property from which the Cabin Rows are read;
            //The Database saves GlassRowEntity only with a CabinKey,
            //So the Cabin Rows must be First Constructed and themselves
            //This will also create the needed Glass Rows , which will also have inside them References of the CabinRows

            // Construct the Glass Rows and Group them by their CabinKey
            var glassRowsGroups = GlassRows.Select(r => r.ToGlassOrderRow())
                .GroupBy(row=> row.CabinRowKey);
            
            // Construct the Cabin Rows by passing inside the Group that has the matching CabinKey
            IEnumerable<CabinOrderRow> cabinRows = CabinRows.Select(cabinRowEntity =>
            {
                // Get the Group of Glasses with the Same Cabin Key
                var glassRows = glassRowsGroups.FirstOrDefault(group => group.Key == cabinRowEntity.CabinKey)?.ToList() 
                ?? [GlassOrderRow.CreateNew("NOTFOUNDGLASSROW", "NOTFOUNDGLASSROW",0,new(),cabinRowEntity.CabinKey,"",false,0,0)];
                // Conver the CabinOrderRowEntity into a CabinOrderRow
                return cabinRowEntity.ToCabinOrderRow(glassRows);
            });

            // Put the Glasses Order Together
            GlassesOrder order = new()
            {
                OrderId = this.OrderId,
                Created = this.Created,
                LastModified = this.LastModified,
                Notes = this.Notes,
            };
            order.AddNewRows(cabinRows);
            return order;
        }
    }

    public class GlassOrderRowEntity : DbEntity
    {
        public string ReferencePA0 { get; set; } = "????";
        public bool IsFromStock { get; set; } = false;
        public int Quantity { get; set; }
        public Guid CabinRowKey { get; set; }
        public Glass OrderedGlass { get; set; } = new();
        public string? SpecialDrawString { get; set; }
        public int? SpecialDrawNumber { get; set; }
        public OrderStatus Status { get; set; }
        public int FilledQuantity { get; set; }
        public int CancelledQuantity { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public GlassOrderRowEntity()
        {

        }
        private GlassOrderRowEntity(GlassOrderRow row)
        {
            ReferencePA0 = row.ReferencePA0;
            Notes = row.Notes;
            Quantity = row.Quantity;
            CabinRowKey = row.CabinRowKey;
            OrderedGlass = row.OrderedGlass.GetDeepClone();
            Status = row.Status;
            IsFromStock = row.IsFromStock;
            FilledQuantity = row.FilledQuantity;
            CancelledQuantity = row.CancelledQuantity;
            OrderId = row.OrderId;
            LastModified = row.LastModified;
            SpecialDrawNumber = row.SpecialDrawNumber;
            SpecialDrawString = row.SpecialDrawString;
            if (ObjectId.TryParse(row.RowId, out ObjectId rowId) && rowId != default)
            {
                Id = rowId;
            }
            else
            {
                //do Nothing if there is no Parsable Id in the Row . The Repository will take care of Inserts of new Items
            }
        }
        public GlassOrderRow ToGlassOrderRow()
        {
            var row = GlassOrderRow.CreateNew(
                ReferencePA0,
                Notes,
                Quantity,
                OrderedGlass.GetDeepClone(),
                CabinRowKey,
                Id.ToString(),
                IsFromStock,
                FilledQuantity,
                CancelledQuantity);
            row.OrderId = OrderId;
            row.LastModified = LastModified;
            row.Created = Created;
            row.SpecialDrawString = SpecialDrawString;
            row.SpecialDrawNumber = SpecialDrawNumber;
            return row;
        }

        public static GlassOrderRowEntity CreateEntity(GlassOrderRow row)
        {
            return new(row);
        }
    }

    /// <summary>
    /// An Entity Containing Information about a CabinRow in a GlassesOrder
    /// The Ordered Cabins DO NOT HAVE GLASSES SAVED IN THE DATABASE. THEY ARE RETRIEVED AND ADDED WHEN THE CabinOrderRow Object is Constructed
    /// </summary>
    public class CabinRowEntity : DbEntity
    {
        public string ReferencePA0 { get; set; } = "????";
        public int Quantity { get; set; }
        public CabinEntity OrderedCabin { get; set; } = new();
        public Guid SynthesisKey { get; set; }
        public Guid CabinKey { get; set; }
        public OrderStatus Status { get; set; }
        public string OrderId { get; set; } = string.Empty;

        private CabinRowEntity() { }

        public CabinRowEntity(CabinOrderRow row)
        {
            ReferencePA0 = row.ReferencePA0;
            Notes = row.Notes;
            Quantity = row.Quantity;
            OrderedCabin = new CabinEntity(row.OrderedCabin);
            Status = row.Status;
            SynthesisKey = row.SynthesisKey;
            CabinKey= row.CabinKey;
            OrderId = row.OrderId;
            LastModified = row.LastModified;
            if (ObjectId.TryParse(row.RowId,out ObjectId rowId) && rowId != default)
            {
                Id = rowId;
            }
            else
            {
                //Do not Create an Id if there is not one , The Repository will take care of the Inserts 
                //Otherwise something is wrong
            }
        }

        /// <summary>
        /// Transforms this entity into a CabinOrderRow
        /// </summary>
        /// <param name="glassRows">The Glass Rows inside the Row , They Must be Retrieved Matching the Cabin Key to Construct the Row</param>
        /// <returns></returns>
        public CabinOrderRow ToCabinOrderRow(IEnumerable<GlassOrderRow> glassRows)
        {
            var row = CabinOrderRow.CreateNew(ReferencePA0, Notes, Quantity, OrderedCabin.ToCabin(), glassRows, SynthesisKey, CabinKey, Id.ToString());
            row.OrderId = OrderId;
            row.Created = Created;
            row.LastModified = LastModified;

            // Add the Glasses also to the Parent Cabin
            foreach (var glassRow in row.GlassesRows)
            {
                row.OrderedCabin.Glasses.Add(glassRow.OrderedGlass);
            }

            return row;
        }

        public static CabinRowEntity CreateEmpty()
        {
            return new();
        }
    }
}
