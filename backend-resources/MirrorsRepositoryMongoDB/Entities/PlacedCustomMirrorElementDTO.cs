using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using MirrorsLib.Services.PositionService;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class PlacedCustomMirrorElementDTO : MirrorElementShortDTO , IDeepClonable<PlacedCustomMirrorElementDTO> , IEqualityComparerCreator<PlacedCustomMirrorElementDTO>
    {
        public PlacedCustomMirrorElementDTO()
        {
            
        }
        public PlacedCustomMirrorElementDTO(CustomMirrorElement customElement):base(customElement)
        {
            CustomElementType = customElement.CustomElementType;
        }
        public LocalizedString CustomElementType { get; set; } = LocalizedString.Undefined();

        public override PlacedCustomMirrorElementDTO GetDeepClone()
        {
            var clone = (PlacedCustomMirrorElementDTO) base.GetDeepClone();
            clone.CustomElementType = this.CustomElementType.GetDeepClone();
            return clone;
        }

        public CustomMirrorElement ToCustomMirrorElement(IMirrorsDataProvider dataProvider)
        {
            //Get the Photos from the Default Element
            var defaultCustomElement = dataProvider.GetCustomElements(this.DefaultElementRefId).FirstOrDefault();
            var elementInfo = this.GetBaseElementInfoWithoutPhotos();

            if (defaultCustomElement != null)
            {
                elementInfo.PhotoUrl = defaultCustomElement.PhotoUrl;
                elementInfo.PhotoUrl2 = defaultCustomElement.PhotoUrl2;
                elementInfo.IconUrl = defaultCustomElement.IconUrl;
            }

            return new CustomMirrorElement(elementInfo,this.CustomElementType.GetDeepClone());
        }

        public static IEqualityComparer<PlacedCustomMirrorElementDTO> GetComparer()
        {
            return new PlacedCustomMirrorElementDTOEqualityComparer();
        }
    }
    public class PlacedCustomMirrorElementDTOEqualityComparer : IEqualityComparer<PlacedCustomMirrorElementDTO>
    {
        private readonly MirrorElementShortDTOBaseEqualityComparer baseComparer = new();
        private readonly LocalizedStringEqualityComparer localizedStringComparer = new();

        public bool Equals(PlacedCustomMirrorElementDTO? x, PlacedCustomMirrorElementDTO? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            return baseComparer.Equals(x, y)
                && localizedStringComparer.Equals(x!.CustomElementType, y!.CustomElementType);
        }

        public int GetHashCode([DisallowNull] PlacedCustomMirrorElementDTO obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
