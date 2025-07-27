using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Supports
{
    public abstract class MirrorSupportInfo : IDeepClonable<MirrorSupportInfo>, IEqualityComparerCreator<MirrorSupportInfo>
    {
        public MirrorSupportType SupportType { get; protected set; }
        public double MinDistanceFromSandblast { get; set; }
        public double MinDistanceFromOtherModules { get; set; }
        /// <summary>
        /// Weather it constraints modules of the mirror inside its boundary
        /// The boundary gets definined along with that of the parent mirror and/or Sandblast
        /// </summary>
        public bool IsModulesBoundary { get; set; }
        public abstract MirrorSupportInfo GetDeepClone();
        public static MirrorSupportInfo Undefined() => new UndefinedMirrorSupportInfo();

        public static IEqualityComparer<MirrorSupportInfo> GetComparer()
        {
            return new MirrorSupportInfoEqualityComparer();
        }
    }
    public class MirrorSupportInfoEqualityComparer : IEqualityComparer<MirrorSupportInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to not compare Collision Distances</param>
        public MirrorSupportInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            multiComparer = new(disregardCollisionDistances);
            frameComparer = new(disregardCollisionDistances);
            backFrameComparer = new(disregardCollisionDistances);
        }

        private readonly MirrorMultiSupportsEqualityComparer multiComparer;
        private readonly MirrorVisibleFrameSupportEqualityComparer frameComparer;
        private readonly MirrorBackFrameSupportEqualityComparer backFrameComparer;

        public bool Equals(MirrorSupportInfo? x, MirrorSupportInfo? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            if (x.GetType() != y.GetType()) return false;

            return x switch
            {
                MirrorMultiSupports multi => multiComparer.Equals(multi, (MirrorMultiSupports)y),
                MirrorVisibleFrameSupport frame => frameComparer.Equals(frame, (MirrorVisibleFrameSupport)y),
                MirrorBackFrameSupport backFrame => backFrameComparer.Equals(backFrame, (MirrorBackFrameSupport)y),
                UndefinedMirrorSupportInfo und => true,
                _ => throw new NotSupportedException($"The Provided {nameof(MirrorSupportInfo)} type is not Supported for Equality Comparison : {x.GetType().Name}"),
            };
        }

        public int GetHashCode([DisallowNull] MirrorSupportInfo obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return obj switch
            {
                MirrorMultiSupports multi => multiComparer.GetHashCode(multi),
                MirrorVisibleFrameSupport frame => frameComparer.GetHashCode(frame),
                MirrorBackFrameSupport backFrame => backFrameComparer.GetHashCode(backFrame),
                UndefinedMirrorSupportInfo und => 17,
                _ => throw new NotSupportedException($"The Provided {nameof(MirrorSupportInfo)} type is not Supported for HashCode Generation : {obj.GetType().Name}"),
            };
        }
    }
    public class MirrorSupportInfoBaseEqualityComparer : IEqualityComparer<MirrorSupportInfo>
    {
        private readonly bool disregardCollisionDistances;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to not compare Collision Distances</param>
        public MirrorSupportInfoBaseEqualityComparer(bool disregardCollisionDistances = false)
        {
            this.disregardCollisionDistances = disregardCollisionDistances;
        }

        public bool Equals(MirrorSupportInfo? x, MirrorSupportInfo? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.SupportType == y.SupportType &&
                x.IsModulesBoundary == y.IsModulesBoundary &&
                (disregardCollisionDistances ||
                    x.MinDistanceFromSandblast == y.MinDistanceFromSandblast &&
                     x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules);
        }

        public int GetHashCode([DisallowNull] MirrorSupportInfo obj)
        {
            int hash = HashCode.Combine(obj.SupportType, obj.IsModulesBoundary);
            if (!disregardCollisionDistances)
            {
                hash = HashCode.Combine(hash, obj.MinDistanceFromSandblast, obj.MinDistanceFromOtherModules);
            }
            return hash;
        }
    }


}

