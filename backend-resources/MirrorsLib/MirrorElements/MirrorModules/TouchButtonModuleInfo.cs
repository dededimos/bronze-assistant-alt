
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using ShapesLibrary;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.MirrorModules
{
    public class TouchButtonModuleInfo : MirrorModuleInfo, IMirrorPositionable, IButtonRegulator, IPowerable , IRateableIP , IDeepClonable<TouchButtonModuleInfo>
    {
        public double Watt { get; set; }
        public RectangleInfo RearDimensions { get; set; } = RectangleInfo.ZeroRectangle();
        public ShapeInfo FrontDimensionsButton { get; set; } = RectangleRingInfo.RectangleRingZero();

        /// <summary>
        /// The Offset X of the first Button from the Side of its Container 
        /// </summary>
        public double ButtonOffsetXFromRearRectangle { get; set; }
        /// <summary>
        /// The Offset Y of the first Button from the Side of its Container 
        /// </summary>
        public double ButtonOffsetYFromRearRectangle { get; set; }

        /// <summary>
        /// The Number of touch Buttons on the Module
        /// </summary>
        public int NumberOfButtons { get; set; }
        /// <summary>
        /// The Distance of the Touch Buttons with Each Other
        /// </summary>
        public double TouchButtonsDistance { get; set; }

        public IPRating IP { get; set; } = new();
        public List<string> RegulatedItems { get; set; } = new();
        public double MinDistanceFromSupport { get; set; }
        public double MinDistanceFromSandblast { get; set; }
        public double MinDistanceFromOtherModules { get; set; }
        public TouchButtonModuleInfo()
        {
            ModuleType = MirrorModuleType.TouchButtonModuleType;
        }
        public double GetEnergyConsumption()
        {
            return Watt;
        }

        public double GetTransformerNominalPower()
        {
            return GetEnergyConsumption();
        }
        public override TouchButtonModuleInfo GetDeepClone()
        {
            var clone = (TouchButtonModuleInfo)this.MemberwiseClone();
            clone.RearDimensions = this.RearDimensions.GetDeepClone();
            clone.FrontDimensionsButton = this.FrontDimensionsButton.GetDeepClone();
            clone.IP = this.IP.GetDeepClone();
            return clone;
        }

        public void SetPosition(PointXY point, PointXY parentCenter)
        {
            RearDimensions.LocationX = point.X;
            RearDimensions.LocationY = point.Y;
            FrontDimensionsButton.LocationX = point.X + ButtonOffsetXFromRearRectangle;
            FrontDimensionsButton.LocationY = point.Y + ButtonOffsetYFromRearRectangle;
        }

        /// <summary>
        /// Returns the Position of the Touch Module(main back frame) Center
        /// </summary>
        /// <returns></returns>
        public PointXY GetPosition()
        {
            return RearDimensions.GetLocation();
        }
    }
    public class TouchButtonModuleInfoEqualityComparer : IEqualityComparer<TouchButtonModuleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public TouchButtonModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
        {
            baseComparer = new();
            rectComparer = new(disregardCollisionDistances);
            shapeComparer = new(disregardCollisionDistances);
            ipComparer = new();
            this.disregardCollisionDistances = disregardCollisionDistances;
        }

        private readonly MirrorModuleInfoBaseEqualityComparer baseComparer;
        private readonly RectangleInfoEqualityComparer rectComparer;
        private readonly ShapeInfoEqualityComparer shapeComparer;
        private readonly IPRatingEqualityComparer ipComparer;
        private readonly bool disregardCollisionDistances;

        public bool Equals(TouchButtonModuleInfo? x, TouchButtonModuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Watt == y!.Watt &&
                ipComparer.Equals(x.IP, y.IP) &&
                rectComparer.Equals(x.RearDimensions, y.RearDimensions) &&
                shapeComparer.Equals(x.FrontDimensionsButton, y.FrontDimensionsButton) &&
                x.ButtonOffsetXFromRearRectangle == y.ButtonOffsetXFromRearRectangle &&
                x.ButtonOffsetYFromRearRectangle == y.ButtonOffsetYFromRearRectangle &&
                x.NumberOfButtons == y.NumberOfButtons &&
                x.TouchButtonsDistance == y.TouchButtonsDistance &&
                x.RegulatedItems.SequenceEqual(y.RegulatedItems) &&
                (disregardCollisionDistances ||
                   (x.MinDistanceFromSupport == y.MinDistanceFromSupport &&
                    x.MinDistanceFromSandblast == y.MinDistanceFromSandblast &&
                    x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules));
        }

        public int GetHashCode([DisallowNull] TouchButtonModuleInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }
}
