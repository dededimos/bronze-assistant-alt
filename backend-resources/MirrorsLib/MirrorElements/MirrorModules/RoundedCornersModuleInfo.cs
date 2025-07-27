using CommonHelpers;
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.MirrorModules
{
    public class RoundedCornersModuleInfo : MirrorModuleInfo, IDeepClonable<RoundedCornersModuleInfo>
    {
        public double TopLeft { get; set; }
        public double TopRight { get; set; }
        public double BottomLeft { get; set; }
        public double BottomRight { get; set; }
        /// <summary>
        /// Returns True if all the corners have the same radius
        /// </summary>
        public bool HasTotalRadius { get => !HasMixedRadius; }
        /// <summary>
        /// Returns True if at least one corner has a different radius than the others
        /// </summary>
        public bool HasMixedRadius {  get=> double.IsNaN(TotalRadius); }
        public double TotalRadius
        {
            get => CommonVariousHelpers.AreEqual(TopLeft, TopRight, BottomLeft, BottomRight) ? TopLeft : double.NaN;
            set
            {
                if (!CommonVariousHelpers.AreEqual(TopLeft, TopRight, BottomLeft, BottomRight) || TopLeft != value)
                {
                    SetRadius(value);
                }
            }
        }
        public RoundedCornersModuleInfo()
        {
            ModuleType = MirrorModuleType.RoundedCornersModuleType;
        }
        public void SetRadius(double radius)
        {
            TopLeft = radius;
            TopRight = radius;
            BottomLeft = radius;
            BottomRight = radius;
        }

        /// <summary>
        /// Creates a Rounded Corners ModuleInfo from a Rectangle Info
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static RoundedCornersModuleInfo CreateFromRectangle(RectangleInfo rect)
        {
            return new RoundedCornersModuleInfo()
            {
                TopLeft = rect.TopLeftRadius,
                TopRight = rect.TopRightRadius,
                BottomLeft = rect.BottomLeftRadius,
                BottomRight = rect.BottomRightRadius
            };
        }
        public static RoundedCornersModuleInfo ZeroCorners() => new();
        public override RoundedCornersModuleInfo GetDeepClone()
        {
            var clone = (RoundedCornersModuleInfo)this.MemberwiseClone();
            return clone;
        }
    }
    public class RoundedCornersModuleInfoEqualityComparer : IEqualityComparer<RoundedCornersModuleInfo>
    {
        public bool Equals(RoundedCornersModuleInfo? x, RoundedCornersModuleInfo? y)
        {
            //base Comparer
            var baseComparer = new MirrorModuleInfoBaseEqualityComparer();

            return baseComparer.Equals(x, y) &&
                x!.TopRight== y!.TopRight &&
                x.TopLeft == y.TopLeft &&
                x.BottomLeft == y.BottomLeft &&
                x.BottomRight == y.BottomRight;
        }

        public int GetHashCode([DisallowNull] RoundedCornersModuleInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }


}
