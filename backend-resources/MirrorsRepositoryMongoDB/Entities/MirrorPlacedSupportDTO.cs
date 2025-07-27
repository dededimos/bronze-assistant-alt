using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Repositories;
using ShapesLibrary;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorPlacedSupportDTO : MirrorElementShortDTO, IDeepClonable<MirrorPlacedSupportDTO> , IEqualityComparerCreator<MirrorPlacedSupportDTO>
    {
        public MirrorPlacedSupportDTO()
        {
            
        }
        public MirrorPlacedSupportDTO(MirrorPlacedSupport placedSupport) : base(placedSupport)
        {
            Support = placedSupport.SupportInfo;
            RefFinishElementId = placedSupport.Finish.ElementId;
            SupportRearShape = placedSupport.SupportRearShape;
            SupportFrontShape = placedSupport.SupportFrontShape;
            SupportSideShape = placedSupport.SupportSideShape;
            FormedBoundary = placedSupport.FormedBoundary;
        }

        public MirrorSupportInfo Support { get; set; } = MirrorSupportInfo.Undefined();
        public string RefFinishElementId { get; set; } = string.Empty;
        public ShapeInfo? SupportRearShape { get; set; }
        public ShapeInfo? SupportFrontShape { get; set; }
        public ShapeInfo? SupportSideShape { get; set; }
        public ShapeInfo? FormedBoundary { get; set; }

        public MirrorPlacedSupport ToPlacedSupport(IMirrorsDataProvider dataProvider)
        {
            //Get the Photos from the Default Element
            var defaultSupport = dataProvider.GetSupports(this.DefaultElementRefId).FirstOrDefault();
            var elementInfo = this.GetBaseElementInfoWithoutPhotos();
            if (defaultSupport != null)
            {
                elementInfo.PhotoUrl = defaultSupport.PhotoUrl;
                elementInfo.PhotoUrl2 = defaultSupport.PhotoUrl2;
                elementInfo.IconUrl = defaultSupport.IconUrl;
            }
            var finish = dataProvider.GetFinishElements(RefFinishElementId).FirstOrDefault() ?? MirrorFinishElement.EmptyFinish();

            MirrorPlacedSupport support = new(Support.GetDeepClone(), elementInfo, finish,SupportRearShape?.GetDeepClone(),SupportFrontShape?.GetDeepClone())
            {
                SupportSideShape = SupportSideShape?.GetDeepClone(),
                FormedBoundary = FormedBoundary?.GetDeepClone(),
            };
            return support;
        }

        public override MirrorPlacedSupportDTO GetDeepClone()
        {
            var clone = (MirrorPlacedSupportDTO)base.GetDeepClone();
            clone.Support = Support.GetDeepClone();
            clone.SupportRearShape = SupportRearShape?.GetDeepClone();
            clone.SupportFrontShape = SupportFrontShape?.GetDeepClone();
            clone.SupportSideShape = SupportSideShape?.GetDeepClone();
            clone.FormedBoundary = FormedBoundary?.GetDeepClone();
            return clone;
        }
        public static MirrorPlacedSupportDTO? Create(MirrorPlacedSupport? placedSupport)
        {
            if (placedSupport is null) return null;
            else return new(placedSupport);
        }

        public static IEqualityComparer<MirrorPlacedSupportDTO> GetComparer()
        {
            return new MirrorPlacedSupportDTOEqualityComparer();
        }
    }
    public class MirrorPlacedSupportDTOEqualityComparer : IEqualityComparer<MirrorPlacedSupportDTO>
    {
        private readonly MirrorElementShortDTOBaseEqualityComparer baseComparer = new();
        private readonly MirrorSupportInfoEqualityComparer supportInfoComparer = new(false);
        private readonly ShapeInfoEqualityComparer shapeComparer = new(false);

        public bool Equals(MirrorPlacedSupportDTO? x, MirrorPlacedSupportDTO? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return baseComparer.Equals(x, y)
                && supportInfoComparer.Equals(x!.Support, y!.Support)
                && x.RefFinishElementId == y.RefFinishElementId
                && shapeComparer.Equals(x.SupportRearShape, y.SupportRearShape)
                && shapeComparer.Equals(x.SupportFrontShape, y.SupportFrontShape)
                && shapeComparer.Equals(x.SupportSideShape, y.SupportSideShape)
                && shapeComparer.Equals(x.FormedBoundary, y.FormedBoundary);
        }

        public int GetHashCode([DisallowNull] MirrorPlacedSupportDTO obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
