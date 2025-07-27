using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Interfaces;
using ShapesLibrary;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Supports
{
    public class MirrorPlacedSupport : MirrorElementBase, IMirrorPositionable, IDeepClonable<MirrorPlacedSupport>, IEqualityComparerCreator<MirrorPlacedSupport>
    {
        public MirrorSupportInfo SupportInfo { get; private set; }
        public MirrorFinishElement Finish { get; set; } = MirrorFinishElement.EmptyFinish();
        public ShapeInfo? SupportRearShape { get; set; }
        public ShapeInfo? SupportFrontShape { get; set; }
        public ShapeInfo? SupportSideShape { get; set; }
        public ShapeInfo? FormedBoundary { get; set; }

        public double MinDistanceFromSupport { get; } = 0; //Always zero;
        public double MinDistanceFromSandblast { get => SupportInfo.MinDistanceFromSandblast; }
        public double MinDistanceFromOtherModules { get => SupportInfo.MinDistanceFromOtherModules; }

        public MirrorPlacedSupport(MirrorSupport support, ShapeInfo? supportShape, ShapeInfo? supportFrontShape) : this(support.SupportInfo, support, support.Finish, supportShape, supportFrontShape) { }
        public MirrorPlacedSupport(MirrorSupportInfo supportInfo, IMirrorElement elementInfo, MirrorFinishElement finish, ShapeInfo? supportRearShape, ShapeInfo? supportFrontShape)
            : base(elementInfo)
        {
            SupportInfo = supportInfo.GetDeepClone();
            Finish = finish;
            SupportRearShape = supportRearShape?.GetDeepClone();
            SupportFrontShape = supportFrontShape?.GetDeepClone();
        }

        public PointXY GetPosition()
        {
            return SupportRearShape?.GetLocation() ?? new(0, 0);
        }

        public void SetPosition(PointXY point, PointXY parentCenter)
        {
            if (SupportRearShape is null) return;
            SupportRearShape.LocationX = point.X;
            SupportRearShape.LocationY = point.Y;
        }

        public override MirrorPlacedSupport GetDeepClone()
        {
            var clone = (MirrorPlacedSupport)this.MemberwiseClone();
            clone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            clone.SupportRearShape = this.SupportRearShape?.GetDeepClone();
            clone.SupportFrontShape = this.SupportFrontShape?.GetDeepClone();
            clone.SupportSideShape = this.SupportSideShape?.GetDeepClone();
            clone.FormedBoundary = this.FormedBoundary?.GetDeepClone();
            clone.SupportInfo = this.SupportInfo.GetDeepClone();
            clone.Finish = this.Finish.GetDeepClone();
            return clone;
        }

        public new static IEqualityComparer<MirrorPlacedSupport> GetComparer()
        {
            return new MirrorPlacedSupportEqualityComparer();
        }
    }
    public class MirrorPlacedSupportEqualityComparer : IEqualityComparer<MirrorPlacedSupport>
    {
        private readonly MirrorElementEqualityComparer elementInfoComparer = new();
        private readonly MirrorSupportInfoEqualityComparer supportInfoComparer = new(true);
        private readonly MirrorFinishElementEqualityComparer finishComparer = new();
        private readonly ShapeInfoEqualityComparer shapeComparer = new();
        public bool Equals(MirrorPlacedSupport? x, MirrorPlacedSupport? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return elementInfoComparer.Equals(x, y) &&
                supportInfoComparer.Equals(x.SupportInfo, y.SupportInfo) &&
                finishComparer.Equals(x.Finish, y.Finish) &&
                shapeComparer.Equals(x.SupportRearShape, y.SupportRearShape) &&
                shapeComparer.Equals(x.SupportFrontShape, y.SupportFrontShape) &&
                shapeComparer.Equals(x.SupportSideShape, y.SupportSideShape) &&
                shapeComparer.Equals(x.FormedBoundary, y.FormedBoundary);
        }

        public int GetHashCode([DisallowNull] MirrorPlacedSupport obj)
        {
            return HashCode.Combine(elementInfoComparer.GetHashCode(obj),
                                    supportInfoComparer.GetHashCode(obj.SupportInfo),
                                    finishComparer.GetHashCode(obj.Finish),
                                    obj.SupportRearShape is null ? 17 : shapeComparer.GetHashCode(obj.SupportRearShape),
                                    obj.SupportFrontShape is null ? 37 : shapeComparer.GetHashCode(obj.SupportFrontShape),
                                    obj.FormedBoundary is null ? 43 : shapeComparer.GetHashCode(obj.FormedBoundary));
        }
    }

}

