
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements.MirrorExtras;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.MirrorModules
{
    public class ScreenModuleInfo : MirrorModuleInfo, IMirrorPositionable, IButtonRegulator,IWithButtonRegulation, IPowerable , IRateableIP ,IDeepClonable<ScreenModuleInfo>
    {
        public double Watt { get; set; }
        public RectangleInfo FrontDimensions { get; set; } = new(0, 0);
        public RectangleInfo RearDimensions { get; set; } = new(0, 0);
        public RectangleInfo Speaker1Dimensions { get; set; } = new(0, 0);
        public RectangleInfo Speaker2Dimensions { get; set; } = new(0, 0);
        public bool HasIntegratedBluetooth { get; set; }
        public IPRating IP { get; set; } = new();
        public bool NeedsTouchButton { get; set; }
        public int NumberOfButtons { get; set; }
        public List<string> RegulatedItems { get; set; } = new();
        public double MinDistanceFromSupport { get; set; }
        public double MinDistanceFromSandblast { get; set; }
        public double MinDistanceFromOtherModules { get; set; }
        public ScreenModuleInfo()
        {
            ModuleType = MirrorModuleType.ScreenModuleType;
        }
        public double GetEnergyConsumption()
        {
            return Watt;
        }

        public double GetTransformerNominalPower()
        {
            return GetEnergyConsumption();
        }
        public override ScreenModuleInfo GetDeepClone()
        {
            var clone = (ScreenModuleInfo)this.MemberwiseClone();
            clone.FrontDimensions = this.FrontDimensions.GetDeepClone();
            clone.RearDimensions = this.RearDimensions.GetDeepClone();
            clone.Speaker1Dimensions = this.Speaker1Dimensions.GetDeepClone();
            clone.Speaker2Dimensions = this.Speaker2Dimensions.GetDeepClone();
            clone.IP = this.IP.GetDeepClone();
            return clone;
        }

        public void SetPosition(PointXY point, PointXY parentCenter)
        {
            FrontDimensions.LocationX = point.X;
            FrontDimensions.LocationY = point.Y;
            RearDimensions.LocationX = point.X;
            RearDimensions.LocationY = point.Y;
            
            Speaker1Dimensions.LocationX = point.X;
            Speaker1Dimensions.LocationY = point.Y;
            //need to find the mirrord point to the Line X= parentCenterX (the Y Axis)
            var mirroredPointToYaxis = MathCalculations.Points.FindMirrorPointToYAxis(point, parentCenter.X);
            Speaker2Dimensions.LocationX = mirroredPointToYaxis.X;
            Speaker2Dimensions.LocationY = mirroredPointToYaxis.Y;
        }

        /// <summary>
        /// Returns the Center's Position of the Screen Modules Main Body
        /// </summary>
        /// <returns></returns>
        public PointXY GetPosition()
        {
            return RearDimensions.GetLocation();
        }
    }
    public class ScreenModuleInfoEqualityComparer : IEqualityComparer<ScreenModuleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public ScreenModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            this.disregardCollisionDistances = disregardCollisionDistances;
            baseComparer = new();
            rectComparer = new(disregardCollisionDistances);
            ipComparer = new();
        }

        private readonly MirrorModuleInfoBaseEqualityComparer baseComparer;
        private readonly RectangleInfoEqualityComparer rectComparer;
        private readonly IPRatingEqualityComparer ipComparer;
        private readonly bool disregardCollisionDistances;

        public bool Equals(ScreenModuleInfo? x, ScreenModuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Watt == y!.Watt &&
                ipComparer.Equals(x.IP, y.IP) &&
                rectComparer.Equals(x.FrontDimensions, y.FrontDimensions) &&
                rectComparer.Equals(x.RearDimensions, y.RearDimensions) &&
                rectComparer.Equals(x.Speaker1Dimensions,y.Speaker1Dimensions) &&
                rectComparer.Equals(x.Speaker2Dimensions, y.Speaker2Dimensions) &&
                x.RegulatedItems.SequenceEqual(y.RegulatedItems) &&
                x.NeedsTouchButton == y.NeedsTouchButton &&
                x.HasIntegratedBluetooth == y.HasIntegratedBluetooth &&
                (disregardCollisionDistances ||
                   (x.MinDistanceFromSupport == y.MinDistanceFromSupport &&
                    x.MinDistanceFromSandblast == y.MinDistanceFromSandblast &&
                    x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules));
        }

        public int GetHashCode([DisallowNull] ScreenModuleInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
