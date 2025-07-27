using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using MirrorsLib.Services.PositionService;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsRepositoryMongoDB.Entities
{
    public class MirrorPlacedElementPositionDTO : MirrorElementShortDTO, IDeepClonable<MirrorPlacedElementPositionDTO> , IEqualityComparerCreator<MirrorPlacedElementPositionDTO>
    {
        public MirrorPlacedElementPositionDTO()
        {
            
        }
        public MirrorPlacedElementPositionDTO(MirrorElementPosition position) : base(position)
        {
            Instructions = position.Instructions;
        }
        public PositionInstructionsBase Instructions { get; set; } = PositionInstructionsBase.UndefinedInstructions();

        public override MirrorPlacedElementPositionDTO GetDeepClone()
        {
            var clone = (MirrorPlacedElementPositionDTO) base.GetDeepClone();
            clone.Instructions = this.Instructions.GetDeepClone();
            return clone;
        }

        public MirrorElementPosition ToMirrorElementPosition(IMirrorsDataProvider dataProvider)
        {
            //Get the Photos from the Default Element
            var defaultPosition = dataProvider.GetPosition(this.DefaultElementRefId);
            var elementInfo = this.GetBaseElementInfoWithoutPhotos();

            elementInfo.PhotoUrl = defaultPosition.PhotoUrl;
            elementInfo.PhotoUrl2 = defaultPosition.PhotoUrl2;
            elementInfo.IconUrl = defaultPosition.IconUrl;

            return new MirrorElementPosition(elementInfo, Instructions.GetDeepClone());
        }
        public static IEqualityComparer<MirrorPlacedElementPositionDTO> GetComparer()
        {
            return new MirrorPlacedElementPositionDTOEqualityComparer();
        }
    }
    public class MirrorPlacedElementPositionDTOEqualityComparer : IEqualityComparer<MirrorPlacedElementPositionDTO>
    {
        private readonly MirrorElementShortDTOBaseEqualityComparer baseComparer = new();
        private readonly PositionInstructionsBaseEqualityComparer instructionsComparer = new();

        public bool Equals(MirrorPlacedElementPositionDTO? x, MirrorPlacedElementPositionDTO? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)
            return baseComparer.Equals(x, y)
                && instructionsComparer.Equals(x!.Instructions, y!.Instructions);
        }

        public int GetHashCode([DisallowNull] MirrorPlacedElementPositionDTO obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
