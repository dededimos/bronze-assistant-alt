using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Repositories;
using ShapesLibrary;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorPlacedSandblastDTO : MirrorElementShortDTO, IDeepClonable<MirrorPlacedSandblastDTO> , IEqualityComparerCreator<MirrorPlacedSandblastDTO>
    {
        public MirrorPlacedSandblastDTO()
        {
            
        }
        public MirrorPlacedSandblastDTO(MirrorPlacedSandblast placedSandblast) : base(placedSandblast)
        {
            SandblastInfo = placedSandblast.SandblastInfo;
            SandblastShape = placedSandblast.SandblastShape;
            FormedBoundary = placedSandblast.FormedBoundary;
        }


        public MirrorSandblastInfo SandblastInfo { get; set; } = MirrorSandblastInfo.Undefined();
        public ShapeInfo? SandblastShape { get; set; } 
        public ShapeInfo? FormedBoundary { get; set; }

        public MirrorPlacedSandblast ToPlacedSandblast(IMirrorsDataProvider dataProvider)
        {
            //Get the Photos from the Default Element
            var defaultSandblast = dataProvider.GetSandblasts(this.DefaultElementRefId).FirstOrDefault();
            var elementInfo = this.GetBaseElementInfoWithoutPhotos();

            if (defaultSandblast != null)
            {
                elementInfo.PhotoUrl = defaultSandblast.PhotoUrl;
                elementInfo.PhotoUrl2 = defaultSandblast.PhotoUrl2;
                elementInfo.IconUrl = defaultSandblast.IconUrl;
            }
            MirrorPlacedSandblast sandblast = new(this.SandblastInfo.GetDeepClone(), elementInfo, this.SandblastShape?.GetDeepClone())
            {
                FormedBoundary = this.FormedBoundary?.GetDeepClone()
            };
            return sandblast;
        }
        public override MirrorPlacedSandblastDTO GetDeepClone()
        {
            var clone = (MirrorPlacedSandblastDTO)base.GetDeepClone();
            clone.SandblastInfo = this.SandblastInfo.GetDeepClone();
            clone.SandblastShape = this.SandblastShape?.GetDeepClone();
            clone.FormedBoundary = this.FormedBoundary?.GetDeepClone();
            return clone;
        }

        public static MirrorPlacedSandblastDTO? Create(MirrorPlacedSandblast? placedSandblast)
        {
            if (placedSandblast is null) return null;
            else return new(placedSandblast);
        }

        public static IEqualityComparer<MirrorPlacedSandblastDTO> GetComparer()
        {
            return new MirrorPlacedSandblastDTOEqualityComparer();
        }
    }

    public class MirrorPlacedSandblastDTOEqualityComparer : IEqualityComparer<MirrorPlacedSandblastDTO>
    {
        private readonly MirrorElementShortDTOBaseEqualityComparer baseComparer = new();
        private readonly MirrorSandblastInfoEqualityComparer sandblastInfoComparer = new(false);
        private readonly ShapeInfoEqualityComparer shapeComparer = new(false);

        public bool Equals(MirrorPlacedSandblastDTO? x, MirrorPlacedSandblastDTO? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return baseComparer.Equals(x, y)
                && sandblastInfoComparer.Equals(x!.SandblastInfo, y!.SandblastInfo)
                && shapeComparer.Equals(x.SandblastShape, y.SandblastShape)
                && shapeComparer.Equals(x.FormedBoundary, y.FormedBoundary);
        }

        public int GetHashCode([DisallowNull] MirrorPlacedSandblastDTO obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
