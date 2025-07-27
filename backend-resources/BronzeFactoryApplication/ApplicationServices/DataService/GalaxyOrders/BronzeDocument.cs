namespace BronzeFactoryApplication.ApplicationServices.DataService.GalaxyOrders;

/// <summary>
/// A Bronze Document from Galaxy
/// </summary>
public class BronzeDocument
{
    /// <summary>
    /// Date Of Order
    /// </summary>
    public DateTime Date { get; set; }
    /// <summary>
    /// Order Document Type ("PA0" , "PAM" , e.t.c.)
    /// </summary>
    public string DocumentSeries { get; set; } 
    /// <summary>
    /// The Number of Document "PAO,PAM e.t.c."
    /// </summary>
    public string DocumentSeriesNumber { get; set; }
    public string DocumentNumber { get => $"{DocumentSeries}-{DocumentSeriesNumber}"; }

    /// <summary>
    /// If this Has Been Fully/Partially/None Transformed into an Invoice/Delivery e.t.c.
    /// </summary>
    public OrderTransformationState StateOfTransformation { get; set; } 
    public string ClientName { get; set; } 
    public string ClientAddress { get; set; } 
    public decimal StartingTotalValue { get => Rows.Sum(r => r.StartingPrice); }
    public decimal DiscountValue { get => Rows.Sum(r => r.TotalDiscount); }
    public decimal NetTotalValue { get => StartingTotalValue - DiscountValue; }
    public List<BronzeProductRow> Rows { get; set; } = new();

    public BronzeDocument(DateTime date,
        string documentSeries,
        string documentNumber,
        OrderTransformationState stateOfTransformation,
        string clientName,
        string clientAddress,
        IEnumerable<BronzeProductRow> products)
    {
        Date = date;
        DocumentSeries = documentSeries;
        DocumentSeriesNumber = documentNumber;
        StateOfTransformation = stateOfTransformation;
        ClientName = clientName;
        ClientAddress = clientAddress;
        Rows.AddRange(products);
    }

}
