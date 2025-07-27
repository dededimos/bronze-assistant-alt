using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Charachteristics;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorElementTraitEntity : MirrorElementEntity , IDeepClonable<MirrorElementTraitEntity> , IEqualityComparerCreator<MirrorElementTraitEntity>
    {
        static IEqualityComparer<MirrorElementTraitEntity> IEqualityComparerCreator<MirrorElementTraitEntity>.GetComparer()
        {
            return new MirrorElementTraitEntityEqualityComparer();
        }

        public bool IsAssignableToAll { get; set; }
        public List<string> TargetTypes { get; set; } = [];
        public List<string> TargetElementIds { get; set; } = [];

        public override MirrorElementTraitEntity GetDeepClone()
        {
            var clone = (MirrorElementTraitEntity)base.GetDeepClone();
            clone.TargetTypes = new(this.TargetTypes);
            clone.TargetElementIds = new(this.TargetElementIds);
            return clone;
        }
        public virtual MirrorElementTraitBase ToMirrorElementTrait()
        {
            return new(this.GetMirrorElementInfo());
        }
    }

    public class MirrorElementTraitEntityBaseEqualityComparer : IEqualityComparer<MirrorElementTraitEntity>
    {
        private readonly MirrorElementEntityBaseEqualityComparer baseComparer = new();

        public bool Equals(MirrorElementTraitEntity? x, MirrorElementTraitEntity? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            if (x.GetType() != y.GetType()) return false;

            return baseComparer.Equals(x, y) &&
                x.IsAssignableToAll == y.IsAssignableToAll &&
                x!.TargetTypes.SequenceEqual(y!.TargetTypes) &&
                x.TargetElementIds.SequenceEqual(y!.TargetElementIds);
        }

        public int GetHashCode([DisallowNull] MirrorElementTraitEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
    public class MirrorElementTraitEntityEqualityComparer : IEqualityComparer<MirrorElementTraitEntity>
    {
        public bool Equals(MirrorElementTraitEntity? x, MirrorElementTraitEntity? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            if (x.GetType() != y.GetType()) return false;

            switch (x)
            {
                case CustomMirrorTraitEntity customTrait:
                    var customTraitComparer = new CustomMirrorTraitEntityEqualityComparer();
                    return customTraitComparer.Equals(customTrait, (CustomMirrorTraitEntity)y);
                default:
                    var baseComparer = new MirrorElementTraitEntityBaseEqualityComparer();
                    return baseComparer.Equals(x, y);
            }
        }

        public int GetHashCode([DisallowNull] MirrorElementTraitEntity obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
