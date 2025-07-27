using GlassesOrdersModels.Models;
using MongoDB.Bson;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.NoSQLModels
{
    public class StockedGlassRowEntity : DbEntity
    {
        public Glass Glass { get; set; }
        public int Quantity { get; set; }



        public StockedGlassRowEntity(ObjectId id , Glass glass , int quantity , string notes , DateTime lastModified)
        {
            Id = id;
            Glass = glass;
            Notes = notes;
            Quantity = quantity;
            LastModified = lastModified;
        }
        public StockedGlassRowEntity(StockedGlassRow row)
        {
            this.Glass = row.Glass;
            this.Id = ObjectId.TryParse(row.RowId, out ObjectId id) ? id : default;
            this.Notes = row.Notes;
            this.Quantity = row.Quantity;
            this.LastModified = row.LastModified;
        }

        public StockedGlassRow ToStockedGlassRow()
        {
            return new StockedGlassRow(this.Id.ToString(), this.Glass, this.Quantity,this.Notes,this.Created,this.LastModified);
        }
    }
}
