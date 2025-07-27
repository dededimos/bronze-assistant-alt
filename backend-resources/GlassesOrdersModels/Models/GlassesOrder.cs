using EnumsNET;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GlassesOrdersModels.Models;

public class GlassesOrder
{
    /// <summary>
    /// The Allowed Pattern for the Order Id (ex. 1524AC)
    /// Summary: What Does This Regex Do?
    ///<para>This regex validates if a string matches one of three possible formats:</para>
    ///<para>A numeric ID with optional trailing letters:</para>
    ///<para>1 to 4 digits(0-9999) , Optionally followed by 0 to 2 letters , ✅ Examples: 123, 99AB, 4567, 1Z , ❌ Not Allowed: ABCDE, 123456, 99ABCD</para>
    ///<para>Any string ending in "Old" ,✅ Examples: "OrderOld", "SomethingOld", "123Old",❌ Not Allowed: "OldSomething", "Older"</para>
    ///<para>Exactly four question marks '????' )</para>
    /// </summary>
    public static readonly Regex OrderIdRegex = new("^(?:[0-9]{1,4}[a-zA-Z]{0,2}|.*Old|\\?\\?\\?\\?)$");
    public static bool TryParseOrderIdMembers(string orderId , out (int orderNo,string orderLetter) parsedMemebers)
    {
        if (OrderIdRegex.IsMatch(orderId))
        {
            parsedMemebers.orderNo = int.Parse(string.Concat(orderId.Where(c => char.IsDigit(c))));
            parsedMemebers.orderLetter = string.Concat(orderId.Where(c => !char.IsDigit(c)));
            return true;
        }
        parsedMemebers.orderLetter = "";
        parsedMemebers.orderNo = 0;
        return false;
    }
    public static int ParseOrderIdNumber(string orderId , bool returnZeroInFailure = false)
    {
        if (OrderIdRegex.IsMatch(orderId))
        {
            var numbers = string.Join("", orderId.Where(c => char.IsDigit(c)));
            return int.Parse(numbers);
        }
        else if (returnZeroInFailure)
        {
            return 0;
        }
        throw new FormatException($"Provided {nameof(orderId)} was not at the Correct format");
    }
    public static string ParseOrderIdLetters(string orderId)
    {
        if (OrderIdRegex.IsMatch(orderId))
        {
            var letters= string.Join("", orderId.Where(c => !char.IsDigit(c)));
            return letters;
        }
        throw new FormatException($"Provided {nameof(orderId)} was not at the Correct format");
    }

    /// <summary>
    /// The Id of the Order
    /// </summary>
    public string OrderId { get; set; } = string.Empty;
    /// <summary>
    /// The time of Creation
    /// </summary>
    public DateTime Created { get; set; }
    /// <summary>
    /// The time last Modified
    /// </summary>
    public DateTime LastModified { get; set; }
    /// <summary>
    /// The Status of the Order
    /// </summary>
    public OrderStatus Status { get => GetCombinedStatus(GlassRows.Select(r=>r.Status));}
    public int GlassesCount { get => GlassRows.Sum(r=>r.Quantity); }
    public int CabinsCount { get => CabinRows.Sum(r=> r.Quantity); }
    public int PA0Count { get => CabinRows.Select(c => c.ReferencePA0).Distinct().Count(); }
    /// <summary>
    /// General Notes of the Order
    /// </summary>
    public string Notes { get; set; } = string.Empty;
    /// <summary>
    /// All the Rows for Cabins
    /// </summary>
    public List<CabinOrderRow> CabinRows { get => GlassRows.Where(r=> r.ParentCabinRow != null).Select(r => r.ParentCabinRow!).Distinct().ToList();}
    /// <summary>
    /// All the Rows for Glasses
    /// </summary>
    public List<GlassOrderRow> GlassRows { get; private set; } = new();
    public IEnumerable<GlassOrderRow> RowsFromStock { get => GlassRows.Where(r => r.IsFromStock); }

    public GlassesOrder()
    {
        
    }

