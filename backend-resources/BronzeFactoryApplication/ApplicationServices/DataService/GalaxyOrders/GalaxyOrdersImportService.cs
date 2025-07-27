using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BronzeFactoryApplication.ApplicationServices.DataService.GalaxyOrders;

public class GalaxyOrdersImportService
{
    public const string configurationSection = "ThirdPartyCalls";
    public const string configurationValueGalaxyLogInCall = "GalaxyLogInCall";
    public const string configurationValueGalaxyCall = "GalaxyCall";
    public const string configurationValueGalaxyKey = "GalaxyKey";

    public const string SKIPFILTER = @"skip=";
    public const string TAKEFILTER = @"&take=";
    public const string DATEFROMFILTER = @"&filters={DATEFROM:[";
    public const string GREATERTHANOREQUAL = @",GreaterOrEqual],DATETO:[";
    public const string DATELESSTHANOREQUAL = @",LessOrEqual]}";

    private const int defaultSkipFilter = 0;
    private const int defaultTakeFilter = 10000;
    private readonly string defaultLogInCall;
    private readonly string defaultCallBase;

    private readonly ILogger<GalaxyOrdersImportService> logger;
    private readonly HttpClient client = new();
    
    

    /// <summary>
    /// The Base Call Url for retrieving Orders
    /// </summary>
    public string CallBase { get; set; }
    /// <summary>
    /// Skip this Number of Results and start Counting the Take filter After this Number
    /// </summary>
    public int SkipFilterValue { get; set; } = defaultSkipFilter;
    /// <summary>
    /// Take from this Number from the Results instead of All
    /// </summary>
    public int TakeFilterValue { get; set; } = defaultTakeFilter;
    /// <summary>
    /// Retrieve Orders by Date Greater or Equal to this Value
    /// </summary>
    public DateTime DateGreaterOrEqualFilterValue { get; set; } = DateTime.Now.AddDays(-15);
    /// <summary>
    /// Retrieve Orders by Date Less or Equal to this Value
    /// </summary>
    public DateTime DateLessOrEqualFilterValue { get; set; } = DateTime.Now;
    /// <summary>
    /// The Call URL to perform a LogIn
    /// </summary>
    public string LogInCall { get; set; }
    /// <summary>
    /// The api Key to log In (password)
    /// </summary>
    private string galaxyKey;

    /// <summary>
    /// Weather it has already logged in
    /// </summary>
    public bool IsLoggedIn { get; set; }
    public bool IsAnyDataCached { get => cache.Any(); }

    private IEnumerable<BronzeDocument> cache = Enumerable.Empty<BronzeDocument>();
    /// <summary>
    /// The cacheMaxDate between All Calls Made (Cannot read from cacheResults becase an Date might have no Results but still be Queried)
    /// </summary>
    private DateTime maxCacheDate = DateTime.MinValue;
    /// <summary>
    /// The cacheMinDate between All Calls Made (Cannot read from cacheResults becase an Date might have no Results but still be Queried)
    /// </summary>
    private DateTime minCacheDate = DateTime.MaxValue;

    public GalaxyOrdersImportService(IConfiguration configuration , ILogger<GalaxyOrdersImportService> logger)
    {
        this.logger = logger;

        var section = configuration.GetRequiredSection(configurationSection);
        if (section is null) throw new InvalidOperationException($"Configuration Error While retrieving Configuration -Section with Section Name : {configurationSection}");

        defaultLogInCall = section.GetValue<string>(configurationValueGalaxyLogInCall)
            ?? throw new InvalidOperationException($"Configuration Error While Retrieving Value with key :{configurationValueGalaxyLogInCall} from section {section}");
        LogInCall = defaultLogInCall;

        defaultCallBase = section.GetValue<string>(configurationValueGalaxyCall)
            ?? throw new InvalidOperationException($"Configuration Error While Retrieving Value with key :{configurationValueGalaxyCall} from section {section}");
        CallBase = defaultCallBase;
        
        galaxyKey = section.GetValue<string>(configurationValueGalaxyKey)
            ?? throw new InvalidOperationException($"Configuration Error While Retrieving Value with key :{configurationValueGalaxyKey} from section {section}");
    }

