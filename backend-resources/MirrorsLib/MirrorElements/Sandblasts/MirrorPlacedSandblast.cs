using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Interfaces;
using ShapesLibrary;
using ShapesLibrary.Services;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Sandblasts
{
    public class MirrorPlacedSandblast : MirrorElementBase, IMirrorPositionable, IDeepClonable<MirrorPlacedSandblast>, IEqualityComparerCreator<MirrorPlacedSandblast>
    {
        public MirrorPlacedSandblast(MirrorSandblast sandblast, ShapeInfo? sandblastShape) : this(sandblast.SandblastInfo, sandblast,sandblastShape) { }
        public MirrorPlacedSandblast(MirrorSandblastInfo sandblastInfo, IMirrorElement elementInfo, ShapeInfo? sandblastShape) :base(elementInfo)
        {
            SandblastInfo = sandblastInfo.GetDeepClone();
            SandblastShape = sandblastShape?.GetDeepClone();
        }

        public MirrorSandblastInfo SandblastInfo { get; private set; }
        /// <summary>
        /// The Shape of the Sandblast
        /// </summary>
        public ShapeInfo? SandblastShape { get; set; }
        public ShapeInfo? FormedBoundary { get; set; }

        public double MinDistanceFromSandblast { get; } = 0; //always zero
        public double MinDistanceFromSupport { get => SandblastInfo.MinDistanceFromSupport; }
        public double MinDistanceFromOtherModules { get => SandblastInfo.MinDistanceFromOtherModules; }

        public void SetPosition(PointXY point, PointXY parentCenter)
        {
            if (SandblastShape is null) return;
            SandblastShape.LocationX = point.X;
            SandblastShape.LocationY = point.Y;
        }

        public PointXY GetPosition()
        {
            return SandblastShape?.GetLocation() ?? new(0,0);
        }

        public override MirrorPlacedSandblast GetDeepClone()
        {
            var clone = (MirrorPlacedSandblast)this.MemberwiseClone();
            clone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            clone.SandblastInfo = this.SandblastInfo.GetDeepClone();
            clone.SandblastShape = this.SandblastShape?.GetDeepClone();
            clone.FormedBoundary = this.FormedBoundary?.GetDeepClone();
            return clone;
        }

        public new static IEqualityComparer<MirrorPlacedSandblast> GetComparer()
        {
            return new MirrorPlacedSandblastEqualityComparer();
        }
    }
    public class MirrorPlacedSandblastEqualityComparer : IEqualityComparer<MirrorPlacedSandblast>
    {
        private readonly MirrorElementEqualityComparer elementInfoComparer = new();
        private readonly MirrorSandblastInfoEqualityComparer sandblastInfoComparer = new();
        private readonly ShapeInfoEqualityComparer shapeComparer = new(false);

        public bool Equals(MirrorPlacedSandblast? x, MirrorPlacedSandblast? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return elementInfoComparer.Equals(x, y) &&
                sandblastInfoComparer.Equals(x.SandblastInfo, y.SandblastInfo) &&
                shapeComparer.Equals(x.SandblastShape,y.SandblastShape) &&
                shapeComparer.Equals(x.FormedBoundary,y.FormedBoundary);
        }

        public int GetHashCode([DisallowNull] MirrorPlacedSandblast obj)
        {
            return HashCode.Combine(elementInfoComparer.GetHashCode(obj),
                                    sandblastInfoComparer.GetHashCode(obj.SandblastInfo),
                                    obj.SandblastShape is null ? 17 : shapeComparer.GetHashCode(obj.SandblastShape),
                                    obj.FormedBoundary is null ? 37 : shapeComparer.GetHashCode(obj.FormedBoundary));
        }
    }
}

