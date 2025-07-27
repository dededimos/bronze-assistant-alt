using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.Services.PositionService;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorElementPositionEntity : MirrorElementEntity, IDeepClonable<MirrorElementPositionEntity>, IEqualityComparerCreator<MirrorElementPositionEntity>
    {
        public PositionInstructionsBase Instructions { get; set; } = PositionInstructionsBase.UndefinedInstructions();

        static IEqualityComparer<MirrorElementPositionEntity> IEqualityComparerCreator<MirrorElementPositionEntity>.GetComparer()
        {
            return new MirrorElementPositionEntityEqualityComparer();
        }

        public override MirrorElementPositionEntity GetDeepClone()
        {
            var clone = (MirrorElementPositionEntity)base.GetDeepClone();
            clone.Instructions = this.Instructions.GetDeepClone();
            return clone;
        }

        public MirrorElementPosition ToPosition()
        {
            var elementInfo = this.GetMirrorElementInfo();
            return new MirrorElementPosition(elementInfo, Instructions.GetDeepClone());
        }
    }

    public class MirrorElementPositionEntityEqualityComparer : IEqualityComparer<MirrorElementPositionEntity>
    {
        private readonly MirrorElementEntityBaseEqualityComparer baseComparer = new();
        private readonly PositionInstructionsBaseEqualityComparer positionComparer = new();
        public bool Equals(MirrorElementPositionEntity? x, MirrorElementPositionEntity? y)
        {
            return baseComparer.Equals(x, y) &&
                positionComparer.Equals(x!.Instructions, y!.Instructions);
        }

        public int GetHashCode([DisallowNull] MirrorElementPositionEntity obj)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not Support a Get Hash Code Implementation");
        }
    }
}
