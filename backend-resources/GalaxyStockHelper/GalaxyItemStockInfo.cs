using System.Text.Json.Serialization;

namespace GalaxyStockHelper
{
    public class GalaxyItemStockInfo
    {
        /// <summary>
        /// The Id of the Information for this specific item in the Galaxy Database
        /// </summary>
        [JsonPropertyName("ID")]
        public string ItemId { get; set; } = string.Empty;

        /// <summary>
        /// The Code of the Item 
        /// </summary>
        [JsonPropertyName("ITCPCODE")]
        public string Code { get; set; } = string.Empty;

        public string FullCode { get; set; } = string.Empty;

        /// <summary>
        /// The Description of the Item
        /// </summary>
        [JsonPropertyName("ITCPDESCRIPTION")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The Site Description (for example Magoula Factory)
        /// </summary>
        [JsonPropertyName("SITEDESC")]
        public string SiteDescription { get; set; } = string.Empty;

        /// <summary>
        /// The Type of the Warehouse (ex. RF , Apothki3 e.t.c.)
        /// </summary>
        [JsonPropertyName("WAREHOUSE")]
        public string WharehouseType { get; set; } = string.Empty;

        /// <summary>
        /// The Description of the Zone of the Item's Location (ex. Pano Ktirio , Ergastirio1 e.t.c.)
        /// </summary>
        [JsonPropertyName("ZONEDESCRIPTION")]
        public string ZoneDescription { get; set; } = string.Empty;

        /// <summary>
        /// The Barcode Number of the Position Of the Item (its Location)
        /// </summary>
        [JsonPropertyName("LOCATIONBARCODE")]
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// The Code of the Aisle(Corridor) of the Item's Location
        /// </summary>
        [JsonPropertyName("AISLECODE")]
        public string AisleCode { get; set; } = string.Empty;

        /// <summary>
        /// The Quantity of the items in the specific location/Position
        /// </summary>
        [JsonPropertyName("LOCATIONQTY")]
        public decimal? Quantity { get; set; }

        /// <summary>
        /// The Description of the First Attribute of the Item
        /// </summary>
        [JsonPropertyName("ATTRIBUTELOOKUPDESCRIPTION1")]
        public string Attribute1Description { get; set; } = string.Empty;

        /// <summary>
        /// The Description of the Second Attribute of the Item
        /// </summary>
        [JsonPropertyName("ATTRIBUTELOOKUPDESCRIPTION2")]
        public string Attribute2Description { get; set; } = string.Empty;

        /// <summary>
        /// The Description of the Third Attribute of the Item
        /// </summary>
        [JsonPropertyName("ATTRIBUTELOOKUPDESCRIPTION3")]
        public string Attribute3Description { get; set; } = string.Empty;

        /// <summary>
        /// The Code of the First Attribute of the Item (Used for Standard Mirrors Charachteristic and Accessories Color)
        /// </summary>
        [JsonPropertyName("ATTRIBUTELOOKUP1CODE")]
        public string Attribute1Code { get; set; } = string.Empty;

        /// <summary>
        /// The Code of the Second Attribute of the Item (Used for Cabins Color Code)
        /// </summary>
        [JsonPropertyName("ATTRIBUTELOOKUP2CODE")]
        public string Attribute2Code { get; set; } = string.Empty;

        /// <summary>
        /// The Code of the Third Attribute of the Item
        /// </summary>
        [JsonPropertyName("ATTRIBUTELOOKUP3CODE")]
        public string Attribute3Code { get; set; } = string.Empty;

        /// <summary>
        /// The Value of the First Attribute of the Item (Used for Cabins Length and Mirrors Length)
        /// </summary>
        [JsonPropertyName("ATTRIBUTEDECIMAL1")]
        public decimal? Attribute1Decimal { get; set; }

        /// <summary>
        /// The Value of the Second Attribute of the Item (Used for Mirrors Height)
        /// </summary>
        [JsonPropertyName("ATTRIBUTEDECIMAL2")]
        public decimal? Attribute2Decimal { get; set; }

        /// <summary>
        /// The Value of the Third Attribute of the Item (Used for Cabins Height)
        /// </summary>
        [JsonPropertyName("ATTRIBUTEDECIMAL3")]
        public decimal? Attribute3Decimal { get; set; }
        /// <summary>
        /// The Category of the Item
        /// </summary>
        [JsonPropertyName("ΚΑΤΗΓΟΡΙΑ_ΤΙΜΟΚΑΤΑΛΟΓΟΥ")]
        public string ItemCategory { get; set; } = string.Empty;

        /// <summary>
        /// Converts the current item to a WharehouseItem object without stock information
        /// </summary>
        /// <returns></returns>
        public WharehouseItem GetWharehouseItemWithoutStockInfo()
        {
            return new WharehouseItem()
            {
                Code = Code ?? string.Empty,
                FullCode = GetItemFullCode(this),
                Description = Description ?? string.Empty,
                ItemCategory = ItemCategory ?? string.Empty,
                Attribute1Description = Attribute1Description ?? string.Empty,
                Attribute2Description = Attribute2Description ?? string.Empty,
                Attribute3Description = Attribute3Description ?? string.Empty,
                Attribute1Code = Attribute1Code ?? string.Empty,
                Attribute2Code = Attribute2Code ?? string.Empty,
                Attribute3Code = Attribute3Code ?? string.Empty,
                Attribute1Decimal = Attribute1Decimal ?? 0,
                Attribute2Decimal = Attribute2Decimal ?? 0,
                Attribute3Decimal = Attribute3Decimal ?? 0,
            };
        }
        /// <summary>
        /// Gets the stock information of the item as a PositionStockInfo object
        /// </summary>
        /// <returns></returns>
        public PositionStockInfo GetPositionStockInfo()
        {
            return new PositionStockInfo()
            {
                WharehouseType = WharehouseType ?? string.Empty,
                Site = SiteDescription ?? string.Empty,
                Zone = ZoneDescription ?? string.Empty,
                Quantity = Quantity ?? 0,
                Aisle = AisleCode ?? string.Empty,
                Position = Location ?? string.Empty
            };
        }

        /// <summary>
        /// Returns the Full Code of an item , Items with charachteristics have a generated Code
        /// </summary>
        /// <param name="stockItem"></param>
        public static string GetItemFullCode(GalaxyItemStockInfo stockItem)
        {
            //If the item has only the 1st Charachteristic Code without Deciaml Values . Its Either a Standard Mirror or an Accessory
            //The Code is Added to the End of the Main Code
            //Only Mirrors and Accessories use the Code Charachteristic No1
            if (!string.IsNullOrEmpty(stockItem.Attribute1Code))
            {
                return $"{stockItem.Code.ToUpper()}{stockItem.Attribute1Code}";
            }
            //If the Item has The 2nd Code Charachtersitic and the 1st and 3rd Decimal , Then its a Custom Cabin Item
            else if (!string.IsNullOrEmpty(stockItem.Attribute2Code) && stockItem.Attribute1Decimal is not null and not 0 && stockItem.Attribute3Decimal is not null and not 0)
            {
                //Trim any dashes '-' from the code and convert to uppercase , then put color code as is then height
                return $"{stockItem.Code.Trim('-').ToUpper()}{stockItem.Attribute1Decimal:0}-{stockItem.Attribute2Code}-{stockItem.Attribute3Decimal:0}";
            }
            //If the item has AttributeDecimal 1 and 2 not empty then its a Custom Mirror (Though we do not know what light or extras it has ...)
            else if (stockItem.Attribute1Decimal is not null and not 0 && stockItem.Attribute2Decimal is not null and not 0)
            {
                //Do not trim the dash here , add the length and height afterwards
                return $"{stockItem.Code.ToUpper()}{stockItem.Attribute1Decimal:0}-{stockItem.Attribute2Decimal:0}";
            }
            //If there are no Attributes like above just return the Code
            else return stockItem.Code;
        }
    }

}
