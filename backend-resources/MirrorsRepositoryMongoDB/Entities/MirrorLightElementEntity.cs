using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorLightElementEntity : MirrorElementEntity, IDeepClonable<MirrorLightElementEntity>, IEqualityComparerCreator<MirrorLightElementEntity>
    {
        public MirrorLightInfo LightInfo { get; set; } = MirrorLightInfo.Undefined();
        public override MirrorLightElementEntity GetDeepClone()
        {
            var clone = (MirrorLightElementEntity)base.GetDeepClone();
            clone.LightInfo = this.LightInfo.GetDeepClone();
            return clone;
        }

        public static MirrorLightElementEntity Undefined() => new();
        public MirrorLightElement ToLight()
        {
            var elementInfo = this.GetMirrorElementInfo();
            return new MirrorLightElement(elementInfo, LightInfo.GetDeepClone());
        }

        static IEqualityComparer<MirrorLightElementEntity> IEqualityComparerCreator<MirrorLightElementEntity>.GetComparer()
        {
            return new MirrorLightElementEntityEqualityComparer();
        }
    }

    public class MirrorLightElementEntityEqualityComparer : IEqualityComparer<MirrorLightElementEntity>
    {
        public bool Equals(MirrorLightElementEntity? x, MirrorLightElementEntity? y)
        {
            var baseComparer = new MirrorElementEntityBaseEqualityComparer();
            var lightComparer = MirrorLightInfo.GetComparer();

            return baseComparer.Equals(x, y) &&
                lightComparer.Equals(x!.LightInfo, y!.LightInfo);
        }

        public int GetHashCode([DisallowNull] MirrorLightElementEntity obj)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not Support a Get Hash Code Implementation");
        }
    }
}
