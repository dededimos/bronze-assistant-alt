using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.MirrorModules
{
    public class MirrorBackLidModuleInfo : MirrorModuleInfo, IMirrorPositionable, IDeepClonable<MirrorBackLidModuleInfo>
    {
        public RectangleInfo LidDimensions { get; set; } = new(0, 0);
        public double MinDistanceFromSupport { get; set; }
        public double MinDistanceFromSandblast { get; set; }
        public double MinDistanceFromOtherModules { get; set; }
        public MirrorBackLidModuleInfo()
        {
            ModuleType = MirrorModuleType.MirrorBackLidModuleType;
        }
        public override MirrorBackLidModuleInfo GetDeepClone()
        {
            var clone = (MirrorBackLidModuleInfo)this.MemberwiseClone();
            clone.LidDimensions = this.LidDimensions.GetDeepClone();
            return clone;
        }

        public void SetPosition(PointXY point, PointXY parentCenter)
        {
            LidDimensions.LocationX = point.X;
            LidDimensions.LocationY = point.Y;
        }

        /// <summary>
        /// Returns the Position of the Lid Rectangle (always the center of the Mirror)
        /// </summary>
        /// <returns></returns>
        public PointXY GetPosition()
        {
            return LidDimensions.GetLocation();
        }
    }
    public class MirrorBackLidModuleInfoEqualityComparer : IEqualityComparer<MirrorBackLidModuleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public MirrorBackLidModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            this.disregardCollisionDistances = disregardCollisionDistances;
            baseComparer = new();
            rectComparer = new(disregardCollisionDistances);
        }

        private readonly MirrorModuleInfoBaseEqualityComparer baseComparer;
        private readonly RectangleInfoEqualityComparer rectComparer;
        private readonly bool disregardCollisionDistances;

        public bool Equals(MirrorBackLidModuleInfo? x, MirrorBackLidModuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                rectComparer.Equals(x!.LidDimensions, y!.LidDimensions) &&
                (disregardCollisionDistances ||
                   (x.MinDistanceFromSupport == y.MinDistanceFromSupport &&
                    x.MinDistanceFromSandblast == y.MinDistanceFromSandblast &&
                    x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules));
        }

        public int GetHashCode([DisallowNull] MirrorBackLidModuleInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

}
