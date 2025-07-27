using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class CustomMirrorElementEntity : MirrorElementEntity, IDeepClonable<CustomMirrorElementEntity>, IEqualityComparerCreator<CustomMirrorElementEntity>
    {
        public LocalizedString CustomElementType { get; set; } = LocalizedString.Undefined();

        static IEqualityComparer<CustomMirrorElementEntity> IEqualityComparerCreator<CustomMirrorElementEntity>.GetComparer()
        {
            return new CustomMirrorElementEntityEqualityComparer();
        }

        public override CustomMirrorElementEntity GetDeepClone()
        {
            var clone = (CustomMirrorElementEntity)base.GetDeepClone();
            clone.CustomElementType = this.CustomElementType.GetDeepClone();
            return clone;
        }
        public CustomMirrorElement ToCustomMirrorElement()
        {
            var elementInfo = this.GetMirrorElementInfo();
            return new CustomMirrorElement(elementInfo, CustomElementType);
        }
    }

    public class CustomMirrorElementEntityEqualityComparer : IEqualityComparer<CustomMirrorElementEntity>
    {
        private readonly MirrorElementEntityBaseEqualityComparer baseComparer = new();
        private readonly LocalizedStringEqualityComparer localizedStringComparer = new();

        public bool Equals(CustomMirrorElementEntity? x, CustomMirrorElementEntity? y)
        {
            return baseComparer.Equals(x, y) &&
                localizedStringComparer.Equals(x!.CustomElementType, y!.CustomElementType);
        }

        public int GetHashCode([DisallowNull] CustomMirrorElementEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
