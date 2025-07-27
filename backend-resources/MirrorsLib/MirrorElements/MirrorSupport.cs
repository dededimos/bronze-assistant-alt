
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Supports;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements
{
    public class MirrorSupport : MirrorElementBase, IDeepClonable<MirrorSupport>, IEqualityComparerCreator<MirrorSupport>
    {
        public MirrorSupportInfo SupportInfo { get; set; }
        public MirrorFinishElement Finish { get; set; }

        public MirrorSupport(IMirrorElement elementInfo, MirrorSupportInfo supportInfo , MirrorFinishElement finish):base(elementInfo)
        {
            SupportInfo = supportInfo;
            Finish = finish;
        }
        public override MirrorSupport GetDeepClone()
        {
            var clone = (MirrorSupport)this.MemberwiseClone();
            clone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            clone.SupportInfo = this.SupportInfo.GetDeepClone();
            clone.Finish = this.Finish.GetDeepClone();
            return clone;
        }
        
        public static MirrorSupport Undefined() => new(MirrorElementBase.Empty(), MirrorSupportInfo.Undefined(),MirrorFinishElement.EmptyFinish());

        public new static IEqualityComparer<MirrorSupport> GetComparer()
        {
            return new MirrorSupportEqualityComparer();
        }
    }
    public class MirrorSupportEqualityComparer : IEqualityComparer<MirrorSupport>
    {
        private readonly MirrorElementEqualityComparer elementComparer = new();
        private readonly MirrorSupportInfoEqualityComparer supportComparer = new();
        private readonly MirrorFinishElementEqualityComparer finishComparer = new();

        public bool Equals(MirrorSupport? x, MirrorSupport? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return elementComparer.Equals(x, y) &&
                supportComparer.Equals(x.SupportInfo, y.SupportInfo) && 
                finishComparer.Equals(x.Finish,y.Finish) &&
                elementComparer.Equals(x.Finish, y.Finish);
        }

        public int GetHashCode([DisallowNull] MirrorSupport obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

    public enum SupportLengthOption
    {
        Undefined,
        FixedLengthOption,
        AsPercentageOfParentsLengthOption,
    }
    public enum SupportVerticalDistanceOption
    {
        Undefined,
        FixedDistanceFromParentTop,
        FixedDistanceFromParentBottom,
        FixedDistanceFromParentCenterTop,
        FixedDistanceFromParentCenterBottom,
        DistanceFromTopAsPercentageOfParentHeight,
        DistanceFromBottomAsPercentageOfParentHeight,
        DistanceFromCenterTopAsPercentageOfParentHeight,
        DistanceFromCenterBottomAsPercentageOfParentHeight,
    }
    public enum DistanceBetweenSupportsOption
    {
        Undefined,
        FixedDistanceBetweenSupports,
        AsPercentageOfParentLength
    }



}