    /// <summary>
    /// Groups all the Glass Rows of the Order By Draw , Thickness and Step
    /// </summary>
    /// <returns></returns>
    public List<List<GlassOrderRow>> GetGlassRowGroups(bool excludeStockGlasses)
    {
        //Combine Quantity of Similar Rows 

        //Find which Glasses are Invlolved for the Group
        var glassRowsInvolved = excludeStockGlasses ? GlassRows.Where(r=>r.IsFromStock is false) : GlassRows;

        //First Group the Rows so that each Group has equal Glasses
        var sameGlassGroups = glassRowsInvolved.GroupBy(row => row.OrderedGlass).Select(g=> g.ToList());
        //After grouping those glasses now we have to combine the items in each list into a single Row
        List<GlassOrderRow> finalRows = new();
        foreach (var sameGlassesList in sameGlassGroups)
        {
            //Create a new Combined Row from the First Row set its quantity to zero and
            //Put total Quantity to All
            GlassOrderRow combinedRow = sameGlassesList.First().GetDeepClone();
            
            //Empty the Parent Cabin
            combinedRow.ParentCabinRow = CabinOrderRow.Empty(); 
            
            //Combine Quantities
            combinedRow.Quantity = sameGlassesList.Sum(r=> r.Quantity);
            
            //Get all Different Notes and WriteThem
            var notesList = sameGlassesList.Select(r => r.Notes).Distinct();
            var notes = string.Join(Environment.NewLine, notesList);
            combinedRow.Notes = notes;
            
            //Get all Different PA0s and WriteThem
            var paosList = sameGlassesList.Select(r => r.ReferencePA0).Distinct();
            var paos = string.Join(Environment.NewLine, paosList);
            combinedRow.ReferencePA0 = paos;

            //Get all The Codes of the Different Cabin Codes
            var codesList = sameGlassesList.Select(r => r.ParentCabinRow?.OrderedCabin.Code ?? "Unavailable Code");
            var codes = string.Join(Environment.NewLine,codesList);
            combinedRow.ParentCabinRow.OrderedCabin.OverrideCode(codes);

            finalRows.Add(combinedRow);
        }

        //Have to create a custom object and override Equals and HashCode so that the Grouping can be compared , otherwise a Custom Comparer
        var groupedGlasses = finalRows.GroupBy(row => new GlassGroupKey(row.OrderedGlass.Draw, row.OrderedGlass.Thickness ?? GlassThicknessEnum.GlassThicknessNotSet, row.OrderedGlass.HasStep, row.OrderedGlass.Finish ?? GlassFinishEnum.GlassFinishNotSet,row.SpecialDrawString))
                                       .OrderBy(group => group.Key)
                                       .Select(group => group.OrderBy(g => g.OrderedGlass.Length)
                                                             .ThenBy(g => g.OrderedGlass.Height)
                                                             .ThenBy(g => g.OrderedGlass.StepLength)
                                                             .ThenBy(g => g.OrderedGlass.StepHeight)
                                                             .ToList());

        return groupedGlasses.ToList();
    }

    /// <summary>
    /// Weather this order can be Edited . Old Orders and Orders with Glasses already Retrieved cannot be Edited
    /// </summary>
    public bool CanBeEdited { get => !(OrderId.EndsWith("-Old") || Status.HasFlag(OrderStatus.Available)); }

    public static OrderStatus GetCombinedStatus(IEnumerable<OrderStatus> statuses)
    {
        OrderStatus status = OrderStatus.Undefined;
        statuses = statuses.Distinct();

        //Add all the appearing Statuses
        foreach (var appearingStatus in statuses)
        {
            status = status.CombineFlags(appearingStatus);
        }
        return status;
    }

    /// <summary>
    /// Adds new Rows to the Order
    /// </summary>
    /// <param name="synthesis"></param>
    public void AddNewRows(IEnumerable<GlassOrderRow> rows)
    {
        this.GlassRows.AddRange(rows);
    }

    /// <summary>
    /// Adds new Rows to the Order based on the Glass rows contained in the specified cabinRows
    /// </summary>
    /// <param name="rows"></param>
    public void AddNewRows(IEnumerable<CabinOrderRow> rows)
    {
        foreach (var row in rows)
        {
            AddNewRows(row.GlassesRows);
        }
    }

    /// <summary>
    /// Removes all the Cabin Rows with the specified cabinRowKey
    /// </summary>
    /// <param name="cabinRowKey"></param>
    public void RemoveCabinRow(Guid cabinRowKey)
    {
        var rowsToRemove = this.GlassRows.Where(r => r.CabinRowKey == cabinRowKey);
        if (!rowsToRemove.Any()) { throw new Exception("CabinRowKey Not Found"); }

        foreach (var row in rowsToRemove)
        {
            this.GlassRows.Remove(row);
        }
        
    }
    /// <summary>
    /// Removes the specified GlassRow
    /// </summary>
    /// <param name="row"></param>
    public bool RemoveGlassRow(GlassOrderRow row)
    {
        return this.GlassRows.Remove(row);
    }

}

/// <summary>
/// Represents the Status of an Order
/// </summary>
[Flags]
public enum OrderStatus
{
    Undefined = 0,
    /// <summary>
    /// The Order is available for all its quantity
    /// </summary>
    Available = 1,
    /// <summary>
    /// Everything is Pending
    /// </summary>
    Pending = 2,
    /// <summary>
    /// The Order is Cancelled
    /// </summary>
    Cancelled = 4,
    /// <summary>
    /// The Order is Partially Available with a part Pending
    /// </summary>
    PartiallyAvailable = Available | Pending,
    /// <summary>
    /// The Order is Partially Available with a part Pending and a Part Cancelled
    /// </summary>
    PartiallyAvailableWithCancelledPieces = Available | Pending | Cancelled,
    /// <summary>
    /// The order is Available with a part Cancelled
    /// </summary>
    AvailableWithCancelledPieces = Available | Cancelled,
    /// <summary>
    /// The Order is Pending with a part Cancelled
    /// </summary>
    PendingWithCancelledPieces = Pending | Cancelled,
}



