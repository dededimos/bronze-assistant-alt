using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using ShapesLibrary;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.Sandblasts
{
    public class CircularSandblast : MirrorSandblastInfo, IDeepClonable<CircularSandblast>
    {
        public double? DistanceFromEdge { get; set; }
        public double? DistanceFromCenter { get; set; }
        public CircularSandblast()
        {
            SandblastType = MirrorSandblastType.CircularSandblast;
        }
        public override CircularSandblast GetDeepClone()
        {
            var clone = (CircularSandblast)MemberwiseClone();
            return clone;
        }
        public override CircleRingInfo GetShapeInfo(ShapeInfo parent)
        {
            var parentBox = parent.GetBoundingBox();

            //If both are null make it as long as the min distance of the Mirror(this way it fits in all shapes)
            double radius = Math.Min(parentBox.Length, parentBox.Height) * 0.5;
            if (DistanceFromEdge is not null)
            {
                //Find the Smallest Circle if distance from edge
                radius -= DistanceFromEdge.Value;
            }
            else if (DistanceFromCenter is not null)
            {
                radius = DistanceFromCenter.Value;
            }
            return new CircleRingInfo(radius, Thickness, parentBox.LocationX, parentBox.LocationY);
        }
    }
    public class CircularSandblastEqualityComparer : IEqualityComparer<CircularSandblast>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public CircularSandblastEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new(disregardCollisionDistances);
        }

        private readonly MirrorSandblastInfoBaseEqualityComparer baseComparer;

        public bool Equals(CircularSandblast? x, CircularSandblast? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.DistanceFromEdge == y!.DistanceFromEdge &&
                x.DistanceFromCenter == y.DistanceFromCenter;
        }

        public int GetHashCode([DisallowNull] CircularSandblast obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj), obj.DistanceFromEdge, obj.DistanceFromCenter);
        }
    }
}

