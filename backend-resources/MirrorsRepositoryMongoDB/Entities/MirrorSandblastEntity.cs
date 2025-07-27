using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Sandblasts;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorSandblastEntity : MirrorElementEntity, IDeepClonable<MirrorSandblastEntity> , IEqualityComparerCreator<MirrorSandblastEntity>
    {
        public MirrorSandblastInfo Sandblast { get; set; } = MirrorSandblastInfo.Undefined();

        static IEqualityComparer<MirrorSandblastEntity> IEqualityComparerCreator<MirrorSandblastEntity>.GetComparer()
        {
            return new MirrorSandblastEntityEqualityComparer();
        }

        public override MirrorSandblastEntity GetDeepClone()
        {
            var clone = (MirrorSandblastEntity)base.GetDeepClone();
            clone.Sandblast = this.Sandblast.GetDeepClone();
            return clone;
        }
        public MirrorSandblast ToSandblast()
        {
            var elementInfo = this.GetMirrorElementInfo();
            return new MirrorSandblast(elementInfo, Sandblast.GetDeepClone());
        }
    }

    public class MirrorSandblastEntityEqualityComparer : IEqualityComparer<MirrorSandblastEntity>
    {
        private readonly MirrorElementEntityBaseEqualityComparer baseComparer = new();
        private readonly MirrorSandblastInfoEqualityComparer sandblastComparer = new();

        public bool Equals(MirrorSandblastEntity? x, MirrorSandblastEntity? y)
        {
            return baseComparer.Equals(x, y) &&
                sandblastComparer.Equals(x!.Sandblast, y!.Sandblast);
        }

        public int GetHashCode([DisallowNull] MirrorSandblastEntity obj)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not Support a Get Hash Code Implementation");
        }
    }
}
