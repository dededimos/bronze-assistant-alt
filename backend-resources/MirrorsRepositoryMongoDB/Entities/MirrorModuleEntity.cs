using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    //We do not care for mirror Module Entity having a Unique Id property
    //All the Entities are Unique in the Database with their Database Id .
    //When they are instantiated in the application they are used to be placed in the Mirrors
    //The Unique Id is assigned for their placement in the mirror only
    public class MirrorModuleEntity : MirrorElementEntity, IDeepClonable<MirrorModuleEntity>, IEqualityComparerCreator<MirrorModuleEntity>
    {
        public MirrorModuleInfo ModuleInfo { get; set; } = MirrorModuleInfo.Undefined();

        static IEqualityComparer<MirrorModuleEntity> IEqualityComparerCreator<MirrorModuleEntity>.GetComparer()
        {
            return new MirrorModuleEntityEqualityComparer();
        }

        public override MirrorModuleEntity GetDeepClone()
        {
            var clone = (MirrorModuleEntity)base.GetDeepClone();
            clone.ModuleInfo = this.ModuleInfo.GetDeepClone();
            return clone;
        }

        public MirrorModule ToMirrorModule()
        {
            var elementInfo = this.GetMirrorElementInfo();
            var moduleInfo = this.ModuleInfo.GetDeepClone();
            return new(elementInfo, moduleInfo);
            //We do not care for mirror Module Entity having a Unique Id property
            //All the Entities are Unique in the Database with their Database Id .
            //When they are instantiated in the application they are used to be placed in the Mirrors
            //The Unique Id is assigned for their placement in the mirror only
        }
    }

    public class MirrorModuleEntityEqualityComparer : IEqualityComparer<MirrorModuleEntity>
    {
        private readonly MirrorElementEntityBaseEqualityComparer baseComparer = new();
        private readonly MirrorModuleInfoEqualityComparer moduleComparer = new();

        public bool Equals(MirrorModuleEntity? x, MirrorModuleEntity? y)
        {
            return baseComparer.Equals(x, y) &&
                moduleComparer.Equals(x!.ModuleInfo, y!.ModuleInfo);
        }

        public int GetHashCode([DisallowNull] MirrorModuleEntity obj)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not Support a Get Hash Code Implementation");
        }
    }
}
