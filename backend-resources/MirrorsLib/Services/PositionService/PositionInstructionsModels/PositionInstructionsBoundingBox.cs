using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.Services.PositionService.Enums;
using ShapesLibrary;
using ShapesLibrary.Enums;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.Services.PositionService.PositionInstructionsModels
{

    public class PositionInstructionsBoundingBox : PositionInstructionsBase,IDeepClonable<PositionInstructionsBoundingBox> 
    {
        public HorizontalDistancing HDistancing { get; set; }
        public double HorizontalDistance { get; set; }
        public VerticalDistancing VDistancing { get; set; }
        public double VerticalDistance { get; set; }

        public PositionInstructionsBoundingBox()
        {
            InstructionsType = PositionInstructionsType.BoundingBoxInstructions;
        }

        public override PositionInstructionsBoundingBox GetDeepClone()
        {
            var clone = (PositionInstructionsBoundingBox)this.MemberwiseClone();
            return clone;
        }

        public override PointXY GetPosition(ShapeInfo shapeInfo)
        {
            var rectangle = shapeInfo.GetBoundingBox();
            //Assuming Origin is the Center of the Rectangle 
            var x = HDistancing switch
            {
                HorizontalDistancing.FromLeftToRight => rectangle.LeftX + HorizontalDistance,
                HorizontalDistancing.FromRightToLeft => rectangle.RightX - HorizontalDistance,
                HorizontalDistancing.FromCenterToLeft => rectangle.LocationX - HorizontalDistance,
                HorizontalDistancing.FromCenterToRight => rectangle.LocationX + HorizontalDistance,
                _ => throw new NotSupportedException($"{nameof(HorizontalDistancing)} value of : {HDistancing} is not supported"),
            };
            var y = VDistancing switch
            {
                VerticalDistancing.FromTopToBottom => rectangle.TopY + VerticalDistance,
                VerticalDistancing.FromBottomToTop => rectangle.BottomY - VerticalDistance,
                VerticalDistancing.FromCenterToTop => rectangle.LocationY - VerticalDistance,
                VerticalDistancing.FromCenterToBottom => rectangle.LocationY + VerticalDistance,
                _ => throw new NotSupportedException($"{nameof(VerticalDistancing)} value of : {VDistancing} is not supported"),
            };
            return new (x, y);
        }
        /// <summary>
        /// Creates Bounding Box position Instructions for placing a <see cref="IMirrorPositionable"/> to the center of the bounding box
        /// </summary>
        /// <returns></returns>
        public static PositionInstructionsBoundingBox BoxCenter()
        {
            return new()
            {
                HDistancing = HorizontalDistancing.FromCenterToRight,
                HorizontalDistance = 0,
                VDistancing = VerticalDistancing.FromCenterToBottom,
                VerticalDistance = 0
            };
        }
    }
    public class PositionInstructionsBoundingBoxEqualityComparer : IEqualityComparer<PositionInstructionsBoundingBox>
    {
        public bool Equals(PositionInstructionsBoundingBox? x, PositionInstructionsBoundingBox? y)
        {
            var baseComparer = new PositionInstructionsBaseBaseEqualityComparer();

            return baseComparer.Equals(x, y) &&
                x!.HDistancing == y!.HDistancing &&
                x.HorizontalDistance == y.HorizontalDistance &&
                x.VDistancing == y.VDistancing &&
                x.VerticalDistance == y.VerticalDistance;
        }

        public int GetHashCode([DisallowNull] PositionInstructionsBoundingBox obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
