using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Charachteristics;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class CustomMirrorTraitEntity : MirrorElementTraitEntity , IDeepClonable<CustomMirrorTraitEntity> , IEqualityComparerCreator<CustomMirrorTraitEntity>
    {
        public LocalizedString CustomTraitType { get; set; } = LocalizedString.Undefined();

        static IEqualityComparer<CustomMirrorTraitEntity> IEqualityComparerCreator<CustomMirrorTraitEntity>.GetComparer()
        {
            return new CustomMirrorTraitEntityEqualityComparer();
        }

        public override CustomMirrorTraitEntity GetDeepClone()
        {
            var clone = (CustomMirrorTraitEntity)base.GetDeepClone();
            clone.CustomTraitType = this.CustomTraitType.GetDeepClone();
            return clone;
        }
        public override CustomMirrorTrait ToMirrorElementTrait()
        {
            return ToCustomMirrorTrait();
        }
        public CustomMirrorTrait ToCustomMirrorTrait()
        {
            return new(base.ToMirrorElementTrait(), this.CustomTraitType.GetDeepClone());
        }
    }

    public class CustomMirrorTraitEntityEqualityComparer : IEqualityComparer<CustomMirrorTraitEntity>
    {
        private readonly MirrorElementTraitEntityBaseEqualityComparer baseComparer = new();
        private readonly LocalizedStringEqualityComparer localizedStringComparer = new();

        public bool Equals(CustomMirrorTraitEntity? x, CustomMirrorTraitEntity? y)
        {
            return baseComparer.Equals(x, y) &&
                localizedStringComparer.Equals(x!.CustomTraitType, y!.CustomTraitType);
        }

        public int GetHashCode([DisallowNull] CustomMirrorTraitEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
