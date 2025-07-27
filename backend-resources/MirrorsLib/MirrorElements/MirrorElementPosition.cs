using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Services.PositionService;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements
{
    public class MirrorElementPosition : MirrorElementBase, IDeepClonable<MirrorElementPosition>
    {
        public PositionInstructionsBase Instructions { get; set; }

        public MirrorElementPosition(IMirrorElement elementInfo, PositionInstructionsBase instructions)
            :base(elementInfo)
        {
            Instructions = instructions.GetDeepClone();
        }

        public override MirrorElementPosition GetDeepClone()
        {
            var clone = (MirrorElementPosition)this.MemberwiseClone();
            clone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            //to remove the compiler ambiguity (because of absence of covariance in interfaces...)
            clone.Instructions = this.Instructions.GetDeepClone();
            return clone;
        }

        /// <summary>
        /// Creates a Default Mirror Element Position , with Instructions to place the Positionable on the Parents Center point
        /// </summary>
        /// <returns></returns>
        public static MirrorElementPosition DefaultPositionElement() => new(MirrorElementBase.DefaultPositionElementInfo(), PositionInstructionsBase.PositionInstructionsToCenter());
        public static MirrorElementPosition NAPositionElement() => new(MirrorElementBase.NAElementInfo(), PositionInstructionsBase.UndefinedInstructions());
    }
    public class MirrorElementPositionEqualityComparer : IEqualityComparer<MirrorElementPosition> 
    {
        private readonly MirrorElementEqualityComparer elementInfoComparer = new();
        private readonly PositionInstructionsBaseEqualityComparer instructionsComparer = new();

        public bool Equals(MirrorElementPosition? x, MirrorElementPosition? y)
        {
            if (ReferenceEquals(x, y))
                return true; // Both are the same instance or both are null

            if (x is null || y is null)
                return false; // One is null, the other is not

            return elementInfoComparer.Equals(x,y) &&
                instructionsComparer.Equals(x.Instructions,y.Instructions);
        }

        public int GetHashCode([DisallowNull] MirrorElementPosition obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
