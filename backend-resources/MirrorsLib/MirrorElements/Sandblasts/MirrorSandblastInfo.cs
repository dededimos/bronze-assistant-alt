using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Sandblasts
{
    public abstract class MirrorSandblastInfo : IDeepClonable<MirrorSandblastInfo>, IEqualityComparerCreator<MirrorSandblastInfo>
    {
        public MirrorSandblastType SandblastType { get; protected set; }
        public double Thickness { get; set; }
        /// <summary>
        /// Weather its constraining the modules inside it or lets them also outside
        /// Defines the bounding box with that of the parent Mirror and/or Frame
        /// </summary>
        public bool IsModulesBoundary { get; set; }
        /// <summary>
        /// Weather it constraints the supports inside it or the supports constrain the sandblast
        /// Defines the bounding box with that of the parent Mirror and/or Frame
        /// </summary>
        public bool IsSupportsBoundary { get; set; }
        public double MinDistanceFromSupport { get; set; }
        public double MinDistanceFromOtherModules { get; set; }
        public abstract MirrorSandblastInfo GetDeepClone();
        /// <summary>
        /// Returns the ShapeInfo of the Sandblast
        /// </summary>
        /// <param name="parent">The Parent Of the Sandblast placement</param>
        /// <returns></returns>
        public abstract ShapeInfo GetShapeInfo(ShapeInfo parent);

        public static MirrorSandblastInfo Undefined() => new UndefinedSandblastInfo();

        public static IEqualityComparer<MirrorSandblastInfo> GetComparer()
        {
            return new MirrorSandblastInfoEqualityComparer();
        }
    }
    public class MirrorSandblastInfoEqualityComparer : IEqualityComparer<MirrorSandblastInfo>
    {
        private bool disregardCollisionDistances;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to not compare Collision Distances</param>
        public MirrorSandblastInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            this.disregardCollisionDistances = disregardCollisionDistances;
        }

        public bool Equals(MirrorSandblastInfo? x, MirrorSandblastInfo? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            if (x.GetType() != y.GetType()) return false;

            switch (x)
            {
                case LineSandblast line:
                    var lineComparer = new LineSandblastEqualityComparer();
                    return lineComparer.Equals(line, (LineSandblast)y);
                case TwoLineSandblast twoLine:
                    var twoLineComparer = new TwoLineSandblastEqualityComparer();
                    return twoLineComparer.Equals(twoLine, (TwoLineSandblast)y);
                case HoledRectangleSandblast holedRect:
                    var holedRectComparer = new HoledRectangleSandblastEqualityComparer();
                    return holedRectComparer.Equals(holedRect, (HoledRectangleSandblast)y);
                case CircularSandblast circleSand:
                    var circComparer = new CircularSandblastEqualityComparer();
                    return circComparer.Equals(circleSand, (CircularSandblast)y);
                case UndefinedSandblastInfo und:
                    return true;
                default:
                    throw new NotSupportedException($"The Provided {nameof(MirrorSandblastInfo)} type is not Supported for Equality Comparison: {x.GetType().Name}");
            }
        }

        public int GetHashCode([DisallowNull] MirrorSandblastInfo obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            switch (obj)
            {
                case LineSandblast line:
                    var lineComparer = new LineSandblastEqualityComparer();
                    return lineComparer.GetHashCode(line);
                case TwoLineSandblast twoLine:
                    var twoLineComparer = new TwoLineSandblastEqualityComparer();
                    return twoLineComparer.GetHashCode(twoLine);
                case HoledRectangleSandblast holedRect:
                    var holedRectComparer = new HoledRectangleSandblastEqualityComparer();
                    return holedRectComparer.GetHashCode(holedRect);
                case CircularSandblast circleSand:
                    var circComparer = new CircularSandblastEqualityComparer();
                    return circComparer.GetHashCode(circleSand);
                case UndefinedSandblastInfo und:
                    return 17;
                default:
                    throw new NotSupportedException($"The Provided {nameof(MirrorSandblastInfo)} type is not Supported for Hash Code Generation: {obj.GetType().Name}");
            }
        }
    }
    public class MirrorSandblastInfoBaseEqualityComparer : IEqualityComparer<MirrorSandblastInfo>
    {
        private bool disregardCollisionDistances;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to not compare Collision Distances</param>
        public MirrorSandblastInfoBaseEqualityComparer(bool disregardCollisionDistances)
        {
            this.disregardCollisionDistances = disregardCollisionDistances;
        }

        public bool Equals(MirrorSandblastInfo? x, MirrorSandblastInfo? y)
        {
            if (ReferenceEquals(x, y)) return true; //If both null or of the same Ref
            if (x is null || y is null) return false; // At least one null and the other not null (both cannot be as its excluded from the ref Equals)

            return x.Thickness == y.Thickness &&
                x.SandblastType == y.SandblastType &&
                x.IsModulesBoundary == y.IsModulesBoundary &&
                x.IsSupportsBoundary == y.IsSupportsBoundary &&
                (disregardCollisionDistances ||
                    x.MinDistanceFromSupport == y.MinDistanceFromSupport && x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules);
        }

        public int GetHashCode([DisallowNull] MirrorSandblastInfo obj)
        {
            int hash = HashCode.Combine(obj.Thickness, obj.SandblastType, obj.IsModulesBoundary, obj.IsSupportsBoundary);
            if (!disregardCollisionDistances)
            {
                hash = HashCode.Combine(hash, obj.MinDistanceFromSupport, obj.MinDistanceFromOtherModules);
            }
            return hash;
        }
    }



}

