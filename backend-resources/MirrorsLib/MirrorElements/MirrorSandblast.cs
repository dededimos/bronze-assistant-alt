
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements.Sandblasts;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using static MirrorsLib.MirrorElements.Sandblasts.TwoLineSandblastEqualityComparer;

namespace MirrorsLib.MirrorElements
{
    public class MirrorSandblast : MirrorElementBase, IDeepClonable<MirrorSandblast>, IEqualityComparerCreator<MirrorSandblast>
    {
        public MirrorSandblastInfo SandblastInfo { get; set; }

        public MirrorSandblast(IMirrorElement elementInfo, MirrorSandblastInfo sandblastInfo) :base(elementInfo)
        {
            SandblastInfo = sandblastInfo.GetDeepClone();
        }

        public override MirrorSandblast GetDeepClone()
        {
            var clone = (MirrorSandblast)this.MemberwiseClone();
            clone.LocalizedDescriptionInfo = this.LocalizedDescriptionInfo.GetDeepClone();
            clone.SandblastInfo = this.SandblastInfo.GetDeepClone();
            return clone;
        }

        public static MirrorSandblast Undefined() => new(MirrorElementBase.Empty(), MirrorSandblastInfo.Undefined());

        public new static IEqualityComparer<MirrorSandblast> GetComparer()
        {
            return new MirrorSandblastEqualityComparer();
        }
    }
    public class MirrorSandblastEqualityComparer : IEqualityComparer<MirrorSandblast>
    {
        private readonly MirrorElementEqualityComparer elementInfoComparer = new();
        private readonly MirrorSandblastInfoEqualityComparer sandblastInfoComparer = new();

        public bool Equals(MirrorSandblast? x, MirrorSandblast? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return elementInfoComparer.Equals(x, y) &&
                sandblastInfoComparer.Equals(x.SandblastInfo, y.SandblastInfo);
        }

        public int GetHashCode([DisallowNull] MirrorSandblast obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}

