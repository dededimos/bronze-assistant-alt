using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BronzeFactoryApplication.ApplicationServices.DataService.GalaxyOrders;

/// <summary>
/// A Product Row in a Bronze Document
/// </summary>
public class BronzeProductRow
{
    //Weather to Ignore the Ending Dash in a Code
    private readonly bool ignoreCodeEndDash;

    private string code = string.Empty;
    public string Code { get => ignoreCodeEndDash ? code.TrimEnd('-') : code; set { if (code != value) code = value; } }

    /// <summary>
    /// Length for Cabins
    /// </summary>
    public string Charachteristic1 { get; set; } = string.Empty;
    /// <summary>
    /// Metal Finish and Glass Finish for Cabins
    /// </summary>
    public string Charachteristic2 { get; set; } = string.Empty;
    /// <summary>
    /// Height for Cabins
    /// </summary>
    public string Charachteristic3 { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal StartingPrice { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal NetPrice { get => Quantity != 0 ? StartingPrice - (TotalDiscount / Convert.ToDecimal(Quantity)) : 0; }
    public decimal DiscountPerItem { get => StartingPrice - NetPrice; }
    public decimal DiscountPercent { get => StartingPrice != 0 ? 1 - (NetPrice / StartingPrice) : 0; }
    public double Quantity { get; set; }

    public BronzeProductRow(string code, 
        string description, 
        decimal startingPrice, 
        decimal totalDiscount, 
        double quantity,
        string charachteristic1,
        string charachteristic2,
        string charachteristic3,
        bool ignoreCodeEndDash = true)
    {
        this.code = code;
        Description = description;
        StartingPrice = startingPrice;
        TotalDiscount = totalDiscount;
        Quantity = quantity;
        Charachteristic1 = charachteristic1;
        Charachteristic2 = charachteristic2;
        Charachteristic3 = charachteristic3;
        this.ignoreCodeEndDash = ignoreCodeEndDash;
    }

}
