using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements.MirrorModules;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.MirrorExtras
{
    public class MirrorLampModuleInfo : MirrorModuleInfo, IMirrorPositionable, IWithButtonRegulation, IPowerable, IRateableIP , IDeepClonable<MirrorLampModuleInfo>
    {
        public double Watt { get; set; }
        public IPRating IP { get; set; } = new();
        public RectangleInfo LampBodyDimensions { get; set; } = new(0, 0);
        public RectangleInfo LampSupportDimensions { get; set; } = new(0, 0);
        public double SupportOffsetXFromBody { get; set; }
        public double SupportOffsetYFromBody { get; set; }
        public int Kelvin { get; set; }
        public int Lumen { get; set; }
        public double TotalLength { get; set; }
        public double TotalHeight { get; set; }
        public double TotalDepth { get; set; }
        public bool NeedsTouchButton { get; set; }
        public double MinDistanceFromSupport { get; set; }
        public double MinDistanceFromSandblast { get; set; }
        public double MinDistanceFromOtherModules { get; set; }
        public MirrorLampModuleInfo()
        {
            ModuleType = MirrorModuleType.MirrorLampModuleType;
        }
        public double GetEnergyConsumption()
        {
            return Watt;
        }

        public double GetTransformerNominalPower()
        {
            return GetEnergyConsumption();
        }
        public override MirrorLampModuleInfo GetDeepClone()
        {
            var clone = (MirrorLampModuleInfo)this.MemberwiseClone();
            clone.LampBodyDimensions = this.LampBodyDimensions.GetDeepClone();
            clone.LampSupportDimensions = this.LampSupportDimensions.GetDeepClone();
            clone.IP = IP.GetDeepClone();
            return clone;
        }

        public void SetPosition(PointXY point, PointXY parentCenter)
        {
            LampBodyDimensions.LocationX = point.X;
            LampBodyDimensions.LocationY = point.Y;
            LampSupportDimensions.LocationX = point.X + SupportOffsetXFromBody;
            LampSupportDimensions.LocationY = point.Y + SupportOffsetYFromBody;
        }

        /// <summary>
        /// Returns the Position of the Center of the Lamp Body
        /// </summary>
        /// <returns></returns>
        public PointXY GetPosition()
        {
            return LampBodyDimensions.GetLocation();
        }
    }
    public class MirrorLampModuleInfoEqualityComparer : IEqualityComparer<MirrorLampModuleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public MirrorLampModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new();
            rectComparer = new(disregardCollisionDistances);
            ipComparer = new();
            this.disregardCollisionDistances = disregardCollisionDistances;
        }

        private readonly MirrorModuleInfoBaseEqualityComparer baseComparer;
        private readonly RectangleInfoEqualityComparer rectComparer;
        private readonly IPRatingEqualityComparer ipComparer;
        private readonly bool disregardCollisionDistances;

        public bool Equals(MirrorLampModuleInfo? x, MirrorLampModuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Watt == y!.Watt &&
                ipComparer.Equals(x.IP, y.IP) &&
                rectComparer.Equals(x.LampBodyDimensions, y.LampBodyDimensions) &&
                rectComparer.Equals(x.LampSupportDimensions, y.LampSupportDimensions) &&
                x.SupportOffsetXFromBody == y.SupportOffsetXFromBody &&
                x.SupportOffsetYFromBody == y.SupportOffsetYFromBody &&
                x.Kelvin == y.Kelvin &&
                x.Lumen == y.Lumen &&
                x.TotalLength == y.TotalLength &&
                x.TotalHeight == y.TotalHeight &&
                x.TotalDepth == y.TotalDepth &&
                x.NeedsTouchButton == y.NeedsTouchButton &&
                (disregardCollisionDistances ||
                   (x.MinDistanceFromSupport == y.MinDistanceFromSupport &&
                    x.MinDistanceFromSandblast == y.MinDistanceFromSandblast &&
                    x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules));
        }

        public int GetHashCode([DisallowNull] MirrorLampModuleInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }

}