    /// <summary>
    /// Returns the Call Url according to the provided filters
    /// </summary>
    /// <param name="minDate">the Greater than DateFilter</param>
    /// <param name="maxDate">the Less than DateFilter</param>
    /// <returns></returns>
    public string GetCallUrl(DateTime minDate , DateTime maxDate)
    {
        var dateGreaterFilter = minDate.ToString("yyyy-MM-dd");
        var dateLessFilter = maxDate.ToString("yyyy-MM-dd");

        StringBuilder builder = new();
        builder.Append(CallBase)
               .Append(SKIPFILTER)
               .Append(SkipFilterValue)
               .Append(TAKEFILTER)
               .Append(TakeFilterValue)
               .Append(DATEFROMFILTER)
               .Append(dateGreaterFilter)
               .Append(GREATERTHANOREQUAL)
               .Append(dateLessFilter)
               .Append(DATELESSTHANOREQUAL);
        return builder.ToString();
    }

    /// <summary>
    /// LogIn to Galaxy Api
    /// </summary>
    /// <returns></returns>
    public async Task LogInAsync()
    {
        StringBuilder builder = new();
        builder.Append(LogInCall)
            .Append(galaxyKey);

        //PERFORM LOG IN
        var response = await client.GetAsync(builder.ToString());
        if (!response.IsSuccessStatusCode)
        {
            var ex = new Exception($"Log in Failed {Environment.NewLine}Status Code :{response.StatusCode}{Environment.NewLine}Response :{response.ReasonPhrase}");
            logger.LogWarning("{message}", ex.Message);
            throw ex;
        }
        else
        {
            IsLoggedIn = true;
            logger.LogInformation("Galaxy Log in Call Successfull");
        }
    }

    /// <summary>
    /// Returns the Bronze Documents Received from Galaxy
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<BronzeDocument>> GetOrdersAsync()
    {
        var cacheAdequacy = GetCacheAdequacy();

        if (cacheAdequacy.ShouldMakeAtLeastOneCall)
        {
            var retrieved = await CallGalaxyApiAsync((DateTime)cacheAdequacy.MinDate1!, (DateTime)cacheAdequacy.MaxDate1!);

            if (cacheAdequacy.ShouldMakeDualCall)
            {
                retrieved = retrieved.Concat(await CallGalaxyApiAsync((DateTime)cacheAdequacy.MinDate2!, (DateTime)cacheAdequacy.MaxDate2!));
            }

            //If the Cache was completely out of Dates it means it will be renewed with the new dates
            if (cacheAdequacy.DatesState == CacheDatesState.CacheDatesNotEnough)
            {
                minCacheDate = DateGreaterOrEqualFilterValue;
                maxCacheDate = DateLessOrEqualFilterValue;
                cache = retrieved.OrderByDescending(r => r.Date);
            }
            else //keep any dates that do
            {
                minCacheDate = minCacheDate <= DateGreaterOrEqualFilterValue ? minCacheDate : DateGreaterOrEqualFilterValue;
                maxCacheDate = maxCacheDate >= DateLessOrEqualFilterValue ? maxCacheDate : DateLessOrEqualFilterValue;
                cache = cache.Concat(retrieved).OrderByDescending(r=>r.Date);
            }
            logger.LogInformation("Galaxy Orders retrieved...");
        }
        else
        {
            logger.LogInformation("Retrieved Only Cached Galaxy Orders ...");
        }
        
        
        return cache;
    }

    /// <summary>
    /// Sets Defaults Values to all the Filters
    /// </summary>
    public void SetToDefaultConfigurationValues()
    {
        LogInCall = defaultLogInCall;
        CallBase = defaultCallBase;
        SkipFilterValue = defaultSkipFilter;
        TakeFilterValue = defaultTakeFilter;
        DateGreaterOrEqualFilterValue = DateTime.Now;
        DateLessOrEqualFilterValue = DateTime.Now;
    }

    /// <summary>
    /// Clears the Cache
    /// </summary>
    public void ClearCache()
    {
        cache = Enumerable.Empty<BronzeDocument>();
    }

