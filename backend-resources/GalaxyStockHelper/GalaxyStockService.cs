using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace GalaxyStockHelper
{
    public class GalaxyStockService
    {
        public GalaxyStockService(ILogger<GalaxyStockService> logger, string apiKey, string baseCall, string loginCall,HttpClient client)
        {
            _logger = logger;
            this.apiKey = apiKey;
            this.baseCall = baseCall;
            this.loginCall = loginCall;
            this.client = client;
        }
        private readonly ILogger<GalaxyStockService> _logger;
        private readonly string apiKey;
        private readonly string baseCall;
        private readonly string loginCall;
        private readonly HttpClient client;
        public const string SKIPFILTERWORDING = @"skip=";
        public const string TAKEFILTERWORDING = @"&take=";
        public const string FILTERSWORDING = @"&filters={";
        public const string SORTBYWORDING = @"&sortby=";
        public const string SORTASCENDINGWORDING = ".asc";

        /// <summary>
        /// The Matches of the Json Property with the GalaxyStock Info Object (Used to make the filters)
        /// </summary>
        public static Dictionary<string, string> JsonPropertiesMatching { get; set; } = new()
        {
            { $"{nameof(GalaxyItemStockInfo.ItemId)}", "ID" },
            { $"{nameof(GalaxyItemStockInfo.Code)}", "ITCPCODE" },
            { $"{nameof(GalaxyItemStockInfo.Description)}", "ITCPDESCRIPTION" },
            { $"{nameof(GalaxyItemStockInfo.SiteDescription)}", "SITEDESC" },
            { $"{nameof(GalaxyItemStockInfo.WharehouseType)}", "WAREHOUSE" },
            { $"{nameof(GalaxyItemStockInfo.ZoneDescription)}", "ZONEDESCRIPTION" },
            { $"{nameof(GalaxyItemStockInfo.Location)}", "LOCATIONBARCODE" },
            { $"{nameof(GalaxyItemStockInfo.AisleCode)}", "AISLECODE" },
            { $"{nameof(GalaxyItemStockInfo.Quantity)}", "LOCATIONQTY" },
            { $"{nameof(GalaxyItemStockInfo.Attribute1Description)}", "ATTRIBUTELOOKUPDESCRIPTION1" },
            { $"{nameof(GalaxyItemStockInfo.Attribute2Description)}", "ATTRIBUTELOOKUPDESCRIPTION2" },
            { $"{nameof(GalaxyItemStockInfo.Attribute3Description)}", "ATTRIBUTELOOKUPDESCRIPTION3" },
            { $"{nameof(GalaxyItemStockInfo.Attribute1Code)}", "ATTRIBUTELOOKUP1CODE" },
            { $"{nameof(GalaxyItemStockInfo.Attribute2Code)}", "ATTRIBUTELOOKUP2CODE" },
            { $"{nameof(GalaxyItemStockInfo.Attribute3Code)}", "ATTRIBUTELOOKUP3CODE" },
            { $"{nameof(GalaxyItemStockInfo.Attribute1Decimal)}", "ATTRIBUTEDECIMAL1" },
            { $"{nameof(GalaxyItemStockInfo.Attribute2Decimal)}", "ATTRIBUTEDECIMAL2" },
            { $"{nameof(GalaxyItemStockInfo.Attribute3Decimal)}", "ATTRIBUTEDECIMAL3" },
            { $"{nameof(GalaxyItemStockInfo.ItemCategory)}", "ΚΑΤΗΓΟΡΙΑ_ΤΙΜΟΚΑΤΑΛΟΓΟΥ" },
        };

        public GalaxyStockServiceFilters Filters { get; set; } = new();
        /// <summary>
        /// Weather it has already logged in
        /// </summary>
        public bool IsLoggedIn { get; set; }

        /// <summary>
        /// Logs in to the Galaxy API using the provided API key
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task LogInAsync()
        {
            _logger.LogInformation("Logging in to Galaxy");
            var response = await client.GetAsync($"{loginCall}{apiKey}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Login failed with status code:{statusCode} and Reason: {reason}", response.StatusCode, response.ReasonPhrase);
                string message = $"Login failed with status code: {response.StatusCode} and Reason: {response.ReasonPhrase}";
                throw new Exception(message);
            }
            else
            {
                _logger.LogInformation("Login successful");
                IsLoggedIn = true;
            }
        }

        /// <summary>
        /// Gets the URL for the API call to get the stock information
        /// </summary>
        /// <returns></returns>
        public string GetCallUrl()
        {
            //find the Comparison Filter Property Json Filled (match the one from the options) , fallback to Code if nothing is found 
            string searchFilterField = JsonPropertiesMatching.TryGetValue(Filters.SearchFilterField, out string? jsonPropertyName) ? jsonPropertyName : string.Empty;
            //Always sort by code if nothing or something wrong is provided
            string sortByFieldFilter = JsonPropertiesMatching.TryGetValue(Filters.SortByFilter, out string? jsonPropertyNameSort) ? jsonPropertyNameSort : "ITCPCODE";
            StringBuilder builder = new();
            builder.Append(baseCall)
                   .Append(SKIPFILTERWORDING)
                   .Append(Filters.SkipFilter)
                   .Append(TAKEFILTERWORDING)
                   .Append(Filters.TakeFilter);

            if (!string.IsNullOrEmpty(searchFilterField))
            {
                builder.Append(FILTERSWORDING)
                       .Append(searchFilterField)
                       .Append(":[") //opens the filter to enter the value
                       .Append(Filters.SearchFilterValue) //The Value to Search For
                       .Append(',') //seperate the value from the comparison operator
                       .Append(Filters.SearchFilterOperator.ToString()) //Set the operator (eqaul , notEqual e.t.c.)
                       .Append("]}"); //Close the filters;
            };

            builder.Append(SORTBYWORDING) //the sort by filter
                   .Append(sortByFieldFilter) //the field to sort by
                   .Append(SORTASCENDINGWORDING); //the sorting type (ascending)

            return builder.ToString();
        }


        /// <summary>
        /// Fetches stock information from the Galaxy API and stores it in a Dictionary
        /// </summary>
        /// <returns>Dictionary with ItemId as key and GalaxyStockInfo as value</returns>
        /// <exception cref="Exception">Thrown when API call fails or deserialization fails</exception>
        public async Task<Dictionary<string, WharehouseItem>> GetStockInfoDictionaryAsync()
        {
            if (!IsLoggedIn)
            {
                _logger.LogInformation("Not logged in, logging in now");
                await LogInAsync();
            }

            Dictionary<string, WharehouseItem> stockInfoDictionary = [];
            string callUrl = GetCallUrl();

            _logger.LogInformation("Fetching stock information with URL: {url}", callUrl);

            try
            {
                HttpResponseMessage response = await client.GetAsync(callUrl);

                // Galaxy Closes the Connection after a while so check if unauthorized login again
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    IsLoggedIn = false;
                    _logger.LogWarning("Call Was unauthorized ... Getting Authorization Key again...and Calling API Again");
                    await LogInAsync();
                    response = await client.GetAsync(callUrl);
                }

                // If Still Failed Throw
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("API call failed with status code: {statusCode} and reason: {reason}",response.StatusCode, response.ReasonPhrase);
                    throw new Exception($"API call failed with status code: {response.StatusCode} and reason: {response.ReasonPhrase}");
                }

                // Deserialize the response directly to our expected structure
                var galaxyResponse = await response.Content.ReadFromJsonAsync<GalaxyStockResponse>();

                if (galaxyResponse == null || galaxyResponse.Items == null)
                {
                    _logger.LogWarning("Failed to deserialize response or Items array is null");
                    return stockInfoDictionary;
                }

                _logger.LogInformation("Received {count} items from API", galaxyResponse.Items.Count);

                int itemCount = 0;

                // Process each item
                foreach (var stockInfoJson in galaxyResponse.Items)
                {
                    try
                    {
                        // Add to dictionary using ItemId as key
                        if (string.IsNullOrEmpty(stockInfoJson.Code))
                        {
                            _logger.LogWarning("An Item with null or empty Code was skipped");
                            itemCount++;
                            continue;
                        }
                        else
                        {
                            //GetThe Full Code of the Item
                            var fullCode = GalaxyItemStockInfo.GetItemFullCode(stockInfoJson);

                            //Add the New Item to the Dictionary if not already present
                            if (!stockInfoDictionary.TryGetValue(fullCode, out WharehouseItem? wharehouseItem))
                            {
                                wharehouseItem = stockInfoJson.GetWharehouseItemWithoutStockInfo();
                                stockInfoDictionary.Add(fullCode, wharehouseItem);
                            }

                            //Add the Stock Information to the Wharehouse Item
                            var stockInfo = stockInfoJson.GetPositionStockInfo();
                            wharehouseItem.StockInfo.Add(stockInfo);
                            itemCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing item: {message}", ex.Message);
                    }
                }

                _logger.LogInformation("Successfully processed {count} items from the API response", itemCount);

                return stockInfoDictionary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching or processing stock information: {message}", ex.Message);
                throw;
            }
        }
    }

}
