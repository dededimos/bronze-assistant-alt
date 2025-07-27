using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements.Charachteristics;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace MirrorsLib.MirrorElements
{
    public interface IMirrorElement : ICodeable, ILocalizedDescription
    {
        string ShortCode { get; }
        string MinimalCode { get; }
        /// <summary>
        /// The Id of the Default Element
        /// <para>If this is an overridden Element , the id helps us find the Default Element from which it was overriden</para>
        /// </summary>
        string ElementId { get; }
        bool IsOverriddenElement { get; }
        string PhotoUrl { get; }
        string PhotoUrl2 { get; }
        string IconUrl { get; }
    }

    public class MirrorElementBase : IMirrorElement, ICodeable,ILocalizedDescription, IDeepClonable<MirrorElementBase> , IEqualityComparerCreator<MirrorElementBase>
    {
        public MirrorElementBase()
        {
            
        }
        public MirrorElementBase(IMirrorElement element)
        {
            ElementId = element.ElementId;
            Code = element.Code;
            ShortCode = element.ShortCode;
            MinimalCode = element.MinimalCode;
            PhotoUrl = element.PhotoUrl;
            PhotoUrl2 = element.PhotoUrl2;
            IconUrl = element.IconUrl;
            LocalizedDescriptionInfo = element.LocalizedDescriptionInfo.GetDeepClone();
            IsOverriddenElement = element.IsOverriddenElement;
        }
        

        public string ElementId { get; set; } = string.Empty;
        /// <summary>
        /// Weather its from the Standard List of Elements or a custom One
        /// <para>Should only be used for the Photos purposing or to indicate weather the user has modified the Mirror,Sandblast e.t.c.</para>
        /// <para>Changing a default element will not change any saved elements that where the default elenment in the Past</para>
        /// </summary>
        public bool IsOverriddenElement { get; set; }
        public virtual string Code { get; set; } = string.Empty;
        /// <summary>
        /// A shorter version of the Code , usually two digits
        /// </summary>
        public string ShortCode { get; set; } = string.Empty;
        /// <summary>
        /// A minimal version of the Code , usually one digit
        /// </summary>
        public string MinimalCode { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string PhotoUrl2 { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public LocalizedDescription LocalizedDescriptionInfo { get; set; } = LocalizedDescription.Empty();
        public Dictionary<string, IMirrorElementTrait> CustomTraits { get; set; } = [];

        public virtual MirrorElementBase GetDeepClone()
        {
            var clone = (MirrorElementBase)MemberwiseClone();
            clone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            clone.CustomTraits = CustomTraits.ToDictionary(kvp=> kvp.Key,kvp=>kvp.Value.GetDeepClone());
            return clone;
        }
        public override string ToString()
        {
            return Code;
        }
        public static MirrorElementBase Empty() => new();
        /// <summary>
        /// Creates a Default Position Element Info
        /// </summary>
        /// <returns></returns>
        public static MirrorElementBase DefaultPositionElementInfo() => new()
        {
            Code = "DefaultPosition",
            ElementId = string.Empty,
            PhotoUrl = string.Empty,
            PhotoUrl2 = string.Empty,
            IconUrl = string.Empty,
            IsOverriddenElement = false,
            LocalizedDescriptionInfo = LocalizedDescription.WithNameOnlyDefaultValue("DefaultPosition")
        };
        /// <summary>
        /// Creates a Not Available (N/A) element Info
        /// </summary>
        /// <returns></returns>
        public static MirrorElementBase NAElementInfo() => new()
        {
            Code="N/A",
            ElementId = string.Empty,
            PhotoUrl = string.Empty,
            PhotoUrl2 = string.Empty,
            IconUrl = string.Empty,
            IsOverriddenElement = false,
            LocalizedDescriptionInfo = LocalizedDescription.WithNameOnlyDefaultValue("N/A")
        };
        public static MirrorElementBase CustomRoundedCornersElementInfo() => new()
        {
            Code = $"0000-CURVE-CUSTOM",
            ElementId = string.Empty,
            PhotoUrl = string.Empty,
            PhotoUrl2 = string.Empty,
            IconUrl = string.Empty,
            IsOverriddenElement = false,
            LocalizedDescriptionInfo = LocalizedDescription.WithNameOnlyDefaultValue("CustomRoundedCorners")
        };

        public static IEqualityComparer<MirrorElementBase> GetComparer()
        {
            return new MirrorElementEqualityComparer();
        }
    }
    public class MirrorElementEqualityComparer : IEqualityComparer<IMirrorElement>
    {
        private LocalizedDescriptionEqualityComparer locStringComparer = new();

        public bool Equals(IMirrorElement? x, IMirrorElement? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.ElementId == y.ElementId &&
                x.IsOverriddenElement == y.IsOverriddenElement &&
                locStringComparer.Equals(x.LocalizedDescriptionInfo, y.LocalizedDescriptionInfo) 
                && x.PhotoUrl == y.PhotoUrl 
                && x.PhotoUrl2 == y.PhotoUrl2 
                && x.IconUrl == y.IconUrl 
                && x.Code == y.Code
                && x.ShortCode == y.ShortCode
                && x.MinimalCode == y.MinimalCode;
        }

        public int GetHashCode([DisallowNull] IMirrorElement obj)
        {
            int hash = HashCode.Combine(obj.ElementId, obj.IsOverriddenElement, locStringComparer.GetHashCode(obj.LocalizedDescriptionInfo), obj.PhotoUrl, obj.PhotoUrl2, obj.IconUrl);
            return HashCode.Combine(hash, obj.Code, obj.ShortCode, obj.MinimalCode);
        }
    }

}
