
using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Interfaces;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System.Diagnostics.CodeAnalysis;

namespace MirrorsLib.MirrorElements.MirrorModules
{
    public class TransformerModuleInfo : MirrorModuleInfo, IMirrorPositionable, IRateableIP , IDeepClonable<TransformerModuleInfo>
    {
        public double Watt { get; set; }
        public RectangleInfo TransformerDimensions { get; set; } = new(0,0);
        public IPRating IP { get; set; } = new();
        public double MinDistanceFromSupport { get; set; }
        public double MinDistanceFromSandblast { get; set; }
        public double MinDistanceFromOtherModules { get; set; }
        public TransformerModuleInfo()
        {
            ModuleType = MirrorModuleType.TransformerModuleType;
        }
        public override TransformerModuleInfo GetDeepClone()
        {
            var clone = (TransformerModuleInfo)this.MemberwiseClone();
            clone.TransformerDimensions = this.TransformerDimensions.GetDeepClone();
            clone.IP = this.IP.GetDeepClone();
            return clone;
        }

        public void SetPosition(PointXY point, PointXY parentCenter)
        {
            TransformerDimensions.LocationX = point.X;
            TransformerDimensions.LocationY = point.Y;
        }

        /// <summary>
        /// Returns the Position of the Transformer 's center
        /// </summary>
        /// <returns></returns>
        public PointXY GetPosition()
        {
            return TransformerDimensions.GetLocation();
        }
    }
    public class TransformerModuleInfoEqualityComparer : IEqualityComparer<TransformerModuleInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disregardCollisionDistances">Weather to disregard the Collision distances for the comparison (Distances from support/sandblast/other)</param>
        public TransformerModuleInfoEqualityComparer(bool disregardCollisionDistances = false)
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

        public bool Equals(TransformerModuleInfo? x, TransformerModuleInfo? y)
        {
            return baseComparer.Equals(x, y) &&
                x!.Watt == y!.Watt &&
                ipComparer.Equals(x.IP, y.IP) &&
                rectComparer.Equals(x.TransformerDimensions, y.TransformerDimensions) &&
                (disregardCollisionDistances ||
                   (x.MinDistanceFromSupport == y.MinDistanceFromSupport &&
                    x.MinDistanceFromSandblast == y.MinDistanceFromSandblast &&
                    x.MinDistanceFromOtherModules == y.MinDistanceFromOtherModules));
        }

        public int GetHashCode([DisallowNull] TransformerModuleInfo obj)
        {
            throw new HashCodeNotSupportedException(this);
        }
    }



}
