using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.Services.PositionService.PositionInstructionsModels;
using ShapesLibrary;
using ShapesLibrary.Services;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.Services.PositionService
{
    public interface IPositionInstructions : IDeepClonable<IPositionInstructions>
    {
        /// <summary>
        /// The Shape for which these instructions where made for
        /// </summary>
        //MirrorOrientedShape ConcerningShape { get; }
        PositionInstructionsType InstructionsType { get; }
        /// <summary>
        /// Returns the Position inside the specified Shape based on the Provided Instructions
        /// The Returned position is an absolute position and not relative to the Shape
        /// </summary>
        /// <param name="shapeInfo"></param>
        /// <returns></returns>
        public PointXY GetPosition(ShapeInfo shapeInfo);

        public static IPositionInstructions Undefined() => new UndefinedPositionInstructions();
    }

    public class PositionInstructionsBase : IPositionInstructions , IDeepClonable<PositionInstructionsBase> , IEqualityComparerCreator<PositionInstructionsBase>
    {
        //public MirrorOrientedShape ConcerningShape { get; set; }
        public PositionInstructionsType InstructionsType { get; protected set; }

        public static IEqualityComparer<PositionInstructionsBase> GetComparer()
        {
            return new PositionInstructionsBaseEqualityComparer();
        }

        public virtual PositionInstructionsBase GetDeepClone()
        {
            throw new NotSupportedException($"{nameof(PositionInstructionsBase)} does not support any {nameof(GetDeepClone)} Implementations , Derived Classes should implement that method");
        }

        /// <summary>
        /// Returns instructions to set the position of an Item to the Center of its Parent
        /// </summary>
        /// <returns></returns>
        public static PositionInstructionsBoundingBox PositionInstructionsToCenter()
        {
            return new()
            {
                VerticalDistance = 0,
                HorizontalDistance = 0,
                HDistancing = Enums.HorizontalDistancing.FromCenterToRight,
                VDistancing = Enums.VerticalDistancing.FromCenterToBottom,
            };
        }
        public static UndefinedPositionInstructions UndefinedInstructions() => new();

        public virtual PointXY GetPosition(ShapeInfo shapeInfo)
        {
            throw new NotSupportedException($"{nameof(PositionInstructionsBase)} does not support any {nameof(GetPosition)} Implementations , Derived Classes should implement that method");
        }

        IPositionInstructions IDeepClonable<IPositionInstructions>.GetDeepClone()
        {
            return GetDeepClone();
        }
    }
    public class PositionInstructionsBaseEqualityComparer : IEqualityComparer<PositionInstructionsBase>
    {
        public bool Equals(PositionInstructionsBase? x, PositionInstructionsBase? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            if (x.GetType() != y.GetType()) return false;

            switch (x)
            {
                case PositionInstructionsRadial radial:
                    var radialComparer = new PositionInstructionsRadialEqualityComparer();
                    return radialComparer.Equals(radial, (PositionInstructionsRadial)y);
                case PositionInstructionsBoundingBox rect:
                    var rectComparer = new PositionInstructionsBoundingBoxEqualityComparer();
                    return rectComparer.Equals(rect, (PositionInstructionsBoundingBox)y);
                case UndefinedPositionInstructions: return true;
                default:
                    throw new NotSupportedException($"The Provided {nameof(PositionInstructionsBase)} type is not Supported for Equality Comparison : {x.GetType().Name}");
            }
        }

        public int GetHashCode([DisallowNull] PositionInstructionsBase obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
    public class PositionInstructionsBaseBaseEqualityComparer : IEqualityComparer<PositionInstructionsBase>
    {
        public bool Equals(PositionInstructionsBase? x, PositionInstructionsBase? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.InstructionsType == y.InstructionsType;
        }

        public int GetHashCode([DisallowNull] PositionInstructionsBase obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

    public class UndefinedPositionInstructions : PositionInstructionsBase, IDeepClonable<UndefinedPositionInstructions>
    {
        public UndefinedPositionInstructions()
        {
            InstructionsType = PositionInstructionsType.UndefinedInstructions;
        }

        public override UndefinedPositionInstructions GetDeepClone()
        {
            return new();
        }

        public override PointXY GetPosition(ShapeInfo shapeInfo)
        {
            return new(0, 0);
        }

    }
}