    /// <summary>
    /// Calls the Galaxy Api with the Passed DateFilters and Returns all the Bronze Documents Retrieved
    /// </summary>
    /// <param name="minDate">The Min Date</param>
    /// <param name="maxDate">The Max Date</param>
    /// <returns></returns>
    private async Task<IEnumerable<BronzeDocument>> CallGalaxyApiAsync(DateTime minDate , DateTime maxDate)
    {
        // Log in if not already
        if (!IsLoggedIn)
        {
            await LogInAsync();
        }

        logger.LogInformation("Making a Call for Galaxy Orders...From :{minDate} to {maxDate}", minDate.ToString("dd-MM-yy"), maxDate.ToString("dd-MM-yy"));
        // Make the Call
        var response = await client.GetAsync(GetCallUrl(minDate, maxDate));

        // Galaxy Closes the Connection after a while so check if unauthorized login again
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            IsLoggedIn = false;
            logger.LogInformation("Call Was unauthorized ... Getting Authorization Key again...and Calling API Again");
            await LogInAsync();
            response = await client.GetAsync(GetCallUrl(minDate, maxDate));
        }

        // If Still Failed Throw
        if (!response.IsSuccessStatusCode)
        {
            var ex = new Exception($"Retrieval Of Galaxy Orders Failed , StatusCode:{response.StatusCode}--Reason Phrase {response.ReasonPhrase}");
            logger.LogWarning("{message}", ex.Message);
            throw ex;
        }

        // Read the Root Object
        Root ? rootObjectReceived = await response.Content.ReadFromJsonAsync<Root>();

        // Throw if Null
        if (rootObjectReceived is null)
        {
            var ex = new Exception("Failed to Read/Deserilize Root Object from Response , Check If Json Response Has Changed Schema...");
            logger.LogError(ex, "{message}", ex.Message);
            throw ex;
        }

        // Create Orders and Return them
        var retrieved = CreateOrdersFromRoot(rootObjectReceived);
        return retrieved;
    }

    /// <summary>
    /// Generates a List of Bronze Order objects from the Received Root Object
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    private static List<BronzeDocument> CreateOrdersFromRoot(Root root)
    {
        //Create Groups according to Document 
        return root.Items.GroupBy(r => r.Document)
            .Where(group => group.Any())
            .Select(group =>
            {
                var groupItems = group.ToList();
                var date = groupItems.First().Date;
                var documentSeries = groupItems.First().AppearingSeriesCode;
                var documentNumber = groupItems.First().Number;
                var stateOfTransformation = GetTransformationState(groupItems.First().TransformationState);
                var client = groupItems.First().ClientName;
                var clientAddress = groupItems.First().Address;
                var items = groupItems.Select(i => 
                new BronzeProductRow(
                    i.ProductCode, 
                    i.ProductDescription, 
                    Convert.ToDecimal(i.Price), 
                    Convert.ToDecimal(i.DiscountValue), i.FirstQuantity,
                    i.Charachteristic1 ?? "",
                    i.Charachteristic2 ?? "",
                    i.Charachteristic3 ?? "")
                );

                return new BronzeDocument(date, documentSeries, documentNumber, stateOfTransformation, client, clientAddress, items);
            }).ToList();
    }

    /// <summary>
    /// Returns the Transformation State of a Document from a String
    /// </summary>
    /// <param name="state">The string representation of the Transformation state</param>
    /// <returns></returns>
    private static OrderTransformationState GetTransformationState(string state)
    {
        switch (state)
        {
            case "Μερικώς":
                return OrderTransformationState.Partially;
            case "Καμία":
                return OrderTransformationState.None;
            default:
                return OrderTransformationState.Fully;
        }
    }

    /// <summary>
    /// Returns a Result Declaring wheather or How Adequate is the Cache
    /// </summary>
    /// <returns></returns>
    private CacheAdequacyResult GetCacheAdequacy()
    {
        //If the Cached Orders have a date from the Filters then search only at the dates that do not exist . 
        //If all exist , return only the cache
        if (cache.Any())
        {
            bool isCacheMinEnough = minCacheDate.Date <= DateGreaterOrEqualFilterValue.Date;
            bool isCacheMaxEnough = maxCacheDate.Date >= DateLessOrEqualFilterValue.Date;

            //Cache Not Adequate Completely Out Of Range
            if(maxCacheDate < DateGreaterOrEqualFilterValue || minCacheDate > DateLessOrEqualFilterValue)
            {
                return CacheAdequacyResult.CacheInadequate(DateGreaterOrEqualFilterValue, DateLessOrEqualFilterValue);
            }
            //Cache is Adequate
            else if (isCacheMinEnough && isCacheMaxEnough)
            {
                return CacheAdequacyResult.CacheAdequate();
            }
            //Only Min
            else if (isCacheMinEnough && !isCacheMaxEnough)
            {
                return CacheAdequacyResult.CacheMinDateAdequate(maxCacheDate, DateLessOrEqualFilterValue);
            }
            //Only Max
            else if (!isCacheMinEnough && isCacheMaxEnough)
            {
                return CacheAdequacyResult.CacheMaxDateAdequate(minCacheDate, DateGreaterOrEqualFilterValue);
            }
            //When both Cache Max min are not Enough -
            // 1) We have to make two calls for the DateRanges before the Cache and After the Cache
            // or
            // 2) The Requested Range is Completely out of the Cache Range
            else
            {
                return CacheAdequacyResult.CacheInBetween(maxCacheDate, minCacheDate, DateGreaterOrEqualFilterValue, DateLessOrEqualFilterValue);
            }
        }
        else
        {
            return CacheAdequacyResult.CacheInadequate(DateGreaterOrEqualFilterValue,DateLessOrEqualFilterValue);
        }
    }

}

