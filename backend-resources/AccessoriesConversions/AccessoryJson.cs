using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AccessoriesConversions
{
    /// <summary>
    /// A Class representing an Accessory that can be serilized to an XML or Json to be given for Site/E-Shop Creation
    /// All Properties Marked in the static Dictionary as non Serilizable are ignore both in XML and Json
    /// The Xml Ignore is Achieved through the ShouldSerilize Methods (they are built in like this in the XML Microsoft Library)
    /// The Json Ignore is Achieved through the Attribute Json Ignore with the When Writing Default Option
    /// All properties will return default values when they are marked as non Serilizable.
    /// All strings and reference type Properties should be nullable , so that to distinguish values from actuall empty props when json Serilization should not be ignored
    /// </summary>
    public class AccessoryJson
    {
        /// <summary>
        /// A Dictionary Determining weather each of the properties of this class Should be Serilized
        /// </summary>
        private static Dictionary<string, bool> shouldSerilizeProperty = typeof(AccessoryJson).GetProperties().ToDictionary(p => p.Name, p => true);
        /// <summary>
        /// Sets which properties should be serilized in XML
        /// </summary>
        /// <param name="propertyNames">The Names of the Properties</param>
        public static void SetSerilizableProperties(params string[] propertyNames )
        {
            foreach (var propName in shouldSerilizeProperty.Keys)
            {
                if (propertyNames.Contains(propName) is false)
                {
                    shouldSerilizeProperty[propName] = false;
                }
            }
        }

        private string? name = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Name 
        { 
            get => ShouldSerilizeName() 
                ? (string.IsNullOrEmpty(name) ? "Undefined" : name)
                : null; 
            set => name = value; 
        }
        public bool ShouldSerilizeName()
        {
            return shouldSerilizeProperty[nameof(Name)];
        }

        private string? description = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Description 
        {
            get => ShouldSerilizeDescription()
                ? (string.IsNullOrEmpty(description) ? "Undefined" : description)
                : null;
            set => description = value;
        }
        public bool ShouldSerilizeDescription()
        {
            return shouldSerilizeProperty[nameof(Description)];
        }

        private string? extendedDescription = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ExtendedDescription
        {
            get => ShouldSerilizeExtendedDescription()
                ? (string.IsNullOrEmpty(extendedDescription) ? "Undefined" : extendedDescription)
                : null;
            set => extendedDescription = value;
        }
        public bool ShouldSerilizeExtendedDescription()
        {
            return shouldSerilizeProperty[nameof(ExtendedDescription)];
        }

        private int sortNo = 99999;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int SortNo
        {
            get => ShouldSerilizeSortNo()
                ? (sortNo == 0 ? 99999 : sortNo)
                : 0;
            set => sortNo = value;
        }
        public bool ShouldSerilizeSortNo()
        {
            return shouldSerilizeProperty[nameof(SortNo)];
        }

        private string? code = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Code
        {
            get => ShouldSerilizeCode()
                ? (string.IsNullOrEmpty(code) ? "Undefined" : code)
                : null;
            set => code = value;
        }
        public bool ShouldSerilizeCode()
        {
            return shouldSerilizeProperty[nameof(Code)];
        }

        private string? mainPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? MainPhotoUrl
        {
            get => ShouldSerilizeMainPhotoUrl()
                ? (string.IsNullOrEmpty(mainPhotoUrl) ? "Undefined" : mainPhotoUrl)
                : null;
            set => mainPhotoUrl = value;
        }
        public bool ShouldSerilizeMainPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(MainPhotoUrl)];
        }

        private List<string>? extraPhotosUrl = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? ExtraPhotosUrl
        {
            get => ShouldSerilizeExtraPhotosUrl()
                ? (extraPhotosUrl is null ? new() : extraPhotosUrl)
                : null;
            set => extraPhotosUrl = value;
        }
        public bool ShouldSerilizeExtraPhotosUrl()
        {
            return shouldSerilizeProperty[nameof(ExtraPhotosUrl)];
        }

        private string? pdfUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PdfUrl
        {
            get => ShouldSerilizePdfUrl()
                ? (string.IsNullOrEmpty(pdfUrl) ? "Undefined" : pdfUrl)
                : null;
            set => pdfUrl = value;
        }
        public bool ShouldSerilizePdfUrl()
        {
            return shouldSerilizeProperty[nameof(PdfUrl)];
        }

        private List<string>? extraPdfsUrl = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? ExtraPdfsUrl
        {
            get => ShouldSerilizeExtraPdfsUrl()
                ? (extraPdfsUrl is null ? new() : extraPdfsUrl)
                : null;
            set => extraPdfsUrl = value;
        }
        public bool ShouldSerilizeExtraPdfsUrl()
        {
            return shouldSerilizeProperty[nameof(ExtraPdfsUrl)];
        }

        private string? defaultFinish = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? DefaultFinish
        {
            get => ShouldSerilizeDefaultFinish()
                ? (string.IsNullOrEmpty(defaultFinish) ? "Undefined" : defaultFinish)
                : null;
            set => defaultFinish = value;
        }
        public bool ShouldSerilizeDefaultFinish()
        {
            return shouldSerilizeProperty[nameof(DefaultFinish)];
        }

        private List<string>? availableFinishes = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? AvailableFinishes
        {
            get => ShouldSerilizeAvailableFinishes()
                ? (availableFinishes is null ? new() : availableFinishes)
                : null;
            set => availableFinishes = value;
        }
        public bool ShouldSerilizeAvailableFinishes()
        {
            return shouldSerilizeProperty[nameof(AvailableFinishes)];
        }

        private string? material = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Material
        {
            get => ShouldSerilizeMaterial()
                ? (string.IsNullOrEmpty(material) ? "Undefined" : material)
                : null;
            set => material = value;
        }
        public bool ShouldSerilizeMaterial()
        {
            return shouldSerilizeProperty[nameof(Material)];
        }

        private string? size = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Size
        {
            get => ShouldSerilizeSize()
                ? (string.IsNullOrEmpty(size) ? "Undefined" : size)
                : null;
            set => size = value;
        }
        public bool ShouldSerilizeSize()
        {
            return shouldSerilizeProperty[nameof(Size)];
        }

        private List<string>? sizeVariationsCodes = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? SizeVariationsCodes
        {
            get => ShouldSerilizeSizeVariationsCodes()
                ? (sizeVariationsCodes is null ? new() : sizeVariationsCodes)
                : null;
            set => sizeVariationsCodes = value;
        }
        public bool ShouldSerilizeSizeVariationsCodes()
        {
            return shouldSerilizeProperty[nameof(SizeVariationsCodes)];
        }

        private string? shape = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Shape
        {
            get => ShouldSerilizeShape()
                ? (string.IsNullOrEmpty(shape) ? "Undefined" : shape)
                : null;
            set => shape = value;
        }
        public bool ShouldSerilizeShape()
        {
            return shouldSerilizeProperty[nameof(Shape)];
        }

        private string? primaryType = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PrimaryType
        {
            get => ShouldSerilizePrimaryType()
                ? (string.IsNullOrEmpty(primaryType) ? "Undefined" : primaryType)
                : null;
            set => primaryType = value;
        }
        public bool ShouldSerilizePrimaryType()
        {
            return shouldSerilizeProperty[nameof(PrimaryType)];
        }

        private List<string>? otherPrimaryTypes = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? OtherPrimaryTypes
        {
            get => ShouldSerilizeOtherPrimaryTypes()
                ? (otherPrimaryTypes is null ? new() : otherPrimaryTypes)
                : null;
            set => otherPrimaryTypes = value;
        }
        public bool ShouldSerilizeOtherPrimaryTypes()
        {
            return shouldSerilizeProperty[nameof(OtherPrimaryTypes)];
        }

        private string? secondaryType = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? SecondaryType
        {
            get => ShouldSerilizeSecondaryType()
                ? (string.IsNullOrEmpty(secondaryType) ? "Undefined" : secondaryType)
                : null;
            set => secondaryType = value;
        }
        public bool ShouldSerilizeSecondaryType()
        {
            return shouldSerilizeProperty[nameof(SecondaryType)];
        }

        private List<string>? otherSecondaryTypes = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? OtherSecondaryTypes
        {
            get => ShouldSerilizeOtherSecondaryTypes()
                ? (otherSecondaryTypes is null ? new() : otherSecondaryTypes)
                : null;
            set => otherSecondaryTypes = value;
        }
        public bool ShouldSerilizeOtherSecondaryTypes()
        {
            return shouldSerilizeProperty[nameof(OtherSecondaryTypes)];
        }

        private List<string>? categories = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? Categories
        {
            get => ShouldSerilizeCategories()
                ? (categories is null ? new() : categories)
                : null;
            set => categories = value;
        }
        public bool ShouldSerilizeCategories()
        {
            return shouldSerilizeProperty[nameof(Categories)];
        }

        private string? series = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Series
        {
            get => ShouldSerilizeSeries()
                ? (string.IsNullOrEmpty(series) ? "Undefined" : series)
                : null;
            set => series = value;
        }
        public bool ShouldSerilizeSeries()
        {
            return shouldSerilizeProperty[nameof(Series)];
        }

        private List<string>? otherSeries = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? OtherSeries
        {
            get => ShouldSerilizeOtherSeries()
                ? (otherSeries is null ? new() : otherSeries)
                : null;
            set => otherSeries = value;
        }
        public bool ShouldSerilizeOtherSeries()
        {
            return shouldSerilizeProperty[nameof(OtherSeries)];
        }

        private List<string>? mountingTypes = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? MountingTypes
        {
            get => ShouldSerilizeMountingTypes()
                ? (mountingTypes is null ? new() : mountingTypes)
                : null;
            set => mountingTypes = value;
        }
        public bool ShouldSerilizeMountingTypes()
        {
            return shouldSerilizeProperty[nameof(MountingTypes)];
        }

        private List<string>? mountingVariationsCodes = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? MountingVariationsCodes
        {
            get => ShouldSerilizeMountingVariationsCodes()
                ? (mountingVariationsCodes is null ? new() : mountingVariationsCodes)
                : null;
            set => mountingVariationsCodes = value;
        }
        public bool ShouldSerilizeMountingVariationsCodes()
        {
            return shouldSerilizeProperty[nameof(MountingVariationsCodes)];
        }

        private List<AccessoryDimension>? dimensions = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<AccessoryDimension>? Dimensions
        {
            get => ShouldSerilizeDimensions()
                ? (dimensions is null ? new() : dimensions)
                : null;
            set => dimensions = value;
        }
        public bool ShouldSerilizeDimensions()
        {
            return shouldSerilizeProperty[nameof(Dimensions)];
        }

        private List<AccessoryPriceMin>? prices = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<AccessoryPriceMin>? Prices
        {
            get => ShouldSerilizePrices()
                ? (prices is null ? new() : prices)
                : null;
            set => prices = value;
        }
        public bool ShouldSerilizePrices()
        {
            return shouldSerilizeProperty[nameof(Prices)];
        }
    }

    public class AccessoryDimension
    {
        public string Dimension { get; set; } = string.Empty;
        public double DimensionValue { get; set; }
    }
    public class AccessoryPriceMin
    {
        public string Price { get; set; } = string.Empty;
        public string RefersTo { get; set; } = string.Empty;
        public decimal PriceValue { get; set;}
    }
}
