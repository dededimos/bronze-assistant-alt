using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Sandblasts
{
    public class HoledRectangleSandblast : MirrorSandblastInfo, IDeepClonable<HoledRectangleSandblast>
    {
        public double EdgeDistance { get; set; }
        /// <summary>
        /// The Corner Radius if it does not follow the parents corner radius
        /// </summary>
        public double CornerRadius { get; set; }
        /// <summary>
        /// If the Parent Glass has Corner Radius , the Sandblast will follow it , depending on the distance it has from the parent
        /// </summary>
        public bool FollowsRectangleGlassCornerRadius { get; set; }
        public HoledRectangleSandblast()
        {
            SandblastType = MirrorSandblastType.HoledRectangleSandblast;
        }
        public override HoledRectangleSandblast GetDeepClone()
        {
            var clone = (HoledRectangleSandblast)MemberwiseClone();
            return clone;
        }
        public override RectangleRingInfo GetShapeInfo(ShapeInfo parent)
        {
            var parentBox = parent.GetBoundingBox();
            RectangleRingInfo sandblast = new(
            parentBox.Length - EdgeDistance * 2,
            parentBox.Height - EdgeDistance * 2,
            Thickness,
            CornerRadius,
            parentBox.LocationX,
            parentBox.LocationY);

            return sandblast;
        }
    }
    public class HoledRectangleSandblastEqualityComparer : IEqualityComparer<HoledRectangleSandblast>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public HoledRectangleSandblastEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new(disregardCollisionDistances);
        }

        private readonly MirrorSandblastInfoBaseEqualityComparer baseComparer;

        public bool Equals(HoledRectangleSandblast? x, HoledRectangleSandblast? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.EdgeDistance == y!.EdgeDistance &&
                x.CornerRadius == y.CornerRadius &&
                x.FollowsRectangleGlassCornerRadius == y.FollowsRectangleGlassCornerRadius;
        }

        public int GetHashCode([DisallowNull] HoledRectangleSandblast obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.EdgeDistance,obj.CornerRadius,obj.FollowsRectangleGlassCornerRadius);
        }
    }
}

