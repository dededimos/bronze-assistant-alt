using CommonInterfacesBronze;
using CommonOrderModels;

namespace MongoDbCommonLibrary.CommonEntities
{
    public class BronzeOrderRowEntity : MongoDatabaseEntity, IDeepClonable<BronzeOrderRowEntity>
    {
        public string ParentOrderNo { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double PendingQuantity { get; set; }
        public double FilledQuantity { get; set; }
        public double CancelledQuantity { get; set; }
        public OrderStatus Status { get; set; }
        public MeasurementUnit RowUnits { get; set; }
        public int LineNumber { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = [];

        public override BronzeOrderRowEntity GetDeepClone()
        {
            var clone = (BronzeOrderRowEntity)base.GetDeepClone();
            clone.Metadata = new(this.Metadata);
            return clone;
        }

        public BronzeOrderRowBase<TItem> CopyBasePropertiesToRow<TItem>(BronzeOrderRowBase<TItem> row)
            where TItem : class, ICodeable, IDeepClonable<TItem>
        {
            row.RowId = this.Id;
            row.ParentOrderNo = this.ParentOrderNo;
            row.Notes = this.Notes;
            row.Quantity = this.Quantity;
            row.FilledQuantity = this.FilledQuantity;
            row.CancelledQuantity = this.CancelledQuantity;
            row.RowUnits = this.RowUnits;
            row.Created = this.Created;
            row.LastModified = this.LastModified;
            row.LineNumber = this.LineNumber;
            row.Metadata = new(this.Metadata);
            return row;
        }
        public void CopyFromModelsBaseProperties<TItem>(BronzeOrderRowBase<TItem> baseModel)
            where TItem : class, ICodeable, IDeepClonable<TItem>
        {
            this.Id = baseModel.RowId;
            this.ParentOrderNo = baseModel.ParentOrderNo;
            this.Notes = baseModel.Notes;
            this.Quantity = baseModel.Quantity;
            this.PendingQuantity = baseModel.PendingQuantity;
            this.FilledQuantity = baseModel.FilledQuantity;
            this.CancelledQuantity = baseModel.CancelledQuantity;
            this.Status = baseModel.Status;
            this.RowUnits = baseModel.RowUnits;
            this.LastModified = baseModel.LastModified;
            this.LineNumber = baseModel.LineNumber;
            this.Metadata = new(baseModel.Metadata);
        }
    }

}
