using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.Repositories;
using ShapesLibrary;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorPlacedLightDTO : MirrorElementShortDTO, IDeepClonable<MirrorPlacedLightDTO> ,IEqualityComparerCreator<MirrorPlacedLightDTO>
    {
        public MirrorLightInfo LightInfo { get; set; } = MirrorLightInfo.Undefined();
        public MirrorAdditionalLightInfo AdditionalLightInfo { get; set; } = MirrorAdditionalLightInfo.Undefined();

        public override MirrorPlacedLightDTO GetDeepClone()
        {
            var clone = (MirrorPlacedLightDTO)base.GetDeepClone();
            clone.LightInfo = this.LightInfo.GetDeepClone();
            clone.AdditionalLightInfo = this.AdditionalLightInfo.GetDeepClone();
            return clone;
        }

        public MirrorLight ToMirrorLight(IMirrorsDataProvider dataProvider)
        {
            //Get the Photos from the Default Element
            var defaultLight = dataProvider.GetLights(this.DefaultElementRefId).FirstOrDefault();
            var elementInfo = this.GetBaseElementInfoWithoutPhotos();

            if (defaultLight != null)
            {
                elementInfo.PhotoUrl = defaultLight.PhotoUrl;
                elementInfo.PhotoUrl2 = defaultLight.PhotoUrl2;
                elementInfo.IconUrl = defaultLight.IconUrl;
            }

            return new MirrorLight(elementInfo, LightInfo.GetDeepClone(), AdditionalLightInfo.GetDeepClone());
        }

        public static IEqualityComparer<MirrorPlacedLightDTO> GetComparer()
        {
            return new MirrorPlacedLightDTOEqualityComparer();
        }
    }
    public class MirrorPlacedLightDTOEqualityComparer : IEqualityComparer<MirrorPlacedLightDTO>
    {
        private readonly MirrorElementShortDTOBaseEqualityComparer baseComparer = new();
        private readonly MirrorLightInfoEqualityComparer lightInfoComparer = new();
        private readonly MirrorAdditionalLightInfoEqualityComparer additionalLightInfoComparer = new();

        public bool Equals(MirrorPlacedLightDTO? x, MirrorPlacedLightDTO? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            return baseComparer.Equals(x, y)
                && lightInfoComparer.Equals(x!.LightInfo, y!.LightInfo)
                && additionalLightInfoComparer.Equals(x.AdditionalLightInfo, y.AdditionalLightInfo);
        }

        public int GetHashCode([DisallowNull] MirrorPlacedLightDTO obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
