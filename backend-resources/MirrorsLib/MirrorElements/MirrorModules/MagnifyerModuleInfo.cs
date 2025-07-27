
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.MirrorModules
{
    public class MagnifierModuleInfo : MirrorModuleInfo, IMirrorPositionable  , IDeepClonable<MagnifierModuleInfo>
   {
        public CircleInfo MagnifierDimensions { get; set; } = new(0);
        public CircleInfo VisibleMagnifierDimensions { get; set; } = new(0);
        public double Magnification { get; set; }
        public double MinDistanceFromSupport { get; set; }
        public double MinDistanceFromSandblast { get; set; }
        public double MinDistanceFromOtherModules { get; set; }
        public MagnifierModuleInfo()
        {
            ModuleType = MirrorModuleType.MagnifierModuleType;
        }
        public override MagnifierModuleInfo GetDeepClone()
        {
            var clone = (MagnifierModuleInfo)this.MemberwiseClone();
            clone.MagnifierDimensions = this.MagnifierDimensions.GetDeepClone();
            clone.VisibleMagnifierDimensions = this.VisibleMagnifierDimensions.GetDeepClone();
            return clone;
        }

        /// <summary>
        /// Sets the Position of the Magnifier , the parents center is  Only used in Items with Speakers (Bluetooth and Screens)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="parentCenter"></param>
        public virtual void SetPosition(PointXY point, PointXY parentCenter)
        {
            MagnifierDimensions.LocationX = point.X;
            MagnifierDimensions.LocationY = point.Y;
            VisibleMagnifierDimensions.LocationX = point.X;
            VisibleMagnifierDimensions.LocationY = point.Y;
        }

        public PointXY GetPosition()
        {
            return MagnifierDimensions.GetLocation();
        }
    }
    public class MagnifierModuleInfoEqualityComparer : IEqualityComparer<MagnifierModuleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public MagnifierModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            this.disregardCollisionDistances = disregardCollisionDistances;
            baseComparer = new();
            circleComparer = new(disregardCollisionDistances);
        }

        private readonly bool disregardCollisionDistances;
        private readonly MirrorModuleInfoBaseEqualityComparer baseComparer;
        private readonly CircleInfoEqualityComparer circleComparer;


        public bool Equals(MagnifierModuleInfo? x, MagnifierModuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                circleComparer.Equals(x!.MagnifierDimensions, y!.MagnifierDimensions) &&
                circleComparer.Equals(x.VisibleMagnifierDimensions, y.VisibleMagnifierDimensions) &&
                x.Magnification == y.Magnification &&
                (disregardCollisionDistances ||
                 (x.MinDistanceFromSupport == y.MinDistanceFromSupport &&
                  x.MinDistanceFromSandblast == y.MinDistanceFromSandblast &&
                  x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules));
        }

        public int GetHashCode([DisallowNull] MagnifierModuleInfo obj)
        {
            int hash = HashCode.Combine(baseComparer.GetHashCode(obj),
                                        circleComparer.GetHashCode(obj.MagnifierDimensions),
                                        circleComparer.GetHashCode(obj.VisibleMagnifierDimensions),
                                        obj.Magnification);
            if (!disregardCollisionDistances)
            {
                hash = HashCode.Combine(hash, obj.MinDistanceFromOtherModules, obj.MinDistanceFromSandblast, obj.MinDistanceFromSupport);
            }
            return hash;
        }
    }
    public class MagnifierSandblastedModuleInfo : MagnifierModuleInfo, IDeepClonable<MagnifierSandblastedModuleInfo>
    {
        public CircleRingInfo SandblastDimensions { get; set; } = new(0,0);
        public MagnifierSandblastedModuleInfo()
        {
            ModuleType = MirrorModuleType.MagnifierSandblastedModuleType;
        }
        public override MagnifierSandblastedModuleInfo GetDeepClone()
        {
            var clone = (MagnifierSandblastedModuleInfo)base.GetDeepClone();
            clone.SandblastDimensions = SandblastDimensions.GetDeepClone();
            return clone;
        }

        /// <summary>
        /// Returns a copy of the Sandblast Dimensions
        /// </summary>
        /// <returns></returns>
        public CircleRingInfo GetSandblastDimensions()
        {
            return SandblastDimensions.GetDeepClone();
        }
        public override void SetPosition(PointXY point, PointXY parentCenter)
        {
            base.SetPosition(point, parentCenter);
            SandblastDimensions.LocationX = point.X;
            SandblastDimensions.LocationY = point.Y;
        }
    }
    public class MagnifierSandblastedModuleInfoEqualityComparer : IEqualityComparer<MagnifierSandblastedModuleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public MagnifierSandblastedModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new(disregardCollisionDistances);
            circleRingComparer = new(disregardCollisionDistances);
        }
        private readonly MagnifierModuleInfoEqualityComparer baseComparer;
        private readonly CircleRingInfoEqualityComparer circleRingComparer;

        public bool Equals(MagnifierSandblastedModuleInfo? x, MagnifierSandblastedModuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                circleRingComparer.Equals(x!.SandblastDimensions, y!.SandblastDimensions);
        }

        public int GetHashCode([DisallowNull] MagnifierSandblastedModuleInfo obj)
        {
            return HashCode.Combine(baseComparer.GetHashCode(obj),circleRingComparer.GetHashCode(obj.SandblastDimensions));
        }
    }
}
