using System;
using CommonHelpers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfacesBronze
{
    /// <summary>
    /// A Class representing a Localized String Value
    /// </summary>
    public class LocalizedString : ILocalizedString, IDeepClonable<LocalizedString> , IEqualityComparerCreator<LocalizedString>
    {
        public const string GREEKIDENTIFIER = "el-GR";
        public const string ENGLISHIDENTIFIER = "en-EN";
        public const string ITALIANIDENTIFIER = "it-IT";

        /// <summary>
        /// Returns the four Letter Identifier out from a two letter Identifier , or the english identifier if one is not found
        /// </summary>
        /// <param name="twoLetterIdentifier">The Two letter language Identifier (ex 'gr','it','en' e.t.c.)</param>
        /// <returns></returns>
        public static string GetFourLetterIsoIdentifier(string twoLetterIdentifier)
        {
            return twoLetterIdentifier.ToUpperInvariant() switch
            {
                "EL" or "GR" => "el-GR",
                "IT" => "it-IT",
                "EN" => "en-EN",
                _ => "en-EN"
            };
        }

        /// <summary>
        /// For some reason Monogo DB sets the Auto Property Default Value to Null instead of string.Empty . This is a workaround to avoid Null Reference Exceptions
        /// </summary>
        private string? defaultValueHelper = string.Empty;
        /// <summary>
        /// The Default Value of the String when there are no Localizations Available
        /// </summary>
        public string DefaultValue { get => defaultValueHelper ?? string.Empty; set => defaultValueHelper = value; }

        /// <summary>
        /// The Localized Values , of the string e.x. "el-GR","en-EN" e.t.c.
        /// </summary>
        public Dictionary<string, string> LocalizedValues { get; } = [];
        public static LocalizedString Undefined() => new("Undefined");

        /// <summary>
        /// Returns the Localized value for the Requested Language Identifier (ex. "EN","GR")
        /// If the Requested Identifier does not exist returns the Default Value
        /// </summary>
        /// <param name="isoFourLetterLanguageIdentifier">The four letter Iso Language Identifier (ex. "el-GR","en-EN" e.t.c.) Case does not Matter</param>
        /// <param name="returnDefaultOnEmpty">Weather to return the Default Value on Empty Localized Values or to return an Empty String</param>
        /// <returns></returns>
        public string GetLocalizedValue(string isoFourLetterLanguageIdentifier,bool returnDefaultOnEmpty = true)
        {
            if (LocalizedValues.TryGetValue(isoFourLetterLanguageIdentifier, out var value))
            {
                return value;
            }
            else if (returnDefaultOnEmpty)
            {
                return DefaultValue;
            }
            else
            {
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Returns the Localized Value minified to the specified max characters.
        /// </summary>
        /// <param name="isoFourLetterLanguageIdentifier">The Iso Four Letter language identifier</param>
        /// <param name="maxCharacters">the Maximum charachters to return</param>
        /// <returns></returns>
        public string GetMinifiedLocalizedValue(string isoFourLetterLanguageIdentifier , int maxCharacters = 33)
        {
            var localizedValue = GetLocalizedValue(isoFourLetterLanguageIdentifier);
            //Replace line changes with spaces
            var minified = localizedValue?.Replace(Environment.NewLine, " ") ?? "";
            // Return the First specified Chars Followed by '...'
            if (string.IsNullOrWhiteSpace(minified)) return " -Undefined- ";
            return (minified.Length <= maxCharacters ? minified : string.Concat(minified.AsSpan(0, maxCharacters - 3), "..."));
        }

        public string GetDefaultValue() => DefaultValue;

        public LocalizedString()
        {
            
        }
        public LocalizedString(string defaultValue)
        {
            this.DefaultValue = defaultValue;
        }
        public LocalizedString(string defaultValue, Dictionary<string, string> localizedValues)
        {
            this.DefaultValue = defaultValue;
            LocalizedValues = new(localizedValues);
        }

        public LocalizedString GetDeepClone()
        {
            LocalizedString clone = new(this.DefaultValue,this.LocalizedValues);
            return clone;
        }

        public static IEqualityComparer<LocalizedString> GetComparer()
        {
            return new LocalizedStringEqualityComparer();
        }
    }

    public class LocalizedStringEqualityComparer : IEqualityComparer<LocalizedString>
    {
        public bool Equals(LocalizedString? x, LocalizedString? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.LocalizedValues.IsEqualToOtherDictionary(y.LocalizedValues) &&
                x.DefaultValue == y.DefaultValue;
        }

        public int GetHashCode([DisallowNull] LocalizedString obj)
        {
            return obj.DefaultValue.GetHashCode();
        }
    }

    public interface ILocalizedString
    {
        /// <summary>
        /// Returns the Localized value for the Requested Language Identifier (ex. "EN","GR")
        /// If the Requested Identifier does not exist returns the Default Value
        /// </summary>
        /// <param name="isoFourLetterLanguageIdentifier">The four letter Iso Language Identifier (ex. "el-GR","en-EN" e.t.c.) Case does not Matter</param>
        /// <returns></returns>
        string GetLocalizedValue(string isoFourLetterLanguageIdentifier, bool returnDefaultOnEmpty = true);
        /// <summary>
        /// Returns the Default Value , without any localization (usually English)
        /// </summary>
        /// <returns></returns>
        string GetDefaultValue();
    }

    public interface ILocalizedDescription
    {
        public LocalizedDescription LocalizedDescriptionInfo { get; }
    }

    public class LocalizedDescription : IDeepClonable<LocalizedDescription> , IEqualityComparerCreator<LocalizedDescription>
    {
        public LocalizedString Name { get; set; } = LocalizedString.Undefined();
        public LocalizedString Description { get; set; } = LocalizedString.Undefined();
        public LocalizedString ExtendedDescription { get; set; } = LocalizedString.Undefined();

        public LocalizedDescription()
        {
            
        }

        public ObjectDescriptionInfo GetDescriptionInfo(string langIdentifier, bool returnDefaultWhenEmpty = true)
        {
            return new ObjectDescriptionInfo(
                this.Name.GetLocalizedValue(langIdentifier, returnDefaultWhenEmpty),
                this.Description.GetLocalizedValue(langIdentifier, returnDefaultWhenEmpty),
                this.ExtendedDescription.GetLocalizedValue(langIdentifier, returnDefaultWhenEmpty));
        }
        public static LocalizedDescription Empty() => new();
        public static LocalizedDescription Create(LocalizedString name , LocalizedString description , LocalizedString extendedDescription)
        {
            return new LocalizedDescription()
            {
                Name = name,
                Description = description,
                ExtendedDescription = extendedDescription
            };
        }
        public static LocalizedDescription WithNameOnlyDefaultValue(string defaultNameValue) => new() { Name = new(defaultNameValue) };
        public LocalizedDescription GetDeepClone()
        {
            return new LocalizedDescription()
            {
                Name = this.Name.GetDeepClone(),
                Description = this.Description.GetDeepClone(),
                ExtendedDescription = this.ExtendedDescription.GetDeepClone()
            };
        }

        public static IEqualityComparer<LocalizedDescription> GetComparer()
        {
            return new LocalizedDescriptionEqualityComparer();
        }
    }

    public class LocalizedDescriptionEqualityComparer : IEqualityComparer<LocalizedDescription>
    {
        private LocalizedStringEqualityComparer localizedStringComparer = new();

        public bool Equals(LocalizedDescription? x, LocalizedDescription? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            
            return localizedStringComparer.Equals(x.Name,y.Name) &&
                localizedStringComparer.Equals(x.Description, y.Description) &&
                localizedStringComparer.Equals(x.ExtendedDescription, y.ExtendedDescription);
        }

        public int GetHashCode([DisallowNull] LocalizedDescription obj)
        {
            var nameHash = obj.Name is not null ? localizedStringComparer.GetHashCode(obj.Name) : 13;
            var descHash = obj.Description is not null ? localizedStringComparer.GetHashCode(obj.Description) : 17;
            var extendDescHash = obj.ExtendedDescription is not null ? localizedStringComparer.GetHashCode(obj.ExtendedDescription) : 19;
            return HashCode.Combine(nameHash, descHash, extendDescHash);
        }
    }

}
