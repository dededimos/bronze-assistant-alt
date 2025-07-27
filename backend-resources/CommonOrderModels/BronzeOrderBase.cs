using CommonInterfacesBronze;

namespace CommonOrderModels
{
    /// <summary>
    /// Mixed Items Order
    /// </summary>
    public abstract class BronzeOrderBase<TRow,TItem>  : IDeepClonable<BronzeOrderBase<TRow,TItem>>
        where TRow : BronzeOrderRowBase<TItem> , IDeepClonable<TRow>
        where TItem : class, ICodeable, IDeepClonable<TItem>
    {
        public const string TotalPAOPAMMetadataKey = "TotalPAOPAM";
        public const string newOrderNo = "????";

        /// <summary>
        /// The Unique Id Of the Order
        /// </summary>
        public string Id { get; set; } = string.Empty;
        public string OrderNo { get; set; } = newOrderNo;
        public List<TRow> Rows { get; set; } = [];
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string Notes { get; set; } = string.Empty;
        public OrderStatus Status { get => GetCombinedStatus(Rows.Select(r => r.Status)); }

        public Dictionary<string, string> Metadata { get; set; } = [];

        /// <summary>
        /// Returns the combined status of a collection of Statuses
        /// </summary>
        /// <param name="statuses"></param>
        /// <returns></returns>
        public static OrderStatus GetCombinedStatus(IEnumerable<OrderStatus> statuses)
        {
            // Find if there are any Rows Pending
            var isPending = statuses.Any(s => s == OrderStatus.PendingOrderStatus);
            var isCancelled = statuses.Any(s => s == OrderStatus.CancelledOrderStatus);
            var isFilled = statuses.Any(s => s == OrderStatus.FilledOrderStatus);

            var status = OrderStatus.UnspecifiedOrderStatus;

            if (isPending) status |= OrderStatus.PendingOrderStatus;
            if (isCancelled) status |= OrderStatus.CancelledOrderStatus;
            if (isFilled) status |= OrderStatus.FilledOrderStatus;

            return status;
        }
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

        public virtual BronzeOrderBase<TRow, TItem> GetDeepClone()
        {
            var clone = (BronzeOrderBase<TRow, TItem>)MemberwiseClone();
            clone.Rows = Rows.GetDeepClonedList();
            clone.Metadata = new(Metadata);
            return clone;
        }
    }
}
