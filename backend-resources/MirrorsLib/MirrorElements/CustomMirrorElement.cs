using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements
{
    public class CustomMirrorElement : MirrorElementBase ,IEqualityComparerCreator<CustomMirrorElement>,IDeepClonable<CustomMirrorElement>
    {
        public LocalizedString CustomElementType { get; set; } = LocalizedString.Undefined();

        public CustomMirrorElement(IMirrorElement mirrorElement,LocalizedString customElementType) : base(mirrorElement)
        {
            CustomElementType = customElementType.GetDeepClone();
        }

        CustomMirrorElement IDeepClonable<CustomMirrorElement>.GetDeepClone()
        {
            var clone = (CustomMirrorElement)this.MemberwiseClone();
            clone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            clone.CustomElementType = this.CustomElementType.GetDeepClone();
            return clone;
        }
        static IEqualityComparer<CustomMirrorElement> IEqualityComparerCreator<CustomMirrorElement>.GetComparer()
        {
            return new CustomMirrorElementEqualityComparer();
        }
    }

    public class CustomMirrorElementEqualityComparer : IEqualityComparer<CustomMirrorElement>
    {
        private readonly MirrorElementEqualityComparer elementComparer = new();
        private readonly LocalizedStringEqualityComparer localizedStringComparer = new();

        public bool Equals(CustomMirrorElement? x, CustomMirrorElement? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return elementComparer.Equals(x, y) &&
                localizedStringComparer.Equals(x.CustomElementType, y.CustomElementType);
        }

        public int GetHashCode([DisallowNull] CustomMirrorElement obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

}
