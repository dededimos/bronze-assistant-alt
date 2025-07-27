using CommonInterfacesBronze;

namespace CommonOrderModels
{
    /// <summary>
    /// Row Object for a Mixed Items Order
    /// </summary>
    public abstract class BronzeOrderRowBase<TItem> : IDeepClonable<BronzeOrderRowBase<TItem>>
        where TItem : class ,ICodeable , IDeepClonable<TItem> 
    {
        public const string PaoPamMetadataKey = "PAOPAM";
        public string RowId { get; set; } = string.Empty;
        public string ParentOrderNo { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double PendingQuantity { get => Quantity - FilledQuantity - CancelledQuantity; }
        public double FilledQuantity { get; set; }
        public double CancelledQuantity { get; set; }
        public OrderStatus Status { get => GetStatus(PendingQuantity, FilledQuantity, CancelledQuantity); }
        /// <summary>
        /// Gets the Order Status of a row with the given quantities
        /// </summary>
        /// <param name="pendingQuantity"></param>
        /// <param name="filledQuantity"></param>
        /// <param name="cancelledQuantity"></param>
        /// <returns></returns>
        public static OrderStatus GetStatus(double pendingQuantity, double filledQuantity, double cancelledQuantity)
        {
            var status = OrderStatus.UnspecifiedOrderStatus;

            if (pendingQuantity > 0) status |= OrderStatus.PendingOrderStatus;
            if (filledQuantity > 0) status |= OrderStatus.FilledOrderStatus;
            if (cancelledQuantity > 0) status |= OrderStatus.CancelledOrderStatus;
            return status;
        }
        public MeasurementUnit RowUnits { get; set; }
        public TItem? RowItem { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public int LineNumber { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = [];

        public void AddMetadata(string key, string metadata)
        {
            Metadata.Add(key, metadata);
        }
        public void RemoveMetadata(string key) { Metadata.Remove(key); }
        public string? GetMetadata(string key)
        {
            Metadata.TryGetValue(key, out var metadata);
            return metadata;
        }

        public virtual BronzeOrderRowBase<TItem> GetDeepClone()
        {
            var clone = (BronzeOrderRowBase<TItem>)MemberwiseClone();
            clone.RowItem = RowItem?.GetDeepClone();
            clone.Metadata = new(Metadata);
            return clone;
        }
    }
}