/// <summary>
/// A result Showing wheather the Cache is Adequate 
/// against a requested Call or how the Date Filter needs to be Modified
/// </summary>
internal class CacheAdequacyResult
{
    public CacheDatesState DatesState { get; set; }
    /// <summary>
    /// The Max Date of the First Call or null if there should not be a call
    /// </summary>
    public DateTime? MaxDate1 { get; set; }
    /// <summary>
    /// The Min Date of the First Call or null if there should not be a call
    /// </summary>
    public DateTime? MinDate1 { get; set; }
    /// <summary>
    /// The Max Date of the Second Call or null if there should not be a Second call
    /// </summary>
    public DateTime? MaxDate2 { get; set; }
    /// <summary>
    /// The Min Date of the Second Call or null if there should not be a Second call
    /// </summary>
    public DateTime? MinDate2 { get; set; }

    public bool ShouldMakeDualCall { get => MaxDate2 != null && MinDate2 != null; }
    public bool ShouldMakeAtLeastOneCall { get => MaxDate1 != null && MinDate1 != null; }

    private CacheAdequacyResult()
    {

    }

    public static CacheAdequacyResult CacheAdequate()
    {
        return new CacheAdequacyResult()
        {
            DatesState = CacheDatesState.CacheIsEnough
        };
    }
    public static CacheAdequacyResult CacheInadequate(DateTime greaterFilter,DateTime lessFilter)
    {
        return new CacheAdequacyResult()
        {
            DatesState = CacheDatesState.CacheDatesNotEnough,
            MinDate1 = greaterFilter,
            MaxDate1 = lessFilter,
        };
    }
    public static CacheAdequacyResult CacheMinDateAdequate(DateTime maxCacheDate , DateTime lessFilter)
    {
        return new CacheAdequacyResult()
        {
            DatesState = CacheDatesState.CacheMinDateIsEnough,
            MinDate1 = maxCacheDate.AddDays(1),
            MaxDate1 = lessFilter
        };
    }
    public static CacheAdequacyResult CacheMaxDateAdequate(DateTime minCacheDate, DateTime greaterFilter)
    {
        return new CacheAdequacyResult()
        {
            DatesState = CacheDatesState.CacheMinDateIsEnough,
            MinDate1 = greaterFilter,
            MaxDate1 = minCacheDate.AddDays(-1),
        };
    }
    public static CacheAdequacyResult CacheInBetween(DateTime maxCacheDate, DateTime minCacheDate, DateTime greaterFilter, DateTime lessFilter) 
    {
        return new CacheAdequacyResult()
        {
            DatesState = CacheDatesState.CacheDatesInBetween,
            MinDate1 = greaterFilter,
            MaxDate1 = minCacheDate.AddDays(-1),
            MinDate2 = maxCacheDate.AddDays(1),
            MaxDate2 = lessFilter,
        };
    }

}

