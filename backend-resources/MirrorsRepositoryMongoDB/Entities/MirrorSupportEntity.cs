using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Supports;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorSupportEntity : MirrorElementEntity, IDeepClonable<MirrorSupportEntity>, IEqualityComparerCreator<MirrorSupportEntity>
    {
        public MirrorSupportInfo Support { get; set; } = MirrorSupportInfo.Undefined();
        public string DefaultSelectedFinishId { get; set; } = string.Empty;

        public List<string> SelectableFinishes { get; set; } = [];

        static IEqualityComparer<MirrorSupportEntity> IEqualityComparerCreator<MirrorSupportEntity>.GetComparer()
        {
            return new MirrorSupportEntityEqualityComparer();
        }

        public override MirrorSupportEntity GetDeepClone()
        {
            var clone = (MirrorSupportEntity)base.GetDeepClone();
            clone.Support = this.Support.GetDeepClone();
            clone.SelectableFinishes = new(this.SelectableFinishes);
            return clone;
        }

        public MirrorSupport ToSupport(IEnumerable<MirrorFinishElement> finishElements)
        {
            var elementInfo = this.GetMirrorElementInfo();
            var finish = finishElements.FirstOrDefault(f => f.ElementId == this.DefaultSelectedFinishId) ?? MirrorFinishElement.EmptyFinish();
            return new MirrorSupport(elementInfo, Support.GetDeepClone(),finish);
        }
    }

    public class MirrorSupportEntityEqualityComparer : IEqualityComparer<MirrorSupportEntity>
    {
        public MirrorSupportEntityEqualityComparer(bool disregardCollisionDistances = false)
        {
            supportComparer = new(disregardCollisionDistances);
        }
        private readonly MirrorElementEntityBaseEqualityComparer baseComparer = new();
        private readonly MirrorSupportInfoEqualityComparer supportComparer;
        public bool Equals(MirrorSupportEntity? x, MirrorSupportEntity? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.SelectableFinishes.SequenceEqual(y!.SelectableFinishes) &&
                x.DefaultSelectedFinishId == y.DefaultSelectedFinishId &&
                supportComparer.Equals(x!.Support, y!.Support);
        }

        public int GetHashCode([DisallowNull] MirrorSupportEntity obj)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not Support a Get Hash Code Implementation");
        }
    }
}
