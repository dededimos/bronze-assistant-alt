
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.MirrorModules
{
    public class BluetoothModuleInfo : MirrorModuleInfo, IRateableIP, IPowerable, IWithButtonRegulation, IMirrorPositionable, IDeepClonable<BluetoothModuleInfo>
    {
        public double Watt { get; set; }
        public RectangleInfo Speaker1Dimensions { get; set; } = new(0, 0);
        public RectangleInfo Speaker2Dimensions { get; set; } = new(0, 0);
        public IPRating IP { get; set; } = new();
        public bool NeedsTouchButton { get; set; }
        public double MinDistanceFromSupport { get; set; }
        public double MinDistanceFromSandblast { get; set; }
        public double MinDistanceFromOtherModules { get; set; }
        public BluetoothModuleInfo()
        {
            ModuleType = MirrorModuleType.BluetoothModuleType;
        }
        public double GetEnergyConsumption()
        {
            return Watt;
        }

        public double GetTransformerNominalPower()
        {
            return GetEnergyConsumption();
        }

        public override BluetoothModuleInfo GetDeepClone()
        {
            var clone = (BluetoothModuleInfo)this.MemberwiseClone();
            clone.Speaker1Dimensions = this.Speaker1Dimensions.GetDeepClone();
            clone.Speaker2Dimensions = this.Speaker2Dimensions.GetDeepClone();
            clone.IP = this.IP.GetDeepClone();
            return clone;
        }

        public void SetPosition(PointXY point, PointXY parentCenter)
        {
            Speaker1Dimensions.LocationX = point.X;
            Speaker1Dimensions.LocationY = point.Y;
            //need to find the mirrord point to the Line X= parentCenterX (the Y Axis)
            var mirroredPointToYaxis = MathCalculations.Points.FindMirrorPointToYAxis(point, parentCenter.X);
            Speaker2Dimensions.LocationX = mirroredPointToYaxis.X;
            Speaker2Dimensions.LocationY = mirroredPointToYaxis.Y;
        }

        /// <summary>
        /// Returns the Position of the Bluetooth Module , which matches the position of the Speaker1
        /// </summary>
        /// <returns></returns>
        public PointXY GetPosition()
        {
            return Speaker1Dimensions.GetLocation();
        }
    }
    public class BluetoothModuleInfoEqualityComparer : IEqualityComparer<BluetoothModuleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public BluetoothModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
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

        public bool Equals(BluetoothModuleInfo? x, BluetoothModuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Watt == y!.Watt &&
                rectComparer.Equals(x.Speaker1Dimensions, y.Speaker1Dimensions) &&
                rectComparer.Equals(x.Speaker2Dimensions, y.Speaker2Dimensions) &&
                ipComparer.Equals(x.IP, y.IP) &&
                x.NeedsTouchButton == y.NeedsTouchButton &&
                (disregardCollisionDistances ||
                    (x.MinDistanceFromSupport == y.MinDistanceFromSupport &&
                    x.MinDistanceFromSandblast == y.MinDistanceFromSandblast &&
                    x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules));
        }

        public int GetHashCode([DisallowNull] BluetoothModuleInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }


}
