using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Supports
{
    public class MirrorMultiSupports : MirrorSupportInfo, IDeepClonable<MirrorMultiSupports>
    {
        public MirrorSupportInstructions TopSupportsInstructions { get; set; } = new();
        public MirrorSupportInstructions BottomSupportsInstructions { get; set; } = new();
        public MirrorMultiSupports()
        {
            SupportType = MirrorSupportType.MirrorMultiSupport;
        }
        public override MirrorMultiSupports GetDeepClone()
        {
            var clone = (MirrorMultiSupports)MemberwiseClone();
            clone.TopSupportsInstructions = TopSupportsInstructions.GetDeepClone();
            clone.BottomSupportsInstructions = BottomSupportsInstructions.GetDeepClone();
            return clone;
        }

        public List<ShapeInfo> GetSupportFrontShapes(ShapeInfo parent)
        {
            return new();
        }

        public List<ShapeInfo> GetSupportRearShapes(ShapeInfo parent)
        {
            List<ShapeInfo> supports = new();
            var parentBox = parent.GetBoundingBox();
            var topSupports = TopSupportsInstructions.GetSupportShapes(parentBox);
            var bottomSupports = BottomSupportsInstructions.GetSupportShapes(parentBox);
            supports.AddRange(topSupports);
            supports.AddRange(bottomSupports);
            return supports;
        }
    }
    public class MirrorMultiSupportsEqualityComparer : IEqualityComparer<MirrorMultiSupports>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to not compare Collision Distances</param>
        public MirrorMultiSupportsEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new(disregardCollisionDistances);
            instructionsComparer = new();
        }
        private readonly MirrorSupportInfoBaseEqualityComparer baseComparer;
        private readonly MirrorSupportInstructionsEqualityComparer instructionsComparer;

        public bool Equals(MirrorMultiSupports? x, MirrorMultiSupports? y)
        {
            return baseComparer.Equals(x, y) &&
                instructionsComparer.Equals(x!.TopSupportsInstructions, y!.TopSupportsInstructions) &&
                instructionsComparer.Equals(x.BottomSupportsInstructions, y.BottomSupportsInstructions);
        }

        public int GetHashCode([DisallowNull] MirrorMultiSupports obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), instructionsComparer.GetHashCode(obj.TopSupportsInstructions), instructionsComparer.GetHashCode(obj.BottomSupportsInstructions));
        }
    }
}

