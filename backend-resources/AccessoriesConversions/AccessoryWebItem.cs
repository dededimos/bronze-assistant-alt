using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AccessoriesConversions
{
    /// <summary>
    /// An Accessory with Certain Properties to Make an XML or JSON file
    /// </summary>
    public class AccessoryWebItem
    {
        /// <summary>
        /// A Dictionary Determining weather each of the properties of this class Should be Serilized
        /// </summary>
        private static Dictionary<string, bool> shouldSerilizeProperty = typeof(AccessoryWebItem).GetProperties().ToDictionary(p => p.Name, p => true);
        /// <summary>
        /// Sets which properties should be serilized in XML
        /// </summary>
        /// <param name="propertyNames">The Names of the Properties</param>
        public static void SetSerilizableProperties(params string[] propertyNames)
        {
            foreach (var propName in shouldSerilizeProperty.Keys)
            {
                if (propertyNames.Contains(propName))
                {
                    shouldSerilizeProperty[propName] = true;
                }
                else
                {
                    shouldSerilizeProperty[propName] = false;
                }
            }
        }
        public static Dictionary<string,bool> GetSerilizableProperties() => shouldSerilizeProperty.ToDictionary(kvp=> kvp.Key,kvp=> kvp.Value);

        /// <summary>
        /// The String of items where they value is empty
        /// </summary>
        private static string emptyValueString = "-";
        /// <summary>
        /// Sets the EmptyValue of items that are serilized but have an Empty string as Value
        /// Otherwise properties with an empty string are not serilized at all.
        /// </summary>
        /// <param name="emptyValue">The Empty value to set</param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void SetEmptyValue(string emptyValue)
        {
            if (!string.IsNullOrEmpty(emptyValue))
            {
                throw new InvalidOperationException($"{nameof(emptyValue)} cannot be an Empty String , Empty string is reserved for non serilizable Properties");
            }
            emptyValueString = emptyValue;
        }

        private string? code = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Code
        {
            get => ShouldSerilizeCode()
                ? (string.IsNullOrEmpty(code) ? emptyValueString : code)
                : null;
            set => code = value;
        }
        public bool ShouldSerilizeCode()
        {
            return shouldSerilizeProperty[nameof(Code)];
        }

        private string? name = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Name
        {
            get => ShouldSerilizeName()
                ? (string.IsNullOrEmpty(name) ? emptyValueString : name)
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
                ? (string.IsNullOrEmpty(description) ? emptyValueString : description)
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
                ? (string.IsNullOrEmpty(extendedDescription) ? emptyValueString : extendedDescription)
                : null;
            set => extendedDescription = value;
        }
        public bool ShouldSerilizeExtendedDescription()
        {
            return shouldSerilizeProperty[nameof(ExtendedDescription)];
        }

        private string? series = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Series
        {
            get => ShouldSerilizeSeries()
                ? (string.IsNullOrEmpty(series) ? emptyValueString : series)
                : null;
            set => series = value;
        }
        public bool ShouldSerilizeSeries()
        {
            return shouldSerilizeProperty[nameof(Series)];
        }

        private string? primaryType = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? PrimaryType
        {
            get => ShouldSerilizePrimaryType()
                ? (string.IsNullOrEmpty(primaryType) ? emptyValueString : primaryType)
                : null;
            set => primaryType = value;
        }
        public bool ShouldSerilizePrimaryType()
        {
            return shouldSerilizeProperty[nameof(PrimaryType)];
        }

        private string? secondaryType = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? SecondaryType
        {
            get => ShouldSerilizeSecondaryType()
                ? (string.IsNullOrEmpty(secondaryType) ? emptyValueString : secondaryType)
                : null;
            set => secondaryType = value;
        }
        public bool ShouldSerilizeSecondaryType()
        {
            return shouldSerilizeProperty[nameof(SecondaryType)];
        }

        private string? finish = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Finish
        {
            get => ShouldSerilizeFinish()
                ? (string.IsNullOrEmpty(finish) ? emptyValueString : finish)
                : null;
            set => finish = value;
        }
        public bool ShouldSerilizeFinish()
        {
            return shouldSerilizeProperty[nameof(Finish)];
        }

        private string? shape = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Shape
        {
            get => ShouldSerilizeShape()
                ? (string.IsNullOrEmpty(shape) ? emptyValueString : shape)
                : null;
            set => shape = value;
        }
        public bool ShouldSerilizeShape()
        {
            return shouldSerilizeProperty[nameof(Shape)];
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

        private string? size = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Size
        {
            get => ShouldSerilizeSize()
                ? (string.IsNullOrEmpty(size) ? emptyValueString : size)
                : null;
            set => size = value;
        }
        public bool ShouldSerilizeSize()
        {
            return shouldSerilizeProperty[nameof(Size)];
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

        private string? thumbnailGeneralPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ThumbnailGeneralPhotoUrl
        {
            get => ShouldSerilizeThumbnailGeneralPhotoUrl()
                ? (string.IsNullOrEmpty(thumbnailGeneralPhotoUrl) ? emptyValueString : thumbnailGeneralPhotoUrl)
                : null;
            set => thumbnailGeneralPhotoUrl = value;
        }
        public bool ShouldSerilizeThumbnailGeneralPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(ThumbnailGeneralPhotoUrl)];
        }

        private string? thumbnailSpecificFinishPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ThumbnailSpecificFinishPhotoUrl
        {
            get => ShouldSerilizeThumbnailGeneralPhotoUrl()
                ? (string.IsNullOrEmpty(thumbnailSpecificFinishPhotoUrl) ? emptyValueString : thumbnailSpecificFinishPhotoUrl)
                : null;
            set => thumbnailSpecificFinishPhotoUrl = value;
        }
        public bool ShouldSerilizeThumbnailSpecificFinishPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(ThumbnailSpecificFinishPhotoUrl)];
        }

        private string? smallGeneralPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? SmallGeneralPhotoUrl
        {
            get => ShouldSerilizeSmallGeneralPhotoUrl()
                ? (string.IsNullOrEmpty(smallGeneralPhotoUrl) ? emptyValueString : smallGeneralPhotoUrl)
                : null;
            set => smallGeneralPhotoUrl = value;
        }
        public bool ShouldSerilizeSmallGeneralPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(SmallGeneralPhotoUrl)];
        }

        private string? smallSpecificFinishPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? SmallSpecificFinishPhotoUrl
        {
            get => ShouldSerilizeSmallSpecificFinishPhotoUrl()
                ? (string.IsNullOrEmpty(smallSpecificFinishPhotoUrl) ? emptyValueString : smallSpecificFinishPhotoUrl)
                : null;
            set => smallSpecificFinishPhotoUrl = value;
        }
        public bool ShouldSerilizeSmallSpecificFinishPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(SmallSpecificFinishPhotoUrl)];
        }

        private string? mediumGeneralPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? MediumGeneralPhotoUrl
        {
            get => ShouldSerilizeMediumGeneralPhotoUrl()
                ? (string.IsNullOrEmpty(mediumGeneralPhotoUrl) ? emptyValueString : mediumGeneralPhotoUrl)
                : null;
            set => mediumGeneralPhotoUrl = value;
        }
        public bool ShouldSerilizeMediumGeneralPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(MediumGeneralPhotoUrl)];
        }

        private string? mediumSpecificFinishPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? MediumSpecificFinishPhotoUrl
        {
            get => ShouldSerilizeMediumSpecificFinishPhotoUrl()
                ? (string.IsNullOrEmpty(mediumSpecificFinishPhotoUrl) ? emptyValueString : mediumSpecificFinishPhotoUrl)
                : null;
            set => mediumSpecificFinishPhotoUrl = value;
        }
        public bool ShouldSerilizeMediumSpecificFinishPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(MediumSpecificFinishPhotoUrl)];
        }

        private string? largeGeneralPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? LargeGeneralPhotoUrl
        {
            get => ShouldSerilizeLargeGeneralPhotoUrl()
                ? (string.IsNullOrEmpty(largeGeneralPhotoUrl) ? emptyValueString : largeGeneralPhotoUrl)
                : null;
            set => largeGeneralPhotoUrl = value;
        }
        public bool ShouldSerilizeLargeGeneralPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(LargeGeneralPhotoUrl)];
        }

        private string? largeSpecificFinishPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? LargeSpecificFinishPhotoUrl
        {
            get => ShouldSerilizeLargeSpecificFinishPhotoUrl()
                ? (string.IsNullOrEmpty(largeSpecificFinishPhotoUrl) ? emptyValueString : largeSpecificFinishPhotoUrl)
                : null;
            set => largeSpecificFinishPhotoUrl = value;
        }
        public bool ShouldSerilizeLargeSpecificFinishPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(LargeSpecificFinishPhotoUrl)];
        }

        private string? fullGeneralPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? FullGeneralPhotoUrl
        {
            get => ShouldSerilizeFullGeneralPhotoUrl()
                ? (string.IsNullOrEmpty(fullGeneralPhotoUrl) ? emptyValueString : fullGeneralPhotoUrl)
                : null;
            set => fullGeneralPhotoUrl = value;
        }
        public bool ShouldSerilizeFullGeneralPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(FullGeneralPhotoUrl)];
        }

        private string? fullSpecificFinishPhotoUrl = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? FullSpecificFinishPhotoUrl
        {
            get => ShouldSerilizeFullSpecificFinishPhotoUrl()
                ? (string.IsNullOrEmpty(fullSpecificFinishPhotoUrl) ? emptyValueString : fullSpecificFinishPhotoUrl)
                : null;
            set => fullSpecificFinishPhotoUrl = value;
        }
        public bool ShouldSerilizeFullSpecificFinishPhotoUrl()
        {
            return shouldSerilizeProperty[nameof(FullSpecificFinishPhotoUrl)];
        }

        private decimal cataloguePrice = 0;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal CataloguePrice
        {
            get => ShouldSerilizeCataloguePrice()
                ? cataloguePrice
                : 0; //return zero otherwise the XML Serilizer does not handle correctly nullable value types ...
            set => cataloguePrice = value;
        }
        public bool ShouldSerilizeCataloguePrice()
        {
            return shouldSerilizeProperty[nameof(CataloguePrice)];
        }

    }

    /// <summary>
    /// A Converter Interface to Convert <see cref="T"/> objects into <see cref="AccessoryWebItem"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAccessoryWebItemConverter<T>
    {
        /// <summary>
        /// Converts a <see cref="T"/> into a <see cref="AccessoryWebItem"/>
        /// </summary>
        /// <param name="item">The <see cref="T"/> object to Convert</param>
        /// <returns>The <see cref="AccessoryWebItem"/> converted Object</returns>
        AccessoryWebItem Convert(T item,string arg);
        void SetConversionOptions(WebItemConversionOptions options);
        void SetConversionOptions(Action<WebItemConversionOptions> action);
    }

    public class WebItemConversionOptions : IDeepClonable<WebItemConversionOptions>
    {
        public string LanguageIdentifier { get; set; } = "en-EN";
        public string PriceGroupId { get; set; } = string.Empty;
        public string[] WebItemSerilizableProperties { get; set; } = typeof(AccessoryWebItem).GetProperties().Select(p => p.Name).ToArray();

        public static WebItemConversionOptions Defaults() => new();

        public WebItemConversionOptions GetDeepClone()
        {
            return new WebItemConversionOptions()
            {
                LanguageIdentifier = this.LanguageIdentifier,
                PriceGroupId = this.PriceGroupId,
                WebItemSerilizableProperties = this.WebItemSerilizableProperties.ToArray()
            };
        }
    }
}