/// <summary>
/// An Enum Stating the State of the Cache Dates Against a set of Filter Dates (min - max)
/// </summary>
public enum CacheDatesState
{
    /// <summary>
    /// When the Dates of Cache are Enough Compared with the Filter Dates
    /// </summary>
    CacheIsEnough,
    /// <summary>
    /// When Only the MinDate of the Cache is Enough
    /// </summary>
    CacheMinDateIsEnough,
    /// <summary>
    /// When Only the MaxDate of the Cache is Enough
    /// </summary>
    CacheMaxDateIsEnough,
    /// <summary>
    /// The Cache Dates are in between the Filter Dates
    /// </summary>
    CacheDatesInBetween,
    /// <summary>
    /// When the Dates of Cache are outside of any of the Filter Dates 
    /// </summary>
    CacheDatesNotEnough,
}

/// <summary>
/// The Root Object Received from the Json Parsing of a Galaxy Orders Call
/// </summary>
public class Root
{
    public List<Item> Items { get; set; } = new();
    public List<Metadata> Metadata { get; set; } = new();
}

/// <summary>
/// The MetaData Class (If we Take results and there are more it will be True)
/// </summary>
public class Metadata
{
    public bool HasNext { get; set; }
}

/// <summary>
/// The Item Class of the Galaxy Call
/// </summary>
public class Item
{
    public string ID { get; set; } = string.Empty;
    public DateTime DATEFROM { get; set; }
    public DateTime DATETO { get; set; }
    
    [JsonPropertyName("Ημερομηνία")]
    public DateTime Date { get; set; }

    [JsonPropertyName("Ημερομηνία ενημέρωσης")]
    public DateTime DateOfLatestInformation { get; set; }

    [JsonPropertyName("Εμφανιζόμενος κωδικός σειράς")]
    public string AppearingSeriesCode { get; set; } = string.Empty;
    [JsonPropertyName("Αριθμός")]
    public string Number { get; set; } = string.Empty;
    [JsonPropertyName("Παραστατικό")]
    public string Document { get; set; } = string.Empty;

    [JsonPropertyName("Κωδικός πελάτη")]
    public string ClientCode { get; set; } = string.Empty;

    [JsonPropertyName("Όνομα πελάτη")]
    public string ClientName { get; set; } = string.Empty;
    [JsonPropertyName("Οδός")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("Κατάσταση μετασχηματισμού")]
    public string TransformationState { get; set; } = string.Empty;

    [JsonPropertyName("Εκτυπομένη εγγραφη")]
    public int PrintedDocument { get; set; }

    [JsonPropertyName("Ενημερωμένη εγγραφή")]
    public int UpdatedDocument { get; set; }

    [JsonPropertyName("Συνολική αξία")]
    public double TotalValue { get; set; }

    [JsonPropertyName("Κωδικός είδους")]
    public string ProductCode { get; set; } = string.Empty;

    [JsonPropertyName("Περιγραφή είδους")]
    public string ProductDescription { get; set; } = string.Empty;

    [JsonPropertyName("1η ποσότητα")]
    public double FirstQuantity { get; set; }

    [JsonPropertyName("2η ποσότητα")]
    public double SecondQuantity { get; set; }

    [JsonPropertyName("3η ποσότητα")]
    public double ThirdQuantity { get; set; }

    [JsonPropertyName("1ο χαρακτηριστικό")] //Μήκος στις Καμπίνες
    public string Charachteristic1 { get; set; } = string.Empty;

    [JsonPropertyName("2ο χαρακτηριστικό")] //Χρώμα και Κρύσταλλο στις Καμπίνες
    public string Charachteristic2 { get; set; } = string.Empty;

    [JsonPropertyName("3ο χαρακτηριστικό")] //Ύψος Στις Καμπίνες
    public string Charachteristic3 { get; set; } = string.Empty;

    [JsonPropertyName("Τιμή")]
    public double Price { get; set; }

    [JsonPropertyName("Αξία προ έκπτωσης")]
    public double ValueBeforeDiscount { get; set; }

    [JsonPropertyName("Αξία έκπτωσης")]
    public double DiscountValue { get; set; }
}

public enum OrderTransformationState
{
    //Καμία
    None,
    //Μερικώς
    Partially,
    //Οταν ειναι κενό είναι πλήρως
    Fully
}
