
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements.MirrorExtras;
using ShapesLibrary;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.MirrorModules
{
    public class ResistancePadModuleInfo : MirrorModuleInfo, IMirrorPositionable, IWithButtonRegulation, IPowerable , IRateableIP ,IDeepClonable<ResistancePadModuleInfo>
    {
        public double Watt { get; set; }
        public ShapeInfo DemisterDimensions { get; set; } = RectangleInfo.ZeroRectangle();
        public IPRating IP { get; set; } = new();
        public bool NeedsTouchButton { get; set; }
        public double MinDistanceFromSupport { get; set; }
        public double MinDistanceFromSandblast { get; set; }
        public double MinDistanceFromOtherModules { get; set; }
        public ResistancePadModuleInfo()
        {
            ModuleType = MirrorModuleType.ResistancePadModuleType;
        }
        public double GetEnergyConsumption()
        {
            return Watt;
        }

        public double GetTransformerNominalPower()
        {
            return 0;
        }
        public override ResistancePadModuleInfo GetDeepClone()
        {
            var clone = (ResistancePadModuleInfo)this.MemberwiseClone();
            clone.DemisterDimensions = this.DemisterDimensions.GetDeepClone();
            clone.IP = this.IP.GetDeepClone();
            return clone;
        }

        public void SetPosition(PointXY point, PointXY parentCenter)
        {
            DemisterDimensions.LocationX = point.X;
            DemisterDimensions.LocationY = point.Y;
        }

        /// <summary>
        /// Returns the Position of the Demister's Center
        /// </summary>
        /// <returns></returns>
        public PointXY GetPosition()
        {
            return DemisterDimensions.GetLocation();
        }
    }
    public class ResistancePadModuleInfoEqualityComparer : IEqualityComparer<ResistancePadModuleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public ResistancePadModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            this.disregardCollisionDistances = disregardCollisionDistances;
            baseComparer = new();
            shapeComparer = new(disregardCollisionDistances);
            ipComparer = new();
        }

        private readonly bool disregardCollisionDistances;
        private readonly MirrorModuleInfoBaseEqualityComparer baseComparer;
        private readonly ShapeInfoEqualityComparer shapeComparer;
        private readonly IPRatingEqualityComparer ipComparer;

        public bool Equals(ResistancePadModuleInfo? x, ResistancePadModuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Watt == y!.Watt &&
                ipComparer.Equals(x.IP, y.IP) &&
                shapeComparer.Equals(x.DemisterDimensions, y.DemisterDimensions) &&
                x.NeedsTouchButton == y.NeedsTouchButton &&
                (disregardCollisionDistances ||
                   (x.MinDistanceFromSupport == y.MinDistanceFromSupport &&
                    x.MinDistanceFromSandblast == y.MinDistanceFromSandblast &&
                    x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules));
        }

        public int GetHashCode([DisallowNull] ResistancePadModuleInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
