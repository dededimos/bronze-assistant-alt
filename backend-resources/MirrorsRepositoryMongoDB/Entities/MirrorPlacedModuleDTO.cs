using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorPlacedModuleDTO : MirrorElementShortDTO , IDeepClonable<MirrorPlacedModuleDTO> , IEqualityComparerCreator<MirrorPlacedModuleDTO>
    {
        public MirrorPlacedModuleDTO()
        {
            
        }
        public MirrorPlacedModuleDTO(MirrorModule module) : base(module)
        {
            ModuleInfo = module.ModuleInfo;
            UniqueId = module.ItemUniqueId;
        }

        public string UniqueId { get; set; } = string.Empty;
        public MirrorModuleInfo ModuleInfo { get; set; } = MirrorModuleInfo.Undefined();

        public MirrorModule ToMirrorModule(IMirrorsDataProvider dataProvider)
        {
            //Get the Photos from the Default Element
            var defaultModule = dataProvider.GetModule(this.DefaultElementRefId);
            var elementInfo = this.GetBaseElementInfoWithoutPhotos();
            if (defaultModule != null)
            {
                elementInfo.PhotoUrl = defaultModule.PhotoUrl;
                elementInfo.PhotoUrl2 = defaultModule.PhotoUrl2;
                elementInfo.IconUrl = defaultModule.IconUrl;
            }

            var mirrorModule = new MirrorModule(elementInfo, ModuleInfo.GetDeepClone());
            //The Id unique id must be preserved , as it is being used by the positions dictionary also 
            mirrorModule.AssignNewUniqueId(UniqueId);
            return mirrorModule;
        }

        public override MirrorPlacedModuleDTO GetDeepClone()
        {
            var clone = (MirrorPlacedModuleDTO)base.GetDeepClone();
            clone.ModuleInfo = ModuleInfo.GetDeepClone();
            return clone;
        }

        public static IEqualityComparer<MirrorPlacedModuleDTO> GetComparer()
        {
            return new MirrorPlacedModuleDTOEqualityComparer();
        }
    }
    public class MirrorPlacedModuleDTOEqualityComparer : IEqualityComparer<MirrorPlacedModuleDTO>
    {
        private readonly MirrorElementShortDTOBaseEqualityComparer baseComparer = new();
        private readonly MirrorModuleInfoEqualityComparer moduleInfoComparer = new(false);

        public bool Equals(MirrorPlacedModuleDTO? x, MirrorPlacedModuleDTO? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            return baseComparer.Equals(x, y)
                && moduleInfoComparer.Equals(x!.ModuleInfo,y!.ModuleInfo)
                && x.UniqueId == y.UniqueId;
        }

        public int GetHashCode([DisallowNull] MirrorPlacedModuleDTO obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
